using MAD.DataWarehouse.Xero.XPM.Database;
using System.Collections.Generic;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    public record class PrepareNextRequestArgs(ApiData ApiData, IReadOnlyDictionary<string, object> Data);
}