# Unit Test Plan: EM-500 System Monitoring

**Version:** 1.0
**Date:** September 15, 2025
**Purpose:** To define the test cases for verifying the functionality of the `FB500_EM_Monitoring` Function Block.

---

## 1. Test Objective

To ensure the system monitoring module correctly generates a "Dirty Filter" alarm after a specified time delay.

## 2. Test Cases

### Test Case 1: Dirty Filter Alarm

*   **Test Name:** `TC1_Dirty_Filter_Alarm`
*   **Objective:** Verify the alarm is generated after the correct delay.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `#Instance_DB.Enable` | `TRUE` | Enable the module. |
| 1.1 | | `#Instance_DB.UDT.Filter_Delay_Sec`| `T#10s` | Set alarm delay for the test. |
| 1.2 | | `#Instance_DB.UDT.Dirty_Filter_DI`| `FALSE` | Ensure input is initially off. |
| 1.3 | *Evaluate* | `#Instance_DB.UDT.Dirty_Filter_Alm`| `FALSE` | **Check:** Alarm is initially off. |
| 2.0 | **Simulate Dirty Filter**| `#Instance_DB.UDT.Dirty_Filter_DI`| `TRUE` | Simulate the DP switch activating. |
| 3.0 | **Wait for Delay** | `#WAIT` | `10000`| Wait for 10000 ms. |
| 3.1 | *Evaluate* | `#Instance_DB.UDT.Dirty_Filter_Alm`| `TRUE` | **Check:** The dirty filter alarm is now active. |
