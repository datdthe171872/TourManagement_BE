# Hướng dẫn Test API GuideNote và TourAcceptanceReport

## 📋 Chuẩn bị

### 1. Cài đặt công cụ
- **VS Code** với extension **REST Client**
- **Postman** (tùy chọn)
- **Database** đã có dữ liệu test

### 2. Dữ liệu test cần thiết
```sql
-- Tạo User cho Tour Guide
INSERT INTO Users (UserName, Email, Password, RoleName, IsActive) 
VALUES ('guide1', 'guide1@example.com', 'hashed_password', 'Tour Guide', 1);

-- Tạo Tour Guide
INSERT INTO TourGuides (UserId, IsActive) 
VALUES (1, 1);

-- Tạo User cho khách hàng
INSERT INTO Users (UserName, Email, Password, RoleName, IsActive) 
VALUES ('customer1', 'customer1@example.com', 'hashed_password', 'Customer', 1);

-- Tạo Booking
INSERT INTO Bookings (UserId, TourId, BookingDate, Status, IsActive) 
VALUES (2, 1, GETDATE(), 'Confirmed', 1);

-- Tạo Tour Guide Assignment
INSERT INTO TourGuideAssignments (TourGuideId, BookingId, AssignmentDate, IsActive) 
VALUES (1, 1, GETDATE(), 1);
```

### 3. Lấy JWT Token
```http
POST {{baseUrl}}/api/Auth/login
Content-Type: application/json

{
  "email": "guide1@example.com",
  "password": "password123"
}
```

## 🚀 Luồng Test Cơ Bản

### **Bước 1: Test tạo Note đầu tiên**
```http
POST {{baseUrl}}/api/GuideNote/notes
Authorization: Bearer {guide_token}
Content-Type: application/json

{
  "assignmentId": 1,
  "title": "Bắt đầu tour",
  "content": "Đã đón khách tại sân bay, thời tiết đẹp",
  "extraCost": 0.00,
  "mediaUrls": []
}
```

**Kết quả mong đợi:**
- Status: 200 OK
- Note được tạo thành công
- **TourAcceptanceReport được tạo tự động** với `TotalExtraCost = 0`

### **Bước 2: Kiểm tra Report tự động**
```http
GET {{baseUrl}}/api/TourAcceptanceReport
Authorization: Bearer {guide_token}
```

**Kết quả mong đợi:**
```json
[
  {
    "reportId": 1,
    "bookingId": 1,
    "tourGuideId": 1,
    "tourGuideName": "guide1",
    "reportDate": "2024-01-15T10:30:00Z",
    "summary": "Auto-generated report",
    "totalExtraCost": 0.00,
    "notes": "Auto-generated report",
    "guideNotes": [
      {
        "noteId": 1,
        "title": "Bắt đầu tour",
        "content": "Đã đón khách tại sân bay, thời tiết đẹp",
        "extraCost": 0.00
      }
    ]
  }
]
```

### **Bước 3: Thêm Note với chi phí phát sinh**
```http
POST {{baseUrl}}/api/GuideNote/notes
Authorization: Bearer {guide_token}
Content-Type: application/json

{
  "assignmentId": 1,
  "title": "Chi phí phát sinh - Ăn trưa",
  "content": "Khách yêu cầu ăn tại nhà hàng cao cấp",
  "extraCost": 100.00,
  "mediaUrls": ["https://example.com/receipt.jpg"]
}
```

**Kết quả mong đợi:**
- Status: 200 OK
- Note được tạo thành công
- **Report được cập nhật tự động** với `TotalExtraCost = 100.00`

### **Bước 4: Kiểm tra Report đã cập nhật**
```http
GET {{baseUrl}}/api/TourAcceptanceReport/1
Authorization: Bearer {guide_token}
```

**Kết quả mong đợi:**
```json
{
  "reportId": 1,
  "totalExtraCost": 100.00,
  "guideNotes": [
    {
      "noteId": 1,
      "extraCost": 0.00
    },
    {
      "noteId": 2,
      "extraCost": 100.00
    }
  ]
}
```

## 🔄 Luồng Test Nâng Cao

### **Test cập nhật Note**
```http
PUT {{baseUrl}}/api/GuideNote/notes/1
Authorization: Bearer {guide_token}
Content-Type: application/json

{
  "title": "Bắt đầu tour (Đã cập nhật)",
  "content": "Đã đón khách tại sân bay, thời tiết đẹp. Khách đến đúng giờ.",
  "mediaUrls": ["https://example.com/airport-photo.jpg"]
}
```

### **Test cập nhật Report (Notes và AttachmentUrl)**
```http
PUT {{baseUrl}}/api/TourAcceptanceReport/1
Authorization: Bearer {guide_token}
Content-Type: application/json

{
  "notes": "Tour hoàn thành thành công. Khách rất hài lòng với dịch vụ.",
  "attachmentUrl": "https://example.com/final-report.pdf"
}
```

### **Test xem Report**
```http
GET {{baseUrl}}/api/TourAcceptanceReport/1
Authorization: Bearer {guide_token}
```

**Lưu ý:** 
- TotalExtraCost được cập nhật tự động khi thêm/sửa/xóa Note
- Notes và AttachmentUrl có thể chỉnh sửa thủ công

### **Test Admin/Operator view**
```http
GET {{baseUrl}}/api/TourAcceptanceReport/booking/1
Authorization: Bearer {admin_token}
```

## ❌ Test Error Cases

### **1. Test với Assignment không tồn tại**
```http
POST {{baseUrl}}/api/GuideNote/notes
Authorization: Bearer {guide_token}
Content-Type: application/json

{
  "assignmentId": 999,
  "title": "Test Error",
  "content": "Test assignment không tồn tại",
  "extraCost": 0.00,
  "mediaUrls": []
}
```

**Kết quả mong đợi:**
```json
{
  "error": "Assignment not found",
  "statusCode": 404
}
```

### **2. Test với quyền không đủ**
```http
POST {{baseUrl}}/api/GuideNote/notes
Authorization: Bearer {customer_token}
Content-Type: application/json

{
  "assignmentId": 1,
  "title": "Test Error",
  "content": "Test quyền không đủ",
  "extraCost": 0.00,
  "mediaUrls": []
}
```

**Kết quả mong đợi:**
```json
{
  "error": "Forbidden",
  "statusCode": 403
}
```

## 🔔 Test Notification

### **Kiểm tra notification sau khi tạo Note**
```http
GET {{baseUrl}}/api/Notification
Authorization: Bearer {customer_token}
```

**Kết quả mong đợi:**
```json
{
  "notifications": [
    {
      "notificationId": 1,
      "title": "Ghi chú hướng dẫn mới",
      "message": "Bạn có một ghi chú hướng dẫn mới. Hãy kiểm tra để biết thêm chi tiết.",
      "type": "GuideNote",
      "relatedEntityId": "1",
      "isRead": false
    }
  ]
}
```

## 📊 Test Scenarios

### **Scenario 1: Tour hoàn thành bình thường**
1. Tạo note "Bắt đầu tour" (extraCost: 0)
2. Tạo note "Ăn trưa" (extraCost: 50)
3. Tạo note "Kết thúc tour" (extraCost: 25)
4. Cập nhật report tổng kết
5. Kiểm tra TotalExtraCost = 75

### **Scenario 2: Tour có nhiều chi phí phát sinh**
1. Tạo note "Bắt đầu tour" (extraCost: 0)
2. Tạo note "Thay đổi lịch trình" (extraCost: 100)
3. Tạo note "Ăn tối cao cấp" (extraCost: 200)
4. Tạo note "Dịch vụ đặc biệt" (extraCost: 150)
5. Kiểm tra TotalExtraCost = 450

### **Scenario 3: Tour có vấn đề**
1. Tạo note "Bắt đầu tour" (extraCost: 0)
2. Tạo note "Thời tiết xấu" (extraCost: 50)
3. Tạo note "Khách không hài lòng" (extraCost: 0)
4. Cập nhật report với ghi chú vấn đề

## 🛠️ Debug Tips

### **1. Kiểm tra Database**
```sql
-- Kiểm tra Notes
SELECT * FROM GuideNotes WHERE IsActive = 1;

-- Kiểm tra Reports
SELECT * FROM TourAcceptanceReports WHERE IsActive = 1;

-- Kiểm tra Media
SELECT * FROM GuideNoteMedia WHERE IsActive = 1;

-- Kiểm tra Notifications
SELECT * FROM Notifications WHERE Type IN ('GuideNote', 'TourAcceptanceReport');
```

### **2. Kiểm tra Logs**
- Xem console logs của application
- Kiểm tra database logs
- Xem network requests trong browser dev tools

### **3. Common Issues**
- **Token expired**: Lấy token mới
- **Assignment not found**: Kiểm tra dữ liệu test
- **Permission denied**: Kiểm tra role của user
- **Database connection**: Kiểm tra connection string

## 📝 Checklist Test

- [ ] Tạo note đầu tiên → Report tự động tạo
- [ ] Thêm note với extraCost → Report cập nhật tổng chi phí
- [ ] Cập nhật note → Thay đổi được lưu
- [ ] Xóa note → Note bị ẩn (soft delete)
- [ ] Report tự động tạo khi có note đầu tiên
- [ ] Report tự động cập nhật TotalExtraCost khi thêm/sửa/xóa note
- [ ] Report có thể cập nhật Notes và AttachmentUrl thủ công
- [ ] Admin xem reports → Có quyền truy cập
- [ ] Notification được tạo → User nhận được thông báo
- [ ] Error handling → Trả về lỗi phù hợp
- [ ] Authorization → Chỉ đúng role mới truy cập được

## 🎯 Kết quả mong đợi

Sau khi test hoàn tất, bạn sẽ có:
- ✅ API hoạt động ổn định
- ✅ Business logic đúng (auto-report, cost tracking)
- ✅ Authorization hoạt động
- ✅ Notification system hoạt động
- ✅ Error handling tốt
- ✅ Data integrity được đảm bảo 