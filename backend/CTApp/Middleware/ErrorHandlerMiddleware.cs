using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTApp.Response;
using System.Net;
using System.Text.Json;

namespace CTApp.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest, // 400
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
                KeyNotFoundException => (int)HttpStatusCode.NotFound, // 404
                InvalidOperationException => (int)HttpStatusCode.Conflict, // 409
                TimeoutException => (int)HttpStatusCode.RequestTimeout, // 408
                NotImplementedException => (int)HttpStatusCode.NotImplemented, // 501
                _ => (int)HttpStatusCode.InternalServerError // 500
            };

            context.Response.StatusCode = statusCode;

            var responseMessage = exception switch
            {
                ArgumentException argEx => argEx.Message, 
                UnauthorizedAccessException unauthEx => unauthEx.Message,
                KeyNotFoundException keyNotFoundEx => keyNotFoundEx.Message, 
                InvalidOperationException InvalOpEx => InvalOpEx.Message,  
                TimeoutException => "El tiempo de espera para la solicitud ha expirado.",
                NotImplementedException => "Esta funcionalidad aún no está implementada.",
                _ => exception.Message
            };

            if (string.IsNullOrWhiteSpace(responseMessage))
            {
                responseMessage = "Ocurrió un error inesperado en el servidor.";
            }

            var response = ApiResponse<object>.ErrorResponse(responseMessage);

            response.Errors.Clear();
            response.Errors.Add(exception.GetType().Name);

            return context.Response.WriteAsJsonAsync(response);
        }


    }
}
