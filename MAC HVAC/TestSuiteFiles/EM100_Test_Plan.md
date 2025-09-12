# Test Plan: EM-100 Supply Fan (`FB100_EM_SupplyFan`)

**Version:** 1.0
**Date:** September 12, 2025
**Purpose:** This document provides the complete set of manual test cases for the Supply Fan Equipment Module. These tests are to be implemented in the TIA Portal Test Suite to verify correct functionality.

---

## Test Cases

### Test Case 1: `TC1_Normal_Start_Stop`

**Objective:** Verify the fan starts and stops correctly with normal feedback.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `FALSE` | Ensure module is disabled. |
| 1.1 | | `#Instance_DB.UDT.VFD_Fault_DI` | `FALSE` | No initial faults. |
| 2.0 | **Command ON** | `#Instance_DB.Enable` | `TRUE` | Enable the module. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `TRUE` | Check: Start command is sent. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.Is_Running` | `FALSE` | Check: Not running yet (no feedback). |
| 3.0 | **Simulate Feedback** | `#Instance_DB.UDT.Run_Fdbk_DI`| `TRUE` | Provide run feedback. |
| 3.1 | | `#Instance_DB.UDT.Airflow_Status_DI`| `TRUE` | Provide airflow feedback. |
| 3.2 | *Evaluate* | `#Instance_DB.UDT.Is_Running` | `TRUE` | Check: Is_Running status is now true. |
| 4.0 | **Command OFF** | `#Instance_DB.Enable` | `FALSE` | Disable the module. |
| 4.1 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE` | Check: Start command is turned off. |
| 4.2 | *Evaluate* | `#Instance_DB.UDT.Is_Running` | `FALSE` | Check: Is_Running status is now false. |

---

### Test Case 2: `TC2_VFD_Fault`

**Objective:** Verify a VFD fault immediately shuts down the fan and raises an alarm.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Module is running. |
| 1.1 | | `#Instance_DB.UDT.Run_Fdbk_DI`| `TRUE` | |
| 1.2 | | `#Instance_DB.UDT.Airflow_Status_DI`| `TRUE` | |
| 1.3 | | `#Instance_DB.UDT.VFD_Fault_DI` | `FALSE` | |
| 2.0 | **Simulate VFD Fault** | `#Instance_DB.UDT.VFD_Fault_DI` | `TRUE` | VFD trips. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.VFD_Fault_Alm` | `TRUE` | Check: VFD fault alarm is active. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE` | Check: Start command is shut off. |
| 2.3 | *Evaluate* | `#Instance_DB.UDT.Is_Running` | `FALSE` | Check: Is_Running status is now false. |
| 3.0 | **Clear VFD Fault** | `#Instance_DB.UDT.VFD_Fault_DI` | `FALSE` | Fault is cleared. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.VFD_Fault_Alm` | `FALSE`| Check: Alarm resets. |
| 3.2 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `TRUE` | Check: Fan restarts as Enable is still true. |

---

### Test Case 3: `TC3_Fan_Failure_No_Feedback`

**Objective:** Verify the fan failure alarm activates if feedback is not received within the configured delay.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Command fan ON. |
| 1.1 | | `#Instance_DB.UDT.Run_Fdbk_DI` | `FALSE` | No feedback is present. |
| 1.2 | | `#Instance_DB.UDT.Airflow_Status_DI`| `FALSE`| No feedback is present. |
| 1.3 | | `#Instance_DB.UDT.Fault_Delay_Sec`| `T#5s` | Set fault delay. |
| 2.0 | **Wait for Fault** | `#WAIT` | `5000` | Wait for 5000 ms (matching the delay). |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Fan_Failure_Alm`| `TRUE` | Check: The failure alarm is now active. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.Start_Cmd_DO` | `FALSE` | Check: Start command is disabled on fault. |
| 3.0 | **Acknowledge/Reset** | `#Instance_DB.Enable` | `FALSE` | Reset the module by disabling it. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.Fan_Failure_Alm`| `FALSE` | Check: Alarm resets on disable. |
