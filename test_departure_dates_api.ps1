# Test script for DepartureDates API
Write-Host "Testing DepartureDates API..." -ForegroundColor Green

try {
    $response = Invoke-WebRequest -Uri "https://localhost:7012/api/DepartureDates" -Method GET -UseBasicParsing
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Response Content:" -ForegroundColor Yellow
    Write-Host $response.Content
} catch {
    Write-Host "Error occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host "`nTesting with HTTP instead of HTTPS..." -ForegroundColor Green

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5298/api/DepartureDates" -Method GET -UseBasicParsing
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Response Content:" -ForegroundColor Yellow
    Write-Host $response.Content
} catch {
    Write-Host "Error occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
} 