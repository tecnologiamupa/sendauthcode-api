 
using MicroserviceWhatsapp.Data.Request;
using MicroserviceWhatsapp.Data.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Application.Middleware
{
    
        public class MiddleareMeth
        {
            public static async Task<ResponseIdentityNumbers> GetIdentityNumberAsync(string identityNumber, string url)
            {
                using var httpClient = new HttpClient();
                var requestUrl = $"{url}personal-info/identity-number/{identityNumber}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("accept", "*/*");

                var response = await httpClient.SendAsync(request);
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseIdentityNumbers>(jsonString);

                return result ?? new ResponseIdentityNumbers();
            }
        public static string GetAuthorizationToken(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "HttpRequest is required.");

            if (request.Headers.TryGetValue("Authorization", out var token) && !string.IsNullOrWhiteSpace(token))
            {
                return token.ToString();
            }

            throw new ArgumentNullException("Authorization token is required.");
        }
        
        public static string GenerateJwtToken(
        string secretKey,
        string issuer,
        string audience,
        IDictionary<string, object> claims,
        int expireMinutes = 60)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Secret key must be provided.", nameof(secretKey));

            if (string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentException("Issuer must be provided.", nameof(issuer));

            if (string.IsNullOrWhiteSpace(audience))
                throw new ArgumentException("Audience must be provided.", nameof(audience));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Map dictionary to list of Claim objects
            var tokenClaims = claims.Select(c =>
            {
                string claimType = MapToStandardClaimType(c.Key);
                string claimValue = c.Value?.ToString() ?? string.Empty;
                return new Claim(claimType, claimValue);
            }).ToList();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // Optional helper to map keys like "email" or "sub" to standard ClaimTypes
        private static string MapToStandardClaimType(string key)
        {
            return key.ToLower() switch
            {
                "email" => ClaimTypes.Email,
                "name" => ClaimTypes.Name,
                "role" => ClaimTypes.Role,
                "nameidentifier" or "sub" => ClaimTypes.NameIdentifier,
                _ => key
            };
        }
        public static bool ValidateJwtToken(string token, string secretKey, string issuer, string audience)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            // Eliminar prefijo "Bearer " si existe
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = CleanBearerToken(token);

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Secret key must be provided.", nameof(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string CleanBearerToken(string token)
        {
            return token?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
                ? token.Substring("Bearer ".Length).Trim()
                : token;
        }



        public static async Task<ResponseAPIWhatsApp> EnviarCodigoWhatsAppAsync(string telefono, string codigo, WSConfig _wSConfig)
        {
            var url = $"{_wSConfig.URL}/{_wSConfig.Version}/{_wSConfig.Phone_Number_ID}/messages";
            var bearerToken = _wSConfig.User_Access_Token; // tu token

            var payload = new
            {
                messaging_product = "whatsapp",
                to = telefono.Replace("-",""),
                type = "template",
                template = new
                {
                    name = "codigo",
                    language = new { code = "es" },
                    components = new object[]
                    {
                new
                {
                    type = "body",
                    parameters = new object[]
                    {
                        new { type = "text", text = codigo }
                    }
                },
                new
                {
                    type = "button",
                    sub_type = "url",
                    index = "0",
                    parameters = new object[]
                    {
                        new { type = "text", text = codigo }
                    }
                }
                    }
                }
            };

            using var httpClient = new HttpClient();
            using var content = new StringContent(
                JsonConvert.SerializeObject(payload), // Use JsonConvert.SerializeObject instead of JsonSerializer.Serialize
                Encoding.UTF8,
                "application/json"
            );

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            using var response = await httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseAPIWhatsApp>(responseContent);
            return result;
        }
        public static IDictionary<string, string> GetJwtTokenInfo(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token), "El token JWT es requerido.");

            // ✅ Eliminar "Bearer " si está presente
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring("Bearer ".Length).Trim();

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                throw new SecurityTokenException("El token JWT no tiene un formato válido.");

            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
        }






    }

}
