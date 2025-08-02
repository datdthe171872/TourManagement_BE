# TourGuide API Documentation

## Get All TourGuides of TourOperator

Lấy danh sách tất cả các TourGuide của TourOperator hiện tại với khả năng tìm kiếm theo Username và lọc theo trạng thái IsActive.

### Endpoint
```
GET /api/TourOperator/tourguides
```

### Authorization
- **Required**: Bearer Token
- **Role**: Tour Operator

### Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| Username | string | No | Tìm kiếm TourGuide theo username (partial match) |
| IsActive | boolean | No | Lọc theo trạng thái active (true/false). Nếu không truyền sẽ lấy tất cả |
| PageNumber | int | No | Số trang (mặc định: 1) |
| PageSize | int | No | Số lượng item trên mỗi trang (mặc định: 10) |

### Request Examples

#### 1. Lấy tất cả TourGuide với tìm kiếm và phân trang
```
GET /api/TourOperator/tourguides?Username=guide&IsActive=true&PageNumber=1&PageSize=10
```

#### 2. Lấy chỉ TourGuide đang active
```
GET /api/TourOperator/tourguides?IsActive=true&PageNumber=1&PageSize=10
```

#### 3. Lấy chỉ TourGuide đang inactive
```
GET /api/TourOperator/tourguides?IsActive=false&PageNumber=1&PageSize=10
```

#### 4. Lấy tất cả TourGuide (không lọc trạng thái)
```
GET /api/TourOperator/tourguides?PageNumber=1&PageSize=10
```

#### 5. Tìm kiếm TourGuide theo username
```
GET /api/TourOperator/tourguides?Username=john&PageNumber=1&PageSize=10
```

### Response Format

#### Success Response (200 OK)
```json
{
  "tourGuides": [
    {
      "tourGuideId": 1,
      "userId": 123,
      "userName": "john_guide",
      "email": "john@example.com",
      "phoneNumber": "+1234567890",
      "address": "123 Main St, City",
      "avatar": "https://example.com/avatar.jpg",
      "isActive": true,
      "tourOperatorId": 456
    }
  ],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

#### No Results Response (200 OK)
```json
{
  "message": "Không tìm thấy tour guide nào phù hợp với từ khóa tìm kiếm.",
  "data": {
    "tourGuides": [],
    "totalCount": 0,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 0,
    "hasNextPage": false,
    "hasPreviousPage": false
  }
}
```

#### Error Responses

**401 Unauthorized**
```json
{
  "message": "Unauthorized"
}
```

**404 Not Found**
```json
{
  "message": "Không tìm thấy thông tin TourOperator cho user này."
}
```

**500 Internal Server Error**
```json
{
  "message": "Có lỗi xảy ra khi lấy danh sách tour guide",
  "error": "Error details..."
}
```

### Notes

- API chỉ trả về TourGuide thuộc về TourOperator đang đăng nhập
- Tìm kiếm Username sử dụng partial match (contains)
- Phân trang được tính toán tự động dựa trên PageNumber và PageSize
- Nếu không truyền IsActive, API sẽ trả về tất cả TourGuide (cả active và inactive)
- Response bao gồm thông tin phân trang để frontend có thể hiển thị pagination controls 