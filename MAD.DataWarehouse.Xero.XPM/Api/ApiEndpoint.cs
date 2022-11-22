using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal class ApiEndpoint
    {
        public string Name { get; set; }
        public string JobName { get; set; }

        public string HttpClientName { get; set; }

        public IDictionary<string, string> AdditionalHeaders { get; } = new Dictionary<string, string>();

        public delegate Task PrepareRequestDelegate(PrepareRequestArgs args);
        public delegate Task<IDictionary<string, object>> PrepareNextRequestDelegate(PrepareNextRequestArgs args);

        public event PrepareRequestDelegate PrepareRequest;
        public event PrepareNextRequestDelegate PrepareNextRequest;

        internal async Task OnPrepareRequest(PrepareRequestArgs args)
        {
            await (this.PrepareRequest?.Invoke(args) ?? Task.CompletedTask);
        }

        internal async Task<IDictionary<string, object>> OnPrepareNextRequest(PrepareNextRequestArgs args)
        {
            return await (this.PrepareNextRequest?.Invoke(args) ?? Task.FromResult(default(IDictionary<string, object>)));
        }
    }
}
