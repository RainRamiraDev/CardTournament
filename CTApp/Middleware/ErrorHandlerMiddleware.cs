using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTApp.Response;

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
                // Continúa el pipeline de la aplicación
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción y maneja el error
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var response = ApiResponse<object>.ErrorResponse(
                new List<string> { "Ocurrió un error inesperado." },
                exception.StackTrace
            );

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
