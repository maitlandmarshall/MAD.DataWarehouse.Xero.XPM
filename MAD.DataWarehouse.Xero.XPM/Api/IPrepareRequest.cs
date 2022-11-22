using System.Threading.Tasks;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal interface IPrepareRequest
    {
        Task OnPrepareRequest(PrepareRequestArgs args);
    }
}