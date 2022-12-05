using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Extract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("job.api/list")]
    [ApiEndpoint("invoice.api/list")]
    internal class DatePaginatedEndpoint : IPrepareRequest, IPrepareNextRequest
    {
        public Task OnPrepareRequest(PrepareRequestArgs args)
        {
            // If there is no year query parameter, then we are starting from year 1
            if (args.Data.TryGetValue("year", out var year) == false)
            {
                year = (long)2015;
                args.Data["year"] = year;
            }

            // Generate from and to
            var startDate = new DateTime((int)(long)year, 1, 1);
            var endDate = startDate.AddYears(1);

            var queryParams = HttpUtility.ParseQueryString(args.Request.RequestUri.Query);
            queryParams.Add("from", startDate.ToString("yyyyMMdd"));
            queryParams.Add("to", endDate.ToString("yyyyMMdd"));

            var uriBuilder = new UriBuilder(args.Request.RequestUri);
            uriBuilder.Query = queryParams.ToString();

            args.Request.RequestUri = uriBuilder.Uri;

            return Task.CompletedTask;
        }

        public Task<IDictionary<string, object>> OnPrepareNextRequest(PrepareNextRequestArgs args)
        {
            var year = (int)(long)args.Data["year"];
            year++;

            if (year > DateTime.Now.Year)
                return Task.FromResult(null as IDictionary<string, object>);

            return Task.FromResult(new Dictionary<string, object>
            {
                { "year", year}
            } as IDictionary<string, object>);
        }
    }
}
