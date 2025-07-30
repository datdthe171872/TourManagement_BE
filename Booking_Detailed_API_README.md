# Booking Detailed API Documentation

## Overview
This document describes the updated Booking API endpoints for the Tour Management System. The API now provides detailed, structured responses with comprehensive booking information organized into logical sections.

## Authentication
All endpoints require JWT authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Role-Based Endpoints with Detailed Responses

### 1. Get Customer Bookings (Detailed)
**Endpoint:** `GET /api/booking/customer`
**Role Required:** Customer
**Description:** Retrieves all bookings for the authenticated customer with detailed response structure.

**Query Parameters (optional):**
- `TourName`: Search by tour title (case-insensitive, partial match)

**Example:**
```
GET /api/booking/customer?TourName=Hạ%20Long
```

### 2. Get Tour Operator Bookings (Detailed)
**Endpoint:** `GET /api/booking/tour-operator`
**Role Required:** Tour Operator
**Description:** Retrieves all bookings for tours created by the authenticated tour operator with detailed response structure.

**Query Parameters (optional):**
- `TourName`: Search by tour title
- `UserName`: Search by customer user name

**Example:**
```
GET /api/booking/tour-operator?TourName=Hạ%20Long&UserName=Nguyen
```

### 3. Get All Bookings (Admin, Detailed)
**Endpoint:** `GET /api/booking/admin`
**Role Required:** Admin
**Description:** Retrieves all bookings in the system with detailed response structure.

**Query Parameters (optional):**
- `TourName`: Search by tour title
- `UserName`: Search by customer user name

**Example:**
```
GET /api/booking/admin?TourName=Travel%20Viet&UserName=John
```

### 4. Get All Bookings (General, Detailed)
**Endpoint:** `GET /api/booking`
**Role Required:** Any authenticated user
**Description:** Retrieves all bookings with detailed response structure.

**Query Parameters (optional):**
- `TourName`: Search by tour title
- `UserName`: Search by customer user name

**Example:**
```
GET /api/booking?TourName=Hạ%20Long
```

## Detailed Response Format

All booking endpoints now return a structured response with the following format:

```json
{
  "bookings": [
    {
      "bookingId": 1,
      "tour": {
        "title": "Hạ Long Bay Adventure",
        "maxSlots": 50,
        "transportation": "Bus",
        "startPoint": "Hanoi"
      },
      "booking": {
        "bookingDate": "2024-01-15T10:30:00",
        "contract": "contract_url_here",
        "noteForTour": "Special dietary requirements"
      },
      "guest": {
        "numberOfAdults": 2,
        "numberOfChildren": 1,
        "numberOfInfants": 0,
        "totalGuests": 3
      },
      "billingInfo": {
        "username": "John Doe",
        "email": "john@example.com",
        "phone": "0123456789",
        "address": "123 Main St, Hanoi"
      },
      "paymentInfo": {
        "totalPrice": 1500.00,
        "paymentStatus": "Paid",
        "bookingStatus": "Confirmed"
      }
    }
  ]
}
```

## Response Structure Details

### Tour Information
- **title**: The name of the tour
- **maxSlots**: Maximum number of slots available for the tour
- **transportation**: Type of transportation used (optional)
- **startPoint**: Starting location of the tour (optional)

### Booking Information
- **bookingDate**: When the booking was made
- **contract**: URL or reference to the booking contract (optional)
- **noteForTour**: Special notes or requirements for the tour (optional)

### Guest Information
- **numberOfAdults**: Number of adult guests
- **numberOfChildren**: Number of child guests
- **numberOfInfants**: Number of infant guests
- **totalGuests**: Calculated total of all guests

### Billing Information
- **username**: Customer's display name
- **email**: Customer's email address
- **phone**: Customer's phone number (optional)
- **address**: Customer's address (optional)

### Payment Information
- **totalPrice**: Total cost of the booking
- **paymentStatus**: Current payment status (e.g., "Paid", "Pending", "Failed")
- **bookingStatus**: Current booking status (e.g., "Confirmed", "Pending", "Cancelled")

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
  "message": "Not Found"
}
```

## Migration Notes

This update replaces the previous flat response structure with a more organized, hierarchical structure that:

1. **Improves readability**: Information is logically grouped
2. **Enhances maintainability**: Clear separation of concerns
3. **Provides better context**: Related information is grouped together
4. **Supports future extensions**: Easy to add new fields to appropriate sections

## Backward Compatibility

The old endpoints are still available but now return the new detailed structure. If you need the old flat structure, you can modify the service layer to use the original methods.

## Testing

Use the provided `Booking_Detailed_API_Test.http` file to test all endpoints with the new response format. 