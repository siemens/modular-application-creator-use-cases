# Assessment: SDK-style Conversion

## Projects to Convert

| Project | Path | packages.config | Custom Imports | Special Type | XAML/WPF | Risk |
|---------|------|----------------|----------------|-------------|----------|------|
| MAC_use_cases | MAC_use_cases\MAC_use_cases.csproj | No (PackageReference) | Custom Target (CleanProjPkgs) | Class library with WPF UI | Yes (XAML files) | Medium |
| MAC_use_cases.Tests | MAC_use_cases.Tests\MAC_use_cases.Tests.csproj | No (PackageReference) | Custom Targets (StrongNamer, UpdateAppConfig) | Test project | No | Medium |

## Already SDK-style
None

## Project Details

### MAC_use_cases
- **Target Framework**: .NET Framework 4.8
- **Project Type**: Class library with WPF UI components
- **NuGet**: Already using PackageReference (no packages.config)
- **File Includes**: ~25 explicit `<Compile>` items for source files
- **XAML Pages**: 6 XAML resource dictionaries and UI pages
- **Special Features**:
  - NuGetizer configuration for creating NuGet packages
  - Custom TIA Portal integration (Siemens.Engineering reference)
  - Custom MSBuild target: `CleanProjPkgs` (runs after Clean)
  - Registry-based TIA Portal path detection
  - Embedded resources (TIA library ZIP files)
  - Content files (images, Excel templates)
- **Assembly Attributes**: Standard attributes in AssemblyInfo.cs (Title, Company, Copyright, Version)
- **Complexity Factors**:
  - WPF/XAML content requiring proper SDK handling
  - Custom MSBuild targets that need preservation
  - Interop assembly (Microsoft.Office.Interop.Excel) with EmbedInteropTypes
  - Package creation configured via NuGetizer

### MAC_use_cases.Tests
- **Target Framework**: .NET Framework 4.8
- **Project Type**: Unit test library
- **NuGet**: Already using PackageReference
- **File Includes**: 4 explicit `<Compile>` items, plus linked files from main project
- **Linked Files**: Links to source files from MAC_use_cases project
- **Special Features**:
  - StrongNamer package with custom target modifications
  - Custom targets: `ModifyReferencePathForStrongNamer`, `RestoreOriginalReferencePath`, `UpdateAppConfig`
  - Inline RoslynCodeTaskFactory task for App.config manipulation
  - Conditional EqmProjectPath resolution
- **Assembly Attributes**: Standard attributes in AssemblyInfo.cs
- **Complexity Factors**:
  - Heavy custom MSBuild logic for assembly strong naming
  - Runtime App.config manipulation via custom task
  - Complex reference path filtering

## Baseline

- **Solution builds**: ✅ Yes (successful)
- **Warning count**: 0
- **Configuration**: Debug|x64 (MAC_use_cases), Debug|AnyCPU (Tests)

## Key Findings

1. **Already PackageReference-based**: Both projects are already using PackageReference instead of packages.config, which simplifies the conversion significantly.

2. **Custom MSBuild Logic**: Both projects contain custom build targets that must be carefully preserved:
   - MAC_use_cases: CleanProjPkgs target
   - MAC_use_cases.Tests: StrongNamer integration, reference path manipulation, App.config updates

3. **WPF/XAML Content**: MAC_use_cases contains XAML resource dictionaries and UI pages that need proper SDK-style handling.

4. **AssemblyInfo Attributes**: Both projects have AssemblyInfo.cs files with standard attributes. SDK-style projects auto-generate some of these (Version, FileVersion, InformationalVersion), which may cause duplicates. These will need to be handled during conversion.

5. **Interop Assembly**: MAC_use_cases references Microsoft.Office.Interop.Excel with `EmbedInteropTypes=True`, which should be preserved.

6. **NuGet Package Creation**: MAC_use_cases is configured to create NuGet packages via NuGetizer. This configuration should be preserved.

7. **Linked Files**: MAC_use_cases.Tests links to source files from the main project using wildcard patterns. SDK-style conversion should maintain this pattern.

8. **TIA Portal Integration**: MAC_use_cases has deep integration with Siemens TIA Portal, including registry-based path detection and custom references.

## Conversion Strategy

1. **Convert MAC_use_cases first** (dependency order - Tests depends on it)
2. **Convert MAC_use_cases.Tests second**
3. **Preserve all custom MSBuild targets** during conversion
4. **Handle XAML content** properly (ensure WPF SDK support)
5. **Manage AssemblyInfo.cs attributes** to avoid duplicates
6. **Validate build after each conversion**
7. **Run tests after both conversions** to ensure functionality is preserved

## Risk Assessment

- **Medium Risk**: Both projects have custom MSBuild logic that must be carefully preserved
- **WPF/XAML handling**: Requires proper SDK configuration
- **Custom tooling dependencies**: StrongNamer and NuGetizer integration must continue working
- **No TFM change**: Target frameworks remain .NET Framework 4.8 (not upgrading to .NET Core/.NET)
