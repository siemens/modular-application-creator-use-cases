# MAC GUI - Project Manifest

**Version:** 1.0
**Date:** September 12, 2025
**Purpose:** To define the complete set of files and directories required to build, test, and run the Modular Application Creator (MAC) GUI Visual Studio project.

---

## 1. Overview

This document serves as a manifest of the core project components. Due to technical limitations preventing a full recursive copy, this file lists the necessary items that must be gathered to create a complete, isolated development environment.

## 2. Required Components

To set up the project, the following files and directories must be copied from the root of the source repository into a new solution directory.

### 2.1. Root Directory Files

These files are required at the top level of the solution.

*   `MAC_use_cases.sln` - The main Visual Studio Solution file.
*   `nuget.config` - NuGet package source configuration.
*   `dotnet-tools.json` - .NET local tools manifest.

### 2.2. Project Directories

These directories contain the C# projects and all related source code, resources, and configurations. They must be copied in their entirety.

*   `MAC_use_cases/` - The main project for the MAC GUI application.
*   `MAC_use_cases.Tests/` - The test project containing all unit and integration tests.

## 3. Build & Run Instructions

1.  **Gather Components:** Create a new root folder and copy the files and directories listed above into it.
2.  **Open Solution:** Open the `MAC_use_cases.sln` file in Visual Studio.
3.  **Restore Dependencies:** Build the solution (press `Ctrl+Shift+B` or `F6`). Visual Studio should automatically restore all required NuGet packages based on the project and config files.
4.  **Run Application:** Set the `MAC_use_cases` project as the startup project and run it (press `F5`).
5.  **Run Tests:** Use the Test Explorer window in Visual Studio to discover and run the tests in the `MAC_use_cases.Tests` project.
