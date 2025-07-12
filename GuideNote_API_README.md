# Guide Note API Documentation

## Tổng quan
Guide Note API cho phép Tour Guide tạo, quản lý và lưu trữ các ghi chú liên quan đến các tour assignment của họ.

## Base URL
```
https://localhost:7001/api/GuideNote
```

## Yêu cầu xác thực
- **Role**: Chỉ Tour Guide mới có thể sử dụng API này
- **Authorization**: Bearer Token (JWT)

## Endpoints

### 1. Lấy danh sách Assignment có sẵn cho Tour Guide hiện tại
**GET** `/api/GuideNote/assignments`

Lấy danh sách tất cả các tour assignment mà Tour Guide hiện tại được phân công.

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
[
  {
    "assignmentId": 5,
    "tourId": 1,
    "bookingId": 10,
    "assignedDate": "2024-01-15",
    "isLeadGuide": true,
    "booking": {
      "bookingId": 10,
      "customerName": "Nguyễn Văn A",
      "customerPhone": "0123456789",
      "customerEmail": "customer@example.com"
    },
    "tourGuide": {
      "tourGuideId": 3,
      "userId": 5,
      "fullName": "Trần Thị B",
      "phoneNumber": "0987654321"
    }
  }
]
```

### 2. Lấy danh sách Guide Note của Tour Guide hiện tại
**GET** `/api/GuideNote`

Lấy tất cả các note mà Tour Guide hiện tại đã tạo.

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
[
  {
    "noteId": 1,
    "assignmentId": 5,
    "title": "Ghi chú về điểm đến",
    "content": "Khách hàng có yêu cầu đặc biệt về ăn chay",
    "createdAt": "2024-01-15T10:30:00Z",
    "mediaUrls": [
      "https://example.com/image1.jpg",
      "https://example.com/image2.jpg"
    ]
  }
]
```

### 2. Tạo mới Guide Note
**POST** `/api/GuideNote`

Tạo một note mới cho một tour assignment cụ thể.

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "assignmentId": 5,
  "title": "Ghi chú về điểm đến",
  "content": "Khách hàng có yêu cầu đặc biệt về ăn chay. Cần chuẩn bị thực đơn phù hợp.",
  "mediaUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ]
}
```

**Validation Rules:**
- `assignmentId`: **Bắt buộc**, phải là ID của một tour assignment thuộc về Tour Guide hiện tại
- `title`: Tùy chọn, tối đa 255 ký tự
- `content`: Tùy chọn, nội dung ghi chú
- `mediaUrls`: Tùy chọn, danh sách URL của các file media (ảnh, video, etc.)

**Response:**
```json
{
  "message": "Note created successfully"
}
```

### 3. Cập nhật Guide Note
**PUT** `/api/GuideNote/{id}`

Cập nhật thông tin của một note đã tồn tại.

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Path Parameters:**
- `id`: ID của note cần cập nhật

**Request Body:**
```json
{
  "title": "Ghi chú cập nhật về điểm đến",
  "content": "Khách hàng có yêu cầu đặc biệt về ăn chay. Cần chuẩn bị thực đơn phù hợp. Đã liên hệ với nhà hàng.",
  "mediaUrls": [
    "https://example.com/updated-image1.jpg",
    "https://example.com/updated-image2.jpg"
  ]
}
```

**Validation Rules:**
- `title`: **Bắt buộc**, tối đa 255 ký tự
- `content`: Tùy chọn, nội dung ghi chú
- `mediaUrls`: Tùy chọn, danh sách URL của các file media

**Response:**
```json
{
  "message": "Note updated successfully"
}
```

### 4. Xóa Guide Note
**DELETE** `/api/GuideNote/{id}`

Xóa một note (soft delete - chỉ đánh dấu không active).

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Path Parameters:**
- `id`: ID của note cần xóa

**Response:**
```json
{
  "message": "Note deleted successfully"
}
```

## Cách sử dụng

### Bước 1: Đăng nhập và lấy JWT Token
```bash
POST /api/Auth/login
{
  "email": "guide@example.com",
  "password": "password123"
}
```

### Bước 2: Kiểm tra các Assignment có sẵn
```bash
GET /api/GuideNote/assignments
Authorization: Bearer <jwt_token>
```

### Bước 3: Tạo Guide Note
```bash
POST /api/GuideNote
Authorization: Bearer <jwt_token>
{
  "assignmentId": 5,
  "title": "Ghi chú tour Hà Nội - Sapa",
  "content": "Khách hàng có yêu cầu đặc biệt về ăn chay. Cần chuẩn bị thực đơn phù hợp.",
  "mediaUrls": [
    "https://example.com/dietary-requirements.jpg"
  ]
}
```

### Bước 4: Xem danh sách Guide Note
```bash
GET /api/GuideNote
Authorization: Bearer <jwt_token>
```

## Lưu ý quan trọng

1. **Quyền truy cập**: Chỉ Tour Guide mới có thể tạo và quản lý Guide Note
2. **Assignment ID**: Bạn chỉ có thể tạo note cho các tour assignment mà bạn được phân công
3. **Media URLs**: Các URL media phải là URL hợp lệ và có thể truy cập được
4. **Soft Delete**: Khi xóa note, dữ liệu không bị xóa hoàn toàn mà chỉ được đánh dấu không active
5. **Tự động tạo thời gian**: Thời gian tạo note sẽ được tự động gán khi tạo mới

## Troubleshooting

### Lỗi "Assignment not found"
Lỗi này xảy ra khi:
- `assignmentId` không tồn tại trong hệ thống
- `assignmentId` không thuộc về Tour Guide hiện tại
- Assignment đã bị xóa hoặc không active

**Cách khắc phục:**
1. Sử dụng endpoint `GET /api/GuideNote/assignments` để lấy danh sách assignment có sẵn
2. Chọn một `assignmentId` hợp lệ từ danh sách trả về
3. Đảm bảo bạn đang sử dụng tài khoản Tour Guide đúng

### Lỗi "Guide not found"
Lỗi này xảy ra khi:
- Tài khoản hiện tại không phải là Tour Guide
- Tour Guide đã bị xóa hoặc không active

**Cách khắc phục:**
1. Đảm bảo bạn đã đăng nhập với tài khoản có role "Tour Guide"
2. Liên hệ admin để kiểm tra trạng thái tài khoản

## Error Responses

### 400 Bad Request
```json
{
  "message": "Assignment not found"
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
```json
{
  "message": "Not your note"
}
```

### 500 Internal Server Error
```json
{
  "message": "Guide not found"
}
```

## Ví dụ sử dụng với cURL

### Lấy danh sách Assignment có sẵn
```bash
curl -X GET "https://localhost:7001/api/GuideNote/assignments" \
  -H "Authorization: Bearer <your_jwt_token>"
```

### Tạo Guide Note
```bash
curl -X POST "https://localhost:7001/api/GuideNote" \
  -H "Authorization: Bearer <your_jwt_token>" \
  -H "Content-Type: application/json" \
  -d '{
    "assignmentId": 5,
    "title": "Ghi chú về khách hàng",
    "content": "Khách hàng có yêu cầu đặc biệt về ăn chay",
    "mediaUrls": ["https://example.com/image.jpg"]
  }'
```

### Lấy danh sách Guide Note
```bash
curl -X GET "https://localhost:7001/api/GuideNote" \
  -H "Authorization: Bearer <your_jwt_token>"
``` 