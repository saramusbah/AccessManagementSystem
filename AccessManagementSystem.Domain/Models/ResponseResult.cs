namespace AccessManagementSystem.Domain.Models
{
    public static class ResponseResult
    {
        public static ResponseResult<TData> SucceededWithData<TData>(TData result)
            where TData : class
        {
            return ResponseResult<TData>.SucceededWithData(result);
        }

        public static ResponseResult<object> Succeeded()
        {
            return new ResponseResult<object>();
        }

        public static ResponseResult<object> Failed(ErrorCode errorCode = ErrorCode.Error, params string[] errors)
        {
            return ResponseResult<object>.Failed(errorCode, errors);
        }

        public static ResponseResult<TData> Failed<TData>(ErrorCode errorCode = ErrorCode.Error, params string[] errors)
            where TData : class
        {
            return ResponseResult<TData>.Failed(errorCode, errors);
        }
    }
}
