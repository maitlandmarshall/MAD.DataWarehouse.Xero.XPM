using MAD.DataWarehouse.Xero.XPM.Api.Models;
using MAD.DataWarehouse.Xero.XPM.Database;
using Microsoft.EntityFrameworkCore;
using MIFCore.Hangfire.APIETL.Extract;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api.Endpoints
{
    [ApiEndpointSelector(".*")]
    [ApiEndpointName("category.api/list")]
    [ApiEndpointName("clientgroup.api/list")]
    [ApiEndpointName("client.api/list")]
    internal class TenantEndpointRegisterer : IDefineEndpoints, IHandleResponse
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IDbContextFactory<XeroDbContext> dbContextFactory;
        private IEnumerable<Connection> connections;

        public TenantEndpointRegisterer(IHttpClientFactory httpClientFactory, IDbContextFactory<XeroDbContext> dbContextFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.dbContextFactory = dbContextFactory;
        }

        public async IAsyncEnumerable<ApiEndpoint> DefineEndpoints(string endpointName)
        {
            if (this.connections is null)
            {
                var httpClient = this.httpClientFactory.CreateClient("xero");
                var connections = await httpClient.GetFromJsonAsync<IEnumerable<Connection>>("https://api.xero.com/connections");

                // Filter for the practice manager connections
                this.connections = connections.Where(y => y.TenantType == "PRACTICEMANAGER");
            }

            foreach (var tenant in this.connections)
            {
                yield return new ApiEndpoint($"{endpointName} ({tenant.TenantName})", "xero")
                {
                    AdditionalHeaders =
                    {
                        {  "xero-tenant-id", tenant.TenantId.ToString() }
                    }
                };
            }
        }

        public async Task OnHandleResponse(HandleResponseArgs args)
        {
            using var db = await this.dbContextFactory.CreateDbContextAsync();
            db.ApiData.Add(args.ApiData);

            await db.SaveChangesAsync();
        }
    }
}
