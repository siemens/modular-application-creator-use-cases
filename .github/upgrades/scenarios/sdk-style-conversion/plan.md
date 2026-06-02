# Plan: SDK-style Conversion

## Overview
Convert both projects in the MAC_use_cases solution from legacy project format to SDK-style. Both projects are already using PackageReference (no packages.config migration needed), but contain custom MSBuild logic and special features that must be preserved.

## Tasks

### Task 1: Convert MAC_use_cases Project
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

### Task 2: Convert MAC_use_cases.Tests Project
**ID**: `02-convert-test-project`
**Description**: Convert the test project to SDK-style format while preserving StrongNamer integration, custom MSBuild targets, linked files from the main project, and App.config manipulation logic.

**Scope**:
- Convert project file to SDK-style format
- Preserve StrongNamer package integration
- Preserve custom MSBuild targets: ModifyReferencePathForStrongNamer, RestoreOriginalReferencePath, UpdateAppConfig
- Maintain RoslynCodeTaskFactory inline task for App.config updates
- Preserve linked file patterns from MAC_use_cases project
- Handle AssemblyInfo.cs attributes
- Maintain all PackageReferences
- Keep target framework at .NET Framework 4.8
- Preserve ProjectReference to MAC_use_cases

**Expected Complications**:
- Heavy custom MSBuild logic for StrongNamer integration
- Inline custom task (RoslynCodeTaskFactory) must be preserved
- Linked files with wildcard patterns
- Complex reference path manipulation

**Risk Level**: Medium

**Validation**:
- Project builds successfully with no errors
- No new warnings introduced
- Linked files are properly included
- StrongNamer integration works correctly
- Tests can be discovered and run
- App.config manipulation functions correctly

---

### Task 3: Final Validation
**ID**: `03-final-validation`
**Description**: Build the entire solution, run tests, and verify all functionality is preserved.

**Scope**:
- Clean and rebuild the entire solution
- Verify zero build errors
- Verify no new warnings (or same baseline)
- Run all unit tests
- Verify NuGet package creation still works (MAC_use_cases)
- Confirm all custom MSBuild targets execute correctly
- Validate that projects are now SDK-style

**Risk Level**: Low

**Success Criteria**:
- Solution builds successfully
- All tests pass (or same baseline as before conversion)
- No regression in warnings
- NuGet package creation works
- Custom build logic functions as expected
- Both projects are confirmed SDK-style format

## Execution Order

1. **Task 1** → Convert MAC_use_cases (main project first, as Tests depends on it)
2. **Task 2** → Convert MAC_use_cases.Tests (dependent project)
3. **Task 3** → Final validation (comprehensive verification)

## Notes

- **No Target Framework Changes**: Both projects remain on .NET Framework 4.8
- **PackageReference Already Used**: No packages.config migration needed
- **Custom Logic Preservation**: Critical to maintain all custom MSBuild targets and logic
- **Commit Strategy**: After each task (per user preferences)
