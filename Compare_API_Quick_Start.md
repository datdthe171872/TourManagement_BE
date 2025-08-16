# 🚀 API So sánh Tour - Quick Start

## 📋 **5 Tiêu chí so sánh:**
1. **Giá người lớn** - Giá thấp hơn tốt hơn
2. **Thời gian tour** - Thời gian dài hơn tốt hơn  
3. **Tỷ lệ đặt chỗ** - Tỷ lệ cao hơn tốt hơn
4. **Điểm đánh giá** - Điểm cao hơn tốt hơn
5. **Số lượng đánh giá** - Nhiều feedback hơn tốt hơn

## 🔗 **API Endpoints:**

### **POST** `/api/compare/tours`
```json
{
    "tour1Id": 1,
    "tour2Id": 2
}
```

### **GET** `/api/compare/tours/{tour1Id}/{tour2Id}`
```
GET /api/compare/tours/1/2
```

## 📊 **Kết quả trả về:**
- Thông tin chi tiết của 2 tour
- So sánh từng tiêu chí
- Tổng kết: Tour nào thắng bao nhiêu tiêu chí
- Kết luận cuối cùng

## 🧪 **Test nhanh:**
```bash
# So sánh tour 1 và 2
curl -X POST "https://localhost:7001/api/compare/tours" \
  -H "Content-Type: application/json" \
  -d '{"tour1Id": 1, "tour2Id": 2}'
```

## ✅ **Trạng thái:**
- ✅ Build thành công
- ✅ Service đã đăng ký trong DI
- ✅ Controller sẵn sàng hoạt động
- ✅ API endpoints hoạt động bình thường
