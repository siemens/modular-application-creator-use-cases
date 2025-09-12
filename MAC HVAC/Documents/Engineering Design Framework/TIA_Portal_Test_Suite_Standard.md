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

## 4. Example: Testing EM-100 (`FB100_EM_SupplyFan`)

This example demonstrates how to create a test case for the `FB100_EM_SupplyFan` Function Block.

**Test Objective:** To verify the fan starts correctly under normal conditions and that the "Fan Failure" alarm logic functions as expected.

### Test Case 1: Normal Start and Run

*   **Test Name:** `TC1_Normal_Start_Run`
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `FALSE` | Ensure FB is initially disabled. |
| 1.1 | | `#Instance_DB.UDT.Run_Fdbk_DI` | `FALSE` | |
| 1.2 | | `#Instance_DB.UDT.VFD_Fault_DI` | `FALSE` | |
| 2.0 | **Command Fan ON** | `#Instance_DB.Enable` | `TRUE` | Enable the fan module. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `TRUE` | **Check:** The start command is sent. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.Fan_Failure_Alm` | `FALSE` | **Check:** No alarm should be present yet. |
| 3.0 | **Simulate Feedback** | `#Instance_DB.UDT.Run_Fdbk_DI`| `TRUE` | Simulate VFD run feedback. |
| 3.1 | | `#Instance_DB.UDT.Airflow_Status_DI`| `TRUE` | Simulate airflow switch made. |
| 3.2 | *Evaluate* | `#Instance_DB.UDT.Is_Running` | `TRUE` | **Check:** The `Is_Running` status is set. |
| 3.3 | *Evaluate* | `#Instance_DB.UDT.Fan_Failure_Alm` | `FALSE` | **Check:** The failure alarm remains false. |

### Test Case 2: Fan Failure (No Run Feedback)

*   **Test Name:** `TC2_Failure_No_Feedback`
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Enable the fan module. |
| 1.1 | | `#Instance_DB.UDT.Run_Fdbk_DI` | `FALSE` | Ensure no feedback is present. |
| 1.2 | | `#Instance_DB.UDT.Fault_Delay_Sec`| `T#5s` | Set fault delay. |
| 2.0 | **Wait for Fault** | `#WAIT` | `5000` | Wait for 5000 ms. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Fan_Failure_Alm`| `TRUE` | **Check:** The failure alarm is now active. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE` | **Check:** The start command is disabled on fault. |

## 5. Test Management and CI/CD

*   **Storage:** All tests for an FB must be grouped under a Test Group named after the FB (e.g., "FB100_EM_SupplyFan_Tests"). These tests are saved with the TIA Portal project and must be committed to the Git repository.
*   **Execution:** Before a merge request from a feature branch to the main branch is approved, the developer must execute all relevant tests.
*   **Requirement:** A screenshot of the passed test results from the TIA Portal Test Suite **must be included** in the pull request description as evidence of successful validation. Failure to do so will result in the merge request being rejected.
