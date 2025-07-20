# Feedback API - Updated Version

## Overview
Feedback API cung cấp các chức năng quản lý feedback/đánh giá tour. API đã được cập nhật với tính năng mới để lấy tất cả feedback của user đã đăng nhập.

## Base URL
```
http://localhost:5298/api/Feedback
```

## Authentication
Tất cả API đều yêu cầu JWT token trong header:
```
Authorization: Bearer <your_jwt_token>
```

## API Endpoints

### 1. Get All Feedbacks
**GET** `/api/Feedback`

Lấy danh sách tất cả feedback với tìm kiếm và phân trang.

**Query Parameters:**
- `pageNumber` (int, optional): Số trang (mặc định: 1)
- `pageSize` (int, optional): Số lượng item mỗi trang (mặc định: 10)
- `tourId` (int, optional): Lọc theo tour ID
- `userId` (int, optional): Lọc theo user ID
- `rating` (int, optional): Lọc theo rating

**Response:**
```json
{
  "feedbacks": [
    {
      "ratingId": 1,
      "tourId": 1,
      "userId": 1,
      "rating": 5,
      "comment": "Tour rất tuyệt vời!",
      "mediaUrl": "https://example.com/image.jpg",
      "createdAt": "2024-01-01T10:00:00Z",
      "isActive": true,
      "tourName": "Tour Hà Nội - Sapa",
      "userName": "Nguyễn Văn A",
      "userEmail": "user@example.com"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

### 2. Get Feedback by ID
**GET** `/api/Feedback/{id}`

Lấy chi tiết feedback theo ID.

**Response:**
```json
{
  "ratingId": 1,
  "tourId": 1,
  "userId": 1,
  "rating": 5,
  "comment": "Tour rất tuyệt vời!",
  "mediaUrl": "https://example.com/image.jpg",
  "createdAt": "2024-01-01T10:00:00Z",
  "isActive": true,
  "tourName": "Tour Hà Nội - Sapa",
  "userName": "Nguyễn Văn A",
  "userEmail": "user@example.com"
}
```

### 3. Create New Feedback
**POST** `/api/Feedback`

Tạo feedback mới.

**Request Body:**
```json
{
  "tourId": 1,
  "userId": 1,
  "rating": 5,
  "comment": "Tour rất tuyệt vời! Hướng dẫn viên rất nhiệt tình.",
  "mediaUrl": "https://example.com/feedback-image.jpg"
}
```

**Response:**
```json
{
  "message": "Tạo feedback thành công",
  "data": {
    "ratingId": 1,
    "tourId": 1,
    "userId": 1,
    "rating": 5,
    "comment": "Tour rất tuyệt vời! Hướng dẫn viên rất nhiệt tình.",
    "mediaUrl": "https://example.com/feedback-image.jpg",
    "createdAt": "2024-01-01T10:00:00Z",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "Nguyễn Văn A",
    "userEmail": "user@example.com"
  }
}
```

### 4. Update Feedback
**PUT** `/api/Feedback/{id}`

Cập nhật feedback.

**Request Body:**
```json
{
  "rating": 4,
  "comment": "Tour tốt, nhưng có thể cải thiện thêm về dịch vụ.",
  "mediaUrl": "https://example.com/updated-feedback-image.jpg"
}
```

**Response:**
```json
{
  "message": "Cập nhật feedback thành công",
  "data": {
    "ratingId": 1,
    "tourId": 1,
    "userId": 1,
    "rating": 4,
    "comment": "Tour tốt, nhưng có thể cải thiện thêm về dịch vụ.",
    "mediaUrl": "https://example.com/updated-feedback-image.jpg",
    "createdAt": "2024-01-01T10:00:00Z",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "Nguyễn Văn A",
    "userEmail": "user@example.com"
  }
}
```

### 5. Soft Delete Feedback
**DELETE** `/api/Feedback/{id}`

Xóa mềm feedback.

**Response:**
```json
{
  "message": "Xóa feedback thành công"
}
```

### 6. Get My Feedbacks (NEW)
**GET** `/api/Feedback/my-feedbacks`

**Lấy tất cả feedback của user đã đăng nhập.** Đây là API mới được thêm vào.

**Response:**
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
        "mediaUrl": "https://example.com/image.jpg",
        "createdAt": "2024-01-01T10:00:00Z",
        "isActive": true,
        "tourName": "Tour Hà Nội - Sapa",
        "userName": "Nguyễn Văn A",
        "userEmail": "user@example.com"
      }
    ],
    "totalCount": 1,
    "pageNumber": 1,
    "pageSize": 1,
    "totalPages": 1
  }
}
```

## Error Responses

### 400 Bad Request
```json
{
  "message": "User đã đánh giá tour này rồi"
}
```

### 401 Unauthorized
```json
{
  "message": "Token không hợp lệ hoặc đã hết hạn"
}
```

### 404 Not Found
```json
{
  "message": "Không tìm thấy thông tin feedback với id này."
}
```

### 500 Internal Server Error
```json
{
  "message": "Có lỗi xảy ra khi lấy danh sách feedback",
  "error": "Error details"
}
```

## Notes

1. **API `my-feedbacks`** là tính năng mới được thêm vào để user có thể xem tất cả feedback của mình.
2. API này sử dụng JWT token để xác định user đã đăng nhập.
3. Tất cả feedback được sắp xếp theo thời gian tạo mới nhất.
4. API trả về thông tin đầy đủ bao gồm tên tour, tên user và email. 