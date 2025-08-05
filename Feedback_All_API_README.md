# Feedback All API - Lấy tất cả feedback với search theo TourId

## Tổng quan
API này cho phép lấy tất cả feedback trong hệ thống với khả năng tìm kiếm và lọc theo nhiều tiêu chí khác nhau, bao gồm TourId, UserId, Rating và trạng thái IsActive.

## Endpoint
```
GET /api/feedback/all
```

## Quyền truy cập
- **Không yêu cầu authentication** - API này có thể được truy cập bởi tất cả người dùng

## Parameters (Query String)

| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| `PageNumber` | int | No | Số trang hiện tại | 1 |
| `PageSize` | int | No | Số lượng feedback trên mỗi trang | 10 |
| `TourId` | int? | No | ID của tour để lọc feedback | null |
| `UserId` | int? | No | ID của user để lọc feedback | null |
| `Rating` | int? | No | Rating để lọc feedback (1-5) | null |
| `IsActive` | bool? | No | Trạng thái active của feedback | null |

## Response Format

### Success Response (200 OK)
```json
{
  "message": "Lấy danh sách tất cả feedback thành công",
  "data": {
    "feedbacks": [
      {
        "ratingId": 1,
        "tourId": 1,
        "userId": 1,
        "rating": 5,
        "comment": "Tour rất tuyệt vời!",
        "mediaUrl": "https://example.com/image.jpg",
        "createdAt": "2024-01-15T10:30:00Z",
        "isActive": true,
        "tourName": "Tour Hà Nội - Sapa",
        "userName": "john_doe",
        "userEmail": "john@example.com"
      }
    ],
    "totalCount": 50,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5
  }
}
```

### Empty Response (200 OK)
```json
{
  "message": "Không tìm thấy feedback nào phù hợp với điều kiện tìm kiếm.",
  "data": {
    "feedbacks": [],
    "totalCount": 0,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 0
  }
}
```

### Error Response (500 Internal Server Error)
```json
{
  "message": "Có lỗi xảy ra khi lấy danh sách feedback",
  "error": "Error details..."
}
```

## Ví dụ sử dụng

### 1. Lấy tất cả feedback (không có filter)
```
GET /api/feedback/all
```

### 2. Lấy feedback theo TourId cụ thể
```
GET /api/feedback/all?TourId=1
```

### 3. Lấy feedback với phân trang
```
GET /api/feedback/all?PageNumber=1&PageSize=5
```

### 4. Lấy feedback theo UserId
```
GET /api/feedback/all?UserId=1
```

### 5. Lấy feedback theo Rating
```
GET /api/feedback/all?Rating=5
```

### 6. Lấy feedback theo trạng thái IsActive
```
GET /api/feedback/all?IsActive=true
```

### 7. Lấy feedback với nhiều filter kết hợp
```
GET /api/feedback/all?TourId=1&Rating=5&IsActive=true&PageNumber=1&PageSize=10
```

### 8. Lấy feedback của tour cụ thể với rating cao
```
GET /api/feedback/all?TourId=1&Rating=4&PageSize=20
```

## Lưu ý

1. **Phân trang**: API hỗ trợ phân trang với `PageNumber` và `PageSize`
2. **Lọc kết hợp**: Có thể kết hợp nhiều filter cùng lúc
3. **Sắp xếp**: Kết quả được sắp xếp theo thời gian tạo mới nhất (CreatedAt DESC)
4. **Trường TourId**: Luôn được trả về trong response để dễ dàng xác định feedback thuộc về tour nào
5. **Không yêu cầu authentication**: API này có thể được sử dụng cho mục đích public

## So sánh với các API khác

| API | Authentication | Phạm vi | Đặc điểm |
|-----|----------------|---------|----------|
| `/api/feedback/all` | Không cần | Tất cả feedback | Có thể lọc theo TourId, UserId, Rating, IsActive |
| `/api/feedback/admin` | Admin only | Tất cả feedback | Lọc theo Username, RatingId |
| `/api/feedback/tour-operator` | Tour Operator only | Feedback của tour operator | Chỉ feedback thuộc tour của operator |
| `/api/feedback/my-feedbacks` | Authenticated user | Feedback của user hiện tại | Chỉ feedback của user đang đăng nhập |

## Testing

Sử dụng file `Feedback_All_API_Test.http` để test các trường hợp khác nhau của API này. 