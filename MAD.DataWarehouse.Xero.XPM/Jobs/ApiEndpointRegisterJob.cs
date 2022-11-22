using Hangfire;
using MAD.DataWarehouse.Xero.XPM.Api;
using MAD.DataWarehouse.Xero.XPM.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Jobs
{
    internal class ApiEndpointRegisterJob
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ApiEndpointRegister apiEndpointRegister;

        public ApiEndpointRegisterJob(IHttpClientFactory httpClientFactory, ApiEndpointRegister apiEndpointRegister)
        {
            this.httpClientFactory = httpClientFactory;
            this.apiEndpointRegister = apiEndpointRegister;
        }

        [DisableConcurrentExecution(600)]
        public async Task EnsureEndpointsAreRegistered()
        {
            // If there are already endpoints registered, then we don't need to do anything otherwise it will cause double ups.
            if (this.apiEndpointRegister.Endpoints.Any())
                return;

            var httpClient = this.httpClientFactory.CreateClient("xero");
            var connections = await httpClient.GetFromJsonAsync<IEnumerable<Connection>>("https://api.xero.com/connections");

            // Filter for the practice manager connections
            connections = connections.Where(y => y.TenantType == "PRACTICEMANAGER");

            var endpointsToExtract = new[]
            {
                "client.api/list",
                "category.api/list",
                "clientgroup.api/list"
            };

            foreach (var tenant in connections)
            {
                foreach (var endpoint in endpointsToExtract)
                {
                    this.apiEndpointRegister.Register(new ApiEndpoint
                    {
                        Name = endpoint,
                        JobName = $"{endpoint} ({tenant.TenantName})",
                        AdditionalHeaders =
                        {
                            {  "xero-tenant-id", tenant.TenantId.ToString() }
                        },
                        HttpClientName = "xero"
                    });
                }
            }
        }
    }
}
