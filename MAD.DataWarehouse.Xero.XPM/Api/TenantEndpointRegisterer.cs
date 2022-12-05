using MAD.DataWarehouse.Xero.XPM.Database;
using Microsoft.EntityFrameworkCore;
using MIFCore.Hangfire.APIETL;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpointSelector(".*")]
    internal class TenantEndpointRegisterer : IDefineEndpoints
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
                var httpClient = this.httpClientFactory.CreateClient();
                var connections = await httpClient.GetFromJsonAsync<IEnumerable<Connection>>("https://api.xero.com/connections");

                // Filter for the practice manager connections
                this.connections = connections.Where(y => y.TenantType == "PRACTICEMANAGER");
            }

            foreach (var tenant in this.connections)
            {
                yield return new ApiEndpoint($"{endpointName} ({tenant.TenantName})")
                {
                    AdditionalHeaders =
                    {
                        {  "xero-tenant-id", tenant.TenantId.ToString() }
                    }
                };
            }
        }

        //public async Task OnHandleResponse(HandleResponseArgs args)
        //{
        //    using var db = await this.dbContextFactory.CreateDbContextAsync();
        //    db.ApiData.Add(args.ApiData);

        //    await db.SaveChangesAsync();
        //}
    }
}
