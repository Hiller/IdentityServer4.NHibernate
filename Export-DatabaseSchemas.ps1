Param(
    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string] $PublishPath,

    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string] $OutputPath
)

if (Test-Path -Path "$PublishPath\IdentityServer4.NHibernate.Storage.dll" -PathType Leaf) {
    Write-Host "Assembly $PublishPath\IdentityServer4.NHibernate.Storage.dll found. Adding types..."
    Add-Type -AssemblyName "$PublishPath\IdentityServer4.NHibernate.Storage.dll"
}

if (Test-Path -Path "$PublishPath\NHibernate.dll" -PathType Leaf) {
    Write-Host "Assembly $PublishPath\NHibernate.dll found. Adding types..."
    Add-Type -AssemblyName "$PublishPath\NHibernate.dll"
}

$configStoreOptions = New-Object IdentityServer4.NHibernate.Options.ConfigurationStoreOptions
$opStoreOptions = New-Object IdentityServer4.NHibernate.Options.OperationalStoreOptions

$bindingFlags= [Reflection.BindingFlags] "Public,Static"
$exportableConfigurations = [IdentityServer4.NHibernate.Database.Databases].GetMethods($bindingFlags) `
    | Where-Object { ($_.Name -inotlike '*equals*') -and ($_.Name -inotlike '*memory*')}
if ($exportableConfigurations -eq $null) {
    Write-Host "No database configurations found in assembly $PublishPath\IdentityServer4.NHibernate.Storage.dll"
} else {
    Write-Host Found ($exportableConfigurations | Measure-Object).Count configurations

    $exportableConfigurations | ForEach-Object {
        $fileName = $_.Name
        $currentConfiguration = $_.Invoke($null, $null)
        Write-Host Exporting script for $currentConfiguration...
        Write-Host "Creating script $fileName.sql in $OutputPath..."
        [IdentityServer4.NHibernate.Database.Schema.ScriptCreator]::CreateSchemaScriptForDatabase("$OutputPath\$fileName.sql", $currentConfiguration, $configStoreOptions, $opStoreOptions)
    }
}


