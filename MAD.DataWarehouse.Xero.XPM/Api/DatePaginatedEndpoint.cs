using MIFCore.Hangfire;
using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Extract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                var lastSuccess = BackgroundJobContext.Current.BackgroundJob.GetLastSuccess();

                // If there is a last success, then we should just get the trailing week up until now
                if (lastSuccess.HasValue)
                {
                    var from = lastSuccess.Value.AddDays(-7).Date;
                    var to = DateTime.Now.Date.AddDays(2);

                    this.SetUriWithRange(args.Request, from, to);

                    return Task.CompletedTask;
                }

                year = (long)2015;
                args.Data["year"] = year;
            }

            // Generate from and to
            var startDate = new DateTime((int)(long)year, 1, 1);
            var endDate = startDate.AddYears(1);

            this.SetUriWithRange(args.Request, startDate, endDate);

            return Task.CompletedTask;
        }

        public Task<IDictionary<string, object>> OnPrepareNextRequest(PrepareNextRequestArgs args)
        {
            if (args.Data.Any() == false)
                return Task.FromResult(null as IDictionary<string, object>);

            var year = (int)(long)args.Data["year"];
            year++;

            if (year > DateTime.Now.Year)
                return Task.FromResult(null as IDictionary<string, object>);

            return Task.FromResult(new Dictionary<string, object>
            {
                { "year", year}
            } as IDictionary<string, object>);
        }

        private void SetUriWithRange(HttpRequestMessage request, DateTime startDate, DateTime endDate)
        {
            // Parse the current query params to avoid deleting them
            var queryParams = HttpUtility.ParseQueryString(request.RequestUri.Query);
            queryParams.Add("from", startDate.ToString("yyyyMMdd"));
            queryParams.Add("to", endDate.ToString("yyyyMMdd"));

            // Paste the the new params over the old
            var uriBuilder = new UriBuilder(request.RequestUri);
            uriBuilder.Query = queryParams.ToString();

            // Set the uri to the new one with the new query params
            request.RequestUri = uriBuilder.Uri;
        }
    }
}
