using InventoryManagement.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.WebCore.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BitLockerController : ControllerBase
{
    [HttpPost]
    public ActionResult<string> Submit([FromBody] ComputerInformation computerInformation)
    {
        return Ok("Todo...");
    }
}
