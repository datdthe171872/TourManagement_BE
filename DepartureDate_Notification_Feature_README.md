# Tính năng Gửi Notification và Email cho Departure Dates

## Tổng quan
Tính năng này tự động gửi notification và email cho các bên liên quan khi thực hiện các thao tác với Departure Dates:
- **CreateDepartureDates**: Tạo ngày khởi hành mới
- **UpdateDepartureDate**: Cập nhật ngày khởi hành
- **CancelDepartureDate**: Hủy ngày khởi hành

## Cách hoạt động

### 1. CreateDepartureDates
Khi tạo ngày khởi hành mới:
- **Tour Operator**: Nhận notification và email xác nhận việc tạo thành công
- **Nội dung**: Thông tin tour, ngày khởi hành, thời gian tạo

### 2. UpdateDepartureDate
Khi cập nhật ngày khởi hành:
- **Tour Operator**: Nhận notification và email xác nhận việc cập nhật
- **Users có Booking**: Nhận notification và email thông báo ngày khởi hành đã thay đổi
- **Nội dung**: Ngày khởi hành mới, thời gian cập nhật, số lượng booking bị ảnh hưởng

### 3. CancelDepartureDate
Khi hủy ngày khởi hành:
- **Tour Operator**: Nhận notification và email xác nhận việc hủy
- **Users có Booking**: Nhận notification và email thông báo ngày khởi hành đã bị hủy
- **Nội dung**: Ngày khởi hành bị hủy, thời gian hủy, thông tin về booking bị ảnh hưởng

## Cấu trúc Code

### Interface mới trong IDepartureDateService
```csharp
Task SendDepartureDateCreatedNotificationsAsync(DepartureDate departureDate, int tourOperatorId);
Task SendDepartureDateUpdatedNotificationsAsync(DepartureDate departureDate, int tourOperatorId);
Task SendDepartureDateCancelledNotificationsAsync(DepartureDate departureDate, int tourOperatorId);
```

### Constants mới trong StatusConstants
```csharp
public static class NotificationType
{
    public const string DepartureDateCreated = "DepartureDateCreated";
    public const string DepartureDateUpdated = "DepartureDateUpdated";
    public const string DepartureDateCancelled = "DepartureDateCancelled";
    // ... other types
}
```

### Dependencies
- `INotificationService`: Gửi notification trong hệ thống
- `EmailHelper`: Gửi email thông báo

## Cách sử dụng

### 1. Tạo Departure Date
```http
POST /api/DepartureDates
Authorization: Bearer {token}
Content-Type: application/json

{
  "tourId": 1,
  "startDate": "2024-12-25T00:00:00"
}
```

### 2. Cập nhật Departure Date
```http
PUT /api/DepartureDates
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "departureDate1": "2024-12-26T00:00:00"
}
```

### 3. Hủy Departure Date
```http
PUT /api/DepartureDates/{departureDateId}/cancel
Authorization: Bearer {token}
```

## Xử lý lỗi
- Tất cả các notification và email được gửi trong try-catch blocks
- Lỗi gửi notification/email không làm ảnh hưởng đến thao tác chính
- Lỗi được log ra console để debug

## Lưu ý
- Notification được gửi ngay lập tức sau khi thao tác thành công
- Email được gửi bất đồng bộ để không làm chậm response
- Chỉ gửi email cho users có địa chỉ email hợp lệ
- Tất cả notification được lưu vào database và có thể truy vấn sau

## Testing
Để test tính năng này:
1. Tạo một departure date mới
2. Kiểm tra notification trong database
3. Kiểm tra email được gửi (nếu có cấu hình email)
4. Cập nhật departure date và kiểm tra notification cho users có booking
5. Hủy departure date và kiểm tra notification cho tất cả users liên quan 