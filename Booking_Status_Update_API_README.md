# Booking Status Update APIs

## Tổng quan
Hai API này cho phép **Tour Operator** cập nhật trạng thái thanh toán và trạng thái đặt tour của các booking thuộc về họ.

## Quyền truy cập
- **Chỉ dành cho Tour Operator role**
- Yêu cầu JWT token hợp lệ
- Chỉ có thể cập nhật booking thuộc về tour operator đó

## API Endpoints

### 1. Update Payment Status
**Endpoint:** `PUT /api/Booking/update-payment-status`

**Mô tả:** Cập nhật trạng thái thanh toán của booking

**Request Body:**
```json
{
  "bookingId": 1,
  "paymentStatus": "Paid"
}
```

**Các trạng thái thanh toán hợp lệ:**
- `PaymentStatus.Pending` - Chờ thanh toán
- `PaymentStatus.Paid` - Đã thanh toán
- `PaymentStatus.Failed` - Thanh toán thất bại
- `PaymentStatus.Cancelled` - Đã hủy thanh toán
- `PaymentStatus.Refunded` - Đã hoàn tiền

**Response:**
```json
{
  "bookingId": 1,
  "userId": 123,
  "tourId": 456,
  "departureDateId": 789,
  "bookingDate": "2024-01-15T10:30:00",
  "numberOfAdults": 2,
  "numberOfChildren": 1,
  "numberOfInfants": 0,
  "noteForTour": "Special dietary requirements",
  "totalPrice": 1500.00,
  "contract": "contract_url_here",
  "bookingStatus": "Confirmed",
  "paymentStatus": "Paid",
  "isActive": true,
  "userName": "John Doe",
  "tourTitle": "Ha Long Bay Adventure",
  "companyName": "Vietnam Tours",
  "tourOperatorId": 101
}
```

### 2. Update Booking Status
**Endpoint:** `PUT /api/Booking/update-booking-status`

**Mô tả:** Cập nhật trạng thái đặt tour của booking

**Request Body:**
```json
{
  "bookingId": 1,
  "bookingStatus": "Confirmed"
}
```

**Các trạng thái booking hợp lệ:**
- `BookingStatus.Pending` - Chờ xác nhận
- `BookingStatus.Confirmed` - Đã xác nhận
- `BookingStatus.InProgress` - Đang thực hiện
- `BookingStatus.Completed` - Đã hoàn thành
- `BookingStatus.Cancelled` - Đã hủy
- `BookingStatus.Rejected` - Bị từ chối

**Response:** (Tương tự như API update payment status)

## Tính năng đặc biệt

### 1. Validation
- Kiểm tra booking tồn tại và thuộc về tour operator
- Validate trạng thái hợp lệ
- Kiểm tra quyền truy cập

### 2. Auto Update Related Data
- **Payment Status API:** Tự động cập nhật trạng thái của tất cả payment records liên quan
- Khi set status = "Paid", tự động set `AmountPaid = Amount` và `PaymentDate = DateTime.Now`

### 3. Notification System
- Tự động gửi notification cho customer khi trạng thái thay đổi
- Notification type: "Payment" hoặc "Booking"
- Related entity ID: booking ID

### 4. Error Handling
- `400 Bad Request`: Trạng thái không hợp lệ hoặc booking không tồn tại
- `401 Unauthorized`: Token không hợp lệ
- `403 Forbidden`: Không phải tour operator hoặc không có quyền cập nhật booking này

## Ví dụ sử dụng

### Cập nhật thanh toán thành công
```bash
curl -X PUT "https://api.example.com/api/Booking/update-payment-status" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOUR_OPERATOR_TOKEN" \
  -d '{
    "bookingId": 1,
    "paymentStatus": "Paid"
  }'
```

### Xác nhận booking
```bash
curl -X PUT "https://api.example.com/api/Booking/update-booking-status" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOUR_OPERATOR_TOKEN" \
  -d '{
    "bookingId": 1,
    "bookingStatus": "Confirmed"
  }'
```

## Lưu ý quan trọng

1. **Phân quyền nghiêm ngặt:** Chỉ tour operator mới có thể cập nhật trạng thái
2. **Audit trail:** Mọi thay đổi đều được ghi log và gửi notification
3. **Data consistency:** Đảm bảo tính nhất quán dữ liệu giữa booking và payment
4. **Business logic:** Có thể mở rộng thêm các rule nghiệp vụ trong tương lai

## Testing
Sử dụng file `Booking_Status_Update_API_Test.http` để test các API này với các trường hợp khác nhau. 