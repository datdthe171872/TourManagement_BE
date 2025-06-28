# TourOperator API Documentation

## Tổng quan
API quản lý Tour Operator cung cấp các chức năng CRUD (Create, Read, Update, Delete) cho tour operator.

## Base URL
```
https://localhost:7001/api/TourOperator
```

## Các Endpoint

### 1. Lấy danh sách Tour Operator
**GET** `/api/TourOperator`

**Query Parameters:**
- `pageNumber` (int): Số trang (mặc định: 1)
- `pageSize` (int): Số lượng item trên mỗi trang (mặc định: 10)
- `companyName` (string): Tên công ty để tìm kiếm (tùy chọn)

**Response:**
```json
{
  "tourOperators": [
    {
      "tourOperatorId": 1,
      "companyName": "Công ty Du lịch ABC",
      "description": "Công ty du lịch hàng đầu Việt Nam",
      "companyLogo": "https://example.com/logo.png",
      "address": "123 Đường ABC, Quận 1, TP.HCM",
      "isActive": true,
      "media": [
        {
          "id": 1,
          "tourOperatorId": 1,
          "mediaUrl": "https://example.com/image1.jpg",
          "caption": "Ảnh văn phòng công ty",
          "uploadedAt": "2024-01-15T10:30:00",
          "isActive": true
        },
        {
          "id": 2,
          "tourOperatorId": 1,
          "mediaUrl": "https://example.com/image2.jpg",
          "caption": "Ảnh đội ngũ nhân viên",
          "uploadedAt": "2024-01-15T11:00:00",
          "isActive": true
        }
      ]
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

### 2. Lấy chi tiết Tour Operator
**GET** `/api/TourOperator/{id}`

**Path Parameters:**
- `id` (int): ID của tour operator

**Response:**
```json
{
  "tourOperatorId": 1,
  "userId": 1,
  "companyName": "Công ty Du lịch ABC",
  "description": "Công ty du lịch hàng đầu Việt Nam",
  "companyLogo": "https://example.com/logo.png",
  "licenseNumber": "GP123456789",
  "licenseIssuedDate": "2020-01-01",
  "taxCode": "0123456789",
  "establishedYear": 2015,
  "hotline": "1900-1234",
  "website": "https://www.abctravel.com",
  "facebook": "https://facebook.com/abctravel",
  "instagram": "https://instagram.com/abctravel",
  "address": "123 Đường ABC, Quận 1, TP.HCM",
  "workingHours": "8:00 - 18:00 (Thứ 2 - Thứ 6)",
  "isActive": true,
  "media": [
    {
      "id": 1,
      "tourOperatorId": 1,
      "mediaUrl": "https://example.com/image1.jpg",
      "caption": "Ảnh văn phòng công ty",
      "uploadedAt": "2024-01-15T10:30:00",
      "isActive": true
    },
    {
      "id": 2,
      "tourOperatorId": 1,
      "mediaUrl": "https://example.com/image2.jpg",
      "caption": "Ảnh đội ngũ nhân viên",
      "uploadedAt": "2024-01-15T11:00:00",
      "isActive": true
    }
  ]
}
```

### 3. Tạo mới Tour Operator
**POST** `/api/TourOperator`

**Request Body:**
```json
{
  "userId": 1,
  "companyName": "Công ty Du lịch ABC",
  "description": "Công ty du lịch hàng đầu Việt Nam",
  "companyLogo": "https://example.com/logo.png",
  "licenseNumber": "GP123456789",
  "licenseIssuedDate": "2020-01-01T00:00:00",
  "taxCode": "0123456789",
  "establishedYear": 2015,
  "hotline": "1900-1234",
  "website": "https://www.abctravel.com",
  "facebook": "https://facebook.com/abctravel",
  "instagram": "https://instagram.com/abctravel",
  "address": "123 Đường ABC, Quận 1, TP.HCM",
  "workingHours": "8:00 - 18:00 (Thứ 2 - Thứ 6)"
}
```

**Validation Rules:**
- `userId`: Bắt buộc, phải là số nguyên > 0
- `companyName`: Bắt buộc, tối đa 255 ký tự
- `description`: Tùy chọn, tối đa 1000 ký tự
- `companyLogo`: Tùy chọn, tối đa 500 ký tự
- `licenseNumber`: Tùy chọn, tối đa 100 ký tự
- `taxCode`: Tùy chọn, tối đa 50 ký tự
- `establishedYear`: Tùy chọn, từ 1900 đến 2100
- `hotline`: Tùy chọn, tối đa 20 ký tự
- `website`: Tùy chọn, phải là URL hợp lệ
- `facebook`: Tùy chọn, tối đa 255 ký tự
- `instagram`: Tùy chọn, tối đa 255 ký tự
- `address`: Tùy chọn, tối đa 500 ký tự
- `workingHours`: Tùy chọn, tối đa 200 ký tự

**Response:**
```json
{
  "message": "Tạo tour operator thành công",
  "data": {
    "tourOperatorId": 1,
    "userId": 1,
    "companyName": "Công ty Du lịch ABC",
    "description": "Công ty du lịch hàng đầu Việt Nam",
    "companyLogo": "https://example.com/logo.png",
    "licenseNumber": "GP123456789",
    "licenseIssuedDate": "2020-01-01",
    "taxCode": "0123456789",
    "establishedYear": 2015,
    "hotline": "1900-1234",
    "website": "https://www.abctravel.com",
    "facebook": "https://facebook.com/abctravel",
    "instagram": "https://instagram.com/abctravel",
    "address": "123 Đường ABC, Quận 1, TP.HCM",
    "workingHours": "8:00 - 18:00 (Thứ 2 - Thứ 6)",
    "isActive": true,
    "media": []
  }
}
```

### 4. Cập nhật Tour Operator
**PUT** `/api/TourOperator/{id}`

**Path Parameters:**
- `id` (int): ID của tour operator cần cập nhật

**Request Body:** (Tương tự như CreateTourOperatorRequest nhưng không có userId)

**Response:**
```json
{
  "message": "Cập nhật tour operator thành công",
  "data": {
    "tourOperatorId": 1,
    "userId": 1,
    "companyName": "Công ty Du lịch ABC - Cập nhật",
    "description": "Công ty du lịch hàng đầu Việt Nam - Mô tả cập nhật",
    "companyLogo": "https://example.com/logo-updated.png",
    "licenseNumber": "GP123456789-UPDATED",
    "licenseIssuedDate": "2020-01-01",
    "taxCode": "0123456789",
    "establishedYear": 2015,
    "hotline": "1900-1234",
    "website": "https://www.abctravel-updated.com",
    "facebook": "https://facebook.com/abctravel-updated",
    "instagram": "https://instagram.com/abctravel-updated",
    "address": "456 Đường XYZ, Quận 2, TP.HCM",
    "workingHours": "8:00 - 19:00 (Thứ 2 - Thứ 7)",
    "isActive": true,
    "media": [
      {
        "id": 1,
        "tourOperatorId": 1,
        "mediaUrl": "https://example.com/image1.jpg",
        "caption": "Ảnh văn phòng công ty",
        "uploadedAt": "2024-01-15T10:30:00",
        "isActive": true
      }
    ]
  }
}
```

### 5. Xóa mềm Tour Operator
**DELETE** `/api/TourOperator/{id}`

**Path Parameters:**
- `id` (int): ID của tour operator cần xóa

**Response:**
```json
{
  "message": "Xóa tour operator thành công"
}
```

## Cấu trúc Media

### TourOperatorMediaResponse
```json
{
  "id": 1,
  "tourOperatorId": 1,
  "mediaUrl": "https://example.com/image.jpg",
  "caption": "Mô tả ảnh",
  "uploadedAt": "2024-01-15T10:30:00",
  "isActive": true
}
```

## Xử lý lỗi

### 400 Bad Request
- Dữ liệu không hợp lệ (validation errors)
- Tên công ty đã tồn tại
- User đã có tour operator

### 404 Not Found
- Không tìm thấy tour operator với ID được cung cấp

### 500 Internal Server Error
- Lỗi server

## Lưu ý
- **Media Integration**: Tất cả các API đều trả về danh sách media của tour operator
- **Xóa mềm**: Tour operator sẽ được đánh dấu `IsActive = false` thay vì xóa hoàn toàn khỏi database
- **Tìm kiếm**: Chỉ trả về các tour operator có `IsActive = true`
- **Media Filtering**: Chỉ trả về các media có `IsActive = true`
- **Validation**: Tất cả các request đều được validate theo các quy tắc đã định nghĩa
- **Duplicate check**: Không cho phép tạo tour operator với tên công ty trùng lặp hoặc userId đã tồn tại

## Ví dụ sử dụng

### Tạo nhiều tour operator cho cùng một user:
```bash
# Tạo tour operator đầu tiên
POST /api/TourOperator
{
  "userId": 1,
  "companyName": "Công ty Du lịch ABC",
  "description": "Công ty du lịch trong nước"
}

# Tạo tour operator thứ hai cho cùng user
POST /api/TourOperator
{
  "userId": 1,
  "companyName": "Công ty Du lịch XYZ",
  "description": "Công ty du lịch quốc tế"
}

# Lấy tất cả tour operator của user
GET /api/TourOperator/user/1
```

## Cấu trúc thư mục
```
Data/
├── DTO/
│   ├── Request/
│   │   └── TourOperatorSearchRequest.cs
│   └── Response/
│       ├── TourOperatorResponse.cs
│       └── TourOperatorListResponse.cs
├── Models/
│   └── TourOperator.cs
└── Context/
    └── MyDBContext.cs

Service/
├── ITourOperatorService.cs
└── TourOperatorService.cs

Controllers/
└── TourOperatorController.cs
```

## Lưu ý
- API chỉ trả về các tour operator có `IsActive = true`
- Tìm kiếm theo tên công ty không phân biệt hoa thường
- Pagination bắt đầu từ trang 1
- Mặc định PageSize = 10 nếu không được chỉ định 