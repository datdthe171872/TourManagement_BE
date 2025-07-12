# Tour Guide Assignment API

API này quản lý việc phân công hướng dẫn viên cho các tour.

## Endpoints

### 1. GET /api/TourGuideAssignment
Lấy danh sách tất cả các phân công hướng dẫn viên với khả năng lọc và phân trang.

**Quyền truy cập:** Tour Operator

**Query Parameters:**
- `TourId` (optional): Lọc theo ID tour
- `TourGuideId` (optional): Lọc theo ID hướng dẫn viên
- `BookingId` (optional): Lọc theo ID booking
- `IsLeadGuide` (optional): Lọc theo vai trò lead guide (true/false)
- `IsActive` (optional): Lọc theo trạng thái active (true/false)
- `AssignedDateFrom` (optional): Lọc từ ngày phân công (format: yyyy-MM-dd)
- `AssignedDateTo` (optional): Lọc đến ngày phân công (format: yyyy-MM-dd)
- `PageNumber` (optional): Số trang (mặc định: 1)
- `PageSize` (optional): Số lượng item mỗi trang (mặc định: 10)

**Response:**
```json
{
  "assignments": [
    {
      "id": 1,
      "tourId": 1,
      "bookingId": 1,
      "tourGuideId": 1,
      "assignedDate": "2024-01-15",
      "noteId": null,
      "isLeadGuide": true,
      "isActive": true,
      "tourName": "Tour Name",
      "guideName": "Guide Name",
      "customerName": "Customer Name"
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10
}
```

### 2. GET /api/TourGuideAssignment/my-assignments
Lấy danh sách phân công của hướng dẫn viên hiện tại.

**Quyền truy cập:** Tour Guide

**Response:**
```json
[
  {
    "id": 1,
    "tourId": 1,
    "bookingId": 1,
    "assignedDate": "2024-01-15",
    "isLeadGuide": true
  }
]
```

### 3. POST /api/TourGuideAssignment
Tạo phân công hướng dẫn viên mới.

**Quyền truy cập:** Tour Operator

**Request Body:**
```json
{
  "tourId": 1,
  "bookingId": 1,
  "tourGuideId": 1,
  "assignedDate": "2024-01-15T00:00:00",
  "isLeadGuide": true
}
```

**Response:**
```
Assignment created successfully
```

## Ví dụ sử dụng

### Lấy tất cả phân công
```
GET /api/TourGuideAssignment
```

### Lọc theo tour
```
GET /api/TourGuideAssignment?TourId=1&PageNumber=1&PageSize=10
```

### Lọc theo hướng dẫn viên
```
GET /api/TourGuideAssignment?TourGuideId=1&IsActive=true
```

### Lọc theo khoảng thời gian
```
GET /api/TourGuideAssignment?AssignedDateFrom=2024-01-01&AssignedDateTo=2024-12-31
```

### Lọc lead guide
```
GET /api/TourGuideAssignment?IsLeadGuide=true
```

## Lưu ý

1. API yêu cầu xác thực JWT token
2. Tour Operator có thể xem tất cả phân công
3. Tour Guide chỉ có thể xem phân công của mình
4. Hỗ trợ phân trang để tối ưu hiệu suất
5. Có thể kết hợp nhiều filter cùng lúc 