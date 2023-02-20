using Hangfire;
using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Extract;
using MIFCore.Hangfire.APIETL.Transform;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("job.api/list")]
    internal class JobCostApiEndpointProducer : ITransformModel
    {
        private readonly IBackgroundJobClient backgroundJobClient;

        public JobCostApiEndpointProducer(IBackgroundJobClient backgroundJobClient)
        {
            this.backgroundJobClient = backgroundJobClient;
        }

        public Task OnTransformModel(TransformModelArgs args)
        {
            var job = args.Transform.Object;
            var jobId = job["ID"];

            this.backgroundJobClient.Enqueue<IApiEndpointExtractJob>(y => y.Extract("job.api/costs/{jobId}", new ExtractArgs(new Dictionary<string, object>
            {
                {"jobId", jobId }
            }, null)));

            return Task.CompletedTask;
        }
    }
}
