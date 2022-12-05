using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Transform;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("cost.api/list")]
    internal class ResponseParser : IParseResponse
    {
        public async Task<IEnumerable<IDictionary<string, object>>> OnParse(ParseResponseArgs args)
        {
            var xml = XDocument.Parse(args.ApiData.Data);
            var jsonRaw = JsonConvert.SerializeObject(xml);
            dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(jsonRaw);

            var costContainer = json.Response.Costs as IDictionary<string, object>;

            if (costContainer != null
                && costContainer.Any())
            {
                var costs = (costContainer["Cost"] as IEnumerable<object>)
                    .Cast<IDictionary<string, object>>()
                    .ToList();

                costs.FlattenGraph();

                return costs;
            }
            else
            {
                return null;
            }
        }
    }
}
