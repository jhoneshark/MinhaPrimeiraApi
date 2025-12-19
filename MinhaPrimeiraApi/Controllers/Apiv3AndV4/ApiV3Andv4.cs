using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace MinhaPrimeiraApi.Controllers.Apiv3AndV4;

[Route("api/testesVersioning")]
[ApiController]
[ApiVersion(3)]
[ApiVersion(4)]
public class ApiV3Andv4
{
    [MapToApiVersion(3)]
    [HttpGet]
    public string GetVersion3()
    {
        http://localhost:5041/api/testesVersioning?api-version=3
        return "V3AndV4 API";
    }

    [MapToApiVersion(4)]
    [HttpGet]
    public string GetVersion4()
    {
        //http://localhost:5041/api/testesVersioning?api-version=4
        return "V4AndV4 API";
    }
}