using System;

namespace XPenC.WebApp.Models.Shared
{
    public class ErrorData
    {
        public string RequestId { get; set; }
        public bool ShowException { get; set; }
        public Exception Exception { get; set; }
    }
}