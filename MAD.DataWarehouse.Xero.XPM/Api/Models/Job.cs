using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("job.api/list")]
    internal class Job
    {
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        [Key]
        public Guid UUID { get; set; }
    }
}
