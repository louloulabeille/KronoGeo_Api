using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages.Account.Manager
{
    [Authorize(Roles = "Admin,Manager")]
    public class GestionAccountBase : ComponentBase
    {

    }
}
