using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("cost.api/list")]
    internal class Cost
    {
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        [Key]
        public Guid UUID { get; set; }
    }
}
