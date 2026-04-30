public class ApiResponse<T>
{
    public bool Success{get;set;}
    public T? Data {get;set;}
    public string? Message {get;set;}
     public List<string>? Errors {get;set;}
     public string? TraceId {get;set;}

     public static ApiResponse<T> SuccessResponse(T? data, string? traceId, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            TraceId = traceId
        };
    }

    public static ApiResponse<T> FailResponse(string message, List<string>? errors, string? traceId)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            TraceId = traceId
        };
    }

}
