# Unit Test Plan: EM-400 Damper Control

**Version:** 1.0
**Date:** September 15, 2025
**Purpose:** To define the test cases for verifying the functionality of the `FB400_EM_Damper` Function Block.

---

## 1. Test Objective

To ensure the damper module correctly drives to the minimum ventilation position and correctly responds to the economizer PID command.

## 2. Test Cases

### Test Case 1: Minimum Ventilation Position

*   **Test Name:** `TC1_Min_Vent_Position`
*   **Objective:** Verify the damper goes to its minimum position when the fan is on and economizer is inactive.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Enable the module. |
| 1.1 | | `#Instance_DB.Econ_Mode_Active` | `FALSE` | Ensure economizer is off. |
| 1.2 | | `#Instance_DB.UDT.Min_Fresh_Air_Pos`| `20.0` | Set min position to 20%. |
| 1.3 | *Evaluate* | `#Instance_DB.UDT.Damper_Pos_Cmd_AO`| `20.0` | **Check:** Damper command is at min position. |

### Test Case 2: Economizer Mode Passthrough

*   **Test Name:** `TC2_Econ_Mode`
*   **Objective:** Verify the damper command follows the economizer PID's demand.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 2.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | |
| 2.1 | | `#Instance_DB.Econ_Mode_Active`| `TRUE` | Activate economizer mode. |
| 2.2 | | `#Instance_DB.Econ_PID_Demand` | `65.0` | Simulate 65% demand from economizer PID. |
| 2.3 | *Evaluate* | `#Instance_DB.UDT.Damper_Pos_Cmd_AO`| `65.0` | **Check:** Damper command follows the PID demand. |

### Test Case 3: Damper Failure on Feedback Mismatch

*   **Test Name:** `TC3_Damper_Failure`
*   **Objective:** Verify the damper failure alarm is triggered if command and feedback do not match.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 3.0 | **Initial State** | `#Instance_DB.Econ_PID_Demand` | `70.0` | Command damper to 70%. |
| 3.1 | | `#Instance_DB.UDT.Fault_Delay_Sec`| `T#5s` | Set fault delay for the test. |
| 3.2 | | `#Instance_DB.UDT.Fault_Tolerance_Perc`| `5.0` | Set fault tolerance to 5%. |
| 4.0 | **Simulate Mismatch** | `#Instance_DB.UDT.Damper_Pos_Fdbk_AI`| `40.0` | Simulate a stuck damper. |
| 4.1 | **Wait for Fault** | `#WAIT` | `5000` | Wait for 5000 ms. |
| 4.2 | *Evaluate* | `#Instance_DB.UDT.Damper_Failure_Alm`| `TRUE` | **Check:** The failure alarm is now active. |
