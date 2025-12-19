using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MinhaPrimeiraApi.Controllers.ApiV1;

[Route("api/version1")]
[Route("api/v{version:apiVersion}/version1")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
public class ApiTesteV1Controller
{
    [HttpGet]
    public string GetVersion()
    {
        return "Api Teste Get v1";
    }
}