using Microsoft.AspNetCore.Mvc;

namespace ByteEngageERP.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    // GET
    [HttpGet]
    public string Index()
    {
        return "This IS Home Page";
    }
}