using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.Service.Controllers
{
    public class VersionController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/version")]
        public ActionResult<string> GetVersion()
         => Ok(typeof(VersionController).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
    }
}
