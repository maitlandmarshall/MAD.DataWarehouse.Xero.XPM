using MIFCore.Hangfire.APIETL;
using MIFCore.Hangfire.APIETL.Transform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MAD.DataWarehouse.Xero.XPM.Api
{
    [ApiEndpoint("cost.api/list")]
    [ApiEndpoint("category.api/list")]
    internal class ResponseParser : IParseResponse
    {
        public async Task<IEnumerable<IDictionary<string, object>>> OnParse(ParseResponseArgs args)
        {
            var xml = XDocument.Parse(args.ApiData.Data);
            var jsonRaw = JsonConvert.SerializeObject(xml);
            var json = JsonConvert.DeserializeObject<ExpandoObject>(jsonRaw) as IDictionary<string, object>;

            var listProperty = args.Endpoint.Name switch
            {
                "cost.api/list" => "Costs",
                "category.api/list" => "Categories",
                _ => throw new NotImplementedException()
            };

            var listContainer = (json["Response"] as IDictionary<string, object>)[listProperty] as IDictionary<string, object>;

            if (listContainer != null
                && listContainer.Any())
            {
                var costs = (listContainer.First().Value as IEnumerable<object>)
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
