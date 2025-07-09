# Khắc phục lỗi 401 - Invalid Token

## Nguyên nhân lỗi 401

Lỗi 401 với "invalid_token" thường xảy ra do:

1. **Token chưa được lấy** - Chưa đăng nhập
2. **Token đã hết hạn** - Token có thời hạn 1 giờ
3. **Token không đúng format** - Thiếu "Bearer " prefix
4. **Token không hợp lệ** - Token bị hỏng hoặc sai

## Cách khắc phục

### 1. Đăng nhập để lấy token

```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "your-email@example.com",
  "password": "your-password"
}
```

### 2. Sử dụng token đúng cách

```http
GET /api/TourGuideAssignment
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json
```

### 3. Test authentication

```http
GET /api/TourGuideAssignment/test-auth
Authorization: Bearer YOUR_TOKEN_HERE
```

## Các bước test

### Bước 1: Kiểm tra server đang chạy
```bash
# Kiểm tra port 7001 có đang chạy không
netstat -an | findstr :7001
```

### Bước 2: Đăng nhập
```powershell
# Sử dụng script PowerShell
.\test_api.ps1 -Email "your-email@example.com" -Password "your-password"
```

### Bước 3: Test với Postman/HTTP Client
1. Gọi API login để lấy token
2. Copy token từ response
3. Thêm header: `Authorization: Bearer YOUR_TOKEN`
4. Gọi API TourGuideAssignment

## Lưu ý quan trọng

1. **Token có thời hạn 1 giờ** - Cần đăng nhập lại nếu hết hạn
2. **Phải có prefix "Bearer "** - Không quên dấu cách
3. **Email và password phải đúng** - Kiểm tra thông tin đăng nhập
4. **Role phải phù hợp** - Tour Operator cho GET /api/TourGuideAssignment

## Ví dụ token hợp lệ

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJvcGVyYXRvckBleGFtcGxlLmNvbSIsInJvbGUiOiJUb3VyIE9wZXJhdG9yIiwianRpIjoiZ2l1ZCIsImlzcyI6IlRvdXJNYW5hZ2VtZW50IiwiYXVkIjoiVG91ck1hbmFnZW1lbnQiLCJleHAiOjE3MzA5NzI4MDB9.signature
```

## Debug steps

1. **Kiểm tra token format:**
   - Phải bắt đầu bằng "Bearer "
   - Token phải có 3 phần (header.payload.signature)

2. **Kiểm tra thời gian:**
   - Token có thời hạn 1 giờ
   - Kiểm tra thời gian hiện tại

3. **Kiểm tra role:**
   - Tour Operator: có thể gọi GET /api/TourGuideAssignment
   - Tour Guide: chỉ có thể gọi GET /api/TourGuideAssignment/my-assignments

4. **Kiểm tra database:**
   - User có tồn tại không
   - User có active không
   - Role có đúng không 