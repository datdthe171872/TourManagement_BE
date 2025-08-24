# CORS Frontend Configuration Guide

## Tổng quan
Backend đã được cấu hình để hỗ trợ frontend với CORS policy chi tiết.

## CORS Policy đã cấu hình

### 1. AllowFrontend Policy (Chính)
```csharp
options.AddPolicy("AllowFrontend",
    policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",    // React default
                "http://localhost:5000",    // Alternative port
                "http://localhost:8080",    // Alternative port
                "http://localhost:4200",    // Angular default
                "http://localhost:5173"     // Vite default
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition");
    });
```

### 2. Fallback Policy
```csharp
options.AddPolicy("AllowAll",
    policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
```

## Cấu hình Frontend URL

### 1. appsettings.json
```json
{
  "FrontendUrl": "http://localhost:3000"
}
```

### 2. Sử dụng trong AuthService
```csharp
var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
var verificationLink = $"{frontendUrl}/verify-email?token={verificationToken}";
```

## Các Frontend Framework được hỗ trợ

### ✅ React (Port 3000)
```bash
npx create-react-app my-app
cd my-app
npm start
# Chạy trên http://localhost:3000
```

### ✅ Angular (Port 4200)
```bash
ng new my-app
cd my-app
ng serve
# Chạy trên http://localhost:4200
```

### ✅ Vue.js (Port 8080)
```bash
npm create vue@latest my-app
cd my-app
npm run dev
# Chạy trên http://localhost:8080
```

### ✅ Vite (Port 5173)
```bash
npm create vite@latest my-app
cd my-app
npm run dev
# Chạy trên http://localhost:5173
```

## Thay đổi Frontend URL

### Cách 1: Sửa appsettings.json
```json
{
  "FrontendUrl": "http://localhost:5000"
}
```

### Cách 2: Sửa appsettings.Development.json
```json
{
  "FrontendUrl": "http://localhost:8080"
}
```

### Cách 3: Environment Variables
```bash
# Windows
set FrontendUrl=http://localhost:5000

# Linux/Mac
export FrontendUrl=http://localhost:5000
```

## API Endpoints cho Frontend

### 1. Email Verification
```
POST /api/Auth/verify-email
Content-Type: application/json

{
  "token": "verification-token-from-email"
}
```

### 2. Login
```
POST /api/Auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

### 3. Register
```
POST /api/Auth/register
Content-Type: application/json

{
  "userName": "username",
  "email": "user@example.com",
  "password": "password123",
  "roleName": "Customer"
}
```

## Testing CORS

### 1. Test với Postman
- Gửi request từ Postman (không có CORS issue)
- Kiểm tra response headers

### 2. Test với Browser
- Mở DevTools → Console
- Kiểm tra CORS errors
- Verify preflight requests

### 3. Test với Frontend
```javascript
// Test API call từ frontend
fetch('https://localhost:7001/api/Auth/login', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    email: 'test@example.com',
    password: 'password123'
  })
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
```

## Troubleshooting

### Lỗi CORS thường gặp

#### 1. "Access to fetch at '...' from origin '...' has been blocked by CORS policy"
**Nguyên nhân:** Frontend URL không có trong CORS policy
**Giải pháp:** Thêm URL vào `WithOrigins()` hoặc sửa `FrontendUrl` trong appsettings.json

#### 2. "Request header field content-type is not allowed by Access-Control-Allow-Headers"
**Nguyên nhân:** CORS policy không cho phép header
**Giải pháp:** Đã cấu hình `AllowAnyHeader()` - không cần sửa

#### 3. "Method POST is not allowed by Access-Control-Allow-Methods"
**Nguyên nhân:** CORS policy không cho phép method
**Giải pháp:** Đã cấu hình `AllowAnyMethod()` - không cần sửa

### Debug CORS
```csharp
// Thêm logging để debug CORS
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();
    Console.WriteLine($"Request from origin: {origin}");
    await next();
});
```

## Production Configuration

### 1. appsettings.Production.json
```json
{
  "FrontendUrl": "https://yourdomain.com"
}
```

### 2. CORS Policy cho Production
```csharp
if (app.Environment.IsProduction())
{
    app.UseCors("AllowFrontend"); // Chỉ cho phép domain thật
}
else
{
    app.UseCors("AllowAll"); // Cho phép tất cả trong development
}
```

## Kết luận

✅ **Backend đã được cấu hình đầy đủ để hỗ trợ frontend**
✅ **CORS policy linh hoạt cho nhiều ports và frameworks**
✅ **Cấu hình URL dễ thay đổi qua appsettings.json**
✅ **Hỗ trợ cả development và production**

Frontend developer có thể sử dụng bất kỳ framework nào và chạy trên bất kỳ port nào được liệt kê trong CORS policy.
