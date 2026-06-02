# 01-convert-main-project: Convert MAC_use_cases Project

**ID**: `01-convert-main-project`
**Description**: Convert the MAC_use_cases class library project to SDK-style format while preserving WPF/XAML support, NuGetizer configuration, TIA Portal integration, and custom build targets.

**Scope**:
- Convert project file to SDK-style format
- Ensure WPF/XAML content is properly handled
- Preserve NuGetizer package creation configuration
- Preserve custom MSBuild target: CleanProjPkgs
- Preserve TIA Portal registry-based path detection
- Maintain all PackageReferences
- Handle AssemblyInfo.cs attributes (remove auto-generated ones, keep custom)
- Preserve interop assembly references (Microsoft.Office.Interop.Excel with EmbedInteropTypes)
- Maintain embedded resources and content files
- Keep target framework at .NET Framework 4.8

**Expected Complications**:
- WPF/XAML content requires proper SDK configuration
- Custom MSBuild targets must be preserved
- NuGetizer integration must continue working
- Interop assembly configuration must be maintained
- AssemblyInfo.cs may have conflicting auto-generated attributes

**Risk Level**: Medium

**Validation**:
- Project builds successfully with no errors
- No new warnings introduced
- All XAML resources are included
- NuGet package is created (if applicable)
- Custom CleanProjPkgs target functions correctly

---
