# Test API Script for Tour Guide Assignment
param(
    [string]$BaseUrl = "https://localhost:7001",
    [string]$Email = "operator@example.com",
    [string]$Password = "password123"
)

Write-Host "Testing Tour Guide Assignment API..." -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow
Write-Host "Email: $Email" -ForegroundColor Yellow

# Step 1: Login to get token
Write-Host "`n1. Logging in to get token..." -ForegroundColor Cyan

$loginBody = @{
    email = $Email
    password = $Password
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$BaseUrl/api/Auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    
    Write-Host "✅ Login successful!" -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Gray
    
    # Step 2: Test authentication
    Write-Host "`n2. Testing authentication..." -ForegroundColor Cyan
    
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }
    
    $authResponse = Invoke-RestMethod -Uri "$BaseUrl/api/TourGuideAssignment/test-auth" -Method GET -Headers $headers
    Write-Host "✅ Authentication test successful!" -ForegroundColor Green
    Write-Host "User ID: $($authResponse.userId)" -ForegroundColor Gray
    Write-Host "Email: $($authResponse.email)" -ForegroundColor Gray
    Write-Host "Role: $($authResponse.role)" -ForegroundColor Gray
    
    # Step 3: Get assignments
    Write-Host "`n3. Getting tour guide assignments..." -ForegroundColor Cyan
    
    $assignmentsResponse = Invoke-RestMethod -Uri "$BaseUrl/api/TourGuideAssignment?PageNumber=1&PageSize=5" -Method GET -Headers $headers
    Write-Host "✅ Assignments retrieved successfully!" -ForegroundColor Green
    Write-Host "Total Count: $($assignmentsResponse.totalCount)" -ForegroundColor Gray
    Write-Host "Page Number: $($assignmentsResponse.pageNumber)" -ForegroundColor Gray
    Write-Host "Page Size: $($assignmentsResponse.pageSize)" -ForegroundColor Gray
    
    if ($assignmentsResponse.assignments.Count -gt 0) {
        Write-Host "`nFirst Assignment:" -ForegroundColor Yellow
        $firstAssignment = $assignmentsResponse.assignments[0]
        Write-Host "  ID: $($firstAssignment.id)" -ForegroundColor Gray
        Write-Host "  Tour ID: $($firstAssignment.tourId)" -ForegroundColor Gray
        Write-Host "  Guide Name: $($firstAssignment.guideName)" -ForegroundColor Gray
        Write-Host "  Customer Name: $($firstAssignment.customerName)" -ForegroundColor Gray
    } else {
        Write-Host "No assignments found." -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green 