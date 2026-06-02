# 02-convert-test-project: Convert MAC_use_cases.Tests Project

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
