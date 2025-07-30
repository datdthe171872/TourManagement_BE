# Report API - Tóm tắt

## 🎯 Mục đích
Tạo báo cáo tổng hợp cho Customer và Tour Operator với thông tin chi tiết về booking, tour, chi phí và tổng thanh toán.

## 📋 API Endpoints

### 1. Customer Report
```http
GET /api/Report/customer
Authorization: Bearer {customer_token}
```

**Quyền:** Customer  
**Chức năng:** Lấy báo cáo của chính customer đó  
**Search:** `?username=xxx`

### 2. Tour Operator Report  
```http
GET /api/Report/operator
Authorization: Bearer {operator_token}
```

**Quyền:** Tour Operator  
**Chức năng:** Lấy báo cáo của tất cả booking thuộc các tour của operator  
**Search:** `?username=xxx&bookingId=123`

## 📊 Dữ liệu trả về

### ReportResponse
```json
{
  "username": "string",        // Tên người dùng
  "bookingId": "int",          // ID booking
  "tourTitle": "string",       // Tiêu đề tour
  "contract": "string",        // URL hợp đồng (hoặc "N/A")
  "totalPrice": "decimal",     // Giá tour gốc
  "totalExtraCost": "decimal", // Tổng chi phí phát sinh
  "total": "decimal"           // Tổng thanh toán (TotalPrice + TotalExtraCost)
}
```

## 🔧 Tính năng chính

### ✅ Phân quyền
- **Customer**: Chỉ thấy booking của chính mình
- **Tour Operator**: Chỉ thấy booking của các tour thuộc về mình

### ✅ Tính toán tự động
- **TotalExtraCost**: Tổng từ TourAcceptanceReports
- **Total**: TotalPrice + TotalExtraCost

### ✅ Tìm kiếm và lọc
- **Username**: Partial match (case-insensitive)
- **BookingId**: Exact match
- **Kết hợp**: Username và BookingId

### ✅ Lọc dữ liệu
- Chỉ hiển thị booking active (IsActive = true)
- Chỉ hiển thị report active (IsActive = true)

## 🗂️ Files đã tạo

### 1. DTO
- `Data/DTO/Response/ReportResponse.cs`

### 2. Service
- `Service/IReportService.cs`
- `Service/ReportService.cs`

### 3. Controller
- `Controllers/ReportController.cs`

### 4. Documentation
- `Report_API_Test.http`
- `Report_API_README.md`
- `Report_API_Summary.md`

### 5. Registration
- Đã đăng ký `IReportService` trong `Program.cs`

## 🚀 Luồng hoạt động

### Customer Flow
1. Customer đăng nhập → Lấy JWT token
2. Gọi API `/api/Report/customer`
3. Hệ thống lọc booking theo UserId
4. Trả về danh sách báo cáo

### Tour Operator Flow
1. Tour Operator đăng nhập → Lấy JWT token
2. Gọi API `/api/Report/operator`
3. Hệ thống lọc booking theo TourOperatorId
4. Trả về danh sách báo cáo

## 🔒 Bảo mật

### Authentication
- Tất cả API đều yêu cầu JWT token

### Authorization
- Customer API: Chỉ role "Customer"
- Operator API: Chỉ role "Tour Operator"

### Data Isolation
- Customer chỉ thấy dữ liệu của mình
- Operator chỉ thấy dữ liệu của các tour thuộc về mình

## 📝 Lưu ý

1. **Nullable Handling**: Đã xử lý nullable cho TotalPrice và TotalExtraCost
2. **Soft Delete**: Chỉ hiển thị dữ liệu active
3. **Performance**: Sử dụng Include để eager loading
4. **Error Handling**: Có xử lý lỗi khi Tour Operator không tồn tại

## ✅ Status
- ✅ Build thành công
- ✅ API endpoints hoàn thành
- ✅ Documentation đầy đủ
- ✅ Test cases sẵn sàng
- ✅ Error handling đã implement 