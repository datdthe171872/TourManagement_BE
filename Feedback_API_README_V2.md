# Feedback API Documentation (Version 2)

## Overview
Hệ thống Feedback API đã được cập nhật với các thay đổi sau:
- **Xóa bỏ**: API `GET /api/Feedback` và `GET /api/Feedback/{id}` (public)
- **Thêm mới**: API cho Admin và Tour Operator với quyền truy cập riêng biệt
- **Tính năng mới**: Hệ thống báo cáo feedback cho Tour Operator

## API Endpoints

### 🔐 Admin APIs

#### 1. Get All Feedbacks (Admin Only)
```
GET /api/Feedback/admin
```

**Authorization**: `Admin` role required

**Query Parameters**:
- `Username` (optional): Tìm kiếm theo tên người dùng
- `RatingId` (optional): Tìm kiếm theo ID feedback
- `PageNumber` (default: 1): Số trang
- `PageSize` (default: 10): Số lượng item mỗi trang

**Response**:
```json
{
  "message": "Lấy danh sách feedback thành công",
  "data": {
    "feedbacks": [
      {
        "ratingId": 1,
        "tourId": 1,
        "userId": 1,
        "rating": 5,
        "comment": "Tour rất tuyệt vời!",
        "mediaUrl": "https://res.cloudinary.com/...",
        "createdAt": "2024-01-01T10:00:00Z",
        "isActive": true,
        "tourName": "Tour Hà Nội - Sapa",
        "userName": "customer1",
        "userEmail": "customer1@example.com"
      }
    ],
    "totalCount": 50,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5
  }
}
```

#### 2. Update Feedback Status (Admin Only)
```
PUT /api/Feedback/update-status
```

**Authorization**: `Admin` role required

**Request Body**:
```json
{
  "ratingId": 1,
  "isActive": false
}
```

**Response**:
```json
{
  "message": "Cập nhật trạng thái feedback thành công - đã ẩn"
}
```

### 🏢 Tour Operator APIs

#### 1. Get Feedbacks for Tour Operator's Tours
```
GET /api/Feedback/tour-operator
```

**Authorization**: `Tour Operator` role required

**Query Parameters**:
- `RatingId` (optional): Tìm kiếm theo ID feedback
- `PageNumber` (default: 1): Số trang
- `PageSize` (default: 10): Số lượng item mỗi trang

**Response**:
```json
{
  "message": "Lấy danh sách feedback thành công",
  "data": {
    "feedbacks": [
      {
        "ratingId": 1,
        "tourId": 1,
        "userId": 1,
        "rating": 5,
        "comment": "Tour rất tuyệt vời!",
        "mediaUrl": "https://res.cloudinary.com/...",
        "createdAt": "2024-01-01T10:00:00Z",
        "isActive": true,
        "tourName": "Tour Hà Nội - Sapa",
        "userName": "customer1",
        "userEmail": "customer1@example.com"
      }
    ],
    "totalCount": 25,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 3
  }
}
```

#### 2. Report Feedback
```
POST /api/Feedback/report
```

**Authorization**: `Tour Operator` role required

**Request Body**:
```json
{
  "ratingId": 1,
  "reason": "Feedback này chứa nội dung không phù hợp và xúc phạm đến dịch vụ của chúng tôi"
}
```

**Response**:
```json
{
  "message": "Báo cáo feedback thành công. Admin sẽ được thông báo về vấn đề này."
}
```

**Notification**: Khi Tour Operator báo cáo feedback, tất cả Admin sẽ nhận được notification với thông tin chi tiết.

### 👤 Customer APIs

#### 1. Create Feedback (with image upload)
```
POST /api/Feedback
```

**Authorization**: Any authenticated user

**Content-Type**: `multipart/form-data`

**Form Data**:
- `TourId` (required): ID của tour
- `Rating` (optional): Đánh giá từ 1-5
- `Comment` (optional): Nội dung feedback
- `ImageFile` (optional): File ảnh (jpg, jpeg, png, gif, max 10MB)

**Response**:
```json
{
  "message": "Tạo feedback thành công",
  "data": {
    "ratingId": 1,
    "tourId": 1,
    "userId": 1,
    "rating": 5,
    "comment": "Tour rất tuyệt vời!",
    "mediaUrl": "https://res.cloudinary.com/...",
    "createdAt": "2024-01-01T10:00:00Z",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "customer1",
    "userEmail": "customer1@example.com"
  }
}
```

#### 2. Get My Feedbacks
```
GET /api/Feedback/my-feedbacks
```

**Authorization**: Any authenticated user

**Response**:
```json
{
  "message": "Lấy danh sách feedback thành công",
  "data": {
    "feedbacks": [
      {
        "ratingId": 1,
        "tourId": 1,
        "userId": 1,
        "rating": 5,
        "comment": "Tour rất tuyệt vời!",
        "mediaUrl": "https://res.cloudinary.com/...",
        "createdAt": "2024-01-01T10:00:00Z",
        "isActive": true,
        "tourName": "Tour Hà Nội - Sapa",
        "userName": "customer1",
        "userEmail": "customer1@example.com"
      }
    ],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1
  }
}
```

## Tính năng mới

### 🔔 Notification System

#### Feedback Violation Notification
Khi Admin ẩn một feedback, user sẽ nhận được notification:
```
Title: "Vi phạm tiêu chuẩn cộng đồng"
Message: "Feedback của bạn đã vi phạm tiêu chuẩn cộng đồng và đã bị ẩn."
```

#### Feedback Report Notification
Khi Tour Operator báo cáo feedback, Admin sẽ nhận được notification:
```
Title: "Báo cáo Feedback mới"
Message: "Có một feedback (ID: 1) bị báo cáo bởi Tour Operator (ID: 1). Lý do: Feedback này chứa nội dung không phù hợp..."
```

## Thay đổi so với Version 1

### ❌ Đã xóa bỏ:
- `GET /api/Feedback` - API public lấy tất cả feedback
- `GET /api/Feedback/{id}` - API public lấy chi tiết feedback

### ✅ Đã thêm mới:
- `GET /api/Feedback/admin` - API Admin lấy tất cả feedback với search
- `GET /api/Feedback/tour-operator` - API Tour Operator lấy feedback của họ
- `POST /api/Feedback/report` - API Tour Operator báo cáo feedback

### 🔄 Đã cải thiện:
- Phân quyền rõ ràng cho từng role
- Hệ thống notification cho Admin khi có báo cáo
- Sắp xếp feedback theo thời gian tạo mới nhất
- Tìm kiếm linh hoạt theo Username và RatingId

## Lưu ý bảo mật

1. **Admin APIs**: Chỉ Admin mới có thể truy cập
2. **Tour Operator APIs**: Chỉ Tour Operator mới có thể truy cập feedback của tour của họ
3. **Customer APIs**: Bất kỳ user đã đăng nhập nào cũng có thể tạo và xem feedback của mình
4. **Image Upload**: Chỉ chấp nhận file ảnh, kích thước tối đa 10MB
5. **Report System**: Tour Operator chỉ có thể báo cáo feedback của tour thuộc về họ

## Error Handling

### Common Error Responses:

**401 Unauthorized**:
```json
{
  "message": "Token không hợp lệ hoặc đã hết hạn"
}
```

**403 Forbidden**:
```json
{
  "message": "Bạn không có quyền truy cập API này"
}
```

**404 Not Found**:
```json
{
  "message": "Không tìm thấy feedback với id này."
}
```

**500 Internal Server Error**:
```json
{
  "message": "Có lỗi xảy ra khi xử lý yêu cầu",
  "error": "Chi tiết lỗi"
}
``` 