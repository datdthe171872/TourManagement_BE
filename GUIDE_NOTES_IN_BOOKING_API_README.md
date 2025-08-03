# Guide Notes trong Booking API - Thay đổi và Cập nhật

## Tổng quan
Đã thêm `ExtraCost` và `Content` của `GuideNotes` vào Response cho tất cả API get của booking.

## Các thay đổi đã thực hiện

### 1. Cập nhật BookingDetailResponse
**File:** `Data/DTO/Response/BookingDetailResponse.cs`

- Thêm property `GuideNotes` kiểu `List<GuideNotesInfo>`
- Tạo class `GuideNotesInfo` chứa:
  - `NoteId`: ID của note
  - `Title`: Tiêu đề note
  - `Content`: Nội dung note
  - `ExtraCost`: Chi phí phát sinh
  - `CreatedAt`: Ngày tạo

### 2. Cập nhật TourBookingDetailResponse
**File:** `Data/DTO/Response/TourBookingDetailResponse/TourBookingDetailResponse.cs`

- Thêm property `GuideNotes` kiểu `List<GuideNotesInfo>`
- Thêm using statement cho `TourManagement_BE.Data.DTO.Response`

### 3. Cập nhật BookingService
**File:** `Service/BookingService.cs`

- Thêm `System.Collections.Generic` using statement
- Cập nhật tất cả các method detailed để include `TourGuideAssignments` và `GuideNotes`:
  - `GetBookingsDetailedAsync`
  - `GetCustomerBookingsDetailedAsync`
  - `GetTourOperatorBookingsDetailedAsync`
  - `GetAllBookingsForAdminDetailedAsync`
- Cập nhật method `MapToBookingDetailResponse` để map GuideNotes từ tất cả assignments
- Thêm method mới `GetBookingByIdDetailedAsync` để lấy booking chi tiết theo ID

### 4. Cập nhật IBookingService Interface
**File:** `Service/IBookingService.cs`

- Thêm method `GetBookingByIdDetailedAsync`

### 5. Cập nhật BookingController
**File:** `Controllers/BookingController.cs`

- Thêm endpoint mới `GET /api/booking/{bookingId}/detailed` để lấy booking chi tiết theo ID

### 6. Cập nhật TourBookingDetailController
**File:** `Controllers/TourBookingDetailController.cs`

- Thêm include `TourGuideAssignments` và `GuideNotes` vào query

### 7. Cập nhật TourBookingDetailMapping
**File:** `Mapping/TourBookingDetail/TourBookingDetailMapping.cs`

- Thêm mapping cho `GuideNotes` property
- Thêm using statement cho `TourManagement_BE.Data.DTO.Response`

## Các API đã được cập nhật

### 1. BookingController
- `GET /api/booking` - Lấy danh sách booking chi tiết
- `GET /api/booking/customer` - Lấy booking của customer
- `GET /api/booking/tour-operator` - Lấy booking của tour operator
- `GET /api/booking/admin` - Lấy tất cả booking cho admin
- `GET /api/booking/{bookingId}/detailed` - Lấy booking chi tiết theo ID (mới)

### 2. TourBookingDetailController
- `GET /api/tourbookingdetail/ViewBookingDetail/{bookingId}` - Xem chi tiết booking

## Cấu trúc Response mới

### BookingDetailResponse
```json
{
  "bookingId": 1,
  "tour": { ... },
  "booking": { ... },
  "guest": { ... },
  "billingInfo": { ... },
  "paymentInfo": { ... },
  "guideNotes": [
    {
      "noteId": 1,
      "title": "Note Title",
      "content": "Note Content",
      "extraCost": 100.00,
      "createdAt": "2024-01-01T00:00:00"
    }
  ]
}
```

### TourBookingDetailResponse
```json
{
  "bookingId": 1,
  "userId": 1,
  "userName": "user@example.com",
  "tourId": 1,
  "departureDateId": 1,
  "departureDate": "2024-01-01T00:00:00",
  "bookingDate": "2024-01-01T00:00:00",
  "numberOfAdults": 2,
  "numberOfChildren": 1,
  "numberOfInfants": 0,
  "noteForTour": "Special request",
  "totalPrice": 1000.00,
  "contract": "contract_url",
  "bookingStatus": "Confirmed",
  "paymentStatus": "Paid",
  "isActive": true,
  "payments": [ ... ],
  "acceptanceReport": { ... },
  "bookingExtraCharges": [ ... ],
  "guideNotes": [
    {
      "noteId": 1,
      "title": "Note Title",
      "content": "Note Content",
      "extraCost": 100.00,
      "createdAt": "2024-01-01T00:00:00"
    }
  ]
}
```

## Lưu ý
- Chỉ các GuideNotes có `IsActive = true` mới được include trong response
- Chỉ các TourGuideAssignments có `IsActive = true` mới được xem xét
- GuideNotes được lấy từ tất cả TourGuideAssignments của booking
- Project build thành công với 143 warnings (không có errors)

## Testing
Để test các API đã cập nhật, có thể sử dụng các file test HTTP có sẵn:
- `Booking_API_Test.http`
- `Booking_Detailed_API_Test.http`
- `TourBookingDetail_API_Test.http` 