# Sequence of Operations & Functional Test Procedure

**Project:** Constant Volume Air Handler Unit (AHU) Controller
**Reference:** Software Design Specification, Version 1.0

## 1. Sequence of Operations (SOO)

### 1.1. General Operation & Modes

The AHU will operate in one of three modes, determined by a time schedule, a command from a Building Automation System (BAS), or a manual selection on the HMI:

*   **Off Mode:** The entire AHU is disabled. The supply fan, heating, and cooling are all off. The outside air damper is commanded closed.
*   **Occupied Mode:** The unit is active during normal building hours.
    *   The **Supply Fan** runs continuously to provide ventilation.
    *   The controller actively maintains the **Occupied Discharge Air Temperature (DAT) Setpoint**.
    *   The system will utilize free cooling via the **Economizer** when outside air conditions are favorable.
*   **Unoccupied Mode:** The unit operates in a setback mode during off-hours to save energy.
    *   The **Supply Fan** will cycle on only when there is a demand for heating or cooling.
    *   The controller will maintain a wider temperature band based on **Unoccupied Heating and Cooling Setpoints**.

### 1.2. Supply Fan Control

*   **Start/Stop:** The Supply Fan (`EM-100`) is commanded to start whenever the unit is in `Occupied` mode, or when there is a call for heating or cooling in `Unoccupied` mode.
*   **Proof of Flow:** After the fan is commanded to start, the system expects to see proof of operation from the **Run Feedback** and **Airflow Switch** within 5 seconds.
*   **Failure:** If proof is not established, the fan will be commanded off, and a **"Fan Failure"** alarm will be generated. The unit will not operate until the alarm is cleared. A **"VFD Fault"** alarm will also shut down the fan immediately.

### 1.3. Temperature Control

A single PID loop (`TO_PID_DAT_Control`) manages all heating and cooling to maintain the active Discharge Air Temperature (DAT) Setpoint. The output of this PID is bipolar (-100% to +100%) and drives the heating and cooling valves.

#### 1.3.1. Economizer / Free Cooling Mode (`EM-400`)

*   **Activation:** The system will enter Economizer mode if **ALL** of the following conditions are true:
    1.  There is a demand for cooling (DAT is above setpoint).
    2.  Outside Air Temperature (OAT) is below the Return Air Temperature (RAT) by a configurable amount (e.g., 2°F).
    3.  OAT is below a configurable high-limit (e.g., 65°F).
*   **Operation:**
    *   Mechanical cooling (`EM-200`) is **locked out** and cannot run.
    *   A dedicated economizer PID loop (`TO_PID_Econ_Control`) will modulate the outside air damper to bring in cool air and maintain the DAT setpoint.
*   **Deactivation:** If any of the activation conditions become false, the economizer will exit, and the damper will return to its minimum position.

#### 1.3.2. Chilled Water Cooling Mode (`EM-200`)

*   **Activation:** The system will enable chilled water cooling if **ALL** of the following conditions are true:
    1.  There is a demand for cooling (the `TO_PID_DAT_Control` output is > 0).
    2.  The system is **NOT** in Economizer mode.
    3.  The chilled water coil freeze-stat (`CHW_Freeze_Stat_DI`) is in a normal state.
*   **Operation:** The positive output of the `TO_PID_DAT_Control` (0 to 100%) is scaled and sent to the `CHW_Valve_Cmd_AO` to modulate the chilled water valve.
*   **Safety Trip:** A trip on the `CHW_Freeze_Stat_DI` input will immediately command the valve closed (0%) and generate a **"Chilled Water Freeze Alarm"**.

#### 1.3.3. Hot Water Heating Mode (`EM-300`)

*   **Activation:** The system will enable hot water heating if **ALL** of the following conditions are true:
    1.  There is a demand for heating (the `TO_PID_DAT_Control` output is < 0).
    2.  The hot water coil freeze-stat (`HW_Freeze_Stat_DI`) is in a normal state.
*   **Operation:** The negative output of the `TO_PID_DAT_Control` (-100% to 0) is inverted and scaled (to 0-100%) and sent to the `HW_Valve_Cmd_AO` to modulate the hot water valve.
*   **Safety Trip:** A trip on the `HW_Freeze_Stat_DI` input will immediately command the valve closed (0%) and generate a **"Hot Water Freeze Alarm"**.

### 1.4. Ventilation Control (`EM-400`)

*   Whenever the supply fan is running and the unit is not in Economizer mode, the outside air damper will be modulated to a minimum position (e.g., 20%) to ensure the space receives adequate fresh air.

### 1.5. System Monitoring & Alarms (`EM-500`)

*   **Dirty Filter:** A "Dirty Filter" alarm is generated if the filter differential pressure switch is active for more than 10 seconds. This is a maintenance alarm and does not shut down the unit.
*   **Safety Alarms:** All safety alarms (Fan Failure, VFD Fault, CHW Freeze Alarm, HW Freeze Alarm) are critical and will shut down the associated equipment.

## 2. Functional Test Procedure

This procedure is to be used by a commissioning technician to verify the correct operation of the AHU controller.

**Pre-Test Checks:**
1.  Verify all I/O wiring matches the specification document.
2.  Power on the controller and establish a connection with TIA Portal.
3.  Verify all analog sensor readings (temperatures) are reasonable.

**Test Procedure:**
*(Use the HMI or TIA Portal to override values and observe results)*

| Step | Action | Expected Result |
| :--- | :--- | :--- |
| **Fan Control** |
| 1 | Command unit to **Occupied** mode from the HMI. | Supply Fan command turns ON. Fan speed reference goes to 100% (e.g., 10V). |
| 2 | While fan is running, disable the **Run Feedback** input. | After 5 seconds, the fan command turns OFF and a "Fan Failure" alarm appears on the HMI. |
| 3 | Clear alarm. Simulate a **VFD Fault** input. | Fan command turns OFF immediately and a "VFD Fault" alarm appears. |
| **Heating Control** |
| 4 | Clear alarms. Set DAT Setpoint to 75°F. Force the DAT reading to 65°F. | The `HW_Valve_Cmd_AO` should modulate towards 100% to bring the DAT up to setpoint. |
| 5 | Simulate a **HW Freeze Stat** fault. | The `HW_Valve_Cmd_AO` should go to 0 immediately. A "Hot Water Freeze Alarm" appears. |
| **Cooling Control** |
| 6 | Clear alarms. Set DAT Setpoint to 55°F. Force DAT to 65°F. Force OAT to 80°F (to disable economizer). | The `CHW_Valve_Cmd_AO` should modulate towards 100% to bring the DAT down to setpoint. |
| 7 | While cooling, simulate a **CHW Freeze Stat** fault. | The `CHW_Valve_Cmd_AO` should go to 0 immediately. A "Chilled Water Freeze Alarm" appears. |
| **Economizer Control** |
| 8 | Clear alarms. Set DAT Setpoint to 55°F. Force DAT to 65°F. Force OAT to 50°F and RAT to 75°F. | The system enters Economizer mode. **The `CHW_Valve_Cmd_AO` must remain at 0.** The Damper command modulates to control the DAT. |
| 9 | Force OAT to 70°F (above the high-limit). | Economizer mode exits. The damper returns to its minimum position. The `CHW_Valve_Cmd_AO` begins to modulate to control temperature. |
| **System Monitoring** |
| 10 | Simulate a **Dirty Filter Status** input. | After 10 seconds, a "Dirty Filter" alarm appears. The unit continues to run. |
