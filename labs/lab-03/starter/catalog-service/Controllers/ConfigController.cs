using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FoodApp
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        IConfiguration cfg;
        AILogger logger;

        public ConfigController(IConfiguration config, AILogger ai)
        {
            cfg = config;
            logger = ai;
        }

        // https://localhost:5001/config/
        [HttpGet]
        public ActionResult GetConfig()
        {
            logger.LogEvent("ConfigController.GetConfig", "Getting config",);
            var config = cfg.Get<AppConfig>();
            return Ok(config);  
        }

        // https://localhost:5001/config/getEnvVars
        [HttpGet("getEnvVars")]
        public ActionResult GetEnvVars()
        {
            var val = Environment.GetEnvironmentVariables();
            return Ok(val);  
        }
    }
}