# DepartureDates API Documentation

## Tổng quan
API quản lý ngày khởi hành cho các tour du lịch. Cho phép tạo, xem và quản lý các ngày khởi hành của tour.

## Endpoints

### 1. Tạo ngày khởi hành cho tour
**POST** `/api/DepartureDates`

Tạo các ngày khởi hành cho một tour. Số lượng ngày khởi hành sẽ được tự động tính dựa trên `DurationInDays` của tour.

**Quyền truy cập:** TourOperator

**Request Body:**
```json
{
  "tourId": 1,
  "startDate": "2024-02-01T00:00:00"
}
```

**Response:**
```json
{
  "message": "Tạo ngày khởi hành thành công"
}
```

**Lưu ý:**
- Ngày bắt đầu phải là ngày trong tương lai
- Số lượng ngày khởi hành = DurationInDays của tour (tối đa 12 ngày)
- Khoảng cách giữa các ngày = DurationInDays + 1

**Ví dụ:**
- Tour 3 ngày 2 đêm → tạo 3 ngày khởi hành, cách nhau 4 ngày
- Tour 5 ngày 4 đêm → tạo 5 ngày khởi hành, cách nhau 6 ngày
- Tour 15 ngày 14 đêm → tạo 12 ngày khởi hành (giới hạn tối đa), cách nhau 16 ngày

### 2. Lấy tất cả ngày khởi hành
**GET** `/api/DepartureDates`

Lấy danh sách tất cả các ngày khởi hành đang hoạt động.

**Quyền truy cập:** Tất cả user đã đăng nhập

**Response:**
```json
{
  "message": "Lấy danh sách ngày khởi hành thành công",
  "data": [
    {
      "id": 1,
      "tourId": 1,
      "tourTitle": "Tour Hà Nội - Sapa",
      "departureDate": "2024-02-01T00:00:00",
      "isActive": true,
      "totalBookings": 5,
      "availableSlots": 15
    }
  ]
}
```

### 3. Lấy ngày khởi hành theo TourId
**GET** `/api/DepartureDates/tour/{tourId}`

Lấy danh sách ngày khởi hành của một tour cụ thể.

**Quyền truy cập:** Tất cả user đã đăng nhập

**Parameters:**
- `tourId` (int): ID của tour

**Response:**
```json
{
  "message": "Lấy danh sách ngày khởi hành theo tour thành công",
  "data": [
    {
      "id": 1,
      "tourId": 1,
      "tourTitle": "Tour Hà Nội - Sapa",
      "departureDate": "2024-02-01T00:00:00",
      "isActive": true,
      "totalBookings": 5,
      "availableSlots": 15
    }
  ]
}
```

### 4. Lấy tất cả ngày khởi hành của TourOperator
**GET** `/api/DepartureDates/operator`

Lấy danh sách tất cả ngày khởi hành của các tour thuộc về TourOperator hiện tại đang đăng nhập.

**Quyền truy cập:** TourOperator

**Response:**
```json
{
  "message": "Lấy danh sách ngày khởi hành của TourOperator thành công",
  "data": [
    {
      "id": 1,
      "tourId": 1,
      "tourTitle": "Tour Hà Nội - Sapa",
      "departureDate": "2024-02-01T00:00:00",
      "isActive": true,
      "totalBookings": 5,
      "availableSlots": 15
    }
  ]
}
```

### 5. Lấy ngày khởi hành với booking cho TourOperator
**GET** `/api/DepartureDates/operator/with-bookings`

Lấy danh sách ngày khởi hành và thông tin booking của tất cả tour thuộc về TourOperator hiện tại đang đăng nhập.

**Quyền truy cập:** TourOperator

**Response:**
```json
{
  "message": "Lấy danh sách ngày khởi hành với booking thành công",
  "data": [
    {
      "id": 1,
      "tourId": 1,
      "tourTitle": "Tour Hà Nội - Sapa",
      "departureDate": "2024-02-01T00:00:00",
      "isActive": true,
      "totalBookings": 2,
      "availableSlots": 18,
      "bookings": [
        {
          "bookingId": 1,
          "userId": 5,
          "userName": "Nguyễn Văn A",
          "userEmail": "nguyenvana@email.com",
          "bookingDate": "2024-01-15T10:30:00",
          "numberOfAdults": 2,
          "numberOfChildren": 1,
          "numberOfInfants": 0,
          "totalPrice": 1500000,
          "bookingStatus": "Confirmed",
          "paymentStatus": "Paid"
        }
      ]
    }
  ]
}
```

## Validation Rules

### CreateDepartureDateRequest
- `tourId`: Phải lớn hơn 0
- `startDate`: Không được để trống và phải là ngày trong tương lai

## Error Responses

### 400 Bad Request
```json
{
  "message": "Dữ liệu không hợp lệ",
  "errors": [
    "Ngày bắt đầu phải là ngày trong tương lai"
  ]
}
```

### 400 Bad Request (Tour không tồn tại)
```json
{
  "message": "Không thể tạo ngày khởi hành. Vui lòng kiểm tra lại thông tin tour và ngày bắt đầu."
}
```

## Business Logic

1. **Tính toán số lượng ngày khởi hành:**
   - Số lượng = DurationInDays của tour
   - Giới hạn tối đa: 12 ngày khởi hành

2. **Tính toán khoảng cách ngày khởi hành:**
   - Khoảng cách = DurationInDays + 1
   - Ví dụ: Tour 3 ngày 2 đêm → khoảng cách = 4 ngày

3. **Kiểm tra thời gian:**
   - Không cho phép tạo ngày khởi hành trong quá khứ
   - Chỉ cho phép tạo ngày trong tương lai

4. **Quyền truy cập:**
   - Chỉ TourOperator mới có thể tạo ngày khởi hành
   - Tất cả user đã đăng nhập có thể xem danh sách
   - TourOperator chỉ xem được tour của mình

5. **Phân biệt API:**
   - `/operator`: Chỉ lấy danh sách ngày khởi hành của TourOperator hiện tại
   - `/operator/with-bookings`: Lấy ngày khởi hành + thông tin booking chi tiết của TourOperator hiện tại 