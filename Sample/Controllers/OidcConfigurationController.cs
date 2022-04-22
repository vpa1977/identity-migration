using Microsoft.AspNetCore.Mvc;

namespace Sample.Controllers;

public class OidcConfigurationController : Controller
{
    private readonly ILogger<OidcConfigurationController> _logger;

    public OidcConfigurationController(ILogger<OidcConfigurationController> logger)
    {
        _logger = logger;
    }

    [HttpGet("_configuration/{clientId}")]
    public IActionResult GetClientRequestParameters([FromRoute]string clientId)
    {
        throw new NotImplementedException();
    }
}
