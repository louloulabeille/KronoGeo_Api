using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class PhotoDTO
    {
        public string? PathPhoto { get; set; } = default;
        public string? Name { get; set; } = default;
        public string? PathComplet
        {
            get
            {
                if (PathPhoto is not null && Name is not null)
                    return Path.Combine(PathPhoto, Name);
                else
                    return string.Empty;
            }
        }
    }
}
