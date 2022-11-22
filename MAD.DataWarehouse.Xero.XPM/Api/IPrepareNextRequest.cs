using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal interface IPrepareNextRequest
    {
        Task<IDictionary<string, object>> OnPrepareNextRequest(PrepareNextRequestArgs args);
    }
}