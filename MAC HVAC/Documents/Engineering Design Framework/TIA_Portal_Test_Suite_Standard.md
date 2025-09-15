# TIA Portal Test Suite - Application Standard

**Version:** 1.0
**Date:** September 12, 2025
**Purpose:** To define the standard for creating and executing automated unit tests for Equipment Module (EM) Function Blocks using the TIA Portal Test Suite. This ensures code quality, correctness, and maintainability.

---

## 1. Introduction

The TIA Portal Test Suite is a mandatory tool for this project. Its use is not optional. Every Equipment Module Function Block (FB) must have an associated test case that validates its logic against the functional requirements defined in the Sequence of Operations. A passing test is required before any code can be merged into the main branch.

This standard provides a consistent methodology for creating, managing, and executing these tests.

## 2. Test Case Philosophy

*   **Isolation:** Tests should be designed to validate the logic of a single FB in isolation.
*   **Completeness:** Test cases must cover normal operation, all specified fault conditions, and boundary conditions.
*   **Clarity:** Tests should be clearly named and structured to be understandable by other engineers.

## 3. Test Case Structure

Each test case created in the Test Suite editor for an FB shall be structured with the following sections:

1.  **Interface:** The FB to be tested is defined here.
2.  **Test Definition:** Tests are defined in a table. Each row represents a step in the test sequence. The key columns are:
    *   **Test ID:** A unique identifier for the test step (e.g., `1.0`, `2.0`).
    *   **Test Step Name:** A clear description of the action being taken (e.g., "Command Fan ON", "Simulate Airflow Feedback").
    *   **Parameter:** The input or static tag being manipulated or evaluated.
    *   **Value:** The value being written to an input or the expected value of an output.
    *   **Comment:** An explanation of the purpose of the test step.

## 4. Example: Testing EM-200 (`FB200_EM_Cooling`)

This example demonstrates how to create a test case for the `FB200_EM_Cooling` Function Block, which controls a modulating valve.

**Test Objective:** To verify the chilled water valve modulates correctly based on an analog demand signal and that the freeze protection safety works.

### Test Case 1: Modulating Valve Control

*   **Test Name:** `TC1_Modulation_Test`
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Ensure FB is enabled. |
| 1.1 | | `#Instance_DB.UDT.CHW_Freeze_Stat_DI`| `FALSE` | Ensure no safety faults are active. |
| 1.2 | | `#Instance_DB.Cooling_Demand` | `0.0` | Set cooling demand to 0%. |
| 1.3 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Cmd_AO`| `0.0` | **Check:** Valve command is at 0%. |
| 2.0 | **Test 50% Demand**| `#Instance_DB.Cooling_Demand` | `50.0` | Simulate a 50% cooling demand from the PID. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Cmd_AO`| `50.0` | **Check:** Valve command tracks the demand. |
| 3.0 | **Test 100% Demand**| `#Instance_DB.Cooling_Demand` | `100.0` | Simulate a 100% cooling demand. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Cmd_AO`| `100.0` | **Check:** Valve command goes to 100%. |
| 4.0 | **Test 0% Demand** | `#Instance_DB.Cooling_Demand` | `0.0` | Remove cooling demand. |
| 4.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Cmd_AO`| `0.0` | **Check:** Valve command returns to 0%. |

### Test Case 2: Freeze Stat Safety Trip

*   **Test Name:** `TC2_Freeze_Safety_Trip`
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Enable the module. |
| 1.1 | | `#Instance_DB.Cooling_Demand` | `75.0` | Set cooling demand to 75%. |
| 1.2 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Cmd_AO`| `75.0` | **Check:** Valve is open. |
| 2.0 | **Simulate Fault** | `#Instance_DB.UDT.CHW_Freeze_Stat_DI`| `TRUE` | Simulate the freeze stat tripping. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Freeze_Alm`| `TRUE` | **Check:** The freeze alarm is now active. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Cmd_AO`| `0.0` | **Check:** Valve is commanded closed on fault. |

## 5. Test Management and CI/CD

*   **Storage:** All tests for an FB must be grouped under a Test Group named after the FB (e.g., "FB100_EM_SupplyFan_Tests"). These tests are saved with the TIA Portal project and must be committed to the Git repository.
*   **Execution:** Before a merge request from a feature branch to the main branch is approved, the developer must execute all relevant tests.
*   **Requirement:** A screenshot of the passed test results from the TIA Portal Test Suite **must be included** in the pull request description as evidence of successful validation. Failure to do so will result in the merge request being rejected.
