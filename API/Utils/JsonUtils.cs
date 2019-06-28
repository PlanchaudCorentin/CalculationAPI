using System.Collections.Generic;
using System.Text.RegularExpressions;
using API.Models;
using Newtonsoft.Json.Linq;

namespace API.Utils
{
    public class JsonUtils
    {
        public static JObject ToJson(List<Stat> stats, Regex regex, string id, string periodicity)
        {
            JObject tObject = new JObject();
            Dictionary<string, JArray> dic = new Dictionary<string, JArray>();
            foreach (Stat stat in stats)
            {
                JArray j;
                if (dic.TryGetValue(stat.device_type, out j))
                {
                    JObject o  = new JObject();
                    o.Add(regex.Match(stat.device_type).Groups[0].Value, stat.calculated_value);
                    o.Add(periodicity, (int) stat.timestamp.GetType().GetProperty(periodicity).GetValue(stat.timestamp));
                    j.Add(o);
                }
                else
                {
                    j = new JArray();
                    JObject o = new JObject();
                    o.Add(regex.Match(stat.device_type).Groups[0].Value, stat.calculated_value);
                    o.Add(periodicity, (int) stat.timestamp.GetType().GetProperty(periodicity).GetValue(stat.timestamp));
                    j.Add(o);
                    dic.Add(stat.device_type, j);
                }
            }
            foreach (KeyValuePair<string, JArray> keyValuePair in dic)
            {
                tObject.Add(keyValuePair.Key, keyValuePair.Value);
            }
            JObject rObject = new JObject();
            rObject.Add(id, tObject);
            return rObject;
        }
    }
}