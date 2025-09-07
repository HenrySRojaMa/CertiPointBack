using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CertiPoint.Middlewares
{
    public static class TokenHandler
    {
        public static string GetToken(object user)
        {
            JwtSecurityTokenHandler jwth = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SecurityKey"));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        //new Claim(ClaimTypes.NameIdentifier, idUser),
                        //new Claim(ClaimTypes.Name, userName),
                        //new Claim(ClaimTypes.Locality, institution)
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = jwth.CreateToken(tokenDescriptor);
            return jwth.WriteToken(token);
        }
    }
}
