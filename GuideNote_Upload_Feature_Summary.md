# 📸 GuideNote Upload Attachment Feature

## ✅ **Đã thêm tính năng Upload Ảnh cho GuideNote API**

### 🎯 **Tính năng mới:**

#### 1. **API Upload Attachment**
- **Endpoint:** `POST /api/GuideNote/upload-attachment`
- **Content-Type:** `multipart/form-data`
- **Authorization:** Bearer Token với role "Tour Guide"

#### 2. **Cập nhật Create Note API**
- **Thêm field:** `attachmentUrls` trong request body
- **Hỗ trợ:** Multiple attachments per note

### 📋 **API Endpoints đã cập nhật:**

#### **1. Upload Attachment**
```http
POST https://localhost:7012/api/GuideNote/upload-attachment
Authorization: Bearer {{token}}
Content-Type: multipart/form-data

// Upload file trong form data với field name "file"
```

**Response:**
```json
{
  "message": "File uploaded successfully",
  "attachmentUrl": "/uploads/guidenotes/abc123.jpg",
  "fileName": "abc123.jpg"
}
```

#### **2. Create Note with Attachments**
```http
POST https://localhost:7012/api/GuideNote/notes
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "bookingId": 1,
  "title": "Test Note with Images",
  "content": "This is a test note content with image attachments",
  "extraCost": 25.50,
  "attachmentUrls": [
    "/uploads/guidenotes/abc123.jpg",
    "/uploads/guidenotes/def456.png"
  ]
}
```

### 🔧 **Files đã sửa:**

#### 1. **Data/DTO/Request/CreateGuideNoteByTourGuideRequest.cs**
```csharp
public class CreateGuideNoteByTourGuideRequest
{
    [Required]
    public int BookingId { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    public decimal? ExtraCost { get; set; }
    
    public List<string>? AttachmentUrls { get; set; } // ✅ Mới thêm
}
```

#### 2. **Controllers/GuideNoteController.cs**
```csharp
// ✅ API Upload Attachment mới
[HttpPost("upload-attachment")]
public async Task<ActionResult> UploadAttachment(IFormFile file)
{
    // Validation file type và size
    // Lưu file vào /wwwroot/uploads/guidenotes/
    // Trả về URL
}
```

#### 3. **Service/GuideNoteService.cs**
```csharp
// ✅ Xử lý attachment URLs khi tạo note
if (request.AttachmentUrls != null && request.AttachmentUrls.Count > 0)
{
    foreach (var url in request.AttachmentUrls)
    {
        var media = new GuideNoteMedia
        {
            NoteId = note.NoteId,
            MediaUrl = url,
            UploadedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.GuideNoteMedia.Add(media);
    }
    await _context.SaveChangesAsync();
}
```

### 📸 **Tính năng Upload:**

#### **File Types được hỗ trợ:**
- ✅ **JPG, JPEG** - Ảnh JPEG
- ✅ **PNG** - Ảnh PNG
- ✅ **GIF** - Ảnh GIF
- ✅ **PDF** - File PDF

#### **File Size Limit:**
- ✅ **Tối đa 10MB** per file

#### **Upload Path:**
- ✅ **`/wwwroot/uploads/guidenotes/`**
- ✅ **Tự động tạo thư mục** nếu chưa tồn tại
- ✅ **Unique filename** với GUID để tránh conflict

#### **Security Features:**
- ✅ **File type validation**
- ✅ **File size validation**
- ✅ **Unique filename generation**
- ✅ **Authorization required**

### 🧪 **Cách sử dụng:**

#### **Bước 1: Upload ảnh**
```javascript
// Frontend - Upload file
const formData = new FormData();
formData.append('file', fileInput.files[0]);

const response = await fetch('/api/GuideNote/upload-attachment', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token
    },
    body: formData
});

const result = await response.json();
// result.attachmentUrl chứa URL của file đã upload
```

#### **Bước 2: Tạo note với ảnh**
```javascript
// Frontend - Create note with attachments
const noteData = {
    bookingId: 1,
    title: "Note with Images",
    content: "This note has image attachments",
    extraCost: 25.50,
    attachmentUrls: [
        "/uploads/guidenotes/abc123.jpg",
        "/uploads/guidenotes/def456.png"
    ]
};

const response = await fetch('/api/GuideNote/notes', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(noteData)
});
```

### 📁 **Database Schema:**

#### **GuideNoteMedia Table:**
```sql
CREATE TABLE GuideNoteMedia (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NoteId INT NOT NULL,
    MediaUrl NVARCHAR(MAX) NOT NULL,
    UploadedAt DATETIME2,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (NoteId) REFERENCES GuideNote(NoteId)
);
```

### 🎯 **Workflow hoàn chỉnh:**

1. **TourGuide upload ảnh** → `POST /api/GuideNote/upload-attachment`
2. **Nhận URL** từ response
3. **Tạo note với attachment URLs** → `POST /api/GuideNote/notes`
4. **Hệ thống tự động lưu** vào `GuideNoteMedia` table
5. **Customer nhận notification** về note mới có ảnh

### ✅ **Kết quả:**

- ✅ **Build thành công**
- ✅ **API upload attachment hoạt động**
- ✅ **Hỗ trợ multiple attachments**
- ✅ **File validation đầy đủ**
- ✅ **Security measures**
- ✅ **Database integration**

**Bây giờ TourGuide có thể upload ảnh và tạo note với attachments một cách dễ dàng!** 🚀 