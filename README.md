# Microservicio WhatsApp

Microservicio para el envío de mensajes de WhatsApp y validación de identidad con autenticación mediante JWT.

## 📋 Descripción

Este microservicio proporciona una API REST para:
- Envío de mensajes de WhatsApp a través de la API de Facebook/Meta
- Validación de identidad mediante códigos de verificación
- Autenticación mediante tokens JWT 

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas con Clean Architecture:

```
sendauthcode/
├── MicroserviceWhatsapp.API/          # Capa de presentación (API REST)
├── MicroserviceWhatsapp.Application/  # Lógica de negocio y servicios
├── MicroserviceWhatsapp.Domain/       # Entidades y contexto de base de datos
└── MicroserviceWhatsapp.Data/         # DTOs y modelos de datos
```

### Capas del Proyecto

- **API**: Controladores y endpoints REST
- **Application**: Servicios, interfaces y middleware
- **Domain**: Entidades, DbContext y migraciones de Entity Framework
- **Data**: Requests, Responses y modelos de transferencia de datos

## 🚀 Tecnologías

- .NET 6.0+
- Entity Framework Core
- MySQL
- JWT Authentication
- WhatsApp Business API (Facebook Graph API)
- Swagger/OpenAPI

## 📦 Requisitos Previos

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- Cuenta de WhatsApp Business API
- Visual Studio 2022 o Visual Studio Code

## ⚙️ Configuración

### 1. Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd sendauthcode
```

### 2. Configurar Base de Datos

Crear una base de datos MySQL:

```sql
CREATE DATABASE mupa_whatsapp;
```

### 3. Configurar Variables de Entorno

Copiar el archivo de ejemplo y configurar las credenciales:

```bash
cd MicroserviceWhatsapp.API
copy appsettings.example.json appsettings.Development.json
```

Editar `appsettings.Development.json` con tus credenciales:

```json
{
  "ConnectionStrings": {
    "MessageConnectionString": "Host=localhost; Port=3306; Database=mupa_whatsapp; Username=root; Password=tu_password;"
  },
  "ConfiguracionFB": {
    "Business_ID": "tu_business_id",
    "Phone_Number_ID": "tu_phone_number_id",
    "User_Access_Token": "tu_access_token",
    "ServiceIdentity": "url_servicio_identidad",
    "Usuario": "tu_usuario",
    "Password": "tu_password",
    "APICV": "url_api_cv", 
  },
  "JwtSettings": {
    "SecretKey": "tu_clave_secreta_minimo_32_caracteres"
  }
}
```

### 4. Ejecutar Migraciones

```bash
dotnet ef database update --project MicroserviceWhatsapp.Domain --startup-project MicroserviceWhatsapp.API
```

### 5. Ejecutar el Proyecto

```bash
dotnet run --project MicroserviceWhatsapp.API
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📖 API Endpoints

### Validar Identidad

```http
POST /api/v1/SendMessage/ValidIdentityId
Content-Type: application/json

{
  "IdentityId": "123456789"
}
```

**Respuesta:**
```json
{
  "statusCode": 200,
  "message": "Código enviado exitosamente",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs..."
  }
}
```

### Validar Código

```http
POST /api/v1/SendMessage/ValidCodSendWS
Authorization: Bearer {token}
Content-Type: application/json

{
  "code": "123456"
}
```

**Respuesta:**
```json
{
  "statusCode": 200,
  "message": "Código validado correctamente",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs..."
  }
}
```

## 🔐 Seguridad

### JWT Authentication

El microservicio utiliza JWT (JSON Web Tokens) para la autenticación. Los tokens tienen las siguientes configuraciones:

- **Token de código**: Expira en 5 minutos
- **Token de login**: Expira en 60 minutos

### Variables Sensibles

⚠️ **IMPORTANTE**: Nunca subir credenciales al repositorio. Los siguientes archivos deben mantenerse en `.gitignore`:

- `appsettings.Development.json`
- `appsettings.Production.json`

## 🛠️ Desarrollo

### Estructura de Carpetas

```
MicroserviceWhatsapp.API/
├── Controllers/           # Controladores de la API
├── Properties/           # Configuración de lanzamiento
└── appsettings.json      # Configuración (sin credenciales)

MicroserviceWhatsapp.Application/
├── Interface/            # Interfaces de servicios
├── Middleware/           # Middleware personalizado
├── Service/             # Implementación de servicios
└── Template/            # Plantillas de mensajes

MicroserviceWhatsapp.Domain/
├── Models/              # Entidades de dominio
├── Migrations/          # Migraciones de EF Core
└── ServicioMensajeriaDbContext.cs

MicroserviceWhatsapp.Data/
├── Request/             # DTOs de peticiones
└── Response/            # DTOs de respuestas
```

### Agregar Migraciones

```bash
dotnet ef migrations add NombreDeLaMigracion --project MicroserviceWhatsapp.Domain --startup-project MicroserviceWhatsapp.API
```

### Revertir Migración

```bash
dotnet ef migrations remove --project MicroserviceWhatsapp.Domain --startup-project MicroserviceWhatsapp.API
```

## 🧪 Testing

```bash
dotnet test
```

## 📝 Notas Adicionales

### WhatsApp Business API

Para usar este microservicio necesitas:
1. Cuenta de Facebook Business
2. Aplicación de Facebook configurada
3. WhatsApp Business API activada
4. Número de teléfono verificado

## 🐛 Solución de Problemas

### Error de Conexión a la Base de Datos

Verificar que:
- MySQL esté ejecutándose
- Las credenciales en `appsettings.Development.json` sean correctas
- El usuario tenga permisos en la base de datos

### Error de Token Inválido

Verificar que:
- El token no haya expirado
- El `SecretKey` sea el mismo usado para generar el token
- El header `Authorization` incluya el prefijo `Bearer`

### Error al Enviar Mensajes de WhatsApp

Verificar que:
- El token de acceso de Facebook sea válido
- El número de teléfono esté verificado en WhatsApp Business
- La aplicación tenga los permisos necesarios
 

**Nota**: Este proyecto está en desarrollo activo. Para más información, consulta la documentación o contacta al equipo de desarrollo.
