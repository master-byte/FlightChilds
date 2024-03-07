using System;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Kidstarter.Api.Configuration
{
    public sealed class AuthSettings
    {
        private string secretKey;

        public string SecretKey
        {
            get => this.secretKey;
            set
            {
                this.SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(value));
                this.SigningCredentials = new SigningCredentials(this.SecurityKey, SecurityAlgorithms.HmacSha256);
                this.secretKey = value;
            }
        }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public DateTime Expiration => this.IssuedAt.Add(this.ValidFor);

        public DateTime NotBefore => DateTime.UtcNow;

        public DateTime IssuedAt => DateTime.UtcNow;

        public TimeSpan ValidFor { get; set; }

        public SymmetricSecurityKey SecurityKey { get; private set; }

        public SigningCredentials SigningCredentials { get; private set; }
    }
}