using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Email
{
    public enum EmailResultStatus
    {
        Success,
        Failure
    }

    public class EmailResult
    {
        public required string To { get; set; }
        public required EmailResultStatus Status { get; set; } = EmailResultStatus.Success;
        public required string Message { get; set; } = "Email sent successfully.";
        public Exception? Exception { get; set; }
    }
}
