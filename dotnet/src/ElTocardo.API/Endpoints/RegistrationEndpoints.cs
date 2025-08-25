using ElTocardo.API.Options;
using ElTocardo.Application.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace ElTocardo.API.Endpoints;

public static class RegistrationEndpoints
{
    private static string Tags => "Registration";

    public static IEndpointRouteBuilder MapRegistrationEndpoints(this IEndpointRouteBuilder app)
  {
        app.MapPost("v1/users/register", async (
            [FromServices] IUserEndpointService userEndpointService,
            [FromBody] RegisterUserRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await userEndpointService.RegisterUserAsync(request.Username, request.Email, request.Password, cancellationToken);
            return result.IsSuccess
                ? Results.Ok("User registered")
                : Results.Conflict(new ProblemDetails { Title = "Registration failed", Detail = result.ReadError().Message });
        })
        .WithName("RegisterUser")
        .WithTags(Tags)
        .WithOpenApi();

        app.MapPost("v1/users/reset-password", async (
            [FromServices] IUserEndpointService userEndpointService,
            [FromBody] ResetPasswordRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await userEndpointService.InitiatePasswordResetAsync(request.Email, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(new { Token = result.ReadValue() }) // In production, do not return token directly
                : Results.BadRequest(new ProblemDetails { Title = "Reset password failed", Detail = result.ReadError().Message });
        })
        .WithName("InitiateResetPassword")
        .WithTags(Tags)
        .WithOpenApi();

        app.MapPost("v1/users/reset-password/confirm", async (
            [FromServices] IUserEndpointService userEndpointService,
            [FromBody] ConfirmResetPasswordRequest request,
            CancellationToken cancellationToken) =>
        {
            var result = await userEndpointService.ConfirmPasswordResetAsync(request.Email, request.Token, request.NewPassword, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(new { Message = "Password reset successful" })
                : Results.BadRequest(new ProblemDetails { Title = "Password reset failed", Detail = result.ReadError().Message });
        })
        .WithName("ConfirmResetPassword")
        .WithTags(Tags)
        .WithOpenApi();

        app.MapDelete("v1/users/unregister", async (
            [FromServices] IUserEndpointService userEndpointService,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            CancellationToken cancellationToken) =>
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var result = await userEndpointService.UnregisterUserAsync(user, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new ProblemDetails { Title = "Unregistration failed", Detail = result.ReadError().Message });
        })
        .WithName("UnregisterUser")
        .RequireAuthorization()
        .WithTags(Tags)
        .WithOpenApi();

        return app;
    }

    /// <summary>
    /// Request DTO for user registration.
    /// </summary>
    public record RegisterUserRequest(string Username, string Email, string Password);

    /// <summary>
    /// Request DTO for user login.
    /// </summary>
    public record LoginRequest(string Username, string Password);

    /// <summary>
    /// Request DTO for initiating password reset.
    /// </summary>
    public record ResetPasswordRequest(string Email);

    /// <summary>
    /// Request DTO for confirming password reset.
    /// </summary>
    public record ConfirmResetPasswordRequest(string Email, string Token, string NewPassword);
}
