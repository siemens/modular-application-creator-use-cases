# **HMI User Experience and Test Scenarios**

## **RTU Controller Project**

**Document Purpose:** This document defines the standard user interaction patterns, user experience (UX) flows, and specific operational scenarios for the Basic RTU Controller HMI. It is intended to guide front-end HMI developers and serve as a basis for creating functional and user acceptance test cases.

**Reference Documents:**

* Software Design Specification (SDS)  
* Programming Best Practices & Design Framework

### **1\. User Personas**

To ensure the HMI is intuitive and effective, we will design for two primary user personas:

* **The Building Operator:**  
  * **Goal:** Ensure occupant comfort and operational efficiency.  
  * **Needs:** Quick status overview, easy access to setpoint adjustments, clear scheduling information.  
  * **Technical Level:** Basic. Interacts with the HMI as an appliance.  
* **The Maintenance Technician:**  
  * **Goal:** Diagnose faults, perform maintenance, and verify correct equipment operation.  
  * **Needs:** Detailed alarm information, manual control overrides, access to PID tuning and system delays, ability to trend data.  
  * **Technical Level:** Advanced. Interacts with the HMI as a diagnostic tool.

### **2\. Core HMI Interaction Principles**

The following principles apply globally across the HMI to ensure a consistent and predictable user experience.

* **Persistent Navigation:** A navigation bar shall be present at the bottom or side of every screen, providing one-tap access to **Overview**, **Alarms**, **Settings**, and **Trends**. The currently active screen's button will be visually highlighted.  
* **Status Header:** A header will be locked to the top of every screen. It must display:  
  * Current Date & Time.  
  * Unit Status (e.g., RUNNING, OFF, FAULT).  
  * An **Alarm Banner** that only appears when an unacknowledged alarm is active.  
* **Consistent Color Coding:**  
  * **Green:** Indicates a running or active state (e.g., fan running, compressor on).  
  * **Red:** Indicates a fault or tripped safety condition. Flashing red indicates an unacknowledged critical alarm.  
  * **Yellow:** Indicates a warning or maintenance alert (e.g., Dirty Filter).  
  * **Blue:** Indicates a component is in a manual override mode.  
  * **Grey:** Indicates an inactive or off state.  
* **Action Confirmation:** Any action that changes the state of the system or a critical parameter requires user confirmation via a pop-up dialog. This includes acknowledging alarms, changing setpoints, and entering/exiting manual mode.

### **3\. User Scenarios & Test Cases**

The following scenarios describe common user interactions from start to finish. Each step includes an "Expected Outcome" that can be used to validate the HMI's functionality.

#### **Scenario A: At-a-Glance Daily Status Check**

* **Persona:** Building Operator  
* **Goal:** Quickly verify that the RTU is running correctly with no issues.

| Step | User Action | Expected Outcome |
| :---- | :---- | :---- |
| 1 | Walks up to the HMI panel. | The **Main Overview** screen is displayed by default. |
| 2 | Observes the graphical P\&ID. | All components are animated and colored according to their real-time state (e.g., supply fan graphic is green and spinning). Live temperature values are displayed and updating. |
| 3 | Observes the Status Header. | The header shows STATUS: RUNNING and no Alarm Banner is visible. |

#### **Scenario B: Responding to a High-Pressure Fault Alarm**

* **Persona:** Maintenance Technician  
* **Goal:** Safely acknowledge and diagnose a critical compressor fault.

| Step | User Action | Expected Outcome |
| :---- | :---- | :---- |
| 1 | Is notified of an issue. Approaches the HMI. | The Status Header displays a flashing red **Alarm Banner** with the text "1 New Critical Alarm". |
| 2 | Taps the Alarm Banner. | The HMI immediately navigates to the **Alarms Screen**. |
| 3 | Views the alarm list. | The High Pressure Fault alarm is at the top of the list, colored red, with a status of "Unacknowledged". The compressor graphic on the Overview screen is also red. |
| 4 | Investigates and resolves the physical cause of the high pressure. | (No HMI interaction) |
| 5 | On the Alarms Screen, selects the alarm and taps the "Acknowledge" button. | A confirmation pop-up appears: "Acknowledge 'High Pressure Fault'?". The user taps "Yes". |
| 6 | Observes the alarm list after acknowledgment. | The alarm text stops flashing and its status changes to "Acknowledged". The Alarm Banner in the header disappears. **Note:** The alarm remains in the active list until the physical switch resets. |
| 7 | Taps the "Reset" button (once the condition is clear). | A confirmation pop-up appears. After confirming, the alarm moves from the "Active Alarms" list to the "Alarm History" list. The compressor is now available to run. |

#### **Scenario C: Adjusting the Occupied Cooling Setpoint**

* **Persona:** Building Operator  
* **Goal:** Adjust the temperature setpoint for the building.

| Step | User Action | Expected Outcome |
| :---- | :---- | :---- |
| 1 | Taps the "Settings" button in the navigation bar. | A password entry screen (numeric keypad) is displayed. |
| 2 | Enters the Operator-level password and taps "Enter". | The **Settings Screen** loads, showing non-critical parameters like temperature setpoints. |
| 3 | Taps the input field next to "Occupied Cooling Setpoint". | A numeric keypad pop-up is displayed over the current screen. |
| 4 | Enters a new value and taps the "Enter" key on the pop-up. | A confirmation pop-up appears: "Change Occupied Cooling Setpoint to 72.0°F?". |
| 5 | Taps "Yes" on the confirmation pop-up. | The pop-up closes. The new value is now displayed in the settings field. The PLC begins using the new setpoint for its control logic. |

#### **Scenario D: Manually Testing the Supply Fan**

* **Persona:** Maintenance Technician  
* **Goal:** Manually operate the supply fan to verify VFD and motor operation.

| Step | User Action | Expected Outcome |
| :---- | :---- | :---- |
| 1 | Navigates to the Settings screen and enters the Technician-level password. | The full Settings screen is displayed, including a "Manual Control" or "Diagnostics" button. |
| 2 | Navigates to the Manual Control screen. | A list of controllable components (Supply Fan, Compressor, etc.) is shown, each with a status of "Auto". |
| 3 | Taps the "Manual" button for the Supply Fan. | A confirmation pop-up appears. User confirms. |
| 4 | Observes the Supply Fan control area. | The status indicator changes to "Manual" and is highlighted in blue. "Start", "Stop", and speed control buttons/fields, previously disabled, are now enabled. |
| 5 | Taps the "Start" button. | The PLC commands the fan to start. The button provides visual feedback (e.g., depresses). The fan status graphic on the Overview screen changes to green/running. |
| 6 | Taps the "Auto" button to return to normal operation. | A confirmation pop-up appears. User confirms. |
| 7 | Observes the Supply Fan control area. | The status indicator returns to "Auto", the blue highlighting is removed, and all manual control buttons are disabled. The fan's operation is now governed by the automatic PLC logic. |

