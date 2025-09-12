# Modular Application Creator (MAC) Code Generation Guide

## Basic RTU Controller

**Version:** 1.0
**Author:** Jules
**Date:** September 12, 2025

### 1. Introduction

This document serves as a guide for developers working on the code generation capabilities of the Modular Application Creator (MAC) tool, specifically for the Basic Rooftop HVAC Unit (RTU) Controller project. It details the C# classes and methods responsible for programmatically generating the TIA Portal PLC project from the specifications laid out in the `Software Design Specification.md`.

The primary goal of this automated approach is to ensure consistency, enforce standards, and dramatically reduce the manual engineering time required to create new RTU controller projects.

### 2. Core Generation Logic: `RtuGeneration.cs`

The heart of the generation process is the static class `RtuGeneration`, located at `MAC_use_cases/Model/UseCases/RtuGeneration.cs`. This class contains a suite of methods, each responsible for generating a specific part of the PLC program.

#### Key Methods:

*   **`GenerateSystemSettingsDB(PlcDevice plcDevice)`**
    This method creates the global Data Block `DB_SystemSettings`. It defines all the system-wide configurable parameters (e.g., temperature setpoints, timers) and their default values, providing a single source of truth for tuning.

*   **`Generate_EM<XXX>... (PlcDevice plcDevice)`**
    A separate method exists for each of the five Equipment Modules (e.g., `Generate_EM100_SupplyFan`). Each method follows a standard pattern:
    1.  Instantiates an `XmlFB` object with the correct name (e.g., `FB_EM100_SupplyFan`).
    2.  Sets the programming language to SCL.
    3.  Defines the FB's interface (Input, Output, Static) according to the SDS.
    4.  Generates the required SCL logic as a C# string.
    5.  Injects the SCL code into the FB structure.
    6.  Calls `GenerateSclBlock(plcDevice)` to create the file for TIA Portal.

*   **`Generate_OB1_Main(PlcDevice plcDevice, MAC_use_casesEM module)`**
    This is the final orchestration method. It performs the following actions:
    1.  Creates an instance Data Block (e.g., `iDB_EM100_SupplyFan`) for each of the five Equipment Module FBs.
    2.  Generates the main `OB1` block.
    3.  Injects the top-level SCL program that calls each of the EM instances, connects their inputs and outputs, and implements the main RTU state machine (Mode, Demand, and Safety logic).

### 3. Data Structures and UDTs

A critical aspect of the framework is its library-based approach to handling complex data types.

*   **UDT Wrappers:** The C# project does not define the structure of UDTs directly. Instead, it uses C# "wrapper" classes that correspond to UDTs pre-defined in a TIA Portal library.
*   **Placeholder Location:** For the Basic RTU project, the necessary (but currently conceptual) UDT wrappers are located in `MAC_use_cases/TiaImports/GeneratedClasses/RTU/`.
*   **Workflow Expectation:** The intended workflow is that a PLC engineer will create the required UDTs (e.g., `UDT_EM100_Interface`) in the `Lib_MAC_use_cases` TIA Portal library. The MAC framework's tools would then be used to auto-generate the final C# wrapper classes, replacing the placeholders. The generation code in `RtuGeneration.cs` can then use these wrappers to create instances of the UDTs in the project.

### 4. UI Trigger

The entire code generation process is initiated from the MAC application's user interface.

*   **Button:** A button with the content "Generate Basic RTU Project" was added to `MAC_use_cases/UI/FirstPage.xaml`.
*   **Event Handler:** The `Click` event for this button is handled by the `GenerateRtuButton_Click` method in `MAC_use_cases/UI/FirstPage.xaml.cs`. This method is the primary entry point that calls the `RtuGeneration` methods in the correct sequence.

---
### **Jules's Corner**

This implementation was developed by Jules. A key consideration during development was the inability to compile or test the generated SCL code against a live TIA Portal SDK. Therefore, the code was written with strict adherence to the Software Design Specification and common SCL practices. A high-priority backlog item should be to establish a continuous integration environment with PLCSIM Advanced to allow for automated testing of the generated code, which would significantly improve robustness and reliability.
---
