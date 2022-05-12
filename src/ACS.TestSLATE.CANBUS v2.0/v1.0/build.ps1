import-module -force  "C:\PROGRAM FILES (X86)\JACOBS\TEST SLATE API\lib\psake\psake.psm1"
import-module -force  "C:\PROGRAM FILES (X86)\JACOBS\TEST SLATE API\\lib\TestSLATEDevEnv.psm1"

Invoke-Psake -buildfile "default.ps1" BuildLLB -properties @{mode='Release'}