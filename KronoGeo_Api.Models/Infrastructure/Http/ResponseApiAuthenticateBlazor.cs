using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public class ResponseApiAuthenticateBlazor : ResponseApiAuthenticate
    {
        public AuthenticationProperties? AuthenticationProperties { get; set; }
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }
    }
}
