# Tour Operator API

## Mô tả
API để quản lý danh sách các tour operator với chức năng tìm kiếm và phân trang.

## Endpoints

### GET /api/TourOperator
Lấy danh sách tour operator với tìm kiếm theo tên công ty và phân trang.

#### Query Parameters:
- `CompanyName` (optional): Tên công ty để tìm kiếm
- `PageNumber` (optional): Số trang (mặc định: 1)
- `PageSize` (optional): Số lượng item trên mỗi trang (mặc định: 10)

#### Response:
```json
{
  "tourOperators": [
    {
      "tourOperatorId": 1,
      "companyName": "Vietnam Travel",
      "description": "Công ty du lịch hàng đầu Việt Nam",
      "companyLogo": "https://example.com/logo.png",
      "address": "123 Nguyễn Huệ, Quận 1, TP.HCM",
      "isActive": true
    }
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

## Ví dụ sử dụng

### Lấy tất cả tour operator:
```
GET /api/TourOperator
```

### Tìm kiếm theo tên công ty:
```
GET /api/TourOperator?CompanyName=Vietnam
```

### Phân trang:
```
GET /api/TourOperator?PageNumber=2&PageSize=5
```

### Kết hợp tìm kiếm và phân trang:
```
GET /api/TourOperator?CompanyName=Travel&PageNumber=1&PageSize=10
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