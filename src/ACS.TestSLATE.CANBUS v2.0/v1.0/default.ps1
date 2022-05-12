# Build Script Template

# Most of what you will need to change is in these properties
properties {
    $root = Split-Path $psake.build_script_file
    $bin = Join-Path $root "bin"
    $src = Join-Path $root "src"
    $help = Join-Path $root "help"
	$cs_help = "$bin\help\Customer Specific Help"
    $pluginName = "Customer.TestSLATE.Mnemonic"
    $baseversion = "1.0.0"
    $mode = "Debug"
}

Framework '4.0x86'

# By default, run the UnitTests task.
task default -depends UnitTests		

task UnitTests -depends Compile, CopyToCore, BuildLLB, CopyToLabVIEW {
}

task CopyToCore -depends Compile {
	Copy-ToCoreBin "$bin"
}

task CopyToCoreLLB -depends BuildLLB {
	Copy-ToCoreBin "$bin"
}   

task CopyToLabVIEW -depends Compile {
	Copy-ToLabVIEW "$bin"
}

task Clean {
	Clear-BinFolder "$bin"
}

task ? -Description "Helper to display task info" {
    Write-Documentation
}

task UpdateAsmInfo {
        Update-AssemblyInfo (Update-Version $baseversion) "$src"
}

task CopyHelp {
    if (Test-Path "$cs_help") {
		remove-item "$cs_help\*" -recurse -force
	} else {
		New-Item -ItemType Directory "$cs_help"
	}
	copy-item -force -recurse -exclude "*.docx", "*.txt", "*.doc" "$help\*" "$cs_help"
}

# A default .NET project will build to these locations correctly
# No changes needed to this task unless you plan to do something different
task Compile -depends Clean, UpdateAsmInfo, CopyHelp { 
    Invoke-MSBuild "$src\$pluginName.sln" $mode
    copy-item -force -recurse -exclude "*.tmp", "*.xml" "$src\bin\$mode\*" "$bin"	
}

# Make sure your lvproj builds to these locations
# If you plan to do something different, you will need to change this task
task BuildLLB -depends Compile {
	Invoke-LabVIEWBuild "$src\$pluginName.lvproj"
	copy-item -force -recurse -exclude "data" "builds\$pluginName\*" "$bin"
}