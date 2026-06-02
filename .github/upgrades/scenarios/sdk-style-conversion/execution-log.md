
## [2026-06-02 16:34] 01-convert-main-project

**Task 01 Complete**: Successfully converted MAC_use_cases project to SDK-style format.

**What Changed:**
- Converted `MAC_use_cases.csproj` to SDK-style (`<Project Sdk="Microsoft.NET.Sdk">`)
- Preserved WPF/XAML support, custom MSBuild targets, and AssemblyInfo.cs
- Added x64 platform configuration to match solution settings
- Disabled deterministic builds to preserve wildcard versioning in DEBUG mode
- Resolved duplicate PackageReferences and undefined MSBuild properties

**Build Status:** ✅ Main project builds successfully
- Output: `bin\x64\Debug\net48\MAC_use_cases.dll`
- Zero errors, no new warnings
- Target framework unchanged (net48)

**Next:** Task 02 will convert the test project.


## [2026-06-02 16:36] 02-convert-test-project

**Task 02 Complete**: Successfully converted MAC_use_cases.Tests project to SDK-style format.

**What Changed:**
- Converted `MAC_use_cases.Tests.csproj` to SDK-style format
- Preserved all custom MSBuild targets (StrongNamer integration, App.config updates)
- Preserved RoslynCodeTaskFactory inline task for App.config manipulation
- Maintained all linked files from main project (XAML, ZIP resources, source files)
- Updated EqmBinDllPath to match new SDK-style output paths (`bin\x64\$(Configuration)\net48\`)

**Build Status:** ✅ Both projects build successfully
- Zero errors across entire solution
- No new warnings
- Target frameworks unchanged (net48)

**Next:** Task 03 will perform final validation and run tests.

