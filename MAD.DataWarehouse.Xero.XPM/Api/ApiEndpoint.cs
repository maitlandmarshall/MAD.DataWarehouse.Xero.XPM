using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal class ApiEndpoint : IPrepareRequest, IPrepareNextRequest
    {
        public string Name { get; init; }
        public string JobName { get; init; }

        public string HttpClientName { get; init; }

        public IDictionary<string, string> AdditionalHeaders { get; } = new Dictionary<string, string>();

        public delegate Task PrepareRequestDelegate(PrepareRequestArgs args);
        public delegate Task<IDictionary<string, object>> PrepareNextRequestDelegate(PrepareNextRequestArgs args);

        public event PrepareRequestDelegate PrepareRequest;
        public event PrepareNextRequestDelegate PrepareNextRequest;

        public virtual async Task OnPrepareRequest(PrepareRequestArgs args)
        {
            await (this.PrepareRequest?.Invoke(args) ?? Task.CompletedTask);
        }

        public virtual async Task<IDictionary<string, object>> OnPrepareNextRequest(PrepareNextRequestArgs args)
        {
            return await (this.PrepareNextRequest?.Invoke(args) ?? Task.FromResult(default(IDictionary<string, object>)));
        }
    }
}
