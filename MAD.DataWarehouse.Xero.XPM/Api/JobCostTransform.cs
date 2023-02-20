using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Transform;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("job.api/costs/{jobId}")]
    internal class JobCostTransform : ITransformModel
    {
        public Task OnTransformModel(TransformModelArgs args)
        {
            var jobCost = args.Transform.Object;
            jobCost["JobId"] = args.ExtractArgs.RequestData["jobId"];

            return Task.CompletedTask;
        }
    }
}
