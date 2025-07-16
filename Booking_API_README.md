# Booking API Documentation

## Overview
This document describes the Booking API endpoints for the Tour Management System. The API provides role-based access to booking information, with a unified search bar for all roles.

## Authentication
All endpoints require JWT authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Role-Based Endpoints with Unified Search

### 1. Get Customer Bookings
**Endpoint:** `GET /api/booking/customer`
**Role Required:** Customer
**Description:** Retrieves all bookings for the authenticated customer, with optional search by keyword.

**Query Parameter (optional):**
- `keyword`: Search by customer name, tour title, or company name (case-insensitive, partial match)

**Example:**
```
GET /api/booking/customer?keyword=Hạ%20Long
```

### 2. Get Tour Operator Bookings
**Endpoint:** `GET /api/booking/tour-operator`
**Role Required:** Tour Operator
**Description:** Retrieves all bookings for tours created by the authenticated tour operator, with optional search by keyword.

**Query Parameter (optional):**
- `keyword`: Search by customer name, tour title, or company name

**Example:**
```
GET /api/booking/tour-operator?keyword=Nguyen
```

### 3. Get All Bookings (Admin)
**Endpoint:** `GET /api/booking/admin`
**Role Required:** Admin
**Description:** Retrieves all bookings in the system (admin access only), with optional search by keyword.

**Query Parameter (optional):**
- `keyword`: Search by customer name, tour title, or company name

**Example:**
```
GET /api/booking/admin?keyword=Travel%20Viet
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

| Role | Endpoint | Access |
|------|----------|--------|
| Customer | `/api/booking/customer` | Can view their own bookings |
| Tour Operator | `/api/booking/tour-operator` | Can view bookings for their tours |
| Admin | `/api/booking/admin` | Can view all bookings |
| Tour Guide | None | No booking access |

## Notes

1. **Unified Search:** All roles use the same `keyword` parameter to search bookings by customer name, tour title, or company name.
2. **Customer Role**: Can only see bookings where `UserId` matches their authenticated user ID
3. **Tour Operator Role**: Can only see bookings for tours where `TourOperatorId` matches their tour operator ID
4. **Admin Role**: Can see all active bookings in the system
5. All endpoints return only active bookings (`IsActive = true`)
6. The system automatically filters bookings based on the authenticated user's role and ID
7. JWT token must contain the user's role and ID for proper authorization
8. The search is case-insensitive and matches any part of the name/title/company.

## Testing

You can test these endpoints using the provided HTTP test files or tools like Postman. Make sure to:
1. Login first to get a valid JWT token
2. Include the token in the Authorization header
3. Use the correct endpoint and the `keyword` parameter as needed 