# Tính năng Gửi Email Thông Báo Thanh Toán khi Tạo Booking

## Tổng quan
Khi người dùng tạo booking thành công, hệ thống sẽ tự động gửi email thông báo đến địa chỉ email của user với thông tin chi tiết về thanh toán.

## Nội dung Email
Email thông báo sẽ bao gồm các thông tin sau:
- **Tiêu đề**: "Thông báo: Đặt tour [Mã] thành công - Thông tin thanh toán"
- **Mã đặt tour**: Số định danh duy nhất của booking
- **Số tiền cần thanh toán**: Tổng số tiền (TotalPrice) được tính dựa trên số lượng người và giá tour
- **Hạn thanh toán**: Ngày cuối cùng để thanh toán (PaymentDeadline = DepartureDate - 21 ngày)
- **Thông tin liên hệ**: Số điện thoại (Hotline) của Tour Operator để liên hệ khi có thắc mắc

## Cách thức hoạt động

### 1. Khi tạo Booking
- User gọi API `POST /api/booking/create`
- Hệ thống tạo booking và tính toán thông tin thanh toán
- Gửi email thông báo đến user
- Gửi email thông báo đến Tour Operator

### 2. Tính toán PaymentDeadline
```csharp
var paymentDeadline = departure.DepartureDate1.AddDays(-21);
```
- Hạn thanh toán = Ngày khởi hành - 21 ngày
- Đảm bảo user có đủ thời gian để thanh toán

### 3. Gửi Email
```csharp
await _emailService.SendBookingCreatedPaymentEmailAsync(
    customerEmail, 
    customerName, 
    bookingId, 
    totalPrice, 
    paymentDeadline, 
    tourOperatorPhone
);
```

## Cấu trúc Code

### EmailService
- **Method mới**: `SendBookingCreatedPaymentEmailAsync`
- **Tham số**: email, tên khách hàng, mã booking, số tiền, hạn thanh toán, số điện thoại tour operator

### BookingService
- **Method**: `CreateBookingAsync`
- **Cập nhật**: Thêm logic gửi email thông báo thanh toán
- **Lấy thông tin**: TourOperator.Hotline để làm số điện thoại liên hệ

## Yêu cầu hệ thống

### 1. Database
- Bảng `TourOperator` phải có trường `Hotline`
- Bảng `Tour` phải có quan hệ với `TourOperator`
- Bảng `User` phải có trường `Email` hợp lệ

### 2. Email Service
- Cấu hình SMTP server
- Cấu hình email credentials
- Xử lý lỗi khi gửi email

### 3. Tour Data
- DepartureDate phải cách ngày hiện tại ít nhất 21 ngày
- Tour phải có TourOperator với thông tin Hotline

## Xử lý lỗi
- Nếu không thể gửi email, hệ thống vẫn tiếp tục xử lý booking
- Sử dụng try-catch để đảm bảo không ảnh hưởng đến luồng chính
- Log lỗi để debug và xử lý sau

## Test
Sử dụng file `test_booking_payment_email.http` để test tính năng:
1. Đăng nhập để lấy token
2. Tạo booking mới
3. Kiểm tra email đã được gửi

## Lưu ý
- Email chỉ được gửi khi tạo booking thành công
- Thông tin thanh toán được tính toán chính xác dựa trên số lượng người
- Số điện thoại liên hệ lấy từ TourOperator.Hotline
- Hạn thanh toán được tính tự động dựa trên DepartureDate
