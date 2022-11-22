using Hangfire;
using MAD.DataWarehouse.Xero.XPM.Api;
using MAD.DataWarehouse.Xero.XPM.Database;
using Microsoft.EntityFrameworkCore;
using MIFCore.Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Jobs
{
    internal class EndpointExtractJob
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IDbContextFactory<XeroDbContext> dbContextFactory;
        private readonly ApiEndpointRegister apiEndpointRegister;
        private readonly IBackgroundJobClient backgroundJobClient;

        public EndpointExtractJob(
            IHttpClientFactory httpClientFactory,
            IDbContextFactory<XeroDbContext> dbContextFactory,
            ApiEndpointRegister apiEndpointRegister,
            IBackgroundJobClient backgroundJobClient)
        {
            this.httpClientFactory = httpClientFactory;
            this.dbContextFactory = dbContextFactory;
            this.apiEndpointRegister = apiEndpointRegister;
            this.backgroundJobClient = backgroundJobClient;
        }

        [DisableIdenticalQueuedItems]
        public async Task Extract(string endpointName, ExtractArgs extractArgs = null)
        {
            // If there aren't any endpoints registered, then we can't do anything. So let's just reschedule this job for later.
            if (this.apiEndpointRegister.Endpoints.Any() == false)
            {
                throw new RescheduleJobException(DateTime.Now.AddMinutes(1));
            }

            var endpoint = this.apiEndpointRegister.Get(endpointName);
            await this.ExtractEndpoint(endpoint, extractArgs);
        }

        private async Task ExtractEndpoint(ApiEndpoint endpoint, ExtractArgs extractArgs = null)
        {
            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            var httpClient = this.httpClientFactory.CreateClient(endpoint.HttpClientName);
            var request = await CreateRequest(endpoint, extractArgs);
            var apiData = await this.ExecuteRequest(endpoint, httpClient, request, extractArgs);

            // If OnPrepareNextRequest returns null, then we're done with this endpoint.
            var nextRequestData = await endpoint.OnPrepareNextRequest(new PrepareNextRequestArgs(ApiData: apiData));
            if (nextRequestData == default(IDictionary<string, object>))
                return;

            // Otherwise we need to schedule another job to continue extracting this endpoint.
            this.backgroundJobClient.Enqueue<EndpointExtractJob>(y => y.Extract(endpoint.Name, new ExtractArgs(nextRequestData, apiData.ParentId)));
        }

        private async Task<ApiData> ExecuteRequest(ApiEndpoint endpoint, HttpClient httpClient, HttpRequestMessage request, ExtractArgs extractArgs = null)
        {
            // Get the response payload as a string
            var response = await httpClient.SendAsync(request);
            var data = await response.Content.ReadAsStringAsync();

            // Write an ApiData record to the database
            using var db = await this.dbContextFactory.CreateDbContextAsync();
            var apiData = new ApiData
            {
                Endpoint = endpoint.Name,
                Uri = response.RequestMessage.RequestUri.ToString(),
                Data = data,
                ParentId = extractArgs?.ParentApiDataId
            };

            db.ApiData.Add(apiData);
            await db.SaveChangesAsync();

            return apiData;
        }

        private static async Task<HttpRequestMessage> CreateRequest(ApiEndpoint endpoint, ExtractArgs extractArgs = null)
        {
            // Create a new request, using endpoint.Name as the relative uri
            // i.e endpoint.Name = "getStuff" and httpClient.BaseAddress = "https://someapi/api/"
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint.Name, UriKind.Relative)
            };

            // Stitch on any additional headers
            foreach (var h in endpoint.AdditionalHeaders)
            {
                request.Headers.Add(h.Key, h.Value);
            }

            // OnPrepareRequest after this system has finished up with the request
            await endpoint.OnPrepareRequest(new PrepareRequestArgs(request, extractArgs?.NextRequestData));
            return request;
        }

        public record class ExtractArgs(IDictionary<string, object> NextRequestData, int? ParentApiDataId);
    }
}
