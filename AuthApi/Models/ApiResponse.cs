using System.Net;

namespace AuthApi.Models
{
    public class ApiResponse
    {
        public object? Data { get; set; } = null;
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public List<string>? Errors { get; set; } = null;   

        public static ApiResponse Create(HttpStatusCode statusCode, object? data = default, bool success = false, string msg = "", List<string>? errors = null)
        {
            return new ApiResponse()
            {
                Data = data,
                Success = success,
                Message = msg,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}
