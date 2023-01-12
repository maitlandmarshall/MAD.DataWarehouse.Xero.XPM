using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Extract;
using System;
using System.Threading.Tasks;
using System.Web;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("job.api/list")]
    [ApiEndpoint("invoice.api/list")]
    [ApiEndpoint("client.api/list")]
    internal class AddDetailedQueryParamUrl : IPrepareRequest
    {
        public Task OnPrepareRequest(PrepareRequestArgs args)
        {
            var queryParams = HttpUtility.ParseQueryString(args.Request.RequestUri.Query);
            queryParams.Add("detailed", "true");

            var uriBuilder = new UriBuilder(args.Request.RequestUri);
            uriBuilder.Query = queryParams.ToString();

            args.Request.RequestUri = uriBuilder.Uri;

            return Task.CompletedTask;
        }
    }
}
