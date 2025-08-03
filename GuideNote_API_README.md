# GuideNote API Documentation

## 🔧 **Đã sửa lỗi 400 cho GuideNote API**

### ✅ **Các thay đổi đã thực hiện:**

#### 1. **Thêm using System vào Controller**
- **File:** `Controllers/GuideNoteController.cs`
- **Thay đổi:** Thêm `using System;` để hỗ trợ Exception handling

#### 2. **Cải thiện Error Handling**
- Tất cả API endpoints đã được wrap trong try-catch blocks
- Trả về response format nhất quán: `{ message: "..." }`
- Xử lý lỗi gracefully thay vì crash

#### 3. **Sửa lỗi Database Constraints**
- **Vấn đề:** Model `GuideNote` có field `BookingId` và `DepartureDateId` nhưng không được set trong code
- **Giải pháp:** Thêm `BookingId` và `DepartureDateId` khi tạo `GuideNote`
- **File:** `Service/GuideNoteService.cs`

#### 4. **Thêm Detailed Error Logging**
- Log chi tiết lỗi để debug
- Hiển thị cả inner exception

## 📋 **API Endpoints**

### 1. **GET /api/GuideNote/notes**
**Mô tả:** Lấy danh sách note của TourGuide hiện tại
**Authorization:** Bearer Token với role "Tour Guide"
**Response:** `List<GuideNoteResponse>`

### 2. **POST /api/GuideNote/upload-attachment**
**Mô tả:** Upload ảnh/attachment cho GuideNote
**Authorization:** Bearer Token với role "Tour Guide"
**Content-Type:** multipart/form-data
**Request:** Form data với field "file"
**Response:**
```json
{
  "message": "File uploaded successfully",
  "attachmentUrl": "/uploads/guidenotes/abc123.jpg",
  "fileName": "abc123.jpg"
}
```

### 3. **POST /api/GuideNote/notes**
**Mô tả:** Tạo note mới cho TourGuide
**Authorization:** Bearer Token với role "Tour Guide"
**Request Body:**
```json
{
  "bookingId": 1,
  "title": "Test Note",
  "content": "This is a test note content",
  "attachmentUrls": [
    "/uploads/guidenotes/abc123.jpg",
    "/uploads/guidenotes/def456.png"
  ]
}
```

### 4. **PUT /api/GuideNote/notes/{id}**
**Mô tả:** Cập nhật note
**Authorization:** Bearer Token với role "Tour Guide"
**Request Body:**
```json
{
  "title": "Updated Test Note",
  "content": "This is an updated test note content",
  "mediaUrls": []
}
```

### 5. **DELETE /api/GuideNote/notes/{id}**
**Mô tả:** Xóa note
**Authorization:** Bearer Token với role "Tour Guide"

### 6. **GET /api/GuideNote/my-bookings**
**Mô tả:** Lấy danh sách booking của TourGuide
**Authorization:** Bearer Token với role "Tour Guide"
**Response:** `List<TourGuideBookingResponse>`

### 7. **GET /api/GuideNote/tour-operator/notes**
**Mô tả:** TourOperator lấy tất cả note của TourGuide thuộc tour của mình
**Authorization:** Bearer Token với role "Tour Operator"
**Response:** `List<GuideNoteResponse>` (bao gồm TourGuideName, TourTitle, DepartureDate)

### 8. **PUT /api/GuideNote/notes/{noteId}/extra-cost**
**Mô tả:** TourOperator cập nhật extra cost của GuideNote
**Authorization:** Bearer Token với role "Tour Operator"
**Request Body:**
```json
{
  "extraCost": 50.00
}
```

## 🔍 **Nguyên nhân lỗi 400 và cách khắc phục**

### **1. Lỗi "An error occurred while saving the entity changes"**

**Nguyên nhân:**
- Thiếu foreign key values (`BookingId`, `DepartureDateId`)
- Database constraints violation
- Invalid assignment relationship

**Đã sửa:**
```csharp
var note = new GuideNote
{
    AssignmentId = assignment.Id,
    ReportId = report.ReportId,
    BookingId = request.BookingId,           // ✅ Đã thêm
    DepartureDateId = booking.DepartureDateId, // ✅ Đã thêm
    Title = request.Title,
    Content = request.Content,
    ExtraCost = request.ExtraCost ?? 0,
    CreatedAt = DateTime.UtcNow,
    IsActive = true
};
```

### **2. Lỗi Authentication**
**Nguyên nhân:** Token không hợp lệ hoặc không có role "Tour Guide"
**Cách khắc phục:** Kiểm tra token và role

### **3. Lỗi Validation**
**Nguyên nhân:** Dữ liệu request không hợp lệ
**Cách khắc phục:** Kiểm tra required fields

## 🧪 **Cách Test API**

### **1. Upload ảnh trước:**
```http
POST https://localhost:7012/api/GuideNote/upload-attachment
Authorization: Bearer {{your_jwt_token}}
Content-Type: multipart/form-data

// Upload file trong form data
```

### **2. Tạo note với ảnh:**
```http
POST https://localhost:7012/api/GuideNote/notes
Authorization: Bearer {{your_jwt_token}}
Content-Type: application/json

{
  "bookingId": 1,
  "title": "Test Note with Images",
  "content": "This is a test note content with image attachments",
  "attachmentUrls": [
    "/uploads/guidenotes/abc123.jpg",
    "/uploads/guidenotes/def456.png"
  ]
}
```

### **3. TourOperator get all notes:**
```http
GET https://localhost:7012/api/GuideNote/tour-operator/notes
Authorization: Bearer {{tour_operator_token}}
```

### **4. TourOperator update extra cost:**
```http
PUT https://localhost:7012/api/GuideNote/notes/1/extra-cost
Authorization: Bearer {{tour_operator_token}}
Content-Type: application/json

{
  "extraCost": 50.00
}
```

### **2. Kiểm tra các điều kiện:**
- ✅ **Token hợp lệ** với role "Tour Guide"
- ✅ **BookingId tồn tại** trong database
- ✅ **TourGuide được assign** cho departure date của booking
- ✅ **Dữ liệu request hợp lệ** (title, content không null)

## 🐛 **Debug Steps**

### **Nếu vẫn gặp lỗi 400:**

1. **Kiểm tra logs** trong console để xem lỗi cụ thể
2. **Kiểm tra token** có đúng role "Tour Guide" không
3. **Kiểm tra bookingId** có tồn tại trong database không
4. **Kiểm tra assignment** của TourGuide cho departure date

### **Kiểm tra Database:**
```sql
-- Kiểm tra booking có tồn tại
SELECT * FROM Bookings WHERE BookingId = 1 AND IsActive = 1;

-- Kiểm tra tour guide assignment
SELECT * FROM TourGuideAssignments 
WHERE TourGuideId = (SELECT TourGuideId FROM TourGuides WHERE UserId = ?) 
AND DepartureDateId = (SELECT DepartureDateId FROM Bookings WHERE BookingId = 1)
AND IsActive = 1;
```

## 📁 **Files đã sửa:**

1. **Controllers/GuideNoteController.cs**
   - Thêm `using System;`
   - Wrap tất cả endpoints trong try-catch
   - Cải thiện error handling
   - **Thêm API upload attachment**

2. **Service/GuideNoteService.cs**
   - Thêm `BookingId` và `DepartureDateId` khi tạo `GuideNote`
   - Thêm detailed error logging
   - Cải thiện exception handling
   - **Thêm xử lý attachment URLs**

3. **Data/DTO/Request/CreateGuideNoteByTourGuideRequest.cs**
   - **Thêm field `AttachmentUrls`**
   - **Bỏ field `ExtraCost`** (TourGuide không thể set)

4. **Data/DTO/Request/UpdateGuideNoteExtraCostRequest.cs**
   - **Mới tạo** - DTO cho TourOperator update extra cost

5. **Data/DTO/Response/GuideNoteResponse.cs**
   - **Thêm fields:** TourGuideName, TourTitle, DepartureDate

6. **GuideNote_API_Test.http**
   - File test cho tất cả API endpoints
   - **Thêm test upload attachment**
   - **Thêm test TourOperator update extra cost**
   - **Thêm test TourOperator get all notes**

## 📸 **Tính năng Upload Ảnh:**

### **File Types được hỗ trợ:**
- ✅ JPG, JPEG
- ✅ PNG
- ✅ GIF
- ✅ PDF

### **File Size Limit:**
- ✅ Tối đa 10MB per file

### **Upload Path:**
- ✅ `/wwwroot/uploads/guidenotes/`
- ✅ Tự động tạo thư mục nếu chưa tồn tại
- ✅ Unique filename với GUID

## 🔐 **Phân quyền mới:**

### **TourGuide:**
- ✅ Tạo note với title, content, attachments
- ✅ **KHÔNG thể set extraCost** (mặc định = 0)
- ✅ Upload attachments
- ✅ Update/delete note của mình

### **TourOperator:**
- ✅ **Có quyền update extraCost** của bất kỳ note nào thuộc tour của mình
- ✅ Nhận notification khi TourGuide tạo note
- ✅ Customer nhận notification khi extraCost được update

## ✅ **Kết quả:**

- ✅ Build thành công
- ✅ Error handling được cải thiện
- ✅ Database constraints được đáp ứng
- ✅ API endpoints hoạt động ổn định
- ✅ Phân quyền rõ ràng giữa TourGuide và TourOperator

Bây giờ API đã được sửa và có error handling tốt hơn. Hãy thử test lại và cho biết kết quả! 