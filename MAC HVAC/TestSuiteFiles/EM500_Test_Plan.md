# Test Plan: EM-500 System Monitoring (`FB500_EM_Monitoring`)

**Version:** 1.0
**Date:** September 13, 2025
**Purpose:** This document provides the complete set of manual test cases for the System Monitoring Equipment Module.

---

## Test Cases

### Test Case 5.1: `TC5_1_Dirty_Filter_Alarm`

**Objective:** Verify the "Dirty Filter" alarm is raised after the configured time delay.

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Ensure module is enabled. |
| 1.1 | | `#Instance_DB.UDT.Dirty_Filter_DI` | `FALSE` | Filter is clean initially. |
| 1.2 | | `#Instance_DB.UDT.Filter_Delay_Sec` | `T#10s`| Set alarm delay to 10 seconds. |
| 2.0 | **Simulate Dirty Filter** | `#Instance_DB.UDT.Dirty_Filter_DI` | `TRUE` | Dirty filter switch is activated. |
| 2.1 | *Evaluate* | `#Instance_DB.UDT.Dirty_Filter_Alm` | `FALSE` | **Check:** Alarm should not be immediate. |
| 3.0 | **Wait for Delay** | `#WAIT` | `10000` | Wait for 10000 ms. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.Dirty_Filter_Alm` | `TRUE` | **Check:** Dirty filter alarm is now active. |
| 4.0 | **Reset Condition** | `#Instance_DB.UDT.Dirty_Filter_DI` | `FALSE` | Filter is replaced, switch resets. |
| 4.1 | *Evaluate* | `#Instance_DB.UDT.Dirty_Filter_Alm` | `FALSE` | **Check:** Alarm resets. |
