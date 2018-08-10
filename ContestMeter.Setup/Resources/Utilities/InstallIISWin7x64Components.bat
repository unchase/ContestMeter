%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-WebServerRole
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-WebServer
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-CommonHttpFeatures
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-StaticContent
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-DefaultDocument
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-HttpErrors
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-HttpRedirect
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-DirectoryBrowsing
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-NetFxExtensibility
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ISAPIFilter
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ISAPIExtensions
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ASP
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ASPNET
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ServerSideIncludes
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-HealthAndDiagnostics
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-HttpLogging
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-RequestMonitor
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-HttpTracing
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-Security
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-BasicAuthentication
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-WindowsAuthentication
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-URLAuthorization
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-RequestFiltering
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-IPSecurity
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-Performance
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-WebServerManagementTools
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ManagementConsole
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ManagementScriptingTools
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-ManagementService
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:IIS-IIS6ManagementCompatibility
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:WAS-WindowsActivationService
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:WAS-ProcessModel
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:WAS-NetFxEnvironment
%windir%\system32\dism.exe /Online /Enable-Feature /FeatureName:WAS-ConfigurationAPI
%windir%\system32\dism.exe /online /get-features /format:table>dismTemp.log
@cmd /u /c type dismTemp.log>dism.log
@del dismTemp.log
PAUSE