using GroundhogWeb.Models;
using Microsoft.AspNetCore.Mvc;
using GroundhogWeb.Repositories;

namespace GroundhogWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GuildsController : ControllerBase
{
    private readonly GuildsRepository _guildsRepo;

    public GuildsController(GuildsRepository guildsRepository) =>
        _guildsRepo = guildsRepository;

    [HttpGet]
    public async Task<List<Guild>> Get() =>
        await _guildsRepo.GetAllAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Guild>> Get(string id)
    {
        var guild = await _guildsRepo.GetByIdAsync(id);

        if (guild is null)
        {
            return NotFound();
        }

        return guild;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guild newGuild)
    {
        await _guildsRepo.CreateAsync(newGuild);

        return CreatedAtAction(nameof(Get), new { id = newGuild.Id }, newGuild);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Guild updatedGuild)
    {
        var guild = await _guildsRepo.GetByIdAsync(id);

        if (guild is null)
        {
            return NotFound();
        }

        updatedGuild.Id = guild.Id;

        await _guildsRepo.UpdateAsync(id, updatedGuild);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var guild = await _guildsRepo.GetByIdAsync(id);

        if (guild is null)
        {
            return NotFound();
        }

        await _guildsRepo.RemoveAsync(id);

        return NoContent();
    }
}