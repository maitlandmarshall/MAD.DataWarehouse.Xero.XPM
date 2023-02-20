using Hangfire.Common;
using MIFCore.Hangfire;
using System.Collections.Generic;
using System.Linq;

namespace MAD.DataWarehouse.Xero.XPM.Jobs
{
    internal class JobFilterProvider : IJobFilterProvider
    {
        public IEnumerable<JobFilter> GetFilters(Job job)
        {
            var endpointsToTrackSuccess = new[] { "job.api/list", "invoice.api/list" };

            if (job.Args.ElementAtOrDefault(0) is string endpointName
                && endpointsToTrackSuccess.Contains(endpointName)

                // Only track success if this is the root request (i.e no pagination parameters)
                && job.Args.ElementAtOrDefault(1) is null)
            {
                yield return new JobFilter(new TrackLastSuccessAttribute(endpointName), JobFilterScope.Method, null);
            }

            yield break;
        }
    }
}
