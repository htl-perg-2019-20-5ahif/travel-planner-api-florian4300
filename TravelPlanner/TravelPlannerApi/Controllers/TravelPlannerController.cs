using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelPlanner.Logic;

namespace TravelPlannerApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class TravelPlannerController : ControllerBase
    {

        private readonly ILogger<TravelPlannerController> _logger;
        private readonly HttpClient client;

        public TravelPlannerController(ILogger<TravelPlannerController> logger, IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            _logger = logger;
        }

        [HttpGet]
        [Route("travelPlan")]
        public async Task<ActionResult> Get([FromQuery] string from, [FromQuery] string to, [FromQuery] string start)
        {
            var routesResponse = await client.GetAsync("https://cddataexchange.blob.core.windows.net/data-exchange/htl-homework/travelPlan.json");
            routesResponse.EnsureSuccessStatusCode();
            var json = await routesResponse.Content.ReadAsStringAsync();
            var routes = JsonSerializer.Deserialize<IEnumerable<Route>>(json);
            ConnectionFinder cf = new ConnectionFinder(routes);
            var trips = cf.FindConnection(from, to, start);
            if(trips == null)
            {
                return NotFound();
            }
            return Ok(trips);
        }
    }
}
