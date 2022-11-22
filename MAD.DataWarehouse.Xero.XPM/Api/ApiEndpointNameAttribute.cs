using System;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal class ApiEndpointNameAttribute : Attribute
    {
        public ApiEndpointNameAttribute(string endpoint)
        {
            this.Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
