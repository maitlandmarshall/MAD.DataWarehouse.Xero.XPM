using MIFCore.Hangfire.APIETL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MAD.DataWarehouse.Xero.XPM.Api.Endpoints
{
    [ApiEndpointName("cost.api/list")]
    internal class CostsApiListEndpoint : IPrepareRequest, IPrepareNextRequest
    {
        public Task OnPrepareRequest(PrepareRequestArgs args)
        {
            if (args.Data.TryGetValue("page", out var page) == false)
            {
                page = 1;
                args.Data["page"] = 1;
            }

            var queryParams = HttpUtility.ParseQueryString(args.Request.RequestUri.Query);
            queryParams.Add("page", page.ToString());

            var uriBuilder = new UriBuilder(args.Request.RequestUri);
            uriBuilder.Query = queryParams.ToString();

            args.Request.RequestUri = uriBuilder.Uri;

            return Task.CompletedTask;
        }

        public async Task<IDictionary<string, object>> OnPrepareNextRequest(PrepareNextRequestArgs args)
        {
            if (args.ApiData.Data.Contains("<Records>0</Records>") == false)
            {
                return new Dictionary<string, object>
                {
                    { "page", (int)args.Data["page"] + 1 }
                };
            }

            return default;
        }
    }
}
