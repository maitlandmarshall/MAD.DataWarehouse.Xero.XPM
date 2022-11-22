using System;
using System.Runtime.Serialization;

namespace MAD.DataWarehouse.Xero.XPM
{
    [Serializable]
    internal class XeroSecurityRequirementException : Exception
    {
        public XeroSecurityRequirementException()
        {
        }

        public XeroSecurityRequirementException(string message) : base(message)
        {
        }

        public XeroSecurityRequirementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XeroSecurityRequirementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}