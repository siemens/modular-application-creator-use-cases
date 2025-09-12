# Project Setup and Workflow Guide

## Basic RTU Controller Generation

**Version:** 1.0
**Author:** Jules
**Date:** September 12, 2025

### 1. Introduction
This document provides a complete guide for setting up the development environment, using the Modular Application Creator (MAC) tool to generate a PLC program, and understanding the workflow for the generated TIA Portal project.

---

### 2. MAC Tool Setup (Visual Studio)
This section covers how to set up and run the C# application that generates the PLC code.

#### 2.1. Prerequisites
*   **Microsoft Visual Studio:** 2019 or 2022.
*   **.NET Framework:** Version 4.8 Developer Pack.
*   **Siemens TIA Portal:** Version 18, with an active license.
*   **TIA Portal Openness:** The Openness DLLs for TIA Portal V18 must be installed and available. This is typically an option during the TIA Portal installation.

#### 2.2. Initial Setup
1.  **Clone the Repository:** Clone this Git repository to your local machine.
2.  **Open the Solution:** Navigate to the `MAC_use_cases/` directory and open the `MAC_use_cases.sln` file in Visual Studio.
3.  **Restore NuGet Packages:** Right-click the solution in the Solution Explorer and select "Restore NuGet Packages". This will download all the required dependencies (e.g., `Newtonsoft.Json`, `Siemens.ModularApplicationCreator.Core`).
4.  **Build the Solution:** Build the project (Build > Build Solution). It should compile without errors.

#### 2.3. Running the MAC Tool & Workflow
1.  **Run the Project:** Start the project from Visual Studio (Debug > Start Debugging or press F5). This will launch the MAC application's graphical user interface (GUI).
2.  **Configure Setpoints:** On the main screen, locate the "RTU Project Setpoints" section. Fill in the desired start values for all parameters (temperatures, timers, etc.).
3.  **Generate Project:** Click the **"Generate Basic RTU Project"** button. This will execute the C# code that generates all the necessary PLC blocks (`DB_SystemSettings`, all `FB_EM...` blocks, and `OB1`) inside your target TIA Portal project.

---

### 3. Using the Generated Project (TIA Portal)
This section explains how to work with the output of the MAC tool.

#### 3.1. Prerequisites
*   **Siemens TIA Portal:** Version 18.
*   An open TIA Portal project that will be the target for the generation.

#### 3.2. Post-Generation Workflow
1.  **Verify Generated Blocks:** After running the MAC tool, open your target TIA Portal project. In the project tree, navigate to "Program blocks". You will find all the generated blocks, organized into folders as specified in the `Software Design Specification.md`.
2.  **Create Library UDTs:** As noted in the `MAC_Code_Generation_Guide.md`, the Equipment Module FBs rely on UDTs that must exist in the project library. You must manually create the UDTs (e.g., `UDT_EM100_Interface`) in the `Lib_MAC_use_cases` library with the members specified in the placeholder files.
3.  **Connect Physical I/O:** The generated `OB1` uses simulated values for all hardware inputs (e.g., temperatures, switch statuses). The final step is to replace these simulated values with the actual PLC tags corresponding to your hardware I/O. For example, in the `iDB_EM400_DamperControl` call, you would replace `Return_Air_Temp := 22.5` with `Return_Air_Temp := "Tag_RAT"`.

---

### 4. Providing Feedback and Reporting Errors
To ensure issues are resolved efficiently, please use the following protocol when reporting feedback.

#### 4.1. To Jules (The AI Engineer)
When you encounter an issue, please provide a clear and descriptive message with the following information:

*   **For C# Errors (in the MAC Tool):**
    *   **Action:** Describe the steps you took that led to the error.
    *   **Error Message:** Provide the full error message from the Visual Studio build output or the exception message box that appeared in the GUI.

*   **For SCL/PLC Errors (in TIA Portal):**
    *   **Context:** Mention which block the error occurred in (e.g., `FB_EM100_SupplyFan`).
    *   **Error Message:** After compiling the project in TIA Portal, go to the "Info" > "Compile" tab. Please provide a screenshot or copy-paste the exact error message and its description from this window. This provides the most accurate information for debugging the generated SCL code.
