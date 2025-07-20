# Notification API Documentation

## Tổng quan
Notification API cung cấp các endpoint để quản lý thông báo cho người dùng trong hệ thống quản lý tour.

## Các loại Notification
- **Booking**: Thông báo khi đặt tour thành công
- **GuideNote**: Thông báo khi có ghi chú hướng dẫn mới
- **Feedback**: Thông báo khi có phản hồi mới
- **Registration**: Thông báo khi đăng ký tài khoản thành công

## Endpoints

### 1. Lấy danh sách notifications
```
GET /api/Notification?page={page}&pageSize={pageSize}
```

**Headers:**
- Authorization: Bearer {token}

**Query Parameters:**
- page (optional): Số trang (mặc định: 1)
- pageSize (optional): Số lượng item mỗi trang (mặc định: 10)

**Response:**
```json
{
  "notifications": [
    {
      "notificationId": 1,
      "userId": 1,
      "title": "Đặt tour thành công!",
      "message": "Bạn đã đặt tour thành công. Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận chi tiết.",
      "type": "Booking",
      "relatedEntityId": "123",
      "isRead": false,
      "createdAt": "2024-01-01T10:00:00Z"
    }
  ],
  "totalCount": 5,
  "unreadCount": 3
}
```

### 2. Lấy chi tiết notification
```
GET /api/Notification/{notificationId}
```

**Headers:**
- Authorization: Bearer {token}

**Response:**
```json
{
  "notificationId": 1,
  "userId": 1,
  "title": "Đặt tour thành công!",
  "message": "Bạn đã đặt tour thành công. Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận chi tiết.",
  "type": "Booking",
  "relatedEntityId": "123",
  "isRead": false,
  "createdAt": "2024-01-01T10:00:00Z"
}
```

### 3. Đánh dấu notification đã đọc
```
PUT /api/Notification/{notificationId}/mark-read
```

**Headers:**
- Authorization: Bearer {token}

**Response:**
```json
{
  "message": "Đã đánh dấu notification đã đọc"
}
```

### 4. Đánh dấu tất cả notifications đã đọc
```
PUT /api/Notification/mark-all-read
```

**Headers:**
- Authorization: Bearer {token}

**Response:**
```json
{
  "message": "Đã đánh dấu tất cả notifications đã đọc"
}
```

### 5. Lấy số lượng notifications chưa đọc
```
GET /api/Notification/unread-count
```

**Headers:**
- Authorization: Bearer {token}

**Response:**
```json
3
```

## Tự động tạo Notification

Hệ thống sẽ tự động tạo notification trong các trường hợp sau:

### 1. Khi tạo Booking thành công
- **Type**: "Booking"
- **Title**: "Đặt tour thành công!"
- **Message**: "Bạn đã đặt tour thành công. Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận chi tiết."

### 2. Khi tạo GuideNote
- **Type**: "GuideNote"
- **Title**: "Ghi chú hướng dẫn mới"
- **Message**: "Bạn có một ghi chú hướng dẫn mới. Hãy kiểm tra để biết thêm chi tiết."

### 3. Khi tạo Feedback
- **Type**: "Feedback"
- **Title**: "Phản hồi mới"
- **Message**: "Bạn có một phản hồi mới. Hãy kiểm tra để biết thêm chi tiết."

### 4. Khi đăng ký thành công
- **Type**: "Registration"
- **Title**: "Đăng ký thành công!"
- **Message**: "Chào mừng bạn đến với hệ thống quản lý tour. Tài khoản của bạn đã được tạo thành công."

## Error Responses

### 401 Unauthorized
```json
{
  "message": "Token không hợp lệ"
}
```

### 404 Not Found
```json
{
  "message": "Không tìm thấy notification"
}
```

### 500 Internal Server Error
```json
{
  "message": "Lỗi server: {error message}"
}
```

## Database Schema

### Bảng Notifications
```sql
CREATE TABLE Notifications (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    RelatedEntityId NVARCHAR(100) NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
```

## Testing

Sử dụng file `Notification_API_Test.http` để test các API endpoints.

## Lưu ý
- Tất cả endpoints đều yêu cầu authentication
- User chỉ có thể truy cập notifications của chính mình
- Notifications được sắp xếp theo thời gian tạo mới nhất
- RelatedEntityId chứa ID của entity liên quan (BookingId, GuideNoteId, etc.) 