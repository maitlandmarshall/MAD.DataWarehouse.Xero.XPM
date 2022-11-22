using System.Collections.Generic;

namespace MAD.DataWarehouse.Xero.XPM
{
    public class AppConfig
    {
        public string ConnectionString { get; set; }

        public IEnumerable<string> Scope { get; set; } = new[] { "offline_access", "openid", "profile", "email", "practicemanager" };
    }
}
