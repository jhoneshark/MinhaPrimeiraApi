using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MinhaPrimeiraApi.Controllers.ApiV2;

[Route("api/version2")]
[Route("api/v{version:apiVersion}/version2")]
[ApiController]
[ApiVersion("2.0")]
public class ApiTesteV2Controller
{
    [HttpGet]
    public string GetVersion()
    {
        return "Api Teste Get v2";
    }
}