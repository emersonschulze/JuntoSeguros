using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Junto.Dominio;

namespace Junto.Infra.Security
{
    public class AccessManager
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private SigningConfigurations _signingConfigurations;
        private TokenConfigurations _tokenConfigurations;


        public AccessManager(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;

        }

        public bool ValidateCredentials(AccessCredentials credenciais)
        {
            bool credenciaisValidas = false;
            if (credenciais != null && !String.IsNullOrWhiteSpace(credenciais.UserID))
            {
                if (credenciais.GrantType == "password")
                {
                    var userIdentity = _userManager.FindByNameAsync(credenciais.UserID).Result;
                    if (userIdentity != null)
                    {
                        var resultadoLogin = _signInManager
                            .CheckPasswordSignInAsync(userIdentity, credenciais.Password, false)
                            .Result;
                        if (resultadoLogin.Succeeded)
                        {
                            credenciaisValidas = _userManager.IsInRoleAsync(
                                userIdentity, Roles.ROLE_API_JUNTO).Result;
                        }
                    }
                }
                else if (credenciais.GrantType == "refresh_token")
                {
                    if (!String.IsNullOrWhiteSpace(credenciais.RefreshToken))
                    {
                        RefreshTokenData refreshTokenBase = null;

                        string strTokenArmazenado = credenciais.RefreshToken;
                        if (!String.IsNullOrWhiteSpace(strTokenArmazenado))
                        {
                            refreshTokenBase = JsonConvert.DeserializeObject<RefreshTokenData>(strTokenArmazenado);
                        }

                        credenciaisValidas = (refreshTokenBase != null &&
                            credenciais.UserID == refreshTokenBase.UserID &&
                            credenciais.RefreshToken == refreshTokenBase.RefreshToken);

                        if (credenciaisValidas)
                            credenciais.RefreshToken = "";
                    }
                }
            }

            return credenciaisValidas;
        }

        public Token GenerateToken(AccessCredentials credenciais)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(credenciais.UserID, "Login"),
                new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, credenciais.UserID)
                }
            );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao + TimeSpan.FromMinutes(_tokenConfigurations.Minutes);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            var resultado = new Token()
            {
                Authenticated = true,
                Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty),
                Message = "OK"
            };

            
            var refreshTokenData = new RefreshTokenData();
            refreshTokenData.RefreshToken = resultado.RefreshToken;
            refreshTokenData.UserID = credenciais.UserID;

            resultado.RefreshToken = JsonConvert.SerializeObject(refreshTokenData);

            return resultado;
        }
    }
}