using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages
{
    [Authorize(Roles = "Admin,Manager")]
    public class GestionAccountBase : ComponentBase
    {
    }
}
