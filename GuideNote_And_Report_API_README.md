# GuideNote và TourAcceptanceReport API Documentation

## Tổng quan

Hệ thống quản lý ghi chú và báo cáo cho Tour Guide bao gồm:

1. **GuideNote**: Ghi chú trong quá trình thực hiện tour
2. **TourAcceptanceReport**: Báo cáo hoàn thành tour

## Cấu trúc nghiệp vụ

### GuideNote (Ghi chú)
- Liên kết với `TourGuideAssignment` và `TourAcceptanceReport`
- Chứa thông tin ghi chú, chi phí phát sinh, media
- Được tạo trong quá trình thực hiện tour
- Tự động tạo/cập nhật TourAcceptanceReport

### TourAcceptanceReport (Báo cáo)
- Tổng hợp tất cả notes và chi phí phát sinh
- Được tạo sau khi tour hoàn thành
- Chứa tổng chi phí phát sinh và ghi chú tổng kết

## API Endpoints

### 1. GuideNote API

#### Lấy danh sách notes của guide hiện tại
```http
GET /api/GuideNote/notes
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "noteId": 1,
    "assignmentId": 1,
    "reportId": 1,
    "title": "Ghi chú về thời tiết",
    "content": "Thời tiết xấu, cần thay đổi lịch trình",
    "extraCost": 50.00,
    "createdAt": "2024-01-15T10:30:00Z",
    "mediaUrls": ["url1", "url2"]
  }
]
```

#### Tạo note mới
```http
POST /api/GuideNote/notes
Authorization: Bearer {token}
Content-Type: application/json

{
  "assignmentId": 1,
  "title": "Ghi chú mới",
  "content": "Nội dung ghi chú",
  "extraCost": 25.00,
  "mediaUrls": ["url1", "url2"]
}
```

#### Cập nhật note
```http
PUT /api/GuideNote/notes/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Ghi chú đã cập nhật",
  "content": "Nội dung mới",
  "mediaUrls": ["url1", "url2"]
}
```

#### Xóa note
```http
DELETE /api/GuideNote/notes/{id}
Authorization: Bearer {token}
```

### 2. TourAcceptanceReport API

#### Lấy danh sách reports của guide hiện tại
```http
GET /api/TourAcceptanceReport
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "reportId": 1,
    "bookingId": 1,
    "tourGuideId": 1,
    "tourGuideName": "John Doe",
    "reportDate": "2024-01-15T10:30:00Z",
    "summary": "Tổng kết tour",
    "totalExtraCost": 75.00,
    "notes": "Ghi chú tổng kết",
    "attachmentUrl": "url",
    "isActive": true,
    "guideNotes": [
      {
        "noteId": 1,
        "assignmentId": 1,
        "title": "Ghi chú 1",
        "content": "Nội dung 1",
        "extraCost": 25.00,
        "createdAt": "2024-01-15T10:30:00Z",
        "mediaUrls": ["url1"]
      }
    ]
  }
]
```

#### Lấy chi tiết report theo ID
```http
GET /api/TourAcceptanceReport/{id}
Authorization: Bearer {token}
```

#### Cập nhật report (chỉ Notes và AttachmentUrl)
```http
PUT /api/TourAcceptanceReport/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "notes": "Ghi chú tổng kết từ guide",
  "attachmentUrl": "url_file_báo_cáo"
}
```

#### Lưu ý về TourAcceptanceReport
TourAcceptanceReport được quản lý bán tự động:

- **Tạo**: Tự động khi tạo note đầu tiên cho một booking
- **Cập nhật TotalExtraCost**: Tự động khi thêm/sửa/xóa note
- **Cập nhật Notes/AttachmentUrl**: Thủ công qua PUT endpoint
- **Xóa**: Không cần thiết (soft delete qua note)

#### Lấy reports theo booking ID (Admin/Operator)
```http
GET /api/TourAcceptanceReport/booking/{bookingId}
Authorization: Bearer {token}
```

## Luồng nghiệp vụ

### 1. Trong quá trình tour
1. Guide tạo notes với chi phí phát sinh
2. Hệ thống tự động tạo/cập nhật TourAcceptanceReport
3. Tổng chi phí được tính toán tự động

### 2. Sau khi tour hoàn thành
1. Guide có thể tạo báo cáo tổng kết
2. Admin/Operator có thể xem tất cả reports

## Tính năng đặc biệt

### Auto-report generation
- Tự động tạo report khi có note đầu tiên
- Cập nhật tổng chi phí tự động

### Cost tracking
- Theo dõi chi phí phát sinh từng note
- Tổng hợp chi phí trong report

### Media support
- Hỗ trợ đính kèm media cho notes
- Lưu trữ URL media

### Notification
- Thông báo cho user khi có note/report mới
- Hỗ trợ nhiều loại notification

### Authorization
- Phân quyền theo role (Guide/Admin/Operator)
- Kiểm tra quyền truy cập

## API Design

Các API được thiết kế theo RESTful conventions với cấu trúc rõ ràng:

- **GuideNote**: `/api/GuideNote/notes/*`
- **TourAcceptanceReport**: `/api/TourAcceptanceReport/*`

## Lưu ý

1. **Authentication**: Tất cả API đều yêu cầu JWT token
2. **Authorization**: Chỉ Tour Guide mới có thể tạo/sửa/xóa notes và reports
3. **Validation**: Dữ liệu đầu vào được validate tự động
4. **Soft Delete**: Sử dụng soft delete để bảo toàn dữ liệu
5. **Audit Trail**: Tất cả thay đổi đều được ghi log

## Error Handling

```json
{
  "error": "Guide not found",
  "statusCode": 404
}
```

Các lỗi thường gặp:
- `Guide not found`: Guide không tồn tại
- `Assignment not found`: Assignment không thuộc guide
- `Report not found`: Report không tồn tại
- `Not your note`: Không có quyền truy cập note
- `Report already exists`: Đã có report cho booking này 