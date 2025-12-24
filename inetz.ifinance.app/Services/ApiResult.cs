
using System.Net;


namespace inetz.ifinance.app.Services
{
    public sealed class ApiResult<T>
    {
        public bool IsSuccess { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public T? Data { get; init; }
        public string? Error { get; init; }

        public static ApiResult<T> Success ( T data, HttpStatusCode status )
            => new() { IsSuccess = true, Data = data, StatusCode = status };

        public static ApiResult<T> Failure ( HttpStatusCode status, string? error )
            => new() { IsSuccess = false, StatusCode = status, Error = error };
    }
}
