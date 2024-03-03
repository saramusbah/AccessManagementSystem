namespace AccessManagementSystem.Domain.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class ResponseResult<TData>
        where TData : class
    {
        public ResponseResult()
        {
            ValidationErrors = new List<string>();
        }

        public TData Data { get; private set; }

        public string DeveloperMessage { get; private set; }

        public string ErrorMessage { get; private set; }

        public string ErrorCode { get; private set; }

        public List<string> ValidationErrors { get; private set; }

        [JsonIgnore]
        public bool IsSuccess => string.IsNullOrEmpty(ErrorCode);

        public static ResponseResult<TData> SucceededWithData(TData result)
        {
            return new ResponseResult<TData>
            {
                Data = result,
            };
        }

        public static ResponseResult<TData> Succeeded()
        {
            return new ResponseResult<TData>();
        }

        public static ResponseResult<TData> Failed(ErrorCode errorCode = Models.ErrorCode.Error, params string[] errors)
        {
            var result = new ResponseResult<TData>
            {
                ErrorCode = errorCode.ToString(),
                ErrorMessage = MapMessages(errorCode)
            };
            if (errors != null)
            {
                result.ValidationErrors.AddRange(errors);
            }

            return result;
        }

        private static string MapMessages(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case Models.ErrorCode.Error:
                    return "No error details available.";

                case Models.ErrorCode.ValidationError:
                    return "A validation error occurred.";

                case Models.ErrorCode.InvalidLoginError:
                    return "Invalid login error occurred.";

                case Models.ErrorCode.ExisitingAccountError:
                    return "This account already exists";

                case Models.ErrorCode.NotRegisteredRole:
                    return "This role is not registered";

                case Models.ErrorCode.NotRegisteredUser:
                    return "This user is not registered";

                case Models.ErrorCode.NotRegisteredDoor:
                    return "This door is not registered";
               
                case Models.ErrorCode.NoAccess:
                                return "No Access";

                default:
                    throw new NotImplementedException();
            }
        }
    }
}