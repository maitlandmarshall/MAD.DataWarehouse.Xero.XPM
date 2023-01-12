using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("client.api/list")]
    internal class Client
    {
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        [Key]
        public Guid UUID { get; set; }
    }
}
