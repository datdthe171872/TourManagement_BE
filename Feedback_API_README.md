# Feedback API Documentation

## Tổng quan
Feedback API cung cấp các endpoint để quản lý đánh giá và phản hồi từ người dùng về các tour du lịch.

## Base URL
```
https://localhost:7001/api/Feedback
```

## Endpoints

### 1. Lấy danh sách feedback
**GET** `/api/Feedback`

Lấy danh sách tất cả feedback với phân trang và tìm kiếm.

**Query Parameters:**
- `TourId` (optional): ID của tour
- `UserId` (optional): ID của user
- `Rating` (optional): Điểm đánh giá (1-5)
- `PageNumber` (optional): Số trang (mặc định: 1)
- `PageSize` (optional): Số lượng item mỗi trang (mặc định: 10)

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
      "userName": "Nguyễn Văn A"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

### 2. Lấy chi tiết feedback
**GET** `/api/Feedback/{id}`

Lấy thông tin chi tiết của một feedback theo ID.

**Path Parameters:**
- `id`: ID của feedback

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
  "userName": "Nguyễn Văn A"
}
```

### 3. Tạo mới feedback
**POST** `/api/Feedback`

Tạo một feedback mới.

**Request Body:**
```json
{
  "tourId": 1,
  "userId": 1,
  "rating": 5,
  "comment": "Tour rất tuyệt vời, hướng dẫn viên nhiệt tình!",
  "mediaUrl": "https://example.com/feedback-image.jpg"
}
```

**Validation Rules:**
- `tourId`: Bắt buộc, phải tồn tại trong bảng Tour
- `userId`: Bắt buộc, phải tồn tại trong bảng User
- `rating`: Tùy chọn, phải từ 1-5
- `comment`: Tùy chọn, tối đa 1000 ký tự
- `mediaUrl`: Tùy chọn, tối đa 500 ký tự

**Response:**
```json
{
  "message": "Tạo feedback thành công",
  "data": {
    "ratingId": 1,
    "tourId": 1,
    "userId": 1,
    "rating": 5,
    "comment": "Tour rất tuyệt vời, hướng dẫn viên nhiệt tình!",
    "mediaUrl": "https://example.com/feedback-image.jpg",
    "createdAt": "2024-01-01T10:00:00Z",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "Nguyễn Văn A"
  }
}
```

### 4. Cập nhật feedback
**PUT** `/api/Feedback/{id}`

Cập nhật thông tin của một feedback.

**Path Parameters:**
- `id`: ID của feedback cần cập nhật

**Request Body:**
```json
{
  "rating": 4,
  "comment": "Tour tốt, nhưng có thể cải thiện thêm về dịch vụ ăn uống",
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
    "comment": "Tour tốt, nhưng có thể cải thiện thêm về dịch vụ ăn uống",
    "mediaUrl": "https://example.com/updated-feedback-image.jpg",
    "createdAt": "2024-01-01T10:00:00Z",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "Nguyễn Văn A"
  }
}
```

### 5. Xóa mềm feedback
**DELETE** `/api/Feedback/{id}`

Xóa mềm một feedback (chỉ đánh dấu IsActive = false).

**Path Parameters:**
- `id`: ID của feedback cần xóa

**Response:**
```json
{
  "message": "Xóa feedback thành công"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "message": "Tour không tồn tại"
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
  "message": "Có lỗi xảy ra khi lấy danh sách feedback",
  "error": "Error details"
}
```

## Business Rules

1. **Mỗi user chỉ có thể đánh giá một tour một lần**
2. **Rating phải từ 1-5 sao**
3. **Comment tối đa 1000 ký tự**
4. **MediaUrl tối đa 500 ký tự**
5. **Xóa mềm: chỉ đánh dấu IsActive = false, không xóa thực sự khỏi database**

## Testing

Sử dụng file `Feedback_API_Test.http` để test các API endpoints. 