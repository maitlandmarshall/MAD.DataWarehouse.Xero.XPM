using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Transform;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("job.api/list", "Tasks")]
    internal class JobTaskTransform : ITransformModel
    {
        public Task OnTransformModel(TransformModelArgs args)
        {
            var task = args.Transform.Object;
            var job = args.Transform.GraphObjectSet.Parent;

            task.Add("JobUUID", job["UUID"]);

            return Task.CompletedTask;
        }
    }
}
