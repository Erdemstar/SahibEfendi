using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SahibEfendi.Handler
{
    public class UserHandler
    {
        public static string FindUserIDFromJWTToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var JWTClaims = handler.ReadJwtToken(token).Claims;
                var userId = JWTClaims.FirstOrDefault(c => c.Type == "nameid").Value;

                return userId;
            }
            catch
            {
                return null;
            }

        }
    }
}
