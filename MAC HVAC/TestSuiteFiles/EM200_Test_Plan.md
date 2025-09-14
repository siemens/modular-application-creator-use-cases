# Test Plan: EM-200 Cooling Control (`FB200_EM_Cooling`)

**Version:** 1.0
**Date:** September 13, 2025
**Purpose:** This document provides the complete set of manual test cases for the Cooling Control Equipment Module. These tests are to be implemented in the TIA Portal Test Suite.

---

## Test Cases

### Test Case 2.1: `TC2_1_Normal_Operation`

**Objective:** Verify the compressor starts on demand and stops when satisfied, with no safeties tripped.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `FALSE` | Ensure module is disabled. |
| 1.1 | | `#Instance_DB.UDT.HP_Switch_DI` | `FALSE` | All safeties are normal (not tripped). |
| 1.2 | | `#Instance_DB.UDT.LP_Switch_DI` | `FALSE` | |
| 1.3 | | `#Instance_DB.UDT.Freeze_Stat_DI`| `FALSE` | |
| 2.0 | **Command ON** | `#Instance_DB.Enable` | `TRUE` | Enable the cooling module. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Compressor_Cmd_DO` | `TRUE` | Check: Compressor command is ON. |
| 3.0 | **Command OFF** | `#Instance_DB.Enable` | `FALSE` | Disable the cooling module. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.Compressor_Cmd_DO` | `FALSE` | Check: Compressor command is OFF. |

---

### Test Case 2.2: `TC2_2_Short_Cycle_Prevention`

**Objective:** Verify the minimum run-time and minimum off-time delays are enforced.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `FALSE` | Module is disabled. |
| 1.1 | | `#Instance_DB.UDT.Min_Run_Time_Sec` | `T#3m` | Set min run time to 3 minutes. |
| 1.2 | | `#Instance_DB.UDT.Min_Off_Time_Sec`| `T#3m` | Set min off time to 3 minutes. |
| 2.0 | **Command ON** | `#Instance_DB.Enable` | `TRUE` | Start the compressor. |
| 2.1 | **Try to Stop Early**| `#Instance_DB.Enable` | `FALSE` | Attempt to stop it after 1 minute. |
| 2.2 | *Wait* | `#WAIT` | `60000` | Wait 1 minute. |
| 2.3 | *Evaluate* | `#Instance_DB.UDT.Compressor_Cmd_DO`| `TRUE` | **Check:** Command should still be ON due to min run time. |
| 3.0 | **Wait for Min Run** | `#WAIT`| `120000` | Wait another 2 minutes to satisfy min run time. |
| 3.1 | *Evaluate*| `#Instance_DB.UDT.Compressor_Cmd_DO`| `FALSE` | **Check:** Command should now be OFF. |
| 4.0 | **Try to Start Early**| `#Instance_DB.Enable` | `TRUE` | Attempt to restart after 1 minute. |
| 4.1 | *Wait* | `#WAIT` | `60000` | Wait 1 minute. |
| 4.2 | *Evaluate*| `#Instance_DB.UDT.Compressor_Cmd_DO`| `FALSE` | **Check:** Command should still be OFF due to min off time. |

---

### Test Case 2.3: `TC2_3_Safety_Trips`

**Objective:** Verify that any safety input immediately stops the compressor and raises the correct alarm.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Compressor is running. |
| 1.1 | | `#Instance_DB.UDT.HP_Switch_DI` | `FALSE` | |
| 1.2 | | `#Instance_DB.UDT.LP_Switch_DI` | `FALSE` | |
| 1.3 | | `#Instance_DB.UDT.Freeze_Stat_DI`| `FALSE` | |
| 2.0 | **Simulate HP Fault** | `#Instance_DB.UDT.HP_Switch_DI` | `TRUE` | Trip the high-pressure switch. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Compressor_Cmd_DO`| `FALSE` | **Check:** Compressor stops. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.HP_Fault_Alm` | `TRUE` | **Check:** HP alarm is active. |
| 3.0 | **Reset and Test LP**| `#Instance_DB.UDT.HP_Switch_DI` | `FALSE` | Reset the HP fault. |
| 3.1 | | `#Instance_DB.UDT.LP_Switch_DI` | `TRUE` | Trip the low-pressure switch. |
| 3.2 | *Evaluate* | `#Instance_DB.UDT.LP_Fault_Alm` | `TRUE` | **Check:** LP alarm is active. |
