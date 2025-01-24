
using System.Data.SqlClient;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;


namespace commhub.Utilities
{
    public class Utility
    {
        /*  ************ CREATE SECRET KEY *****************
               This module is meant to generate secret key for each time a user logs in.
               it will ensure uniqueness to avoid clash with other users secret key and 
               JWT.
              */
        [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger documentation
        [NonAction] // Ensure it's not considered as an action
        public string CreateSecretKey()
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const int keyLength = 32; // You can adjust the length as needed

            StringBuilder secretKey = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < keyLength; i++)
            {
                int randomIndex = random.Next(characters.Length);
                secretKey.Append(characters[randomIndex]);
            }

            return secretKey.ToString();
        }

        [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger documentation
        [NonAction] // Ensure it's not considered as an action
        private string GenerateJwtToken(int userId, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("userId", userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [ApiExplorerSettings(IgnoreApi = true)] // Exclude from Swagger documentation
        [NonAction] // Ensure it's not considered as an action
        public async Task<(string token, string secretKey)> CreateToken(int userId, string connnectionstring)
        {
            string secretKey = CreateSecretKey();
            string token = GenerateJwtToken(userId, secretKey);

            try
            {
                using (SqlConnection conn = new SqlConnection(connnectionstring))
                {
                    await conn.OpenAsync();
                    string formattedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                    string sqlQuery = $@"
                        INSERT INTO UsersToken (UserId, Token, Secret_Key, CreatedOnDate) 
                        VALUES (@userId, @token, @secretKey, @createdOnDate)
                    ";

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@token", token);
                        cmd.Parameters.AddWithValue("@secretKey", secretKey);
                        cmd.Parameters.AddWithValue("@createdOnDate", formattedDate);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return (token, secretKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving token to database:");
                throw ex;
            }
        }

        [HttpPost("VerifyToken")] // Ensure it's not considered as an action
        public bool VerifyToken(string Token, string SecretKey)
        {
            try
            {
                string token = Token; // Assuming the token is sent as a form parameter
                string secretKey = SecretKey; // Assuming the secret key is sent as a form parameter

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Disable clock skew tolerance
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return true; // Token is valid
            }
            catch (SecurityTokenExpiredException)
            {
                // Token has expired
                return false;
            }
            catch (Exception ex)
            {
                // Token verification failed
                Console.WriteLine("Error verifying token: " + ex.Message);
                return false;
            }
        }

        public DateTime? ResolveDate(object dbValue)
        {
            return dbValue == DBNull.Value ? (DateTime?)null : (DateTime)dbValue;
        }

        public int? ResolveNullableInt(object dbValue)
        {
            return dbValue == DBNull.Value ? (int?)null : (int)dbValue;
        }
    }
}
