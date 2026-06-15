using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace KronoGeo_Api.Infrastructure.Applications.Helpers
{
    public static class JsonOptions
    {
        public static JsonSerializerOptions GetJsonOptions()
        {
            // - options json ne prend pas la casse
            JsonSerializerOptions option = new()
            {
                PropertyNameCaseInsensitive = true,
            };

            return option;
        }
    }
}
