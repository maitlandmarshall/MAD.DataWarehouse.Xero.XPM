using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("task.api/list")]
    internal class Task
    {
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        [Key]
        public Guid UUID { get; set; }
    }
}
