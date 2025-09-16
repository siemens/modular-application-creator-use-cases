# Unit Test Plan: EM-100 Supply Fan Control

**Version:** 2.0
**Date:** September 15, 2025
**Purpose:** To define the test cases for verifying the functionality of the `FB100_EM_SupplyFan` Function Block.

---

## 1. Test Objective

To ensure the supply fan module correctly responds to start/stop commands and that all fault conditions (VFD Fault, Fan Failure) are correctly handled.

## 2. Test Cases

### Test Case 1: Normal Start and Run

*   **Test Name:** `TC1_Normal_Start_Run`
*   **Objective:** Verify the fan starts correctly and the running status is set.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `FALSE` | |
| 1.1 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE` | **Check:** Start command is off. |
| 2.0 | **Command Fan ON** | `#Instance_DB.Enable` | `TRUE` | Enable the fan module. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `TRUE` | **Check:** The start command is sent. |
| 3.0 | **Simulate Feedback** | `#Instance_DB.UDT.Run_Fdbk_DI`| `TRUE` | Simulate VFD run feedback. |
| 3.1 | | `#Instance_DB.UDT.Airflow_Status_DI`| `TRUE` | Simulate airflow switch made. |
| 3.2 | *Evaluate* | `#Instance_DB.UDT.Is_Running` | `TRUE` | **Check:** The `Is_Running` status is set. |

### Test Case 2: Fan Failure on Start

*   **Test Name:** `TC2_Failure_No_Feedback`
*   **Objective:** Verify the fan failure alarm is triggered if feedback is not received in time.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 4.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Enable the fan module. |
| 4.1 | | `#Instance_DB.UDT.Fault_Delay_Sec`| `T#5s` | Set fault delay for the test. |
| 5.0 | **Wait for Fault** | `#WAIT` | `5000` | Wait for 5000 ms. |
| 5.1 | *Evaluate* | `#Instance_DB.UDT.Fan_Failure_Alm`| `TRUE` | **Check:** The failure alarm is now active. |
| 5.2 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE` | **Check:** The start command is disabled on fault. |

### Test Case 3: VFD Fault Trip

*   **Test Name:** `TC3_VFD_Fault`
*   **Objective:** Verify a VFD fault immediately stops the fan.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 6.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Fan is running. |
| 6.1 | | `#Instance_DB.UDT.Run_Fdbk_DI` | `TRUE` | |
| 7.0 | **Simulate VFD Fault**| `#Instance_DB.UDT.VFD_Fault_DI`| `TRUE` | |
| 7.1 | *Evaluate* | `#Instance_DB.UDT.VFD_Fault_Alm` | `TRUE` | **Check:** VFD Fault alarm is active. |
| 7.2 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE`| **Check:** Start command is immediately turned off. |
