using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Models;
using API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatController : ControllerBase
    {
        private readonly StatContext _context;

        public StatController(StatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stat>>> GetStats()
        {
            return await _context.Stats.ToListAsync();
        }
        
        
        [HttpGet("{id}/{period}")]
        public async Task<JObject> GetStatFromTo(string id, string period)
        {
            string pattern = @"^[a-z]*";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            int per = -1;
            string periodicity = "Hour";
            switch (period)
            {
                case "day": per = -1; break;
                case "week": per = -7; break;
                case "month": per = -30; break;
            }
            List<Stat> stats;
            if (period == "day")
            {
                stats = await _context.Stats.Where(
                        p => p.timestamp >= DateTime.Now.AddDays(per)
                             && p.timestamp <= DateTime.Now
                             && p.mac_address == id)
                    .ToListAsync();
            }
            else if (period == "week")
            {
                string sql = $"Call getDeviceAveragesWeek('{id}')";
                stats = await _context.Stats.FromSql(sql).ToListAsync();
            }
            else
            {
                string sql = $"call GetDeviceAveragesMonth('{id}')";
                stats = await _context.Stats.FromSql(sql).ToListAsync();
            }
            switch (period)
            {
                case "day": periodicity = "Hour"; break;
                case "week": periodicity = "DayOfWeek"; break;
                case "month": periodicity = "Day"; break;
            }
            return JsonUtils.ToJson(stats, regex, id, periodicity);
        }

        [HttpGet("{id}/{type}/{period}")]
        public async Task<JObject> GetStatDTFromTo(string id, string type, string period)
        {
            string pattern = @"^[a-z]*";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            int per = -1;
            string periodicity = "Hour";
            switch (period)
            {
                case "day": per = -1; break;
                case "week": per = -7; break;
                case "month": per = -30; break;
            }
            List<Stat> stats;
            if (period == "day")
            {
                stats= await _context.Stats.Where(
                        p => p.timestamp >= DateTime.Now.AddDays(per)
                             && p.timestamp <= DateTime.Now
                             && p.mac_address == id
                             && p.device_type == type)
                    .ToListAsync();
            }
            
            
            else if (period == "week")
            {
                string sql = $"CALL GetDeviceAveragesWeekByType('{id}', '{type}')";
                stats = await _context.Stats.FromSql(sql).ToListAsync();
            }
            else
            {
                string sql = $"CALL GetDeviceAveragesMonthByType('{id}', '{type}')";
                stats = await _context.Stats.FromSql(sql).ToListAsync();
            }
            switch (period)
            {
                case "day": periodicity = "Hour"; break;
                case "week": periodicity = "DayOfWeek"; break;
                case "month": periodicity = "Day"; break;
            }
            
            JObject tObject = new JObject();
            JArray store = new JArray();
            foreach (Stat stat in stats)
            {
                JObject o  = new JObject();
                o.Add(regex.Match(stat.device_type).Groups[0].Value, stat.calculated_value);
                o.Add(periodicity, (int) stat.timestamp.GetType().GetProperty(periodicity).GetValue(stat.timestamp));
                store.Add(o);
            }
            tObject.Add(type, store);
            JObject rObject = new JObject();
            rObject.Add(id, tObject);

            return rObject;
        }

        
    }
}