using InventoryManagement.Shared.Models;
using InventoryManagement.WebCore.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.WebCore.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HostController : ControllerBase
{
    private readonly PostgresqlDataContext _context;

    public HostController(PostgresqlDataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Submit([FromBody] HostInformation hostInformation)
    {
        var result = await _context.HostInformation.FirstOrDefaultAsync(host => host.Guid == hostInformation.Guid);

        if (result is null) _context.HostInformation.Add(hostInformation);
        else result = hostInformation;

        await _context.SaveChangesAsync();

        return Ok($"{hostInformation.Guid}:{hostInformation.MachineName} added or updated");
    }

    [HttpGet, Authorize]
    public async Task<ActionResult<List<HostInformation>>> GetAllHosts()
    {
        return Ok(await _context.HostInformation.ToListAsync());
    }
}
