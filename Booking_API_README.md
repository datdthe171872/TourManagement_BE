# Booking API Documentation

## Overview
This document describes the Booking API endpoints for the Tour Management System. The API provides role-based access to booking information, with specific search parameters for each role.

## Authentication
All endpoints require JWT authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Role-Based Endpoints with Specific Search Parameters

### 1. Get Customer Bookings
**Endpoint:** `GET /api/booking/customer`
**Role Required:** Customer
**Description:** Retrieves all bookings for the authenticated customer, with optional search by tour name.

**Query Parameters (optional):**
- `TourName`: Search by tour title (case-insensitive, partial match)

**Example:**
```
GET /api/booking/customer?TourName=Hạ%20Long
```

### 2. Get Tour Operator Bookings
**Endpoint:** `GET /api/booking/tour-operator`
**Role Required:** Tour Operator
**Description:** Retrieves all bookings for tours created by the authenticated tour operator, with optional search by tour name and user name.

**Query Parameters (optional):**
- `TourName`: Search by tour title
- `UserName`: Search by customer user name

**Example:**
```
GET /api/booking/tour-operator?TourName=Hạ%20Long&UserName=Nguyen
```

### 3. Get All Bookings (Admin)
**Endpoint:** `GET /api/booking/admin`
**Role Required:** Admin
**Description:** Retrieves all bookings in the system (admin access only), with optional search by tour name and user name.

**Query Parameters (optional):**
- `TourName`: Search by tour title
- `UserName`: Search by customer user name

**Example:**
```
GET /api/booking/admin?TourName=Travel%20Viet&UserName=John
```

## Response Format

All booking endpoints return a list of bookings with the following structure:

```json
{
  "bookings": [
    {
      "bookingId": 1,
      "userId": 123,
      "tourId": 456,
      "departureDateId": 789,
      "bookingDate": "2024-01-15T10:30:00",
      "numberOfAdults": 2,
      "numberOfChildren": 1,
      "numberOfInfants": 0,
      "noteForTour": "Special dietary requirements",
      "totalPrice": 1500.00,
      "contract": "contract_url_here",
      "bookingStatus": "Confirmed",
      "paymentStatus": "Paid",
      "isActive": true,
      "userName": "John Doe",
      "tourTitle": "Hạ Long Bay Adventure",
      "companyName": "Vietnam Travel Co.",
      "tourOperatorId": 101
    }
  ]
}
```

## Error Responses

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
```json
{
  "message": "Forbidden"
}
```

### 404 Not Found
```json
{
  "message": "Tour Operator not found"
}
```

## Role-Based Access Control

| Role | Endpoint | Access | Search Parameters |
|------|----------|--------|-------------------|
| Customer | `/api/booking/customer` | Can view their own bookings | TourName only |
| Tour Operator | `/api/booking/tour-operator` | Can view bookings for their tours | TourName, UserName |
| Admin | `/api/booking/admin` | Can view all bookings | TourName, UserName |
| Tour Guide | None | No booking access | N/A |

## Notes

1. **Specific Search Parameters:** Each role has specific search parameters:
   - Customer: Can search by Tour Name only
   - Tour Operator: Can search by Tour Name and/or User Name
   - Admin: Can search by Tour Name and/or User Name
2. **Customer Role**: Can only see bookings where `UserId` matches their authenticated user ID
3. **Tour Operator Role**: Can only see bookings for tours where `TourOperatorId` matches their tour operator ID
4. **Admin Role**: Can see all active bookings in the system
5. All endpoints return only active bookings (`IsActive = true`)
6. The system automatically filters bookings based on the authenticated user's role and ID
7. JWT token must contain the user's role and ID for proper authorization
8. The search is case-insensitive and matches any part of the name/title
9. All responses include `UserName` and `TourTitle` fields for better user experience

## Testing

You can test these endpoints using the provided HTTP test files or tools like Postman. Make sure to:
1. Login first to get a valid JWT token
2. Include the token in the Authorization header
3. Use the appropriate search parameters for each role 