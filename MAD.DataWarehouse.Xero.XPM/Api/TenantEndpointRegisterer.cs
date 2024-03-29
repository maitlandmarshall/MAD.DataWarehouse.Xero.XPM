﻿using MIFCore.Hangfire.APIETL;
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
        private IEnumerable<Connection> connections;

        public TenantEndpointRegisterer(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async IAsyncEnumerable<ApiEndpoint> DefineEndpoints(string endpointName)
        {
            if (this.connections is null)
            {
                var httpClient = this.httpClientFactory.CreateClient();
                var connections = await httpClient.GetFromJsonAsync<IEnumerable<Connection>>("https://api.xero.com/connections");

                // Filter for the practice manager connections
                this.connections = connections.Where(y => y.TenantType == "PRACTICEMANAGER").ToList();
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
