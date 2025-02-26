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
                ArgumentException => (int)HttpStatusCode.BadRequest,
                InvalidOperationException => (int)HttpStatusCode.Conflict,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            ApiResponse<object> response;

            // Usa el mensaje específico de la ArgumentException
            if (exception is ArgumentException argEx)
            {
                response = ApiResponse<object>.ErrorResponse(argEx.Message);
            }
            else
            {
                response = ApiResponse<object>.ErrorResponse("Ocurrió un error.");
            }

            // Agregar solo el tipo de error a la lista de errores
            response.Errors.Clear(); // Limpiar la lista de errores antes de agregar
            response.Errors.Add(exception.GetType().Name); // Agrega el tipo de error a la lista

            return context.Response.WriteAsJsonAsync(response);
        }









    }
}
