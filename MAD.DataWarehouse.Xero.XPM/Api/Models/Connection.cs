using System;
using System.Text.Json.Serialization;

namespace MAD.DataWarehouse.Xero.XPM.Api.Models
{
    internal class Connection
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("authEventId")]
        public Guid AuthEventId { get; set; }

        [JsonPropertyName("tenantId")]
        public Guid TenantId { get; set; }

        [JsonPropertyName("tenantType")]
        public string TenantType { get; set; }

        [JsonPropertyName("tenantName")]
        public string TenantName { get; set; }

        [JsonPropertyName("createdDateUtc")]
        public DateTimeOffset CreatedDateUtc { get; set; }

        [JsonPropertyName("updatedDateUtc")]
        public DateTimeOffset UpdatedDateUtc { get; set; }
    }
}
