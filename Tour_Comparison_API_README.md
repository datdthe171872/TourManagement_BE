# API So sánh 2 Tour

## Tổng quan
API so sánh 2 tour dựa trên 5 tiêu chí chính để giúp người dùng đưa ra quyết định lựa chọn tour phù hợp.

## 5 Tiêu chí so sánh

### 1. **Giá người lớn** 🏷️
- **Tiêu chí**: Giá thấp hơn tốt hơn
- **Đơn vị**: VNĐ
- **Mô tả**: So sánh trực tiếp giá vé người lớn

### 2. **Thời gian tour** ⏰
- **Tiêu chí**: Thời gian dài hơn tốt hơn
- **Đơn vị**: Ngày
- **Mô tả**: Tour dài ngày thường có nhiều điểm đến hơn

### 3. **Tỷ lệ đặt chỗ** 📊
- **Tiêu chí**: Tỷ lệ cao hơn tốt hơn
- **Đơn vị**: %
- **Mô tả**: Phản ánh độ phổ biến và uy tín của tour

### 4. **Điểm đánh giá** ⭐
- **Tiêu chí**: Điểm cao hơn tốt hơn
- **Đơn vị**: Sao (1-5)
- **Mô tả**: Chất lượng tour theo đánh giá của khách hàng

### 5. **Số lượng đánh giá** 💬
- **Tiêu chí**: Nhiều feedback hơn tốt hơn
- **Đơn vị**: Số lượng
- **Mô tả**: Độ tin cậy của điểm đánh giá

## API Endpoints

### POST `/api/compare/tours`
So sánh 2 tour bằng request body

**Request Body:**
```json
{
    "tour1Id": 1,
    "tour2Id": 2
}
```

### GET `/api/compare/tours/{tour1Id}/{tour2Id}`
So sánh 2 tour bằng URL parameters (nhanh hơn)

**URL Example:** `/api/compare/tours/1/2`

## Response Format

```json
{
    "tour1": {
        "tourId": 1,
        "title": "Tour Hà Nội - Sapa",
        "description": "Khám phá vẻ đẹp miền núi Tây Bắc",
        "durationInDays": "3 ngày",
        "priceOfAdults": 2500000,
        "priceOfChildren": 1800000,
        "priceOfInfants": 500000,
        "maxSlots": 20,
        "slotsBooked": 15,
        "occupancyRate": 75.0,
        "averageRating": 4.5,
        "totalRatings": 28,
        "startPoint": "Hà Nội",
        "transportation": "Xe khách",
        "companyName": "Vietnam Travel"
    },
    "tour2": {
        // Thông tin tương tự cho tour 2
    },
    "result": {
        "tour1Wins": 3,
        "tour2Wins": 2,
        "ties": 0,
        "winner": "Tour 1 thắng (3/5 tiêu chí)",
        "criterionComparisons": [
            {
                "criterionName": "Giá người lớn",
                "winner": "Tour1",
                "description": "Tour 1 có Giá người lớn tốt hơn: 2500000 VNĐ vs 3000000 VNĐ",
                "tour1Value": 2500000,
                "tour2Value": 3000000
            }
            // ... các tiêu chí khác
        ]
    }
}
```

## Cách tính điểm

- **Tour1Wins**: Số tiêu chí mà Tour 1 thắng
- **Tour2Wins**: Số tiêu chí mà Tour 2 thắng  
- **Ties**: Số tiêu chí hòa
- **Winner**: Kết quả tổng thể (Tour nào thắng bao nhiêu tiêu chí)

## Validation

- Không thể so sánh tour với chính nó
- TourId phải là số dương
- Cả 2 tour phải tồn tại và đang hoạt động

## Sử dụng

1. **So sánh nhanh**: Sử dụng GET endpoint với URL parameters
2. **So sánh chi tiết**: Sử dụng POST endpoint với request body
3. **Kết quả**: Hiển thị tour nào tốt hơn về từng tiêu chí và tổng thể

## Lưu ý

- API chỉ so sánh các tour đang hoạt động (IsActive = true)
- Điểm đánh giá được tính trung bình từ các feedback hợp lệ
- Tỷ lệ đặt chỗ = (Số chỗ đã đặt / Số chỗ tối đa) × 100%
- Thời gian tour được parse từ chuỗi "X ngày" thành số
