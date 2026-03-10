using Bank.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Bank.API.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
          

            if (!string.IsNullOrEmpty(token))
            {
                var isValid = await sessionService.ValidateTokenAsync(token);
              

                if (isValid)
                {
                    var session = await sessionService.GetByTokenAsync(token);
                    if (session != null)
                    {
                        context.Items["UserId"] = session.UserId;
                        context.Items["SessionId"] = session.Id;
                        Console.WriteLine($"[Middleware] Set UserId: {session.UserId} in Items");
                    }
                }
            }

            await _next(context);
        }
    }
}