using Hangfire;
using MIFCore.Hangfire;
using MIFCore.Hangfire.APIETL.Extract;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Jobs
{
    internal class ApiEndpointRegisterJob
    {
        private readonly ApiEndpointRegister apiEndpointRegister;

        public ApiEndpointRegisterJob(ApiEndpointRegister apiEndpointRegister)
        {
            this.apiEndpointRegister = apiEndpointRegister;
        }

        [DisableConcurrentExecution(600)]
        [DisableIdenticalQueuedItems]
        public async Task EnsureEndpointsAreRegistered()
        {
            await this.apiEndpointRegister.Register();
        }
    }
}
