# Feedback API Documentation

## Tổng quan
API quản lý feedback và đánh giá tour của khách hàng.

## Các API Endpoints

### 1. Lấy chi tiết feedback theo ID
**Endpoint:** `GET /api/Feedback/{id}`

**Mô tả:** Lấy thông tin chi tiết của một feedback theo ID.

**Tham số:**
- `id` (int, required): ID của feedback cần lấy

**Response thành công (200):**
```json
{
  "message": "Lấy chi tiết feedback thành công",
  "data": {
    "ratingId": 1,
    "tourId": 1,
    "userId": 1,
    "rating": 5,
    "comment": "Tour rất tuyệt vời!",
    "mediaUrl": "https://res.cloudinary.com/...",
    "createdAt": "2024-01-01T00:00:00",
    "isActive": true,
    "tourName": "Tour Hà Nội - Sapa",
    "userName": "user@example.com",
    "userEmail": "user@example.com"
  }
}
```

**Response không tìm thấy (404):**
```json
{
  "message": "Không tìm thấy feedback với id này."
}
```

**Response lỗi server (500):**
```json
{
  "message": "Có lỗi xảy ra khi lấy chi tiết feedback",
  "error": "Error details..."
}
```

### 2. Admin: Lấy tất cả feedback
**Endpoint:** `GET /api/Feedback/admin`

**Mô tả:** Lấy danh sách tất cả feedback với tìm kiếm và phân trang (chỉ Admin).

**Tham số query:**
- `UserName` (string, optional): Tìm kiếm theo tên user
- `RatingId` (int, optional): Tìm kiếm theo ID rating
- `PageNumber` (int, default: 1): Số trang
- `PageSize` (int, default: 10): Số lượng item trên mỗi trang

**Authorization:** Admin role required

### 3. Tour Operator: Lấy feedback về tour của họ
**Endpoint:** `GET /api/Feedback/tour-operator`

**Mô tả:** Lấy danh sách feedback về các tour của Tour Operator hiện tại.

**Tham số query:**
- `UserName` (string, optional): Tìm kiếm theo tên user
- `RatingId` (int, optional): Tìm kiếm theo ID rating
- `PageNumber` (int, default: 1): Số trang
- `PageSize` (int, default: 10): Số lượng item trên mỗi trang

**Authorization:** Tour Operator role required

### 4. Tạo mới feedback
**Endpoint:** `POST /api/Feedback`

**Mô tả:** Tạo mới feedback với upload ảnh.

**Body (multipart/form-data):**
- `TourId` (int, required): ID của tour
- `Rating` (int, required): Điểm đánh giá (1-5)
- `Comment` (string, required): Nội dung feedback
- `ImageFile` (file, optional): Ảnh đính kèm

**Authorization:** Required

### 5. Cập nhật feedback
**Endpoint:** `PUT /api/Feedback/{id}`

**Mô tả:** Cập nhật thông tin feedback.

**Body (JSON):**
```json
{
  "rating": 4,
  "comment": "Tour khá tốt"
}
```

### 6. Xóa mềm feedback
**Endpoint:** `DELETE /api/Feedback/{id}`

**Mô tả:** Xóa mềm feedback (chỉ set IsActive = false).

### 7. Lấy feedback của user đã đăng nhập
**Endpoint:** `GET /api/Feedback/my-feedbacks`

**Mô tả:** Lấy tất cả feedback của user hiện tại.

**Authorization:** Required

### 8. Tour Operator: Báo cáo feedback
**Endpoint:** `POST /api/Feedback/report`

**Mô tả:** Tour Operator báo cáo feedback về tour của họ.

**Body (JSON):**
```json
{
  "ratingId": 1,
  "reason": "Feedback không phù hợp"
}
```

**Authorization:** Tour Operator role required

### 9. Admin: Cập nhật trạng thái feedback
**Endpoint:** `PUT /api/Feedback/update-status`

**Mô tả:** Admin cập nhật trạng thái hiển thị của feedback.

**Body (JSON):**
```json
{
  "ratingId": 1,
  "isActive": false
}
```

**Authorization:** Admin role required

## Cấu trúc Response

### FeedbackResponse
```json
{
  "ratingId": 1,
  "tourId": 1,
  "userId": 1,
  "rating": 5,
  "comment": "Tour rất tuyệt vời!",
  "mediaUrl": "https://res.cloudinary.com/...",
  "createdAt": "2024-01-01T00:00:00",
  "isActive": true,
  "tourName": "Tour Hà Nội - Sapa",
  "userName": "user@example.com",
  "userEmail": "user@example.com"
}
```

### FeedbackListResponse
```json
{
  "feedbacks": [
    {
      "ratingId": 1,
      "tourId": 1,
      "userId": 1,
      "rating": 5,
      "comment": "Tour rất tuyệt vời!",
      "mediaUrl": "https://res.cloudinary.com/...",
      "createdAt": "2024-01-01T00:00:00",
      "isActive": true,
      "tourName": "Tour Hà Nội - Sapa",
      "userName": "user@example.com",
      "userEmail": "user@example.com"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

## Lưu ý
- Tất cả API đều trả về response theo format chuẩn với `message` và `data`
- API lấy feedback theo ID không yêu cầu role cụ thể, ai cũng có thể truy cập
- Chỉ các feedback có `IsActive = true` mới được trả về
- File ảnh upload phải có kích thước < 10MB và định dạng jpg, jpeg, png, gif
- Mỗi user chỉ có thể đánh giá một tour một lần

## Testing
Sử dụng file `Feedback_API_Test.http` và `Feedback_Get_By_ID_Test.http` để test các API. 