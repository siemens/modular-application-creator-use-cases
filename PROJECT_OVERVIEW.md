# Project Overview: HVAC AHU Controller Generator

## 1. Project Purpose

This repository contains a complete solution for generating the PLC (Programmable Logic Controller) software for a **Constant Volume Air Handler Unit (AHU)**. The design is centered around a single heat exchanger unit with separate modulating valves for the heating (hot water) and cooling (chilled water) loops. The project uses the **Siemens Modular Application Creator (MAC)** framework to automate the creation of a TIA Portal project from a C#/.NET application.

The primary goal is to provide a standardized, robust, and maintainable software package for controlling common HVAC equipment.

## 2. Repository Workflow

The development and testing process follows a clear, document-driven workflow. Understanding this workflow is key to understanding the repository.

1.  **Design (`SDS_AHU_Controller.md`)**: The process begins with the **Software Design Specification**. This document defines the hardware, I/O, control logic, and alarm conditions for the AHU controller. It is the primary blueprint for the project.

2.  **Code Generation (`MAC_use_cases/`)**: The C# source code in this directory reads the design principles and generates the final PLC code. It is the "engine" of the project.

3.  **Generated Output (`TiaPortal_Export_Files/`)**: The code generator produces a set of `.scl`, `.udt`, and `.db` files. These are the source files that can be directly imported into a Siemens TIA Portal project to create the functioning PLC program.

4.  **Verification & Validation (`TestSuiteFiles/`)**: This directory contains the assets for testing the generated logic.
    *   **Test Plans (`*.md`)**: These documents define the specific test cases that must be passed to verify the controller's functionality.
    *   **Simulation Scripts (`*.py`)**: These Python scripts are used to simulate the PLC environment (e.g., with PLC Sim Advanced) and execute the test cases defined in the test plans.

## 3. Getting Started (by Role)

This repository is used by different engineering disciplines. Here's where to start based on your role:

*   **For PLC / Controls Engineers:**
    *   Begin by reading the `MAC HVAC/SDS_AHU_Controller.md` to understand the control logic.
    *   Next, review the test cases in the `TestSuiteFiles/` directory to see how that logic is verified.

*   **For C# / .NET Developers:**
    *   Start with the main `README.md` to configure your development environment for the Modular Application Creator.
    *   Then, explore the source code in the `MAC_use_cases/` directory to understand the code generation logic.

## 4. Key Artifacts Quick Reference

| Directory / File                    | Purpose                                                      |
| :---------------------------------- | :----------------------------------------------------------- |
| `MAC HVAC/SDS_AHU_Controller.md`    | The master design document for the AHU controller.           |
| `MAC_use_cases/`                    | The C# source code for the TIA Portal code generator.        |
| `TestSuiteFiles/`                   | Contains all test plans (`.md`) and simulation scripts (`.py`). |
| `TiaPortal_Export_Files/`           | The final, generated PLC source files.                       |
| `README.md`                         | Instructions for setting up the C# development environment.    |
