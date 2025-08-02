# Feedback API Documentation - Updated Version

## Overview

API Feedback đã được cập nhật với các tính năng mới:
- Upload ảnh trực tiếp khi tạo feedback
- Quản lý trạng thái IsActive cho Admin
- Notification tự động khi feedback bị ẩn
- Authorization cho các endpoint cần thiết

## Endpoints

### 1. Create Feedback with Image Upload

**Endpoint:** `POST /api/Feedback`

**Authorization:** Required (Bearer Token)

**Content-Type:** `multipart/form-data`

**Request Parameters:**
- `TourId` (int, required): ID của tour
- `Rating` (int, optional): Đánh giá từ 1-5
- `Comment` (string, optional): Nội dung feedback
- `ImageFile` (file, optional): File ảnh (jpg, jpeg, png, gif, max 10MB)

**Response:**
```json
{
  "message": "Tạo feedback thành công",
  "data": {
    "ratingId": 1,
    "tourId": 1,
    "userId": 123,
    "rating": 5,
    "comment": "Tour rất tuyệt vời!",
    "mediaUrl": "https://res.cloudinary.com/...",
    "createdAt": "2024-01-01T00:00:00Z",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "john_doe",
    "userEmail": "john@example.com"
  }
}
```

### 2. Get My Feedbacks

**Endpoint:** `GET /api/Feedback/my-feedbacks`

**Authorization:** Required (Bearer Token)

**Response:**
```json
{
  "message": "Lấy danh sách feedback thành công",
  "data": {
    "feedbacks": [...],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 5,
    "totalPages": 1
  }
}
```

### 3. Update Feedback Status (Admin Only)

**Endpoint:** `PUT /api/Feedback/update-status`

**Authorization:** Required (Admin Role)

**Content-Type:** `application/json`

**Request Body:**
```json
{
  "ratingId": 1,
  "isActive": false
}
```

**Response:**
```json
{
  "message": "Cập nhật trạng thái feedback thành công - đã ẩn"
}
```

### 4. Get All Feedbacks (Public)

**Endpoint:** `GET /api/Feedback`

**Authorization:** Not Required

**Query Parameters:**
- `TourId` (int, optional): Lọc theo tour
- `UserId` (int, optional): Lọc theo user
- `Rating` (int, optional): Lọc theo rating
- `PageNumber` (int, default: 1): Số trang
- `PageSize` (int, default: 10): Số item trên trang

### 5. Get Feedback Detail

**Endpoint:** `GET /api/Feedback/{id}`

**Authorization:** Not Required

## Features

### Image Upload
- Hỗ trợ upload ảnh trực tiếp khi tạo feedback
- Tự động upload lên Cloudinary
- Validate file size (max 10MB) và file type
- Lưu URL ảnh vào database

### Admin Management
- Chỉ Admin có thể thay đổi trạng thái IsActive
- Khi ẩn feedback, tự động gửi notification cho user
- Notification có nội dung: "Feedback của bạn đã vi phạm tiêu chuẩn cộng đồng và đã bị ẩn."

### Authorization
- Create Feedback: Yêu cầu đăng nhập
- Get My Feedbacks: Yêu cầu đăng nhập
- Update Status: Yêu cầu role Admin
- Get All Feedbacks: Public access
- Get Feedback Detail: Public access

### Validation
- File size validation (max 10MB)
- File type validation (jpg, jpeg, png, gif)
- Rating validation (1-5)
- Comment length validation (max 1000 characters)
- Duplicate feedback prevention

## Error Responses

### 400 Bad Request
```json
{
  "message": "Kích thước file không được vượt quá 10MB"
}
```

### 401 Unauthorized
```json
{
  "message": "Token không hợp lệ hoặc đã hết hạn"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied"
}
```

### 404 Not Found
```json
{
  "message": "Không tìm thấy feedback với id này."
}
```

### 500 Internal Server Error
```json
{
  "message": "Có lỗi xảy ra khi tạo feedback",
  "error": "Error details..."
}
```

## Notes

- User chỉ cần nhập TourId, Rating, Comment khi tạo feedback
- UserId được tự động lấy từ JWT token
- IsActive mặc định là true khi tạo feedback
- Chỉ Admin mới có thể thay đổi IsActive
- Khi IsActive = false, feedback sẽ không hiển thị trong danh sách public
- Notification được gửi tự động khi feedback bị ẩn 