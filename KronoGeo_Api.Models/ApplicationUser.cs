using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<LocalisationGroup>? LocalisationGroups { get; set; }
    }
}
