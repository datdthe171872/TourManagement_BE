# Forgot Password API Documentation

## Overview
This document describes the Forgot Password API endpoints for the Tour Management System. The API provides secure password reset functionality with email notifications.

## Base URL
```
https://localhost:7001/api/Auth
```

## Authentication
These endpoints do NOT require authentication as they are used for password recovery.

## API Endpoints

### 1. Forgot Password
**POST** `/api/Auth/forgot-password`

Request a password reset for a user account.

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Response:**
```json
{
  "message": "Password reset email sent"
}
```

**Error Responses:**
- `400 Bad Request`: Email not found, User account is not active, or validation errors
- `500 Internal Server Error`: Email service configuration issues

### 2. Reset Password
**POST** `/api/Auth/reset-password`

Reset password using a valid reset token.

**Request Body:**
```json
{
  "token": "reset-token-from-email",
  "newPassword": "newPassword123"
}
```

**Response:**
```json
{
  "message": "Password has been reset successfully"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid/expired token, User not found, User account is not active, or validation errors
- `500 Internal Server Error`: Email service configuration issues

## Email Templates

### Password Reset Request Email
Users receive a professionally formatted HTML email containing:
- Personalized greeting with username
- Clear explanation of the password reset request
- Styled "Reset Password" button
- Direct link to reset password
- Expiration warning (1 hour)
- Security notice

### Password Reset Confirmation Email
After successful password reset, users receive:
- Confirmation of successful password change
- Security warning if they didn't perform the action
- Contact information for support

## Security Features

### Token Management
- **Automatic Cleanup**: Old unused tokens are automatically removed when requesting new reset
- **Single Use**: Each token can only be used once
- **Time Limit**: Tokens expire after 1 hour
- **User Validation**: Only active user accounts can request password reset

### Password Validation
- **Minimum Length**: New password must be at least 6 characters
- **Uniqueness**: New password must be different from current password
- **Secure Hashing**: Passwords are hashed using secure algorithms

### Error Handling
- **Graceful Failures**: If email sending fails, tokens are automatically cleaned up
- **Detailed Messages**: Clear error messages for different failure scenarios
- **Configuration Validation**: Email service configuration is validated before use

## Configuration Requirements

### Email Service Configuration (appsettings.json)
```json
{
  "EmailService": {
    "FromAddress": "your-email@gmail.com",
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

### Database Requirements
- `Users` table with `IsActive` field
- `ResetPasswordTokens` table for token management

## Testing Scenarios

### Success Cases
1. **Valid Email**: Request reset for existing active user
2. **Valid Token**: Reset password with valid token
3. **Email Delivery**: Check email inbox for reset link

### Error Cases
1. **Invalid Email**: Non-existent email address
2. **Inactive User**: Email exists but user is inactive
3. **Invalid Token**: Expired or non-existent token
4. **Short Password**: Password less than 6 characters
5. **Same Password**: New password same as current
6. **Missing Fields**: Required fields not provided
7. **Email Configuration**: Incorrect email service settings

## Implementation Details

### ForgotPasswordAsync Method
```csharp
public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
{
    // 1. Validate user exists and is active
    // 2. Clean up old unused tokens
    // 3. Generate new reset token
    // 4. Send email with reset link
    // 5. Handle email sending errors
}
```

### ResetPasswordAsync Method
```csharp
public async Task ResetPasswordAsync(ResetPasswordRequest request)
{
    // 1. Validate token exists and is not expired
    // 2. Validate user exists and is active
    // 3. Check new password is different from current
    // 4. Update password and mark token as used
    // 5. Send confirmation email
}
```

## Best Practices

1. **Rate Limiting**: Consider implementing rate limiting for forgot password requests
2. **Logging**: Log password reset attempts for security monitoring
3. **Monitoring**: Monitor email delivery success rates
4. **User Experience**: Provide clear feedback for all scenarios
5. **Security**: Never expose sensitive information in error messages

## Troubleshooting

### Common Issues
1. **Email Not Received**: Check spam folder and email configuration
2. **Token Expired**: Request new password reset
3. **Invalid Token**: Ensure token is copied correctly from email
4. **Configuration Errors**: Verify email service settings in appsettings.json

### Support
For technical issues, check:
- Email service configuration
- Database connectivity
- Network connectivity for SMTP
- User account status in database 