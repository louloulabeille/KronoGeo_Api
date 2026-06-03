using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace KronoGeo_Api.Infrastructure.Test
{
    public static class EmailTest
    {
        /// <summary>
        /// method to validate email format avec les Regex
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            // - trouver sur internet
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }
    }
}
