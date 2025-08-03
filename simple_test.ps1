# Simple test for GuideNote API
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEwIiwiZW1haWwiOiJmYW5jdWFkYXQxN0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUb3VyIE9wZXJhdG9yIiwianRpIjoiYjZlNjliYTctN2JiMS00YjYzLWI4YWYtZDE1NWE2MmI4NDZlIiwiZXhwIjoxNzU0MjE2MDY5LCJpc3MiOiJUb3VyTWFuYWdlbWVudCIsImF1ZCI6IlRvdXJNYW5hZ2VtZW50In0.NhHvuw_3JEpyujxVPruQrrrcDhZ4qu4OaxLR_GWZ7rk"

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$body = '{"extraCost": 50.00}'

Write-Host "Testing GuideNote API..."
Write-Host "URL: https://localhost:5298/api/GuideNote/notes/6/extra-cost"

try {
    $response = Invoke-RestMethod -Uri "https://localhost:5298/api/GuideNote/notes/6/extra-cost" -Method PUT -Headers $headers -Body $body
    Write-Host "Success!"
    Write-Host "Response: $($response | ConvertTo-Json)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        Write-Host "Status: $($_.Exception.Response.StatusCode)"
    }
} 