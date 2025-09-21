using System.Net;
using System.Text.Json;
using TourManagement_BE.Helper.Exceptions;

namespace TourManagement_BE.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                // Log unexpected errors here if needed
                await HandleSystemExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, BusinessException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var result = JsonSerializer.Serialize(new
            {
                Message = exception.Message,
                StatusCode = context.Response.StatusCode
            });

            return context.Response.WriteAsync(result);
        }

        private static Task HandleSystemExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                Message = "Có lỗi xảy ra trong quá trình xử lý. Vui lòng thử lại sau.",
                StatusCode = context.Response.StatusCode
            });

            return context.Response.WriteAsync(result);
        }
    }

    // Extension method to add the middleware to IApplicationBuilder
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}