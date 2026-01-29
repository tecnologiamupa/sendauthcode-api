using MicroserviceWhatsapp.Application.Interface;
using MicroserviceWhatsapp.Data.Request;
using MicroserviceWhatsapp.Data.Response;
using Microsoft.Extensions.Options;
using System.Text;
using Newtonsoft.Json;
using MicroserviceWhatsapp.Application.Template;
using MicroserviceWhatsapp.Domain;
using MicroserviceWhatsapp.Application.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using NodaTime;
using System.Globalization;
using System; 
using Microsoft.VisualBasic;
using MicroserviceWhatsapp.Domain.Models;

namespace MicroserviceWhatsapp.Application.Service
{

    public class ServiceSendMessage : ISendMessage
    {
        public WSConfig _settings { get; }
        private readonly JwtSettings _jwtSettings;
        private readonly ServicioMensajeriaDbContext _context;
        private const string END_CHAT_MESSAGE = "Gracias por haber usado nuestro canal de Atención al Cliente. \n\n¡Fue un placer para mí atenderte! \n\n¡Te deseo un excelente día!. 👋";

        public ServiceSendMessage(IOptions<WSConfig> settings, ServicioMensajeriaDbContext dbContext, IOptions<JwtSettings>jwtSettings)
        {
            _settings = settings.Value;
            _context = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

       

        public async Task<ResponseMSG<ResponseToken>> VerifiedIdentityId(RequestValidIdentityId request)
        {
            var responseToken = new ResponseToken();

            try
            {
                // Obtener datos desde middleware
                var result = await MiddleareMeth.GetIdentityNumberAsync(request.IdentityId, _settings.APICV);
                var data = result?.Data;

                if (data == null)
                {
                    return new ResponseMSG<ResponseToken>
                    {
                        Message = "No se pudo obtener la información de la identidad.",
                        StatusCode = 404,
                        IsSuccess = false,
                        data = responseToken
                    };
                }

                // Buscar usuario en base de datos
                var user = await _context.Users.FirstOrDefaultAsync(x => x.IdentityId == data.IdentityNumber);

                // Generar código y token
                string code = GenerarCodigoWhatsApp();
                string last4Digits = string.IsNullOrWhiteSpace(data.Mobile) || data.Mobile.Length < 4
                    ? "XXXX"
                    : data.Mobile[^4..];

                string token = MiddleareMeth.GenerateJwtToken(
                    _jwtSettings.SecretKey,
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    new Dictionary<string, object>
                    {
                { "sub", data.Id },
                { "email", data.Email ?? string.Empty },
                { "cedula", data.IdentityNumber ?? string.Empty }
                    });

                responseToken.AccessToken = token;

                var userRequest = new RequestUserLogin
                {
                    FullName = $"{data.FirstName ?? ""} {data.LastName ?? ""}".Trim(),
                    Email = data.Email ?? "No disponible",
                    IdentityId = data.IdentityNumber ?? "No disponible",
                    PhoneNumber = data.phoneCountryCode+""+data.Mobile ?? "No disponible"
                };

                // Guardar/Actualizar usuario y enviar código
                await SaveOrUpdateUserAsync(userRequest, code, false);
                await MiddleareMeth.EnviarCodigoWhatsAppAsync(userRequest.PhoneNumber, code, _settings);

                string message = user == null
                    ? $"Se ha enviado un código a su teléfono que termina en ****{last4Digits}."
                    : $"Su cuenta ya está registrada. Se ha enviado un código a su teléfono que termina en ****{last4Digits}.";

                return new ResponseMSG<ResponseToken>
                {
                    Message = message,
                    StatusCode = 200,
                    IsSuccess = true,
                    data = responseToken,
                    Code = ""
                    
                };
            }
            catch (Exception ex)
            {
                return new ResponseMSG<ResponseToken>
                {
                    Message = $"Error al verificar la identidad: {ex.Message}",
                    StatusCode = 500,
                    IsSuccess = false,
                    data = responseToken
                };
            }
        }


        private DateTime GetLocalDateTime(string timeZoneId)
        {
            DateTime utcDateTime = DateTime.UtcNow;
            Instant instant = Instant.FromDateTimeUtc(utcDateTime);
            DateTimeZone timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];
            ZonedDateTime zonedDateTime = instant.InZone(timeZone);
            return zonedDateTime.ToDateTimeUnspecified();
        }
        private string GenerarCodigoWhatsApp()
        {
            var random = new Random();
            char letra = (char)random.Next('A', 'Z' + 1);
            int numero = random.Next(0, 1000000); // 6 dígitos
            string codigo = $"{letra}{numero:D6}";
            return codigo;
        }
        public async Task<UserLogin> SaveOrUpdateUserAsync(
           RequestUserLogin requestUaer, string CodeWhatsApp, bool _WhatsAppVerified
        )
        {
            // Buscar por cédula o teléfono
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.IdentityId == requestUaer.IdentityId || u.PhoneNumber == requestUaer.PhoneNumber);

            if (user == null)
            {
                // Crear nuevo usuario
                user = new UserLogin
                {
                    FullName = requestUaer.FullName,
                    Email = requestUaer.Email,
                    IdentityId = requestUaer.IdentityId,
                    PhoneNumber = requestUaer.PhoneNumber,
                    WhatsAppCode = CodeWhatsApp,
                    WhatsAppCodeSentAt = GetLocalDateTime("America/Panama"),
                    WhatsAppCodeExpiresAt = GetLocalDateTime("America/Panama").AddMinutes(5),
                    WhatsAppVerified = _WhatsAppVerified,
                    SocialProvider = "Whatsapp",
                    SocialId = "",
                    IsActive = false,
                    CreatedAt = GetLocalDateTime("America/Panama")
                };
                _context.Users.Add(user);
            }
            else
            {
                // Actualizar usuario existente
                user.WhatsAppCode = CodeWhatsApp;
                user.WhatsAppVerified = _WhatsAppVerified;
                user.IsActive = _WhatsAppVerified;
                user.UpdatedAt = GetLocalDateTime("America/Panama");
                user.WhatsAppCodeExpiresAt = GetLocalDateTime("America/Panama").AddMinutes(5);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<ResponseMSG<ResponseToken>> ValidCodSendWS(RequestCode requestSend, string token)
        {
            try
            {
                // Validar el token JWT
                if (!MiddleareMeth.ValidateJwtToken(token, _jwtSettings.SecretKey, _jwtSettings.Issuer, _jwtSettings.Audience))
                {
                    return new ResponseMSG<ResponseToken>
                    {
                        Message = "Token inválido o expirado.",
                        StatusCode = 401,
                        IsSuccess = false,
                        data = null
                    };
                }

                // Limpiar token y extraer claims
                var claims = MiddleareMeth.GetJwtTokenInfo(token);
                if (!claims.TryGetValue("cedula", out var cedula) || string.IsNullOrWhiteSpace(cedula))
                {
                    return new ResponseMSG<ResponseToken>
                    {
                        Message = "No se pudo extraer la cédula del token.",
                        StatusCode = 400,
                        IsSuccess = false,
                        data = null
                    };
                }

                // Buscar al usuario
                var user = await _context.Users.FirstOrDefaultAsync(u => u.IdentityId == cedula);
                if (user == null)
                {
                    return new ResponseMSG<ResponseToken>
                    {
                        Message = "Usuario no encontrado.",
                        StatusCode = 404,
                        IsSuccess = false,
                        data = null
                    };
                }

                // Validar código
                if (user.WhatsAppCode != requestSend.Code)
                {
                    return new ResponseMSG<ResponseToken>
                    {
                        Message = "Código inválido.",
                        StatusCode = 400,
                        IsSuccess = false,
                        data = null
                    };
                }

                // Validar expiración del código
                var now = GetLocalDateTime("America/Panama");
                if (user.WhatsAppCodeExpiresAt != null && user.WhatsAppCodeExpiresAt < now)
                {
                    return new ResponseMSG<ResponseToken>
                    {
                        Message = "El código ha expirado.",
                        StatusCode = 410,
                        IsSuccess = false,
                        data = null
                    };
                }

                var result = await MiddleareMeth.GetIdentityNumberAsync(cedula, _settings.APICV);
                var data = result?.Data;

                // Crear nuevo token de acceso
                var accessToken = MiddleareMeth.GenerateJwtToken(
                    _jwtSettings.SecretKey,
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    new Dictionary<string, object>
                    {
                { "sub", result.Data.Id },
                { "email", user.Email ?? "" },
                { "cedula", user.IdentityId ?? "" }
                    },_jwtSettings.ExpireMinutesLogin);

                // Guardar último login válido (opcional)
                var loginInfo = new RequestUserLogin
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    IdentityId = user.IdentityId,
                };
                await SaveOrUpdateUserAsync(loginInfo, string.Empty, true);

                return new ResponseMSG<ResponseToken>
                {
                    Message = "Código verificado correctamente.",
                    StatusCode = 200,
                    IsSuccess = true,
                    data = new ResponseToken { AccessToken = accessToken , UserInfo = loginInfo }
                };
            }
            catch (Exception ex)
            {
                return new ResponseMSG<ResponseToken>
                {
                    Message = $"Error al validar el código: {ex.Message}",
                    StatusCode = 500,
                    IsSuccess = false,
                    data = null
                };
            }
        }



        public Task<ResponseMSG<string>> SendCodeUser(RequestSendMessage requestSend)
        {
            throw new NotImplementedException();
        }
    }
}
