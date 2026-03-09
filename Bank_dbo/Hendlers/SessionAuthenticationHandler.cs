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
           
            if (Context.User.Identity?.IsAuthenticated == true)
            {
                return Task.FromResult(AuthenticateResult.Success(
                    new AuthenticationTicket(Context.User, Scheme.Name)));
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}