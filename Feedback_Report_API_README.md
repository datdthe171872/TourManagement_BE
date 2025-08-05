# Feedback Report API - Updated

## Overview
The Feedback Report API has been updated to allow **all authenticated users** to report feedback, not just Tour Operators. This change promotes community moderation and allows users to flag inappropriate or misleading content.

## Changes Made

### Before (Restricted Access)
- Only **Tour Operators** could report feedback
- Required `[Authorize(Roles = "Tour Operator")]` attribute
- Only allowed reporting feedback related to their own tours

### After (Universal Access)
- **All authenticated users** can report feedback
- Uses `[Authorize]` attribute (any authenticated user)
- Different validation logic based on user role

## API Endpoint

### Report Feedback
**Endpoint:** `POST /api/Feedback/report`

**Authorization:** Requires authentication (any role)

**Request Body:**
```json
{
    "ratingId": 1,
    "reason": "Feedback này chứa nội dung không phù hợp"
}
```

**Response:**
```json
{
    "message": "Báo cáo feedback thành công. Admin sẽ được thông báo về vấn đề này.",
    "data": {
        "ratingId": 1,
        "reportedBy": "username",
        "userRole": "Customer",
        "tourOperatorId": null
    }
}
```

## User Role Behavior

### 1. **Customer/Tour Guide/Admin**
- Can report any feedback
- No ownership validation required
- `tourOperatorId` will be `null` in response

### 2. **Tour Operator**
- Can report any feedback (including their own tours)
- If reporting feedback about their own tour, `tourOperatorId` will be included
- Maintains existing validation for their own tours

## Validation Logic

### For All Users:
1. **Authentication Check:** User must be logged in
2. **Feedback Existence:** Feedback with specified `ratingId` must exist
3. **User Status:** User account must be active

### For Tour Operators (Additional):
1. **Tour Ownership:** If reporting feedback about their own tour, validates ownership
2. **Tour Operator Status:** Tour operator account must be active

## Error Responses

### 404 Not Found:
```json
{
    "message": "Không tìm thấy thông tin user."
}
```

### 404 Not Found (Tour Operator specific):
```json
{
    "message": "Không tìm thấy feedback với id này hoặc feedback không thuộc về tour của bạn."
}
```

### 404 Not Found (General):
```json
{
    "message": "Không tìm thấy feedback với id này."
}
```

### 401 Unauthorized:
```json
{
    "message": "Token không hợp lệ hoặc đã hết hạn"
}
```

## Notification System

When a feedback is reported:
1. **All Admin users** receive a notification
2. Notification includes:
   - Rating ID
   - Reason for report
   - Reporter's information (username, role)
   - Tour Operator ID (if applicable)

## Testing

Use the provided `Feedback_Report_API_Test.http` file to test the updated functionality:

1. **Test as Customer:** Report feedback with customer token
2. **Test as Tour Guide:** Report feedback with tour guide token
3. **Test as Tour Operator:** Report feedback with tour operator token
4. **Test as Admin:** Report feedback with admin token
5. **Test Error Cases:** Non-existent feedback, unauthenticated requests

## Security Considerations

1. **Authentication Required:** All requests must include valid JWT token
2. **User Validation:** Only active users can report feedback
3. **Feedback Validation:** Only existing feedback can be reported
4. **Role-Based Logic:** Different validation for Tour Operators
5. **Admin Notification:** All reports are sent to admin users for review

## Benefits

1. **Community Moderation:** Allows users to flag inappropriate content
2. **Better Content Quality:** More eyes on feedback content
3. **Flexible Reporting:** Different user types can report for different reasons
4. **Maintained Security:** Still requires authentication and validation
5. **Admin Oversight:** All reports are reviewed by administrators 