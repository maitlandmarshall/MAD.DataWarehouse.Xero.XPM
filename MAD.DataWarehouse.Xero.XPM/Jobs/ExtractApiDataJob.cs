using Hangfire;
using MAD.DataWarehouse.Xero.XPM.Api;
using MAD.DataWarehouse.Xero.XPM.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Jobs
{
    internal class ExtractApiDataJob
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRecurringJobManager recurringJobManager;
        private readonly IDbContextFactory<XeroDbContext> dbContextFactory;

        public ExtractApiDataJob(IHttpClientFactory httpClientFactory, IRecurringJobManager recurringJobManager, IDbContextFactory<XeroDbContext> dbContextFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.recurringJobManager = recurringJobManager;
            this.dbContextFactory = dbContextFactory;
        }

        public async Task CreateRecurringJobs()
        {
            var httpClient = this.httpClientFactory.CreateClient("xero");
            var connections = await httpClient.GetFromJsonAsync<IEnumerable<Connection>>("https://api.xero.com/connections");

            // Filter for the practice manager connections
            connections = connections.Where(y => y.TenantType == "PRACTICEMANAGER");

            var endpointsToExtract = new[]
            {
                "client.api/list"
            };

            // Schedule in the API extraction jobs for each endpoint, for each tenant
            foreach (var connection in connections)
            {
                foreach (var endpoint in endpointsToExtract)
                {
                    this.recurringJobManager.AddOrUpdate<ExtractApiDataJob>(
                        $"{endpoint}-{connection.TenantId}",
                        job => job.Extract(endpoint, connection.TenantId, null),
                        Cron.Daily()
                    );
                }
            }
        }

        public async Task Extract(string endpoint, Guid tenantId, int? parentId = null)
        {
            var httpClient = this.httpClientFactory.CreateClient("xero");
            var response = await httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint, UriKind.Relative),
                Headers =
                {
                    { "xero-tenant-id", tenantId.ToString() }
                }
            });

            var data = await response.Content.ReadAsStringAsync();

            using var db = await this.dbContextFactory.CreateDbContextAsync();

            db.ApiData.Add(new ApiData
            {
                Endpoint = endpoint,
                Uri = response.RequestMessage.RequestUri.ToString(),
                Data = data
            });

            await db.SaveChangesAsync();
        }
    }
}
