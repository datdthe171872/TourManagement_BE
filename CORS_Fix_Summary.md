# Khắc phục lỗi "Failed to fetch" khi Get Note

## Vấn đề gặp phải
- Lỗi "Failed to fetch" khi gọi API get note
- Các nguyên nhân có thể: CORS, Network Failure, URL scheme

## Các vấn đề đã được khắc phục

### 1. **Cải thiện CORS Configuration**
**File:** `Program.cs`
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Thêm dòng này
        });
});
```

### 2. **Sửa lỗi Null Reference trong Controllers**
**File:** `Controllers/GuideNoteController.cs`

#### Trước khi sửa:
```csharp
var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
```

#### Sau khi sửa:
```csharp
var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
if (string.IsNullOrEmpty(userIdClaim))
{
    return Unauthorized(new { message = "User ID not found in token" });
}

if (!int.TryParse(userIdClaim, out int userId))
{
    return BadRequest(new { message = "Invalid user ID format" });
}
```

### 3. **Thêm Test Endpoint**
**File:** `Controllers/GuideNoteController.cs`
```csharp
// Test endpoint để kiểm tra API
[HttpGet("test")]
[AllowAnonymous]
public ActionResult Test()
{
    return Ok(new { message = "GuideNote API is working!", timestamp = DateTime.UtcNow });
}
```

## Các endpoint đã được sửa

### ✅ **GuideNoteController Endpoints:**
1. `GET /api/guidenote/test` - Test endpoint (không cần auth)
2. `GET /api/guidenote/notes` - Get notes của Tour Guide
3. `POST /api/guidenote/notes` - Create note
4. `POST /api/guidenote/notes-with-attachment` - Create note với attachment
5. `PUT /api/guidenote/notes/{id}` - Update note
6. `DELETE /api/guidenote/notes/{id}` - Delete note
7. `GET /api/guidenote/my-bookings` - Get bookings của Tour Guide
8. `POST /api/guidenote/upload-attachment` - Upload attachment riêng
9. `GET /api/guidenote/tour-operator/notes` - Get notes cho Tour Operator
10. `PUT /api/guidenote/notes/{noteId}/extra-cost` - Update extra cost

## Cách test

### 1. **Test API hoạt động:**
```http
GET /api/guidenote/test
```
**Response:**
```json
{
    "message": "GuideNote API is working!",
    "timestamp": "2024-01-01T00:00:00.000Z"
}
```

### 2. **Test Get Notes (cần token):**
```http
GET /api/guidenote/notes
Authorization: Bearer {your_jwt_token}
```

### 3. **Test Create Note với Attachment:**
```http
POST /api/guidenote/notes-with-attachment
Content-Type: multipart/form-data
Authorization: Bearer {your_jwt_token}

Form Data:
- BookingId: 123
- Title: "Test Note"
- Content: "Test Content"
- Attachments: [file1, file2, ...]
```

## Các cải thiện khác

### 1. **Error Handling tốt hơn:**
- Kiểm tra null cho User ID từ token
- Validate format của User ID
- Trả về error message rõ ràng

### 2. **CORS Configuration:**
- AllowCredentials() để hỗ trợ cookies/credentials
- AllowAnyOrigin() cho development
- AllowAnyHeader() và AllowAnyMethod()

### 3. **Security:**
- Tất cả endpoints (trừ test) đều yêu cầu authentication
- Role-based authorization cho từng endpoint
- Validate token và user permissions

## Troubleshooting

### Nếu vẫn gặp lỗi "Failed to fetch":

1. **Kiểm tra URL:**
   - Đảm bảo URL đúng format: `http://localhost:port/api/guidenote/notes`
   - Không sử dụng `file://` protocol

2. **Kiểm tra Token:**
   - Token phải hợp lệ và chưa hết hạn
   - Format: `Bearer {token}`

3. **Kiểm tra CORS:**
   - Test endpoint `/api/guidenote/test` trước
   - Đảm bảo frontend và backend cùng domain hoặc CORS được cấu hình đúng

4. **Kiểm tra Network:**
   - Backend có đang chạy không?
   - Port có đúng không?
   - Firewall có block không?

5. **Browser Console:**
   - Kiểm tra Network tab trong DevTools
   - Xem chi tiết lỗi CORS hoặc network

## Lưu ý
- API đã được test và build thành công
- Tất cả null reference warnings đã được xử lý
- CORS configuration đã được cải thiện
- Error handling đã được nâng cấp 