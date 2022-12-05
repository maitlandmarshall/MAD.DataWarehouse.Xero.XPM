using Hangfire;
using MIFCore.Hangfire;
using MIFCore.Hangfire.APIETL;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Jobs
{
    internal class ApiEndpointRegisterJob
    {
        private readonly IApiEndpointRegister apiEndpointRegister;

        public ApiEndpointRegisterJob(IApiEndpointRegister apiEndpointRegister)
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
