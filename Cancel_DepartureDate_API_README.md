# Cancel DepartureDate API

## Tổng quan
API này cho phép TourOperator hủy một ngày khởi hành (DepartureDate) và tự động cập nhật trạng thái tất cả booking trong ngày khởi hành đó thành "Cancelled".

## Endpoint
```
PUT /api/DepartureDates/{departureDateId}/cancel
```

## Quyền truy cập
- **Role**: TourOperator
- **Authentication**: Bearer Token

## Tham số
| Tên | Loại | Vị trí | Bắt buộc | Mô tả |
|-----|------|--------|----------|-------|
| departureDateId | int | path | Có | ID của ngày khởi hành cần hủy |

## Logic xử lý

### 1. Kiểm tra quyền truy cập
- Xác thực JWT token
- Kiểm tra user có role TourOperator
- Lấy UserId từ token

### 2. Validation
- Kiểm tra departureDateId > 0
- Kiểm tra TourOperator có tồn tại và đang active
- Kiểm tra DepartureDate có tồn tại và thuộc về TourOperator này
- Kiểm tra ngày khởi hành chưa diễn ra (chưa đến ngày)

### 3. Cập nhật dữ liệu
- Set `IsCancelDate = true` cho DepartureDate
- Set `IsActive = false` cho DepartureDate
- Cập nhật `BookingStatus = "Cancelled"` cho tất cả Booking trong DepartureDate này

## Response

### Success Response (200 OK)
```json
{
  "message": "Hủy ngày khởi hành thành công. Tất cả booking trong ngày khởi hành này đã được cập nhật thành Cancelled."
}
```

### Error Responses

#### 400 Bad Request - Invalid ID
```json
{
  "message": "DepartureDateId không hợp lệ"
}
```

#### 400 Bad Request - Cannot Cancel
```json
{
  "message": "Không thể hủy ngày khởi hành. Vui lòng kiểm tra lại thông tin hoặc ngày khởi hành đã diễn ra."
}
```

#### 400 Bad Request - User not found
```json
{
  "message": "Không thể xác định thông tin user"
}
```

#### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

## Ví dụ sử dụng

### Request
```http
PUT /api/DepartureDates/1/cancel
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

### Response
```json
{
  "message": "Hủy ngày khởi hành thành công. Tất cả booking trong ngày khởi hành này đã được cập nhật thành Cancelled."
}
```

## Lưu ý
1. Chỉ TourOperator mới có quyền hủy ngày khởi hành
2. Chỉ có thể hủy ngày khởi hành chưa diễn ra
3. Khi hủy ngày khởi hành, tất cả booking trong ngày đó sẽ tự động chuyển thành trạng thái "Cancelled"
4. DepartureDate sẽ bị ẩn (IsActive = false) và đánh dấu là đã hủy (IsCancelDate = true)

## Testing
Sử dụng file `Cancel_DepartureDate_API_Test.http` để test API này.

---

# Get Cancelled DepartureDates API

## Tổng quan
API này cho phép TourOperator lấy danh sách tất cả các ngày khởi hành đã bị hủy của mình.

## Endpoint
```
GET /api/DepartureDates/operator/cancelled
```

## Quyền truy cập
- **Role**: TourOperator
- **Authentication**: Bearer Token

## Tham số
Không có tham số

## Logic xử lý
1. Xác thực JWT token và kiểm tra role TourOperator
2. Lấy UserId từ token
3. Tìm TourOperator tương ứng với UserId
4. Lấy tất cả TourIds của TourOperator này
5. Lấy tất cả DepartureDates có `IsCancelDate = true` và `IsActive = false`

## Response

### Success Response (200 OK)
```json
{
  "message": "Lấy danh sách ngày khởi hành đã bị hủy thành công",
  "data": [
    {
      "id": 1,
      "tourId": 1,
      "tourTitle": "Tour Name",
      "departureDate": "2024-01-15T00:00:00",
      "isActive": false,
      "totalBookings": 0,
      "availableSlots": 20,
      "tourGuides": []
    }
  ]
}
```

---

# Reactivate DepartureDate API

## Tổng quan
API này cho phép TourOperator bật lại một ngày khởi hành đã bị hủy, với điều kiện ngày khởi hành phải cách hiện tại ít nhất 5 ngày.

## Endpoint
```
PUT /api/DepartureDates/{departureDateId}/reactivate
```

## Quyền truy cập
- **Role**: TourOperator
- **Authentication**: Bearer Token

## Tham số
| Tên | Loại | Vị trí | Bắt buộc | Mô tả |
|-----|------|--------|----------|-------|
| departureDateId | int | path | Có | ID của ngày khởi hành cần bật lại |

## Logic xử lý

### 1. Kiểm tra quyền truy cập
- Xác thực JWT token
- Kiểm tra user có role TourOperator
- Lấy UserId từ token

### 2. Validation
- Kiểm tra departureDateId > 0
- Kiểm tra TourOperator có tồn tại và đang active
- Kiểm tra DepartureDate có tồn tại, thuộc về TourOperator này, và đã bị hủy
- **Kiểm tra ngày khởi hành cách hiện tại ít nhất 5 ngày**

### 3. Cập nhật dữ liệu
- Set `IsCancelDate = false` cho DepartureDate
- Set `IsActive = true` cho DepartureDate
- Khôi phục `BookingStatus = "Pending"` cho tất cả Booking trong DepartureDate này

## Response

### Success Response (200 OK)
```json
{
  "message": "Bật lại ngày khởi hành thành công. Tất cả booking trong ngày khởi hành này đã được khôi phục thành Pending."
}
```

### Error Responses

#### 400 Bad Request - Invalid ID
```json
{
  "message": "DepartureDateId không hợp lệ"
}
```

#### 400 Bad Request - Cannot Reactivate
```json
{
  "message": "Không thể bật lại ngày khởi hành. Vui lòng kiểm tra lại thông tin hoặc ngày khởi hành phải cách hiện tại ít nhất 5 ngày."
}
```

#### 400 Bad Request - User not found
```json
{
  "message": "Không thể xác định thông tin user"
}
```

#### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

## Ví dụ sử dụng

### Request
```http
PUT /api/DepartureDates/1/reactivate
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

### Response
```json
{
  "message": "Bật lại ngày khởi hành thành công. Tất cả booking trong ngày khởi hành này đã được khôi phục thành Pending."
}
```

## Lưu ý quan trọng
1. Chỉ TourOperator mới có quyền bật lại ngày khởi hành
2. **Chỉ có thể bật lại ngày khởi hành cách hiện tại ít nhất 5 ngày**
3. Chỉ có thể bật lại những ngày khởi hành đã bị hủy trước đó
4. Khi bật lại, DepartureDate sẽ trở thành active và có thể nhận booking mới
5. **Các booking đã bị cancelled sẽ được khôi phục thành "Pending"** 