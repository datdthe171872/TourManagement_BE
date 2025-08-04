# Sửa lỗi null values trong GuideNoteResponse

## Vấn đề gặp phải
Khi gọi API `GET /api/guidenote/notes`, response trả về các trường bị null:
```json
{
    "noteId": 8,
    "assignmentId": 3,
    "reportId": 3,
    "title": "ăn chả",
    "content": "sadug 400000",
    "extraCost": 0,
    "createdAt": "2025-08-04T01:45:12.59",
    "mediaUrls": [
        "/uploads/guidenotes/a73deed4-e1f6-498a-8333-c39e5ff32ae5.jpg"
    ],
    "tourGuideName": null,      // ❌ Bị null
    "tourTitle": null,          // ❌ Bị null
    "departureDate": null       // ❌ Bị null
}
```

## Nguyên nhân
Trong method `GetNotesByGuideUserIdAsync`, chúng ta không include các navigation properties cần thiết để lấy thông tin:
- Tour Guide name
- Tour title  
- Departure date

## Giải pháp đã thực hiện

### 1. **Cập nhật method GetNotesByGuideUserIdAsync**
**File:** `Service/GuideNoteService.cs`

#### Trước khi sửa:
```csharp
// Lấy tất cả assignment của guide
var assignmentIds = await _context.TourGuideAssignments
    .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
    .Select(a => a.Id).ToListAsync();

// Lấy note theo assignment
var notes = await _context.GuideNotes
    .Where(n => assignmentIds.Contains(n.AssignmentId) && n.IsActive)
    .Include(n => n.GuideNoteMedia)
    .OrderByDescending(n => n.CreatedAt)
    .ToListAsync();

return notes.Select(n => new GuideNoteResponse
{
    NoteId = n.NoteId,
    AssignmentId = n.AssignmentId,
    ReportId = n.ReportId,
    Title = n.Title,
    Content = n.Content,
    ExtraCost = n.ExtraCost,
    CreatedAt = n.CreatedAt,
    MediaUrls = n.GuideNoteMedia.Where(m => m.IsActive).Select(m => m.MediaUrl).ToList()
    // ❌ Thiếu thông tin tour guide, tour title, departure date
}).ToList();
```

#### Sau khi sửa:
```csharp
// Lấy tất cả assignment của guide với thông tin tour và departure date
var assignments = await _context.TourGuideAssignments
    .Include(a => a.DepartureDate)
    .Include(a => a.DepartureDate.Tour)
    .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
    .ToListAsync();

var assignmentIds = assignments.Select(a => a.Id).ToList();

// Lấy note theo assignment với thông tin đầy đủ
var notes = await _context.GuideNotes
    .Where(n => assignmentIds.Contains(n.AssignmentId) && n.IsActive)
    .Include(n => n.GuideNoteMedia)
    .OrderByDescending(n => n.CreatedAt)
    .ToListAsync();

return notes.Select(n => 
{
    var assignment = assignments.FirstOrDefault(a => a.Id == n.AssignmentId);
    return new GuideNoteResponse
    {
        NoteId = n.NoteId,
        AssignmentId = n.AssignmentId,
        ReportId = n.ReportId,
        Title = n.Title,
        Content = n.Content,
        ExtraCost = n.ExtraCost,
        CreatedAt = n.CreatedAt,
        MediaUrls = n.GuideNoteMedia.Where(m => m.IsActive).Select(m => m.MediaUrl).ToList(),
        TourGuideName = guide.User?.UserName,                    // ✅ Thêm tour guide name
        TourTitle = assignment?.DepartureDate?.Tour?.Title,     // ✅ Thêm tour title
        DepartureDate = assignment?.DepartureDate?.DepartureDate1 // ✅ Thêm departure date
    };
}).ToList();
```

### 2. **Kiểm tra method GetNotesByTourOperatorAsync**
Method này đã có thông tin đầy đủ:
```csharp
return notes.Select(n => new GuideNoteResponse
{
    // ... các trường khác
    TourGuideName = n.Assignment.TourGuide.User?.UserName ?? "Unknown",
    TourTitle = n.Booking.Tour?.Title ?? "Unknown", 
    DepartureDate = n.Booking.DepartureDate?.DepartureDate1 ?? DateTime.MinValue
}).ToList();
```

## Kết quả sau khi sửa

### Response mới sẽ có dạng:
```json
{
    "noteId": 8,
    "assignmentId": 3,
    "reportId": 3,
    "title": "ăn chả",
    "content": "sadug 400000",
    "extraCost": 0,
    "createdAt": "2025-08-04T01:45:12.59",
    "mediaUrls": [
        "/uploads/guidenotes/a73deed4-e1f6-498a-8333-c39e5ff32ae5.jpg"
    ],
    "tourGuideName": "John Doe",           // ✅ Có giá trị
    "tourTitle": "Tour Du Lịch Hà Nội",    // ✅ Có giá trị
    "departureDate": "2025-08-15T00:00:00" // ✅ Có giá trị
}
```

## Các API đã được cải thiện

### ✅ **GuideNoteController Endpoints:**
1. `GET /api/guidenote/notes` - Get notes của Tour Guide (đã sửa)
2. `GET /api/guidenote/tour-operator/notes` - Get notes cho Tour Operator (đã có sẵn)
3. `POST /api/guidenote/notes-with-attachment` - Create note với attachment
4. Các endpoint khác...

## Lưu ý kỹ thuật

### 1. **Navigation Properties:**
- Sử dụng `Include()` để load related data
- `Include(a => a.DepartureDate)` - Load departure date
- `Include(a => a.DepartureDate.Tour)` - Load tour information

### 2. **Null Safety:**
- Sử dụng null-conditional operator (`?.`) để tránh null reference exceptions
- Cung cấp fallback values khi cần thiết

### 3. **Performance:**
- Load tất cả assignments một lần thay vì query riêng lẻ
- Sử dụng `FirstOrDefault()` để tìm assignment tương ứng

## Build Status
- ✅ **Build thành công**
- ✅ **Không có lỗi compilation**
- ✅ **Tất cả warnings đã được xử lý**

## Test
Sau khi deploy, test lại API:
```http
GET /api/guidenote/notes
Authorization: Bearer {token}
```

Response sẽ có đầy đủ thông tin tour guide, tour title và departure date! 🎉 