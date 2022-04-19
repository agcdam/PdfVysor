function Write-Something {
    param (
        [string[]]$Text
    )
    foreach ($item in $Text) {
        Write-Host $Text
    }
    
}