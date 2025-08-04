# Tóm tắt: Kết hợp CreateNote với UploadAttachment

## Tổng quan
Đã tạo thành công một API mới kết hợp chức năng tạo note và upload attachment thành một endpoint duy nhất, giúp tối ưu hóa quy trình làm việc cho Tour Guide.

## Các thay đổi đã thực hiện

### 1. Tạo DTO Request mới
**File:** `Data/DTO/Request/CreateGuideNoteWithAttachmentRequest.cs`
```csharp
public class CreateGuideNoteWithAttachmentRequest
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    public List<IFormFile>? Attachments { get; set; }
}
```

### 2. Cập nhật Interface
**File:** `Service/IGuideNoteService.cs`
- Thêm method: `Task CreateNoteWithAttachmentAsync(int userId, CreateGuideNoteWithAttachmentRequest request);`

### 3. Implement Service Method
**File:** `Service/GuideNoteService.cs`
- Thêm method `CreateNoteWithAttachmentAsync` với các tính năng:
  - Tạo note mới
  - Upload và validate file attachments
  - Lưu file vào thư mục `wwwroot/uploads/guidenotes/`
  - Tạo records trong `GuideNoteMedia`
  - Cập nhật `TourAcceptanceReport`
  - Gửi notification cho customer

### 4. Thêm Controller Endpoint
**File:** `Controllers/GuideNoteController.cs`
- Endpoint: `POST /api/guidenote/notes-with-attachment`
- Sử dụng `[FromForm]` để nhận multipart/form-data
- Chỉ dành cho role "Tour Guide"

## Tính năng của API mới

### Validation
- ✅ Kiểm tra file type (jpg, jpeg, png, gif, pdf)
- ✅ Kiểm tra file size (tối đa 10MB)
- ✅ Kiểm tra quyền của Tour Guide với booking
- ✅ Validate booking và assignment

### File Upload
- ✅ Tạo unique filename với GUID
- ✅ Tự động tạo thư mục nếu chưa tồn tại
- ✅ Lưu file vào `wwwroot/uploads/guidenotes/`
- ✅ Tạo URL tương đối cho file

### Database Operations
- ✅ Tạo `GuideNote` record
- ✅ Tạo `GuideNoteMedia` records cho từng file
- ✅ Tạo hoặc cập nhật `TourAcceptanceReport`
- ✅ Tính toán và cập nhật `TotalExtraCost`

### Notifications
- ✅ Gửi notification cho customer khi có note mới

## Cách sử dụng

### Request Format
```http
POST /api/guidenote/notes-with-attachment
Content-Type: multipart/form-data
Authorization: Bearer {token}

Form Data:
- BookingId: 123
- Title: "Note Title"
- Content: "Note Content"
- Attachments: [file1, file2, ...]
```

### Response
```json
{
    "message": "Note with attachment created successfully"
}
```

## Lợi ích

1. **Hiệu quả hơn**: Chỉ cần 1 API call thay vì 2
2. **Atomic operation**: Tất cả operations được thực hiện trong 1 transaction
3. **User-friendly**: Frontend chỉ cần gọi 1 endpoint
4. **Consistent**: Đảm bảo note và attachments luôn đồng bộ
5. **Error handling**: Nếu có lỗi, tất cả sẽ được rollback

## So sánh với cách cũ

### Cách cũ (2 API calls):
1. `POST /api/guidenote/upload-attachment` → Upload file → Lấy URL
2. `POST /api/guidenote/notes` → Tạo note với AttachmentUrls

### Cách mới (1 API call):
1. `POST /api/guidenote/notes-with-attachment` → Tạo note + Upload files

## Lưu ý
- API cũ vẫn được giữ nguyên để đảm bảo backward compatibility
- Có thể sử dụng cả 2 cách tùy theo nhu cầu
- File upload path: `wwwroot/uploads/guidenotes/`
- File URL format: `/uploads/guidenotes/{filename}` 