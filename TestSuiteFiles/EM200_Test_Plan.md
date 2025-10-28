# Unit Test Plan: EM-200 Heat Exchanger Cooling Valve Control

**Version:** 2.1
**Date:** September 17, 2025
**Purpose:** To define the test cases for verifying the functionality of the `FB200_EM_Cooling` Function Block, which controls the cooling valve on the main heat exchanger.

---

## 1. Test Objective

To ensure the cooling valve module correctly passes an analog demand signal to its output, correctly reads its feedback, and safely shuts down on a freeze alarm from the shared coil.

## 2. Test Cases

### Test Case 1: Valve Command Passthrough

*   **Test Name:** `TC1_Passthrough_and_Feedback`
*   **Objective:** Verify the cooling valve command directly follows the demand signal from the main controller.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Cooling_Demand_In` | `0.0` | Set demand to 0%. This input comes from the main PID. |
| 1.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Position_Cmd_AO` | `0.0` | **Check:** Valve command output is 0%. |
| 2.0 | **Test 50% Demand**| `#Instance_DB.Cooling_Demand_In` | `50.0` | Simulate a 50% demand from the PID. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Position_Cmd_AO`| `50.0` | **Check:** Valve command output tracks the demand. |
| 2.2 | **Simulate Fdbk**| `#Instance_DB.UDT.CHW_Valve_Position_Fdbk_AI`| `50.0` | Simulate the valve physically reaching 50%. |
| 2.3 | *Evaluate* | `#Instance_DB.UDT.Valve_Position_Sts` | `50.0` | **Check:** Internal status reflects the feedback. |

### Test Case 2: Freeze Stat Safety Trip

*   **Test Name:** `TC2_Freeze_Safety_Trip`
*   **Objective:** Verify that a freeze stat fault on the shared coil safely closes the cooling valve, overriding the demand.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 3.0 | **Initial State** | `#Instance_DB.Cooling_Demand_In` | `75.0` | Set demand to 75%. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Position_Cmd_AO`| `75.0` | **Check:** Valve is commanded open. |
| 4.0 | **Simulate Fault** | `#Instance_DB.UDT.CHW_Freeze_Stat_DI`| `TRUE` | Simulate the freeze stat tripping. |
| 4.1 | *Evaluate* | `#Instance_DB.UDT.CHW_Freeze_Alm`| `TRUE` | **Check:** The freeze alarm is now active. |
| 4.2 | *Evaluate* | `#Instance_DB.UDT.CHW_Valve_Position_Cmd_AO`| `0.0` | **Check:** Valve is commanded fully closed on fault, ignoring the input demand. |

### Test Case 3: Valve Failure on Feedback Mismatch

*   **Test Name:** `TC3_Valve_Failure`
*   **Objective:** Verify the valve failure alarm is triggered if command and feedback do not match.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 5.0 | **Initial State** | `#Instance_DB.Cooling_Demand_In` | `50.0` | Command valve to 50%. |
| 5.1 | | `#Instance_DB.UDT.Fault_Delay_Sec`| `T#5s` | Set fault delay for the test. |
| 5.2 | | `#Instance_DB.UDT.Fault_Tolerance_Perc`| `5.0` | Set fault tolerance to 5%. |
| 6.0 | **Simulate Mismatch** | `#Instance_DB.UDT.CHW_Valve_Position_Fdbk_AI`| `20.0` | Simulate a stuck valve (feedback != command). |
| 6.1 | **Wait for Fault** | `#WAIT` | `5000` | Wait for 5000 ms. |
| 6.2 | *Evaluate* | `#Instance_DB.UDT.Valve_Position_Failure_Alm`| `TRUE` | **Check:** The failure alarm is now active. |
