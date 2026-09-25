$error.Clear()
try {
    Invoke-WebRequest -Uri "http://localhost:8080/Admin/ManageCourses.aspx" -UseBasicParsing
} catch {
    $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
    $html = $reader.ReadToEnd()
    Write-Host "ManageCourses Error:"
    $html | Select-String -Pattern "Exception Details" -Context 0,2
}

try {
    Invoke-WebRequest -Uri "http://localhost:8080/Admin/ManageUsers.aspx" -UseBasicParsing
} catch {
    $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
    $html = $reader.ReadToEnd()
    Write-Host "ManageUsers Error:"
    $html | Select-String -Pattern "Exception Details" -Context 0,2
}
