# Feedback API Changes Summary

## 🎯 Yêu cầu ban đầu
Tôi muốn xoá 2 api GetFeedbacks, GetFeedbackDetail, thay vào đó thêm api cho admin get tất cả các feedback có trong bảng TourRatings, các feedback sắp xếp từ mới nhất tới cũ nhất, search được Username, RatingId, 1 api cho Tour Operator có thể get các FeedBack về TourId của TourOperatorId, 1 api cho Tour Operator để Report FeedBack về Tour của họ( chọn RatingId, chọn các lí do vì sao muốn Report) rồi bấm report sẽ gửi Notification cho Admin, admin sẽ biết RatingId nào bị Report vì lí do gì

## ✅ Đã hoàn thành

### ❌ Đã xóa bỏ:
1. **`GET /api/Feedback`** - API public lấy tất cả feedback
2. **`GET /api/Feedback/{id}`** - API public lấy chi tiết feedback

### ✅ Đã thêm mới:

#### 1. Admin APIs
- **`GET /api/Feedback/admin`** - Admin lấy tất cả feedback với search theo Username, RatingId
- **`PUT /api/Feedback/update-status`** - Admin cập nhật trạng thái feedback (đã có từ trước)

#### 2. Tour Operator APIs  
- **`GET /api/Feedback/tour-operator`** - Tour Operator lấy feedback về tour của họ
- **`POST /api/Feedback/report`** - Tour Operator báo cáo feedback

#### 3. Customer APIs (giữ nguyên)
- **`POST /api/Feedback`** - Tạo feedback với upload ảnh
- **`GET /api/Feedback/my-feedbacks`** - Lấy feedback của user đã đăng nhập

### 🔔 Tính năng mới:

#### 1. Notification System
- **Feedback Report Notification**: Khi Tour Operator báo cáo feedback, tất cả Admin sẽ nhận được notification với thông tin chi tiết
- **Feedback Violation Notification**: Khi Admin ẩn feedback, user sẽ nhận được notification (đã có từ trước)

#### 2. Search & Filter
- **Admin**: Search theo Username, RatingId
- **Tour Operator**: Search theo RatingId
- **Sắp xếp**: Tất cả feedback đều được sắp xếp từ mới nhất đến cũ nhất

#### 3. Phân quyền rõ ràng
- **Admin**: Chỉ Admin mới có thể truy cập API admin
- **Tour Operator**: Chỉ Tour Operator mới có thể truy cập feedback của tour của họ
- **Customer**: Bất kỳ user đã đăng nhập nào cũng có thể tạo và xem feedback của mình

## 📁 Files đã tạo/sửa đổi:

### New DTOs:
- `Data/DTO/Request/AdminFeedbackSearchRequest.cs`
- `Data/DTO/Request/TourOperatorFeedbackSearchRequest.cs` 
- `Data/DTO/Request/ReportFeedbackRequest.cs`

### Updated Services:
- `Service/IFeedbackService.cs` - Thêm 3 method mới
- `Service/FeedbackService.cs` - Implement 3 method mới
- `Service/INotificationService.cs` - Thêm method notification mới
- `Service/NotificationService.cs` - Implement notification mới

### Updated Controller:
- `Controllers/FeedbackController.cs` - Xóa 2 API cũ, thêm 3 API mới

### Documentation:
- `Feedback_API_Test_Updated_V2.http` - File test mới
- `Feedback_API_README_V2.md` - Documentation mới
- `FEEDBACK_API_CHANGES_SUMMARY.md` - File tóm tắt này

## 🔧 Technical Details:

### 1. Admin Feedback Search
```csharp
// Search theo Username (contains)
if (!string.IsNullOrWhiteSpace(request.Username))
{
    query = query.Where(tr => tr.User != null && 
                            tr.User.UserName != null && 
                            tr.User.UserName.Contains(request.Username));
}

// Search theo RatingId (exact match)
if (request.RatingId.HasValue)
{
    query = query.Where(tr => tr.RatingId == request.RatingId.Value);
}
```

### 2. Tour Operator Feedback Filter
```csharp
// Chỉ lấy feedback của tour thuộc về Tour Operator
var query = _context.TourRatings
    .Include(tr => tr.Tour)
    .Include(tr => tr.User)
    .Where(tr => tr.Tour.TourOperatorId == tourOperatorId);
```

### 3. Report System
```csharp
// Verify feedback thuộc về tour của Tour Operator
var feedback = await _context.TourRatings
    .Include(tr => tr.Tour)
    .FirstOrDefaultAsync(tr => tr.RatingId == request.RatingId && 
                              tr.Tour.TourOperatorId == tourOperatorId);

// Gửi notification cho tất cả Admin
var adminUsers = await _context.Users
    .Include(u => u.Role)
    .Where(u => u.Role.RoleName == "Admin" && u.IsActive)
    .ToListAsync();
```

### 4. Notification Content
```csharp
// Feedback Report Notification
Title: "Báo cáo Feedback mới"
Message: $"Có một feedback (ID: {ratingId}) bị báo cáo bởi Tour Operator (ID: {tourOperatorId}). Lý do: {reason}"
```

## 🚀 Build Status:
✅ **Build thành công** - Không có lỗi compile

## 📋 Test Cases:
1. **Admin Login** → **Get All Feedbacks** → **Search by Username/RatingId**
2. **Tour Operator Login** → **Get Tour Operator Feedbacks** → **Search by RatingId**
3. **Tour Operator Login** → **Report Feedback** → **Check Admin Notification**
4. **Customer Login** → **Create Feedback** → **Get My Feedbacks**
5. **Admin Login** → **Update Feedback Status** → **Check User Notification**

## 🔒 Security Features:
1. **Role-based Authorization**: Mỗi API chỉ cho phép role tương ứng
2. **Data Isolation**: Tour Operator chỉ thấy feedback của tour của họ
3. **Input Validation**: Validate tất cả input parameters
4. **File Upload Security**: Chỉ chấp nhận file ảnh, kích thước tối đa 10MB
5. **Report Verification**: Tour Operator chỉ có thể báo cáo feedback của tour thuộc về họ

## 🎉 Kết quả:
- ✅ Đã xóa 2 API public như yêu cầu
- ✅ Đã thêm API Admin với search Username, RatingId
- ✅ Đã thêm API Tour Operator lấy feedback của họ
- ✅ Đã thêm API Tour Operator report feedback
- ✅ Đã implement notification system cho Admin
- ✅ Đã sắp xếp feedback từ mới nhất đến cũ nhất
- ✅ Đã phân quyền rõ ràng cho từng role
- ✅ Build thành công, sẵn sàng test 