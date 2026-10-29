using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace KronoGeo_Api.Infrastructure.Applications.Cookies
{
    public static class HttpOnlyCookie
    {
        /// <summary>
        /// Méthod qui construit le cookie pour la sécurisation du coté client de blazor
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public static (ClaimsPrincipal, AuthenticationProperties) BuildCookie
            ( string token, string id , string login )
        {
            // -- mise en place de httponly cookie 
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // -- extraction du role pour l'utilisateur
            var roles = jwtToken.Claims.Where(r => r.Type == "role").ToList();

            // -- création des claims
            var claims = new List<Claim>
                        {
                            new (ClaimTypes.NameIdentifier, id),
                            new (ClaimTypes.Name, login )
                        };

            // -- ajout du ou des roles dans le cookie
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Value));
            }

            var claimsIdentity = new ClaimsIdentity(claims
                , CookieAuthenticationDefaults.AuthenticationScheme);

            // -- ajout dans le cookie le token jwt
            var authProperties = new AuthenticationProperties();
            authProperties.StoreTokens(
            [
                new AuthenticationToken { Name = "jwt_token", Value = token}
            ]);

            //-- retourner les informations pour les emettre le cookie vers le navigateur
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return (claimsPrincipal, authProperties);
        }
    }
}
