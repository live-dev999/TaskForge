# DeployRelease.ps1
# Autor: Denis Prokhorchik
# Date: 04.07.2018
# Version: 1.0

#import-module 

# param(
#     [string]$user,
#     [string]$password,
#     [string]$InstanceName,
#     [string]$DbName
# )
# if($user -eq $null -or $user -eq "" -or $password -eq $null -or $password -eq "")
# {
#     Write-Host "Write username and password" -ForegroundColor Red
#     break
# }

# import-module "sqlps" -DisableNameChecking
# [System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.ConnectionInfo') | Out-Null
# [System.Reflection.Assembly]::LoadWithPartialName('Microsoft.SqlServer.Smo') | Out-Null
# $Server = [Microsoft.SqlServer.Management.Smo.Server]::new('RemoteDefaultInstance')
# $Server.ConnectionContext.ConnectAsUser = $true

# #variables
# # $steps = ([System.Management.Automation.PsParser]::Tokenize((Get-Contentt-Content "$PSScriptRoot\$($MyInvocation.MyCommand.Name)"), [ref]$null) | Where-Object { $_.Type -eq 'Command' -and $_.Content -eq 'Write-ProgressHelper' }).Count
# # $stepCounter = 0
# if($InstanceName -eq $null -or $InstanceName -eq "")
# {
#     $InstanceName =  $env:COMPUTERNAME
#     if (!$env:COMPUTERNAME.Equals("DESKTOP-52DO9SB")) {
#         $InstanceName =  $env:COMPUTERNAME+"\"+(get-itemproperty 'HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server').InstalledInstances;
#     }
# }
# if($DbName -eq $null -or $DbName -eq "")
# {
#     $DbName = "SwimmingCompetitions"
# }

# $directoryPath = split-path -parent $MyInvocation.MyCommand.Definition
# $directoryOutput = $directoryPath + "\Output\"
# $directory_incorrect_queries_verbose=$directoryOutput + "\Incorrect srcipts verbose\"
# $directory_correct_queries_verbose=$directoryOutput + "\Correct srcipts verbose\"
# $directory_incorrect_queries=$directory_incorrect_queries_verbose + "\Incorrect scripts\"

# $directory_Schemas = $directoryPath + "\schemas\"
# $directory_dbo_Tables = $directoryPath + "\dbo\tables\"
# $directory_dbo_Constraints = $directoryPath + "\dbo\constraints\"
# $directory_dbo_Usp = $directoryPath + "\dbo\stored_procedures\"
# $directory_dbo_Utf = $directoryPath + "\dbo\functions\"

# $directory_dbo_Scripts = $directoryPath + "\scripts\"
# $directory_dbo_Tests = $directoryPath + "\tests\"
# $directory_dbo_Tests_Tables = $directory_dbo_Tests + "\tables\"
# $directory_dbo_Tests_Common = $directory_dbo_Tests + "\common\"
# $directory_dbo_Tests_DDL_UC = $directory_dbo_Tests + "\DDL Constraints Tests\"
# $directory_dbo_Tests_DDL_UC_Depends_Functions=$directory_dbo_Tests_DDL_UC+"Depends_Functions"
# $directory_dbo_Tests_DDL_Table = $directory_dbo_Tests + "\DDL Table Tests\"
# $directory_dbo_Tests_DML = $directory_dbo_Tests + "\DML Tests\"

# $Server.ConnectionContext.ConnectAsUserName =$user
# $Server.ConnectionContext.ConnectAsUserPassword = $password
# $Server.ConnectionContext.ConnectTimeout = 30
# $Server.ConnectionContext.ServerInstance = $InstanceName



# function ShowValueVariables ([string]$message, [string]$VariableValue) {
#     # /Write-Output  $message
#     Write-Output "$($message) = $($VariableValue)"
    
# }
# function SqlQuery([string]$pathSqlFiles) {
#     # $files = Get-ChildItem -path  $pathSqlFiles -Filter *.sql | sort-object -desc
#     # $count = $files.Count
#     # $i = 0
    
#     foreach ($f in  Get-ChildItem -path  $pathSqlFiles -Filter *.sql | sort-object -desc ) { 
#        $Global:scriptsCount++
#         $out = $directory_correct_queries_verbose + $f.name.Substring(0, $f.name.LastIndexOf('.')) + ".txt" ; 
#         try{        
#        $verbose=invoke-sqlcmd  -ServerInstance $InstanceName -Database $DbName  -InputFile $f.fullname -ErrorAction Stop -Verbose *>&1 |Out-String 
#       # Write-Host $test -ForegroundColor Red   
#        Write-Host $f.Name -ForegroundColor Green
#        if(-not($verbose -eq $null -or $verbose -eq ""))
#        {
#             Write-Host $verbose -ForegroundColor Yellow
#             Write-Output $verbose| out-file -filePath $out
#        }          
       
#         }
#         catch
#         {
#         $out=$directory_incorrect_queries_verbose + $f.name.Substring(0, $f.name.LastIndexOf('.')) + ".txt" ;
#             Write-Host $f.FullName -ForegroundColor Red
#             Write-Host $_.Exception -ForegroundColor Red
#             Write-Output $_.Exception.ToString() |out-file -filePath $out
#             Copy-Item $f.FullName -Destination $directory_incorrect_queries
#             Write-Output $f.FullName|out-file -filePath $directory_incorrect_queries"PathWays to original scripts.txt" -Append

#         }
#         # $stepCounter = ($i / $count) * 100;
#         # Write-ProgressHelper -Message 'execute Files' -StepNumber ($i)
#         # $i++
#     }
# }
# function CreateDB() {   
 
#     try {        
#         $Server.ConnectionContext.Connect()
#         if ($Server.Databases[$DbName] -ne $null)  {$Server.Databases[$DbName].Drop()}
#         $DB = [Microsoft.SqlServer.Management.Smo.Database]::new($InstanceName, $DbName)       
#         $DB.Create()
#     }
#     catch {
#         write-host $_.Exception.Message -ForegroundColor Red
#         break
#     }
#     finally {
#         # $Server.ConnectionContext.Close()
#     }
#     # Write-Progress -Activity 'Deploy' -Status 'created Db' -PercentComplete 100
# }

# # function Write-ProgressHelper {
# #     param (
# #         [int]$StepNumber,
 
# #         [string]$Message
# #     )
# #     Write-Progress -Activity 'Deploy' -Status $Message -PercentComplete (($StepNumber / $steps) * 100)
# # }





# Write-Output ""

# Clear-Host

# ShowValueVariables "Instance" $InstanceName
# ShowValueVariables "Database" $DbName
# ShowValueVariables "Directory Path" $directoryPath
# ShowValueVariables "Directory Output" $directoryOutput

# ShowValueVariables "Directory Shemas" $directory_Schemas
# ShowValueVariables "Directory dbo_Tables" $directory_dbo_Tables
# ShowValueVariables "Directory dbo_Usp" $directory_dbo_Usp
# ShowValueVariables "Directory dbo_Utf" $directory_dbo_Utf
# ShowValueVariables "Directory dbo_Constraints" $directory_dbo_Constraints
# ShowValueVariables "Directory dbo_Scripts" $directory_dbo_Scripts
# ShowValueVariables "Directory dbo_Tests - Tables" $directory_dbo_Tests_Tables
# ShowValueVariables "Directory dbo_Tests - Common" $directory_dbo_Tests_Common
# ShowValueVariables "Directory dbo_Tests - DDL Constraints Tests" $directory_dbo_Tests_DDL_UC
# ShowValueVariables "Directory dbo_Tests - DDL Table Tests" $directory_dbo_Tests_DDL_Table
# ShowValueVariables "Directory dbo_Tests - DML Tests" $directory_dbo_Tests_DML

# Write-Output ""

# #create Output directory
# New-Item -Path $directoryOutput -ItemType directory -Force;
# Get-ChildItem -Path $directoryOutput -Include * -File -Recurse | ForEach-Object { $_.Delete()}

# New-Item -Path $directory_incorrect_queries -ItemType directory -Force;

# New-Item -Path $directory_correct_queries_verbose -ItemType directory -Force;

# $scriptsCount=0

# CreateDB
# SqlQuery $directory_Schemas
# SqlQuery $directory_dbo_Tables
# SqlQuery $directory_dbo_Usp
# SqlQuery $directory_dbo_Utf
# SqlQuery $directory_dbo_Constraints
# SqlQuery $directory_dbo_Scripts
# SqlQuery $directory_dbo_Tests
# SqlQuery $directory_dbo_Tests_Tables
# SqlQuery $directory_dbo_Tests_Common
# SqlQuery $directory_dbo_Tests_DDL_UC_Depends_Functions
# SqlQuery $directory_dbo_Tests_DDL_UC
# SqlQuery $directory_dbo_Tests_DDL_Table
# SqlQuery $directory_dbo_Tests_DML

# #& 'SwimmingCompetitions.PrepareTools.exe'
# # Write-ProgressHelper -Message 'Doing something' -StepNumber ($stepCounter)
# write-Host ""
# Write-Host $scriptsCount" scripts runnig" -ForegroundColor Blue
# Write-Output $scriptsCount" scripts runnig"|out-file -filePath $directoryOutput"Information_Log.txt"