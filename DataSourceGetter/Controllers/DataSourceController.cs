using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataSourceGetter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataSourceController : ControllerBase
    {

        private readonly ILogger<DataSourceController> _logger;
        private readonly DataSourceGetterService _srv;

        public DataSourceController(ILogger<DataSourceController> logger, DataSourceGetterService srv)
        {
            _logger = logger;
            _srv = srv;
        }

        [HttpGet("{filename}")]
        public string[] Get(string filename)
        {
            _logger.LogError("get called");
            return _srv.GetRowData(filename).Split(';');
        }
    }
}
