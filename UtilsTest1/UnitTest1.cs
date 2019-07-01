using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using API.Models;
using API.Utils;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UtilsTest1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Stat testStatA = new Stat
            {
                calculated_value = (decimal) 12.3,
                device_type = "temperatureSensor",
                id = 1,
                mac_address = "12:12:12:12:12:12",
                timestamp = new DateTime(2019, 6, 30)
            };
            Stat testStatB = new Stat
            {
                calculated_value = (decimal) 14.3,
                device_type = "temperatureSensor",
                id = 2,
                mac_address = "12:12:12:12:12:12",
                timestamp = new DateTime(2019, 7, 1)
            };
            List<Stat> testStats = new List<Stat>();
            testStats.Add(testStatA);
            testStats.Add(testStatB);
            
            JObject objTestA = new JObject();
            objTestA.Add("temperature", 12.3);
            objTestA.Add("Day", 30);
            
            JObject objTestB = new JObject();
            objTestB.Add("temperature", 14.3);
            objTestB.Add("Day", 1);
            
            JArray testArray = new JArray();
            testArray.Add(objTestA);
            testArray.Add(objTestB);
            
            JObject tJObject = new JObject();
            tJObject.Add("temperatureSensor", testArray);
            JObject a = new JObject();
            a.Add("12:12:12:12:12:12", tJObject);
            
            string pattern = @"^[a-z]*";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            JObject b = JsonUtils.ToJson(testStats, regex, "12:12:12:12:12:12", "Day");
            Assert.True(b.ToString() == a.ToString());
        }
    }
}