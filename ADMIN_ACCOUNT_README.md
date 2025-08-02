# Default Admin Account

## Overview
Hệ thống tự động tạo một tài khoản Admin mặc định khi khởi động lần đầu để đảm bảo luôn có ít nhất một tài khoản Admin trong hệ thống.

## Thông tin tài khoản mặc định

### Credentials
- **Username:** `admin`
- **Password:** `123456`
- **Email:** `admin@gmail.com`
- **Role:** Admin
- **Status:** Active

### Thông tin bổ sung
- **Address:** System Admin
- **Phone:** 0000000000
- **Created:** Tự động khi khởi động ứng dụng lần đầu

## Tính năng

### ✅ Tự động tạo
- Tài khoản Admin được tạo tự động khi ứng dụng khởi động
- Chỉ tạo một lần duy nhất, không tạo lại nếu đã tồn tại
- Role "Admin" cũng được tạo tự động nếu chưa có

### ✅ Bảo mật
- Password được hash bằng PasswordHelper
- Tài khoản có quyền Admin đầy đủ
- Có thể thay đổi password sau khi đăng nhập

### ✅ Logging
- Console sẽ hiển thị thông báo khi tạo tài khoản thành công
- Hiển thị thông tin đăng nhập trong console
- Báo lỗi nếu có vấn đề khi tạo tài khoản

## Console Output

### Khi tạo thành công:
```
✅ Default admin account created successfully!
Username: admin
Password: 123456
Email: admin@gmail.com
```

### Khi đã tồn tại:
```
ℹ️  Default admin account already exists.
```

### Khi có lỗi:
```
❌ Error creating default admin account: [Error message]
```

## Sử dụng

1. **Khởi động ứng dụng** - Tài khoản sẽ được tạo tự động
2. **Đăng nhập** với thông tin mặc định
3. **Thay đổi password** ngay sau khi đăng nhập lần đầu
4. **Sử dụng các tính năng Admin** như quản lý feedback, users, etc.

## Lưu ý bảo mật

⚠️ **QUAN TRỌNG:** 
- Thay đổi password ngay sau khi đăng nhập lần đầu
- Không sử dụng password mặc định trong môi trường production
- Backup thông tin tài khoản Admin
- Chỉ có 1 tài khoản Admin mặc định, không tạo thêm

## API Endpoints cho Admin

### Feedback Management
- `PUT /api/Feedback/update-status` - Cập nhật trạng thái feedback

### User Management
- Các API quản lý user khác (nếu có)

### System Management
- Các API quản lý hệ thống khác (nếu có) 