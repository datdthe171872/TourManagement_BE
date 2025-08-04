# API Lấy Booking theo DepartureDateId (Cập nhật)

## 🎯 **Mục đích**
API để lấy tất cả booking trong một DepartureDateId cụ thể. **Đã cập nhật để hỗ trợ cả TourOperator và TourGuide.**

## 📋 **API Endpoint**

### **GET** `/api/DepartureDates/departure-date/{departureDateId}/bookings`

**Mô tả:** Lấy tất cả booking trong một DepartureDateId cụ thể của TourOperator hoặc TourGuide hiện tại

**Quyền truy cập:** TourOperator, TourGuide

**Parameters:**
- `departureDateId` (int, required): ID của ngày khởi hành

**Headers:**
```
Authorization: Bearer {token}
```

## 🔧 **Implementation**

### 1. **Controller Method**
**File:** `Controllers/DepartureDatesController.cs`

```csharp
[HttpGet("departure-date/{departureDateId}/bookings")]
[Authorize(Roles = Roles.TourOperator + "," + Roles.TourGuide)]
public async Task<IActionResult> GetBookingsByDepartureDateId(int departureDateId)
{
    if (departureDateId <= 0)
    {
        return BadRequest(new
        {
            Message = "DepartureDateId không hợp lệ"
        });
    }

    // Lấy UserId từ JWT token
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    {
        return BadRequest(new
        {
            Message = "Không thể xác định thông tin user"
        });
    }

    var departureDateWithBookings = await _departureDateService.GetBookingsByDepartureDateIdAsync(departureDateId, userId);
    
    if (departureDateWithBookings == null)
    {
        return NotFound(new
        {
            Message = "Không tìm thấy ngày khởi hành hoặc bạn không có quyền truy cập"
        });
    }

    return Ok(new
    {
        Message = "Lấy danh sách booking theo ngày khởi hành thành công",
        Data = departureDateWithBookings
    });
}
```

### 2. **Service Interface**
**File:** `Service/IDepartureDateService.cs`

```csharp
Task<DepartureDateWithBookingResponse?> GetBookingsByDepartureDateIdAsync(int departureDateId, int userId);
```

### 3. **Service Implementation**
**File:** `Service/DepartureDateService.cs`

```csharp
public async Task<DepartureDateBookingsWrapperResponse?> GetBookingsByDepartureDateIdAsync(int departureDateId, int userId)
{
    // Kiểm tra xem user có phải là TourOperator không
    var tourOperator = await _context.TourOperators
        .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
    
    // Kiểm tra xem user có phải là TourGuide không
    var tourGuide = await _context.TourGuides
        .FirstOrDefaultAsync(tg => tg.UserId == userId && tg.IsActive);
    
    if (tourOperator == null && tourGuide == null)
        return null;
    
    // Query cơ bản cho DepartureDate
    var departureDateQuery = _context.DepartureDates
        .Include(dd => dd.Tour)
        .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .ThenInclude(b => b.User)
        .Where(dd => dd.Id == departureDateId && dd.IsActive);
    
    // Nếu là TourOperator, kiểm tra quyền sở hữu tour
    if (tourOperator != null)
    {
        departureDateQuery = departureDateQuery.Where(dd => dd.Tour.TourOperatorId == tourOperator.TourOperatorId);
    }
    // Nếu là TourGuide, kiểm tra xem có được assign cho departureDate này không
    else if (tourGuide != null)
    {
        departureDateQuery = departureDateQuery.Where(dd => 
            _context.TourGuideAssignments.Any(tga => 
                tga.DepartureDateId == dd.Id && 
                tga.TourGuideId == tourGuide.TourGuideId && 
                tga.IsActive));
    }
    
    var departureDate = await departureDateQuery.FirstOrDefaultAsync();
    if (departureDate == null)
        return null;

    // Bước 3: Tạo response
    var response = new DepartureDateWithBookingResponse
    {
        Id = departureDate.Id,
        TourId = departureDate.TourId,
        TourTitle = departureDate.Tour.Title,
        DepartureDate = departureDate.DepartureDate1,
        IsActive = departureDate.IsActive,
        TotalBookings = departureDate.Bookings.Count,
        AvailableSlots = departureDate.Tour.MaxSlots - (departureDate.Tour.SlotsBooked ?? 0),
        Bookings = departureDate.Bookings.Select(b => new BookingInfo
        {
            BookingId = b.BookingId,
            UserId = b.UserId,
            UserName = b.User.UserName ?? "Unknown",
            UserEmail = b.User.Email,
            BookingDate = b.BookingDate,
            NumberOfAdults = b.NumberOfAdults,
            NumberOfChildren = b.NumberOfChildren,
            NumberOfInfants = b.NumberOfInfants,
            TotalPrice = b.TotalPrice,
            BookingStatus = b.BookingStatus,
            PaymentStatus = b.PaymentStatus
        }).ToList()
    };

    // Thêm thông tin TourGuide
    response.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);

    return response;
}
```

## 📊 **Response Format**

### **Success Response (200 OK)**
```json
{
  "message": "Lấy danh sách booking theo ngày khởi hành thành công",
  "data": {
    "id": 1,
    "tourId": 1,
    "tourTitle": "Tour Hà Nội - Sapa",
    "departureDate": "2024-02-01T00:00:00",
    "isActive": true,
    "totalBookings": 3,
    "availableSlots": 17,
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
      },
      {
        "bookingId": 2,
        "userId": 6,
        "userName": "Trần Thị B",
        "userEmail": "tranthib@email.com",
        "bookingDate": "2024-01-16T14:20:00",
        "numberOfAdults": 1,
        "numberOfChildren": 0,
        "numberOfInfants": 0,
        "totalPrice": 800000,
        "bookingStatus": "Pending",
        "paymentStatus": "Unpaid"
      }
    ],
    "tourGuides": [
      {
        "tourGuideId": 1,
        "userName": "Guide A",
        "email": "guidea@email.com"
      }
    ]
  }
}
```

### **Error Responses**

#### **400 Bad Request (Invalid DepartureDateId)**
```json
{
  "message": "DepartureDateId không hợp lệ"
}
```

#### **400 Bad Request (Invalid User)**
```json
{
  "message": "Không thể xác định thông tin user"
}
```

#### **404 Not Found**
```json
{
  "message": "Không tìm thấy ngày khởi hành hoặc bạn không có quyền truy cập"
}
```

## 🔒 **Security & Authorization**

### **Quyền truy cập:**
- ✅ **TourOperator**: Có thể truy cập
- ❌ **Tour Guide**: Không thể truy cập
- ❌ **Customer**: Không thể truy cập
- ❌ **Admin**: Không thể truy cập

### **Kiểm tra quyền truy cập:**
- **TourOperator:** Chỉ TourOperator sở hữu tour mới có thể xem booking của DepartureDate đó
  - Kiểm tra `dd.Tour.TourOperatorId == tourOperator.TourOperatorId`
- **TourGuide:** Chỉ TourGuide được assign cho DepartureDate mới có thể xem booking
  - Kiểm tra `TourGuideAssignments` với `DepartureDateId` và `TourGuideId`

## 🎯 **So sánh với API hiện tại**

### **API hiện tại:**
- **Endpoint:** `GET /api/DepartureDates/operator/with-bookings`
- **Chức năng:** Lấy tất cả DepartureDate và booking của Operator
- **Response:** Array của DepartureDateWithBookingResponse

### **API mới:**
- **Endpoint:** `GET /api/DepartureDates/departure-date/{departureDateId}/bookings`
- **Chức năng:** Lấy booking của một DepartureDateId cụ thể
- **Response:** Single DepartureDateWithBookingResponse

## 🧪 **Test Cases**

### **Test Case 1: Valid Request**
```http
GET /api/DepartureDates/departure-date/1/bookings
Authorization: Bearer {tour_operator_token}
```
**Expected:** 200 OK với data

### **Test Case 2: Invalid DepartureDateId**
```http
GET /api/DepartureDates/departure-date/0/bookings
Authorization: Bearer {tour_operator_token}
```
**Expected:** 400 Bad Request

### **Test Case 3: Non-existent DepartureDateId**
```http
GET /api/DepartureDates/departure-date/999/bookings
Authorization: Bearer {tour_operator_token}
```
**Expected:** 404 Not Found

### **Test Case 4: Unauthorized Access**
```http
GET /api/DepartureDates/departure-date/1/bookings
Authorization: Bearer {customer_token}
```
**Expected:** 403 Forbidden

### **Test Case 5: DepartureDate của Operator khác**
```http
GET /api/DepartureDates/departure-date/1/bookings
Authorization: Bearer {other_operator_token}
```
**Expected:** 404 Not Found

## 🚀 **Build Status**
- ✅ **Build thành công**
- ✅ **Không có lỗi compilation**
- ✅ **Tất cả warnings đã được xử lý**

## 📝 **Usage Examples**

### **Frontend Integration:**
```javascript
// Lấy booking của DepartureDateId = 1
const response = await fetch('/api/DepartureDates/departure-date/1/bookings', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});

const result = await response.json();
console.log('Bookings:', result.data.bookings);
```

### **Postman Collection:**
```json
{
  "name": "Get Bookings by DepartureDateId",
  "request": {
    "method": "GET",
    "url": "{{baseUrl}}/api/DepartureDates/departure-date/1/bookings",
    "headers": {
      "Authorization": "Bearer {{token}}"
    }
  }
}
```

## 🎉 **Kết luận**

API đã được cập nhật thành công để lấy tất cả booking trong một DepartureDateId cụ thể. **API này hiện hỗ trợ cả TourOperator và TourGuide.** API này cung cấp:

1. **Tính năng chính:** Lấy booking theo DepartureDateId
2. **Bảo mật:** Kiểm tra quyền truy cập cho cả TourOperator và TourGuide
3. **Response đầy đủ:** Bao gồm thông tin tour, booking và tour guide
4. **Error handling:** Xử lý các trường hợp lỗi một cách rõ ràng

**Thay đổi chính:**
- Route đã được đơn giản hóa từ `/operator/departure-date/{id}/bookings` thành `/departure-date/{id}/bookings`
- Hỗ trợ cả TourOperator và TourGuide roles
- TourGuide chỉ có thể xem booking của các DepartureDate mà họ được assign

API này bổ sung hoàn hảo cho hệ thống và cho phép cả TourOperator và TourGuide quản lý booking một cách chi tiết hơn! 🚀 