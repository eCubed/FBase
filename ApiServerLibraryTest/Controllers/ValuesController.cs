using ApiServerLibraryTest.Models;
using FBase.ApiServer;
using FBase.Foundations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServerLibraryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IApiClientHasher ApiClientHasher { get; set; }

        public ValuesController(IApiClientHasher apiClientHasher)
        {
            ApiClientHasher = apiClientHasher;
        }

        // GET: api/<ValuesController>
        [HttpGet("showheaders")]
        public IActionResult Get()
        {
            return Ok(new
            {
                xApiKey = "TEST-CLIENT-1000",
                xData = "12345-678910-1112",
                xHash = ApiClientHasher.GenerateHash(
                    apiKey: "TEST-CLIENT-1000",
                    secret: "TestSecret-12345",
                    arbitraryInput: "12345-678910-1112")
            });
        }

        // GET api/<ValuesController>/5
        [Authorize(Policy = "RequiresClientIdClaim")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("managerresult/{n}")]
        public IActionResult GetManagerResult(int n)
        {
            List<string> errors = new List<string>();
            errors.Add((n % 2 == 0) ? ManagerErrors.Unauthorized : "Other");

            ManagerResult<ApiClient> managerResult = new ManagerResult<ApiClient>(errors.ToArray());

            return this.DiscernErrorActionResult(managerResult);

        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
