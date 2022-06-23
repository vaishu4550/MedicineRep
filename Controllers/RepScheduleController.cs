using log4net.Config;
using MedicalRepresentativeSchedule.models;
using MedicalRepresentativeSchedule.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Castle.Core.Internal;

namespace MedicalRepresentativeSchedule.Controllers
{
   
    [Route("[controller]")]
    [ApiController]
    public class RepScheduleController : ControllerBase
    {
        private readonly IRepScheduleProvider _repScheduleProvider;
       

        //static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RepScheduleController));
        public RepScheduleController(IRepScheduleProvider repSchedule)
        {
            this._repScheduleProvider = repSchedule;
        }
        [HttpGet("detail")]
        public string[] detail()
        {
            var u = new string[] { "a", "b", "c" };
            return u;
        }
        [HttpGet]
        public async Task<IActionResult> Get(DateTime startDate)
        {
            try
            {
                var res = await _repScheduleProvider.GetRepScheduleAsync(startDate);
                if (!res.IsNullOrEmpty())
                {
                  
                    return new OkObjectResult(res);
                }
                else
                {
                   
                    return NotFound("schedule not received");
                }
            }
            catch (Exception e)
            {
               
                return StatusCode(500);
            }
        }
    }
}
