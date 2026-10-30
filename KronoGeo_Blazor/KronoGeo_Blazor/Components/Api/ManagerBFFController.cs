using KronoGeo_Api.Infrastructure.Exceptions.Http;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KronoGeo_Blazor.Components.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ManagerBFFController (IMediator mediaR
        , ILogger<AuthBFFController> logger) : Controller
    {
        #region private readonly properties
        private readonly IMediator _mediaR = mediaR;
        private readonly ILogger<AuthBFFController> _logger = logger;
        #endregion

        [Authorize(Roles ="Admin,Manager")]
        [HttpGet("ListUsers")]
        public async Task<IActionResult> ListUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                var result = await _mediaR.Send(new());

                return Ok(result);
            }
            catch (SessionExpiredException)
            {
                return this.Ok(new ResponseApiManager() { ApiStatus = EnumApiStatus.Unauthorized });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la remontée de la liste des utilisateurs");
                return this.Ok(new ResponseApiManager()
                { 
                    ApiStatus = EnumApiStatus.Problem, 
                    Message = "Problème lors de la remontée de la liste des utilisateurs"
                });
            }
        }


    }
}
