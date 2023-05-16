using AttendanceWithQrCodes.Models;
using AttendanceWithQrCodes.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AttendanceWithQrCodes.HelperMethods
{
    public class JwtHelper : IJwtHelper
    {
        private readonly JwtOptions _jwtOptions;
        private readonly SymmetricSecurityKey _securityKey;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtHelper(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = _securityKey
            };
        }

        /// <summary>
        /// Generates Jwt token when user wants to log in.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JwtSecurityToken</returns>
        public string GenerateLoginToken(User user)
        {
            SigningCredentials credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]
            {
                new(ClaimTypes.UserData, Convert.ToString(user.Id)),
                new(ClaimTypes.Role, user.Role.Name)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validates user's Jwt token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>User id</returns>
        public int ValidateLoginToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            ClaimsPrincipal claimsPrincipal = handler.ValidateToken(token, _tokenValidationParameters,
                out SecurityToken validatedToken);
            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)validatedToken;

            int userId = int.Parse(jwtSecurityToken.Claims.First(x => x.Type == ClaimTypes.UserData).Value);
            return userId;
        }

    }
}
