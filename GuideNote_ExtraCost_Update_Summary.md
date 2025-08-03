# 🔐 GuideNote ExtraCost Update Feature

## ✅ **Đã thêm tính năng TourOperator Update ExtraCost**

### 🎯 **Thay đổi chính:**

#### 1. **Bỏ ExtraCost khỏi TourGuide POST**
- **File:** `Data/DTO/Request/CreateGuideNoteByTourGuideRequest.cs`
- **Thay đổi:** Bỏ field `ExtraCost` - TourGuide không thể set extraCost
- **Lý do:** Chỉ TourOperator mới có quyền quyết định extra cost

#### 2. **Thêm API TourOperator Update ExtraCost**
- **Endpoint:** `PUT /api/GuideNote/notes/{noteId}/extra-cost`
- **Authorization:** Bearer Token với role "Tour Operator"
- **Chức năng:** Cập nhật extra cost của GuideNote

### 📋 **API Endpoints mới:**

#### **TourOperator Update Extra Cost**
```http
PUT https://localhost:7012/api/GuideNote/notes/1/extra-cost
Authorization: Bearer {{tour_operator_token}}
Content-Type: application/json

{
  "extraCost": 50.00
}
```

**Response:**
```json
{
  "message": "Extra cost updated successfully"
}
```

### 🔧 **Files đã sửa/tạo:**

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
    
    // ❌ Bỏ ExtraCost - TourGuide không thể set
    public List<string>? AttachmentUrls { get; set; }
}
```

#### 2. **Data/DTO/Request/UpdateGuideNoteExtraCostRequest.cs** ⭐ **MỚI**
```csharp
public class UpdateGuideNoteExtraCostRequest
{
    [Required]
    public decimal ExtraCost { get; set; }
}
```

#### 3. **Service/IGuideNoteService.cs**
```csharp
// ✅ Thêm method mới
Task UpdateNoteExtraCostAsync(int tourOperatorId, int noteId, UpdateGuideNoteExtraCostRequest request);
```

#### 4. **Service/GuideNoteService.cs**
```csharp
// ✅ Sửa CreateNoteByTourGuideAsync
var note = new GuideNote
{
    // ... other fields
    ExtraCost = 0, // TourGuide không thể set extraCost, chỉ TourOperator mới có quyền
    // ... other fields
};

// ✅ Thêm method UpdateNoteExtraCostAsync
public async Task UpdateNoteExtraCostAsync(int tourOperatorId, int noteId, UpdateGuideNoteExtraCostRequest request)
{
    // Kiểm tra note có tồn tại không
    // Kiểm tra TourOperator có quyền update note này không
    // Cập nhật extra cost
    // Cập nhật tổng extra cost trong report
    // Tạo notification cho customer
}
```

#### 5. **Controllers/GuideNoteController.cs**
```csharp
// ✅ Thêm API endpoint mới
[HttpPut("notes/{noteId}/extra-cost")]
[Authorize(Roles = "Tour Operator")]
public async Task<ActionResult> UpdateNoteExtraCost(int noteId, [FromBody] UpdateGuideNoteExtraCostRequest request)
{
    var tourOperatorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    await _guideNoteService.UpdateNoteExtraCostAsync(tourOperatorId, noteId, request);
    return Ok(new { message = "Extra cost updated successfully" });
}
```

### 🔐 **Phân quyền mới:**

#### **TourGuide:**
- ✅ Tạo note với title, content, attachments
- ❌ **KHÔNG thể set extraCost** (mặc định = 0)
- ✅ Upload attachments
- ✅ Update/delete note của mình

#### **TourOperator:**
- ✅ **Có quyền update extraCost** của bất kỳ note nào thuộc tour của mình
- ✅ Nhận notification khi TourGuide tạo note
- ✅ Customer nhận notification khi extraCost được update

### 🎯 **Workflow hoàn chỉnh:**

1. **TourGuide tạo note** → `POST /api/GuideNote/notes`
   - ExtraCost = 0 (mặc định)
   - Customer nhận notification

2. **TourOperator review note** → Xem danh sách note

3. **TourOperator update extra cost** → `PUT /api/GuideNote/notes/{id}/extra-cost`
   - Cập nhật extraCost
   - Tự động cập nhật tổng trong TourAcceptanceReport
   - Customer nhận notification về thay đổi

### 🧪 **Test Cases:**

#### **1. TourGuide tạo note (không có extraCost):**
```http
POST https://localhost:7012/api/GuideNote/notes
Authorization: Bearer {{tour_guide_token}}
Content-Type: application/json

{
  "bookingId": 1,
  "title": "Test Note",
  "content": "This is a test note content",
  "attachmentUrls": [
    "/uploads/guidenotes/abc123.jpg"
  ]
}
```

#### **2. TourOperator update extra cost:**
```http
PUT https://localhost:7012/api/GuideNote/notes/1/extra-cost
Authorization: Bearer {{tour_operator_token}}
Content-Type: application/json

{
  "extraCost": 50.00
}
```

### 📊 **Database Impact:**

#### **GuideNote Table:**
- ExtraCost mặc định = 0 khi TourGuide tạo
- Chỉ được update bởi TourOperator

#### **TourAcceptanceReport Table:**
- TotalExtraCost tự động cập nhật khi extraCost thay đổi
- Tính tổng tất cả note của guide cho booking

### 🔔 **Notifications:**

#### **Khi TourGuide tạo note:**
- Customer nhận: "Tour guide {name} has added a note to your booking #{id}"

#### **Khi TourOperator update extra cost:**
- Customer nhận: "Extra cost for your booking #{id} has been updated to ${amount}"

### ✅ **Kết quả:**

- ✅ **Build thành công**
- ✅ **Phân quyền rõ ràng** giữa TourGuide và TourOperator
- ✅ **TourGuide không thể set extraCost**
- ✅ **TourOperator có quyền update extraCost**
- ✅ **Tự động cập nhật report**
- ✅ **Notifications đầy đủ**
- ✅ **API documentation hoàn chỉnh**

**Bây giờ hệ thống có phân quyền rõ ràng: TourGuide tạo note, TourOperator quyết định extra cost!** 🎯 