using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StockApi.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }
        public List<string>? Errors { get; private set; }

        private ApiResponse() { }

        public static ApiResponse<T>  SuccessResponse(T data,string message = "Request succcessful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> FailureResponse(List<string> errors,string message = "Request failed")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }
    }
}
