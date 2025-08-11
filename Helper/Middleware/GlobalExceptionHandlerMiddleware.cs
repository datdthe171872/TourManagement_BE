using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourManagement_BE.Helper.Exceptions;

namespace TourManagement_BE.Helper.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = GetUserFriendlyMessage(exception),
                error = GetErrorCode(exception),
                type = GetErrorType(exception),
                timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = GetStatusCode(exception);

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
            return exception switch
            {
                PaymentDueDateException => exception.Message,
                InsufficientSlotsException => exception.Message,
                TourNotFoundException => exception.Message,
                DepartureDateNotFoundException => exception.Message,
                BookingException => exception.Message,
                _ => "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau."
            };
        }

        private static string GetErrorCode(Exception exception)
        {
            return exception switch
            {
                PaymentDueDateException ex => ex.ErrorCode,
                InsufficientSlotsException ex => ex.ErrorCode,
                TourNotFoundException ex => ex.ErrorCode,
                DepartureDateNotFoundException ex => ex.ErrorCode,
                BookingException ex => ex.ErrorCode,
                _ => "UNKNOWN_ERROR"
            };
        }

        private static string GetErrorType(Exception exception)
        {
            return exception switch
            {
                PaymentDueDateException => "PaymentDueDateError",
                InsufficientSlotsException => "InsufficientSlotsError",
                TourNotFoundException => "TourNotFoundError",
                DepartureDateNotFoundException => "DepartureDateNotFoundError",
                BookingException => "BookingError",
                _ => "SystemError"
            };
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                TourNotFoundException => (int)HttpStatusCode.NotFound,
                DepartureDateNotFoundException => (int)HttpStatusCode.NotFound,
                PaymentDueDateException => (int)HttpStatusCode.BadRequest,
                InsufficientSlotsException => (int)HttpStatusCode.BadRequest,
                BookingException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
