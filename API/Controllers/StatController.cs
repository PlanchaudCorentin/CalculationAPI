using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
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
        
        
        [HttpGet("{id}/{from}/{to}")]
        public async Task<JObject> GetStatFromTo(string id, string from, string to)
        {
            var stats = await _context.Stats.Where(
                    p => p.timestamp >= DateTime.ParseExact(from, "yyyy-MM-ddTHH:mm:ss",null)
                         && p.timestamp <= DateTime.ParseExact(to, "yyyy-MM-ddTHH:mm:ss",null)
                         && p.mac_address == id)
                .ToListAsync();
            JObject tObject = new JObject();
            Dictionary<string, JArray> dic = new Dictionary<string, JArray>();
            foreach (Stat stat in stats)
            {
                JArray j;
                if (dic.TryGetValue(stat.device_type, out j))
                {
                    j.Add(stat.calculated_value);
                }
                else
                {
                    j = new JArray();
                    j.Add(stat.calculated_value);
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
        
        [HttpGet("{id}")]
        public async Task<JObject> GetStatForId(string id)
        {
            var stats = await _context.Stats.Where(p => p.mac_address == id).ToListAsync();
            JObject tObject = new JObject();
            Dictionary<string, JArray> dic = new Dictionary<string, JArray>();
            foreach (Stat stat in stats)
            {
                JArray j;
                if (dic.TryGetValue(stat.device_type, out j))
                {
                    j.Add(stat.calculated_value);
                }
                else
                {
                    j = new JArray();
                    j.Add(stat.calculated_value);
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
        
        [HttpGet("{id}/{type}/{from}/{to}")]
        public async Task<JObject> GetStatDTFromTo(string id, string type, string from, string to)
        {
            var stats = await _context.Stats.Where(
                    p => p.timestamp >= DateTime.ParseExact(from, "yyyy-MM-ddTHH:mm:ss",null)
                         && p.timestamp <= DateTime.ParseExact(to, "yyyy-MM-ddTHH:mm:ss",null)
                         && p.mac_address == id
                         && p.device_type == type)
                .ToListAsync();
            JObject tObject = new JObject();
            JArray store = new JArray();
            foreach (Stat stat in stats)
            {
                store.Add(stat.calculated_value);
            }
            tObject.Add(type, store);
            JObject rObject = new JObject();
            rObject.Add(id, tObject);

            return rObject;
        } 
    }
}