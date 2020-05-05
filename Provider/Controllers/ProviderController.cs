using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Provider.Controllers
{
    [ApiController]
     [Route("api/[controller]")]
    public class ProviderController : ControllerBase
    {
        private IConfiguration _Configuration { get; }

        public ProviderController(IConfiguration configuration)
        {
            this._Configuration = configuration;
        }

        // GET api/provider?validDateTime=[DateTime String]
        [HttpGet]
        public IActionResult Get(string validDateTime)
        {
            if(String.IsNullOrEmpty(validDateTime))
            {
                return BadRequest(new { message = "validDateTime is required" });
            }

            if(this.DataMissing())
            {
                return NotFound();
            }

            DateTime parsedDateTime;

            try
            {
                parsedDateTime = DateTime.Parse(validDateTime);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "validDateTime is not a date or time" });
            }

            return new JsonResult(new {
                test = "NO",
                validDateTime = parsedDateTime.ToString("dd-MM-yyyy HH:mm:ss")
            });
        }

        private bool DataMissing()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), @"../../data");
            string pathWithFile = Path.Combine(path, "somedata.txt");

            return !System.IO.File.Exists(pathWithFile);
        }
    }

}