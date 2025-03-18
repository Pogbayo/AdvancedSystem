using Microsoft.AspNetCore.Mvc;

namespace StockApi.Response
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected ActionResult<ApiResponse<T>> Success<T>(T data , string message )
        {
            return Ok(ApiResponse<T>.SuccessResponse(data, message));
        }
        protected ActionResult<ApiResponse<T>> Failure<T>(List<string> errors, string message )
        {
            return BadRequest(ApiResponse<T>.FailureResponse(errors, message));
        }

        protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(List<string> errors, string message)
        {
            return NotFound(ApiResponse<T>.FailureResponse(errors, message));
        }
        protected ActionResult<ApiResponse<T>> UnAuthorizedResponse<T>(List<string> errors, string message)
        {
            return Unauthorized(ApiResponse<T>.FailureResponse(errors, message));
        }
    }
}
