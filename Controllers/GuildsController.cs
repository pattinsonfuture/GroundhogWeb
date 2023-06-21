using GroundhogWeb.Models;
using GroundhogWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace GroundhogWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuildsController : ControllerBase
{
    private readonly GuildsService _guildsService;

    public GuildsController(GuildsService guildsService) =>
        _guildsService = guildsService;

    [HttpGet]
    public async Task<List<Guild>> Get() =>
        await _guildsService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Guild>> Get(string id)
    {
        var guild = await _guildsService.GetAsync(id);

        if (guild is null)
        {
            return NotFound();
        }

        return guild;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guild newGuild)
    {
        await _guildsService.CreateAsync(newGuild);

        return CreatedAtAction(nameof(Get), new { id = newGuild.Id }, newGuild);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Guild updatedGuild)
    {
        var guild = await _guildsService.GetAsync(id);

        if (guild is null)
        {
            return NotFound();
        }

        updatedGuild.Id = guild.Id;

        await _guildsService.UpdateAsync(id, updatedGuild);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var guild = await _guildsService.GetAsync(id);

        if (guild is null)
        {
            return NotFound();
        }

        await _guildsService.RemoveAsync(id);

        return NoContent();
    }
}