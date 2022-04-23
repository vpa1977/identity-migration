using Microsoft.AspNetCore.Mvc;
using Sample.Hydra;

namespace Sample.Controllers;

public class OidcConfigurationController : Controller
{
    private readonly ILogger<OidcConfigurationController> _logger;
    private readonly OIDCConfigurationService _service;

    public OidcConfigurationController(ILogger<OidcConfigurationController> logger, OIDCConfigurationService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("_configuration/{clientId}")]
    public IActionResult GetClientRequestParameters([FromRoute]string clientId)
    {
        return Ok(_service.GetClientParameters(HttpContext, clientId));
    }
}
