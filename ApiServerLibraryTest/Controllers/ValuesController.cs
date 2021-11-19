using ApiServerLibraryTest.Data;
using ApiServerLibraryTest.Models;
using FBase.ApiServer;
using FBase.ApiServer.OAuth;
using FBase.Cryptography;
using FBase.Foundations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiServerLibraryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private UserManager<TestUser> UserManager { get; set; }

        public ValuesController(UserManager<TestUser> userManager)
        {
            UserManager = userManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string challenge = PckeUtils.GenerateCodeChallengeFromValidCodeVerifier("abcdefg");

            return Ok(challenge);
        }
        
        [HttpGet("protected")]
        [Authorize(Roles = "subscriber")]
        public async Task<IActionResult> GetProtected()
        {
            AuthenticatedInfo<int> authenticatedInfo = await this.ResolveAuthenticatedEntitiesAsync<TestUser, int>(UserManager);

            return Ok(authenticatedInfo);
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
