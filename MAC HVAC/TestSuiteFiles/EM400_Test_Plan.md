# Test Plan: EM-400 Damper/Economizer (`FB400_EM_Damper`)

**Version:** 1.0
**Date:** September 13, 2025
**Purpose:** This document provides the complete set of manual test cases for the Damper and Economizer Control Equipment Module.

---

## Test Cases

### Test Case 4.1: `TC4_1_Minimum_Ventilation`

**Objective:** Verify the damper goes to its minimum position when the fan is running but economizer is not active.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Enable the module. |
| 1.1 | | `#Instance_DB.UDT.Min_Fresh_Air_Pos`| `20.0` | Set min position to 20%. |
| 1.2 | | `#Instance_DB.UDT.Econ_Mode_Active`| `FALSE`| Ensure economizer is inactive. |
| 2.0 | *Evaluate* | `#Instance_DB.UDT.Damper_Pos_Cmd_AO` | `20.0` | **Check:** Damper command goes to min position. |

---

### Test Case 4.2: `TC4_2_Economizer_Activation`

**Objective:** Verify the economizer becomes active when conditions are favorable.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Module is enabled. |
| 1.1 | | `#Instance_DB.UDT.Econ_Temp_Diff` | `2.0` | Set temp differential for activation. |
| 1.2 | | `#Instance_DB.UDT.Econ_High_Limit` | `65.0` | Set high limit for OAT. |
| 2.0 | **Simulate Favorable** | `#Instance_DB.UDT.Return_Air_Temp_AI` | `75.0` | RAT is high. |
| 2.1 | | `#Instance_DB.UDT.Outside_Air_Temp_AI`| `55.0` | OAT is cool and below RAT. |
| 2.2 | | `#Instance_DB.UDT.Econ_Mode_Active`| `TRUE` | **This input is from the main logic, not the module itself.** |
| 2.3 | *Evaluate* | `#Instance_DB.UDT.Damper_Pos_Cmd_AO`| `> 20.0`| **Check:** Damper command should be modulating (not at min). Value depends on PID. |

---

### Test Case 4.3: `TC4_3_Economizer_Deactivation`

**Objective:** Verify the economizer becomes inactive when the outside air is too warm.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Module is enabled. |
| 1.1 | | `#Instance_DB.UDT.Econ_Mode_Active`| `TRUE` | Economizer is active. |
| 1.2 | | `#Instance_DB.UDT.Min_Fresh_Air_Pos`| `20.0` | |
| 2.0 | **Simulate High OAT** | `#Instance_DB.UDT.Outside_Air_Temp_AI`| `70.0` | OAT is now above the high limit of 65.0. |
| 2.1 | | `#Instance_DB.UDT.Econ_Mode_Active`| `FALSE`| **This input is from the main logic.** |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.Damper_Pos_Cmd_AO`| `20.0` | **Check:** Damper command returns to min position. |
