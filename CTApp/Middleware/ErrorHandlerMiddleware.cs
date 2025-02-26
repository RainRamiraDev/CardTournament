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
                ArgumentException argEx => (int)HttpStatusCode.BadRequest,
                InvalidOperationException => (int)HttpStatusCode.Conflict,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            ApiResponse<object> response;

            // Usa el mensaje específico de la ArgumentException
            if (exception is ArgumentException argumentException)
            {
                response = ApiResponse<object>.ErrorResponse(argumentException.Message);
                response.Errors.Add(argumentException.GetType().Name); // Agrega el tipo de error a la lista de errores
            }
            else
            {
                response = ApiResponse<object>.ErrorResponse("Ocurrió un error.");
                response.Errors.Add(exception.GetType().Name); // Agrega el tipo de error a la lista de errores
            }

            return context.Response.WriteAsJsonAsync(response);
        }



    }
}
