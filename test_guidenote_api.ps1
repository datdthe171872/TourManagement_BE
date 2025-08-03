# Test GuideNote API - Update Extra Cost
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEwIiwiZW1haWwiOiJmYW5jdWFkYXQxN0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUb3VyIE9wZXJhdG9yIiwianRpIjoiYjZlNjliYTctN2JiMS00YjYzLWI4YWYtZDE1NWE2MmI4NDZlIiwiZXhwIjoxNzU0MjE2MDY5LCJpc3MiOiJUb3VyTWFuYWdlbWVudCIsImF1ZCI6IlRvdXJNYW5hZ2VtZW50In0.NhHvuw_3JEpyujxVPruQrrrcDhZ4qu4OaxLR_GWZ7rk"

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$body = @{
    extraCost = 50.00
} | ConvertTo-Json

Write-Host "Testing GuideNote API - Update Extra Cost for note ID 6"
Write-Host "URL: https://localhost:5298/api/GuideNote/notes/6/extra-cost"
Write-Host "Method: PUT"
Write-Host "Body: $body"
Write-Host ""

try {
    $response = Invoke-WebRequest -Uri "https://localhost:5298/api/GuideNote/notes/6/extra-cost" -Method PUT -Headers $headers -Body $body
    Write-Host "Success! Status Code: $($response.StatusCode)"
    Write-Host "Response: $($response.Content)"
} catch {
    Write-Host "Error occurred:"
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)"
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody"
    } else {
        Write-Host "Exception: $($_.Exception.Message)"
    }
} 