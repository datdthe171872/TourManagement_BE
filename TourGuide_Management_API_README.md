# Tour Guide Management API

## Overview
This document describes the new features added to the Tour Guide management system, including the ability to hide/show tour guides and enhanced registration logic.

## New Features

### 1. Hide/Show Tour Guide API
**Endpoint:** `PUT /api/TourOperator/tourguide-status`

**Description:** Allows Tour Operators to hide or show tour guides by updating their `IsActive` status. When hiding a tour guide, both the TourGuide record and the associated User account are set to inactive.

**Authorization:** Requires "Tour Operator" role

**Request Body:**
```json
{
    "tourGuideId": 1,
    "isActive": false
}
```

**Response:**
```json
{
    "message": "Đã ẩn tour guide thành công"
}
```

**Important Notes:**
- When hiding a tour guide (`isActive = false`), both the `TourGuides.IsActive` and `Users.IsActive` are set to `false`
- When showing a tour guide (`isActive = true`), both the `TourGuides.IsActive` and `Users.IsActive` are set to `true`
- This ensures that when a tour guide is hidden, their user account is also deactivated, allowing email reuse for new registrations

### 2. Enhanced Tour Guide Registration
**Endpoint:** `POST /api/TourOperator/register-tourguide`

**Description:** Enhanced registration logic with special email validation for tour guides.

#### Email Validation Rules:
- **For Tour Guides:** 
  - If an account with the same email exists and `IsActive = true` → Registration fails
  - If an account with the same email exists and `IsActive = false` → Registration succeeds (removes old inactive account and creates new one)
  - **Note:** This feature allows you to reuse email addresses from deactivated tour guide accounts by completely replacing the old account
- **For Other Roles:** Standard email uniqueness check (registration fails if email exists regardless of status)

#### Additional Features:
1. **Email Notification:** Sends login credentials to the newly created tour guide
2. **Notification to Tour Operator:** Sends a notification to the tour operator about successful registration

**Request Body:**
```json
{
    "userName": "guide1",
    "email": "guide1@example.com",
    "password": "123456",
    "address": "123 Guide Street",
    "phoneNumber": "0123456789",
    "avatar": "https://example.com/avatar1.jpg"
}
```

**Email Content Sent to Tour Guide:**
- Subject: "Tour Guide Account Created - Tour Management System"
- Includes login credentials (email and password)
- Security recommendation to change password after first login

**Notification Sent to Tour Operator:**
- Title: "Tour Guide Registration Success"
- Message: "Tour guide {username} has been registered successfully."
- Type: "TourGuideRegistration"

## API Endpoints Summary

### Tour Guide Management
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/TourOperator/tourguides` | Get list of tour guides with filtering | Tour Operator |
| POST | `/api/TourOperator/register-tourguide` | Register new tour guide | Tour Operator |
| PUT | `/api/TourOperator/tourguide-status` | Hide/show tour guide | Tour Operator |

### Query Parameters for Get Tour Guides
- `pageNumber` (int): Page number for pagination
- `pageSize` (int): Number of items per page
- `username` (string): Search by username (optional)
- `isActive` (bool): Filter by active status (optional)
  - **To show only active tour guides:** Set `isActive=true` or omit the parameter
  - **To show only inactive tour guides:** Set `isActive=false`
  - **To show all tour guides:** Set `isActive=null` and `showInactive=true`
- `showInactive` (bool): Whether to include inactive tour guides when `isActive` is not specified (default: false)
  - **Default behavior:** Only shows active tour guides (IsActive = true)
  - **To show all tour guides:** Set `showInactive=true` (this will show both active and inactive)

## Error Handling

### Common Error Responses

**404 Not Found:**
```json
{
    "message": "Không tìm thấy tour guide với id này hoặc không có quyền cập nhật."
}
```

**400 Bad Request:**
```json
{
    "message": "Email already exists and is active"
}
```

**500 Internal Server Error:**
```json
{
    "message": "Có lỗi xảy ra khi cập nhật trạng thái tour guide",
    "error": "Error details"
}
```

## Testing

Use the provided `TourGuide_Management_API_Test.http` file to test all the new endpoints. Make sure to:

1. Set the `baseUrl` variable to your API base URL
2. Set the `tourOperatorToken` variable to a valid JWT token for a Tour Operator account
3. Update the `tourGuideId` in the test requests to match actual tour guide IDs in your database

### Testing Email Reuse Feature

To test the email reuse functionality:

1. **Register a tour guide** with a specific email
2. **Hide the tour guide** using the `PUT /api/TourOperator/tourguide-status` endpoint (this will deactivate both TourGuide and User)
3. **Use debug endpoint** to verify that both TourGuide and User are inactive
4. **Register another tour guide** with the same email - this should succeed
5. **Verify** that the old account is completely replaced by the new one

This demonstrates how the system allows reusing email addresses by completely replacing deactivated tour guide accounts.

**Debug Endpoint:** `GET /api/TourOperator/debug/check-email?email={email}` - Use this to check the status of all users with a specific email address.

**Important:** When reusing an email from an inactive account, the old account is completely removed and replaced with the new one.

## Dependencies

The new features require the following services to be properly configured:

1. **EmailHelper:** For sending email notifications to tour guides
2. **NotificationService:** For creating notifications
3. **AuthService:** For user registration logic
4. **TourOperatorService:** For tour guide status management

Make sure these services are registered in the dependency injection container in `Program.cs`.

## Database Changes

No new database tables or columns are required. The implementation uses existing tables:
- `Users` table for user accounts
- `TourGuides` table for tour guide information
- `Notifications` table for notifications
- `TourOperators` table for tour operator information

## Security Considerations

1. **Password Security:** Passwords are hashed using `PasswordHelper.HashPassword()` before storage
2. **Authorization:** All endpoints require proper JWT authentication and "Tour Operator" role
3. **Email Validation:** Special logic prevents duplicate active accounts while allowing reuse of inactive account emails
4. **Error Handling:** Sensitive information is not exposed in error messages 