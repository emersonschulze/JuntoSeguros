using Junto.Infra.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace Junto.Infra.Security
{
    public class AccessCredentials
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string GrantType { get; set; }
    }

    public static class Roles
    {
        public const string ROLE_API_JUNTO = "Acesso-API-JUNTO";
    }

    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Minutes { get; set; }
        public int FinalExpiration { get; set; }
    }

    public class Token
    {
        public bool Authenticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }

    public class RefreshTokenData
    {
        public string RefreshToken { get; set; }
        public string UserID { get; set; }
    }
}