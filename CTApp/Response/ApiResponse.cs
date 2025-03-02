using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using CTDto.Users.Admin;
using CTDto.Users.Judge;
using CTDto.Users.Organizer;
using CTDto.Users.Player;
using CTDto.Card;
using System.Text.Json.Serialization;

namespace CTApp.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string StackTrace { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Token { get; set; }


        private ApiResponse(bool success, string message, T data = default, List<string> errors = null, string token = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
            Token = token;
        }



        public static ApiResponse<T> SuccessResponse(string message, T data = default)
            => new ApiResponse<T>(true, message, data);
    
        public static ApiResponse<List<T>> SuccessResponse(string message, IEnumerable<T> data)
            => new ApiResponse<List<T>>(true, message, new List<T>(data));

        public static ApiResponse<T> LoginResponse(string message, T data, string token)
            => new ApiResponse<T>(true, message, data, token: token);

        public static ApiResponse<T> ErrorResponse(string message)
            => new ApiResponse<T>(false, message, errors: new List<string> { message });

        public static ApiResponse<T> ErrorResponse(List<string> errors)
            => new ApiResponse<T>(false, "Ocurrió un error.", errors: errors);

        public static ApiResponse<T> ErrorResponse(string message, params string[] errors)
            => new ApiResponse<T>(false, message, errors: new List<string>(errors));

    }
}
