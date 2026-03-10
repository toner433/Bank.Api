using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;

namespace Bank.API.Handlers
{
    public class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public SessionAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
           

            if (Context.Items.ContainsKey("UserId"))
            {
                var userId = Context.Items["UserId"]?.ToString();
                

                if (!string.IsNullOrEmpty(userId))
                {
                    var claims = new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                   
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

           
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}