using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PiCloudDashboard.Models
{
    public class ResponseResult
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? Response { get; set; }
    }
}
