using MatchMaking.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaking.Service.API.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Produces("application/json")]
public class MatchMakingController
    (IMatchMakingService matchMakingService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task Search(
        [FromBody] Guid userId,
        CancellationToken cancellationToken) =>
        matchMakingService.Request(userId, cancellationToken);

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatch(
        [FromQuery] Guid userId)
    {
        var match = await matchMakingService.GetMatchForUser(userId);

        if (match is null)
        {
            return NotFound();
        }

        return Ok(match);
    }
}