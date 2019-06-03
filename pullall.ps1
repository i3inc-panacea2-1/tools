param([String]$platform="x86")

$baseDir = (Get-Item -Path "..\" -Verbose).FullName
$items = Get-ChildItem -Path $baseDir -Include .git -Recurse -force
foreach ($item in $items){
	#Write-Output "!---> git --work-tree=$($item.parent.FullName) --git-dir=$($item.FullName) pull origin "
	Write-Host "$($item.parent.Name)" -ForegroundColor Green
	& "git" "--work-tree=$($item.parent.FullName)" "--git-dir=$($item.FullName)" "pull" "origin" 
	
	write-host $Result
	if ($LASTEXITCODE -ne 0)
	{
		pause
	}
}
