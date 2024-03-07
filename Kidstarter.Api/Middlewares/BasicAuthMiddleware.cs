using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Kidstarter.Api.Middlewares
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IConfiguration configuration;

        public BasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];

                if (this.IsAuthorized(username, password))
                {
                    await this.next.Invoke(context);
                    return;
                }
            }

            context.Response.Headers["WWW-Authenticate"] = "Basic";

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        public bool IsAuthorized(string username, string password)
        {
            return username.Equals(this.configuration["Prometheus:UserName"], StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals(this.configuration["Prometheus:Password"]);
        }
    }
}