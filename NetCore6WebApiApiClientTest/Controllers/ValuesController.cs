using FBase.ApiClient;
using Microsoft.AspNetCore.Mvc;

namespace NetCore6WebApiApiClientTest.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
  private IHttpClientFactory _HttpClientFactory { get; set; }

  public ValuesController(IHttpClientFactory HttpClientFactory)
  {
    _HttpClientFactory = HttpClientFactory;
  }

  [HttpGet]
  public async Task<IActionResult> GetAsync()
  {
    var client = _HttpClientFactory.CreateClient();

    var res = await HttpCalls.GetAsync<object, object>(
      client,
      "https://jsonplaceholder.typicode.com/todos/1");

    return Ok(res.Body);
  }
}
