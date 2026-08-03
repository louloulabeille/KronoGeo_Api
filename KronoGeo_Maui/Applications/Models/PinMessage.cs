using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Models
{
    public class PinMessage
    {
        public required Pin Pin { get; set; }
        public required bool IsAdded { get; set; } = true;
    }
}
