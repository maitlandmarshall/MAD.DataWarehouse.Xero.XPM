using System;

namespace MAD.DataWarehouse.Xero.XPM.Database
{
    internal class ApiData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public string Endpoint { get; set; }
        public string Uri { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string Data { get; set; }

        public virtual ApiData Parent { get; set; }
    }
}
