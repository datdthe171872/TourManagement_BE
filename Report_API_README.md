# Report API Documentation

## Tổng quan

Report API cung cấp báo cáo tổng hợp cho Customer và Tour Operator, bao gồm thông tin về booking, tour, chi phí và tổng thanh toán.

## Cấu trúc dữ liệu

### ReportResponse
```json
{
  "username": "string",
  "bookingId": "int",
  "tourTitle": "string", 
  "contract": "string",
  "totalPrice": "decimal",
  "totalExtraCost": "decimal",
  "total": "decimal"
}
```

## API Endpoints

### 1. Customer Report API

#### Lấy báo cáo cho Customer
```http
GET /api/Report/customer
Authorization: Bearer {token}
```

**Query Parameters (tùy chọn):**
- `username`: Tìm kiếm theo tên người dùng (partial match)

**Ví dụ:**
```http
GET /api/Report/customer?username=customer1
GET /api/Report/operator?username=customer1
```

**Response:**
```json
[
  {
    "username": "customer1",
    "bookingId": 1,
    "tourTitle": "Tour Hà Nội - Sapa",
    "contract": "contract_url.pdf",
    "totalPrice": 1500000.00,
    "totalExtraCost": 125000.00,
    "total": 1625000.00
  }
]
```

**Quyền truy cập:** Customer

### 2. Tour Operator Report API

#### Lấy báo cáo cho Tour Operator
```http
GET /api/Report/operator
Authorization: Bearer {token}
```

**Query Parameters (tùy chọn):**
- `username`: Tìm kiếm theo tên người dùng (partial match)
- `bookingId`: Tìm kiếm theo ID booking (exact match)

**Ví dụ:**
```http
GET /api/Report/operator?username=customer1
GET /api/Report/operator?bookingId=1
GET /api/Report/operator?username=john&bookingId=2
```

**Response:**
```json
[
  {
    "username": "customer1",
    "bookingId": 1,
    "tourTitle": "Tour Hà Nội - Sapa",
    "contract": "contract_url.pdf",
    "totalPrice": 1500000.00,
    "totalExtraCost": 125000.00,
    "total": 1625000.00
  },
  {
    "username": "customer2", 
    "bookingId": 2,
    "tourTitle": "Tour Hà Nội - Sapa",
    "contract": "contract_url2.pdf",
    "totalPrice": 1800000.00,
    "totalExtraCost": 75000.00,
    "total": 1875000.00
  }
]
```

**Quyền truy cập:** Tour Operator

## Tính năng

### Phân quyền
- **Customer**: Chỉ thấy booking của chính mình
- **Tour Operator**: Chỉ thấy booking của các tour thuộc về mình

### Tính toán tự động
- **TotalExtraCost**: Tổng chi phí phát sinh từ TourAcceptanceReports
- **Total**: TotalPrice + TotalExtraCost

### Tìm kiếm và lọc
- **Username**: Tìm kiếm theo tên người dùng (partial match, case-insensitive)
- **BookingId**: Tìm kiếm theo ID booking (exact match)
- **Kết hợp**: Có thể kết hợp Username và BookingId

### Dữ liệu hiển thị
- **Username**: Tên người dùng đặt tour
- **BookingId**: ID booking
- **TourTitle**: Tiêu đề tour
- **Contract**: URL hợp đồng (hoặc "N/A" nếu không có)
- **TotalPrice**: Giá tour gốc
- **TotalExtraCost**: Tổng chi phí phát sinh
- **Total**: Tổng thanh toán

## Luồng nghiệp vụ

### 1. Customer xem báo cáo
1. Customer đăng nhập
2. Gọi API `/api/Report/customer`
3. Hệ thống trả về tất cả booking của customer
4. Hiển thị thông tin tổng hợp

### 2. Tour Operator xem báo cáo
1. Tour Operator đăng nhập
2. Gọi API `/api/Report/operator`
3. Hệ thống trả về tất cả booking của các tour thuộc operator
4. Hiển thị thông tin tổng hợp

## Error Handling

```json
{
  "error": "Tour Operator not found",
  "statusCode": 404
}
```

Các lỗi thường gặp:
- `Tour Operator not found`: Tour Operator không tồn tại
- `Forbidden`: Không có quyền truy cập
- `Unauthorized`: Token không hợp lệ

## Lưu ý

1. **Authentication**: Tất cả API đều yêu cầu JWT token
2. **Authorization**: Phân quyền theo role (Customer/Tour Operator)
3. **Data Filtering**: Dữ liệu được lọc theo quyền của user
4. **Soft Delete**: Chỉ hiển thị dữ liệu active (IsActive = true)
5. **Calculation**: Total được tính tự động từ TotalPrice + TotalExtraCost 