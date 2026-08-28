using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Xml.Linq;

namespace KronoGeo_Blazor.Components.Api
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    [ApiController]
    public class AuthBFFController(  IMediator mediaR
        , IMemoryCache memoryCache, ILogger<AuthBFFController> logger ) : Controller
    {
        #region private readonly properties
        private readonly IMediator _mediaR = mediaR;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<AuthBFFController> _logger = logger;
        #endregion

        /// <summary>
        /// Bff Backends for Frontends pour mettre en place au niveau
        /// de blazor client pour l'authentification
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RegisterDTO login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = ModelState.ToString() });
                }

                // -- requete vers l'APi
                var result = await _mediaR.Send(new LoginUserCommand() { Register = login });

                if (result.IsSuccess)
                {
                    if (!string.IsNullOrEmpty(result.Register?.Token))
                    {
                        var sessionId = Guid.NewGuid().ToString();
                        // -- enregistrement dans le cache du serveur le token 
                        _memoryCache?.Set($"jwt_{sessionId}", result.Register?.Token, TimeSpan.FromHours(12));
                        // 4. Émettre le Cookie d'authentification vers le navigateur
                        var claims = new List<Claim>
                        {
                            new ("Id", result.Register!.Id),
                            new ("BffSessionId", sessionId) // Clé pour retrouver le JWT plus tard
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "BffCookie");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Cette ligne émet le Cookie HttpOnly de façon transparente !
                        await this.HttpContext.SignInAsync("BffCookie", claimsPrincipal);

                        result.Register.Token = string.Empty;

                        return Ok(result);
                    }
                }
                return Ok(result);

            }
            catch( Exception ex)
            {
                _logger.LogError(ex, "Erreur interne {message}", ex.Message);
                return Ok(
                    new ResponseApiAuthenticate()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = "Erreur interne, une exception a été levé."
                    });
            }
            
        }
    }
}
