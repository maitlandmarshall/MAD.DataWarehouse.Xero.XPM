﻿using System.Collections.Generic;
using System.Net.Http;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    public record class PrepareRequestArgs(HttpRequestMessage Request, IDictionary<string, object> NextRequestData);
}