using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("invoice.api/list")]
    internal class Invoice
    {
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        [Key]
        public Guid UUID { get; set; }
    }
}
