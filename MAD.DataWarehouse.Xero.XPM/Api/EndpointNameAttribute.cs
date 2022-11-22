using System;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    internal class EndpointNameAttribute : Attribute
    {
        private readonly string endpoint;

        public EndpointNameAttribute(string endpoint)
        {
            this.endpoint = endpoint;
        }
    }
}
