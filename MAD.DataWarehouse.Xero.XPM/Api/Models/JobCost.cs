using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("job.api/costs/{jobId}")]
    internal class JobCost
    {
        [Key]
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        public Guid UUID { get; set; }

        [Key]
        [ApiEndpointModelProperty(DestinationType = "NVARCHAR(10)")]
        public string JobId { get; set; }
    }
}
