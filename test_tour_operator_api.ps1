# Test TourOperator API
$baseUrl = "https://localhost:7012"

Write-Host "=== Testing TourOperator API ===" -ForegroundColor Green

# 1. Register TourOperator
Write-Host "1. Registering TourOperator..." -ForegroundColor Yellow
$registerBody = @{
    email = "touroperator@example.com"
    password = "password123"
    userName = "TestTourOperator"
    roleName = "Tour Operator"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"
    Write-Host "Register successful: $($registerResponse)" -ForegroundColor Green
} catch {
    Write-Host "Register failed (might already exist): $($_.Exception.Message)" -ForegroundColor Yellow
}

# 2. Login as TourOperator
Write-Host "2. Logging in as TourOperator..." -ForegroundColor Yellow
$loginBody = @{
    email = "touroperator@example.com"
    password = "password123"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "Login successful. Token: $($token.Substring(0, 50))..." -ForegroundColor Green
} catch {
    Write-Host "Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# 3. Test GetNotesByTourOperator
Write-Host "3. Testing GetNotesByTourOperator..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

try {
    $notesResponse = Invoke-RestMethod -Uri "$baseUrl/api/GuideNote/tour-operator/notes" -Method GET -Headers $headers
    Write-Host "GetNotesByTourOperator successful. Found $($notesResponse.Count) notes" -ForegroundColor Green
    
    if ($notesResponse.Count -gt 0) {
        Write-Host "First note ID: $($notesResponse[0].noteId)" -ForegroundColor Cyan
        
        # 4. Test UpdateNoteExtraCost
        Write-Host "4. Testing UpdateNoteExtraCost..." -ForegroundColor Yellow
        $noteId = $notesResponse[0].noteId
        $updateBody = @{
            extraCost = 75.50
        } | ConvertTo-Json
        
        try {
            $updateResponse = Invoke-RestMethod -Uri "$baseUrl/api/GuideNote/notes/$noteId/extra-cost" -Method PUT -Body $updateBody -Headers $headers
            Write-Host "UpdateNoteExtraCost successful: $($updateResponse.message)" -ForegroundColor Green
        } catch {
            Write-Host "UpdateNoteExtraCost failed: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "No notes found for TourOperator" -ForegroundColor Yellow
    }
} catch {
    Write-Host "GetNotesByTourOperator failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "=== Test completed ===" -ForegroundColor Green 