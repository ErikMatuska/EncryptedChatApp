using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EncryptedChatApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTime()
        {
            return Ok(TimeExtensions.GetCurrentTime());
        }
    }

    public class TimeExtensions
    {
        public static DateTime GetCurrentTime()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Europe Standard Time");
        }
    }
}
