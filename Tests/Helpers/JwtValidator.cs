using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Tests.Helpers
{
    public class JwtValidator
    {
        public static string Validate(string token, string securityKey, string issuer)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityToken validatedToken;
            var validator = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidIssuer = issuer;
            validationParameters.IssuerSigningKey = key;
            validationParameters.ValidateIssuerSigningKey = true;
            validationParameters.ValidateAudience = false;
            validationParameters.ValidateIssuer = true;

            if (validator.CanReadToken(token))
            {
                ClaimsPrincipal principal;
                try
                {
                    principal = validator.ValidateToken(token, validationParameters, out validatedToken);
                    if (principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    {
                        return principal.Claims.Where(c => c.Type == ClaimTypes.Email).First().Value;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return String.Empty;
        }
    }
}