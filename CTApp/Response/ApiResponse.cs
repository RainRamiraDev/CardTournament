using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

using CTDto.Users.Admin;
using CTDto.Users.Judge;
using CTDto.Users.Organizer;
using CTDto.Users.Player;
using CTDto.Card;

namespace CTApp.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public string StackTrace { get; set; }

        public string Token { get; set; }


    public ApiResponse(bool success, string message)
    {
        Success = success;
        Message = message;
        Errors = new List<string>();
    }

    public ApiResponse(bool success, string message, T data, string token = null) : this(success, message)
    {
        Data = data;
        Token = token;
    }

    public ApiResponse(bool success, List<string> errors, string stackTrace = null)
    {
        Success = success;
        Errors = errors;
        Message = success ? "Operación exitosa." : "Ocurrió un error.";
        StackTrace = stackTrace;
    }

    public static ApiResponse<T> SuccessResponse(string message)
    {
        return new ApiResponse<T>(true, message);
    }

    public static ApiResponse<T> SuccessResponse(string message, T data, string token = null)
    {
        return new ApiResponse<T>(true, message, data, token);
    }

    public static ApiResponse<T> ErrorResponse(string message, string stackTrace = null)
    {
        return new ApiResponse<T>(false, new List<string> { message }, stackTrace);
    }

    public static ApiResponse<T> ErrorResponse(List<string> errors, string stackTrace = null)
    {
        return new ApiResponse<T>(false, errors, stackTrace);
    }

    public static ApiResponse<T> SuccessResponse(string message, IEnumerable<CardDto> cards)
    {
        return new ApiResponse<T>(true, message, (T)(object)cards);
    }

    internal static object SuccessResponse(string message, IEnumerable<PlayerDto> players)
    {
        return new ApiResponse<T>(true, message, (T)(object)players);
    }

    internal static object SuccessResponse(string message, IEnumerable<JudgeDto> judges)
    {
        return new ApiResponse<T>(true, message, (T)(object)judges);
    }

    internal static object SuccessResponse(string message, IEnumerable<OrganizerDto> organizers)
    {
        return new ApiResponse<T>(true, message, (T)(object)organizers);
    }

    internal static object SuccessResponse(string message, IEnumerable<AdminDto> admins)
    {
        return new ApiResponse<T>(true, message, (T)(object)admins);
    }

    }
}
