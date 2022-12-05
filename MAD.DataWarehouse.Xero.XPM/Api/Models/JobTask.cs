using MIFCore.Hangfire.APIETL.Load;
using System;
using System.ComponentModel.DataAnnotations;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    [ApiEndpointModel("job.api/list", InputPath = "Tasks_Task")]
    internal class JobTask
    {
        [Key]
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        public Guid UUID { get; set; }

        [Key]
        [ApiEndpointModelProperty(DestinationType = "UNIQUEIDENTIFIER")]
        public Guid JobUUID { get; set; }
    }
}
