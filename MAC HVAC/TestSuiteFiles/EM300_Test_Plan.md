# Test Plan: EM-300 Heating Control (`FB300_EM_Heating`)

**Version:** 1.0
**Date:** September 13, 2025
**Purpose:** This document provides the complete set of manual test cases for the Heating Control Equipment Module.

---

## Test Cases

### Test Case 3.1: `TC3_1_Normal_Operation`

**Objective:** Verify the heater starts on demand and stops when satisfied.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `FALSE` | Ensure module is disabled. |
| 1.1 | | `#Instance_DB.UDT.High_Limit_DI` | `FALSE` | Safety is normal. |
| 2.0 | **Command ON** | `#Instance_DB.Enable` | `TRUE` | Enable the heating module. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Heat_Stage1_Cmd_DO` | `TRUE` | Check: Heater command is ON. |
| 3.0 | **Command OFF** | `#Instance_DB.Enable` | `FALSE` | Disable the heating module. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.Heat_Stage1_Cmd_DO` | `FALSE` | Check: Heater command is OFF. |

---

### Test Case 3.2: `TC3_2_Safety_Trip`

**Objective:** Verify that the high-temperature limit safety immediately stops the heater and raises an alarm.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Heater is running. |
| 1.1 | | `#Instance_DB.UDT.High_Limit_DI` | `FALSE` | Safety is normal. |
| 2.0 | **Simulate High Limit** | `#Instance_DB.UDT.High_Limit_DI` | `TRUE` | Trip the high-limit switch. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Heat_Stage1_Cmd_DO`| `FALSE` | **Check:** Heater stops. |
| 2.2 | *Evaluate* | `#Instance_DB.UDT.High_Limit_Alm` | `TRUE` | **Check:** High limit alarm is active. |
| 3.0 | **Reset Fault** | `#Instance_DB.UDT.High_Limit_DI` | `FALSE` | Reset the safety switch. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.Heat_Stage1_Cmd_DO`| `TRUE` | **Check:** Heater restarts as Enable is still true. |
| 3.2 | *Evaluate* | `#Instance_DB.UDT.High_Limit_Alm` | `FALSE` | **Check:** Alarm resets. |
