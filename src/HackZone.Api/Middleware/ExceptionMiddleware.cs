using System.Net;
using System.Text.Json;
using HackZone.Application.Common.Exceptions;

namespace HackZone.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleAsync(ctx, ex);
        }
    }

    private static async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";
        object response;

        (ctx.Response.StatusCode, response) = ex switch
        {
            NotFoundException e => ((int)HttpStatusCode.NotFound, (object)new { error = e.Message }),
            AppValidationException e => ((int)HttpStatusCode.BadRequest, new { error = "Validatsiya xatosi.", errors = e.Errors }),
            UnauthorizedException e => ((int)HttpStatusCode.Unauthorized, new { error = e.Message }),
            ForbiddenException e => ((int)HttpStatusCode.Forbidden, new { error = e.Message }),
            ConflictException e => ((int)HttpStatusCode.Conflict, new { error = e.Message }),
            BusinessException e => ((int)HttpStatusCode.UnprocessableEntity, new { error = e.Message }),
            _ => ((int)HttpStatusCode.InternalServerError, new { error = "Server xatosi yuz berdi." })
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await ctx.Response.WriteAsync(json);
    }
}
