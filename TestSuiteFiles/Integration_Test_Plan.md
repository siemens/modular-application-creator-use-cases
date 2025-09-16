# Integration Test Plan: Main Temperature Control

**Version:** 1.0
**Date:** September 15, 2025
**Purpose:** To define the test cases for verifying the functionality of the main temperature control loop, which consists of the `TO_PID_DAT_Control` (`PID_Temp`) Technology Object and its interaction with the heating and cooling Equipment Modules.

---

## 1. Test Objective

To ensure the main PID controller correctly responds to changes in temperature by producing a bipolar output signal that drives the heating and cooling demands, and that this output is correctly clamped at its limits.

## 2. Test Setup

*   **Test Environment:** TIA Portal Test Suite, running against a PLCSIM Advanced instance.
*   **Object Under Test:** `TO_PID_DAT_Control` (`PID_Temp` instance).
*   **Key Parameters:**
    *   **Input (to be forced):** `AHU1_DAT_Temp` (Process Variable).
    *   **Configuration:** `Setpoint` will be set to a nominal value (e.g., 70.0). `OutputUpperLimit` = 100.0, `OutputLowerLimit` = -100.0.
    *   **Tuning Note:** For these tests, the PID's integral time (`Ti`) should be set to a high value (e.g., `T#60s`) to simulate the slow response required for stable valve control.
    *   **Output (to be evaluated):** `Output` (The manipulated variable).

---

## 3. Test Cases

### Test Case 1: Heating Response

*   **Test Name:** `TC1_Heating_Response`
*   **Objective:** Verify that the PID output goes negative when the temperature is below the setpoint.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Initial State** | `Setpoint` | `70.0` | Set a baseline setpoint. |
| 1.1 | | `AHU1_DAT_Temp` | `70.0` | Simulate temperature at setpoint. |
| 1.2 | *Evaluate* | `Output` | `0.0` | **Check:** PID output should be zero. |
| 2.0 | **Simulate Heat Demand**| `AHU1_DAT_Temp` | `65.0` | Force temperature below setpoint. |
| 2.1 | *Evaluate* | `Output` | `< 0.0` | **Check:** PID output is negative, demanding heat. |

### Test Case 2: Cooling Response

*   **Test Name:** `TC2_Cooling_Response`
*   **Objective:** Verify that the PID output goes positive when the temperature is above the setpoint.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 3.0 | **Initial State** | `Setpoint` | `70.0` | |
| 3.1 | | `AHU1_DAT_Temp` | `70.0` | |
| 3.2 | *Evaluate* | `Output` | `0.0` | **Check:** PID output should be zero. |
| 4.0 | **Simulate Cool Demand**| `AHU1_DAT_Temp` | `75.0` | Force temperature above setpoint. |
| 4.1 | *Evaluate* | `Output` | `> 0.0` | **Check:** PID output is positive, demanding cooling. |

### Test Case 3: Output Clamping (Upper Limit)

*   **Test Name:** `TC3_Upper_Limit_Clamping`
*   **Objective:** Verify that the PID output does not exceed its configured upper limit.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 5.0 | **Initial State** | `Setpoint` | `70.0` | |
| 5.1 | | `OutputUpperLimit` | `100.0`| |
| 6.0 | **Force High Error**| `AHU1_DAT_Temp` | `90.0` | Force a large temperature error to drive output high. |
| 6.1 | *Evaluate* | `Output` | `100.0` | **Check:** PID output is clamped exactly at the upper limit. |

### Test Case 4: Output Clamping (Lower Limit)

*   **Test Name:** `TC4_Lower_Limit_Clamping`
*   **Objective:** Verify that the PID output does not exceed its configured lower limit.
*   **Test Steps:**
| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 7.0 | **Initial State** | `Setpoint` | `70.0` | |
| 7.1 | | `OutputLowerLimit` | `-100.0`| |
| 8.0 | **Force High Error**| `AHU1_DAT_Temp` | `50.0` | Force a large temperature error to drive output low. |
| 8.1 | *Evaluate* | `Output` | `-100.0` | **Check:** PID output is clamped exactly at the lower limit. |
