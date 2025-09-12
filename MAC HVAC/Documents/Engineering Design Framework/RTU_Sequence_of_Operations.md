# Sequence of Operations & Functional Test Procedure

**Project:** Basic Rooftop Unit (RTU) Controller
**Reference:** Software Design Specification, Version 1.0

## 1. Sequence of Operations (SOO)

### 1.1. General Operation & Modes

The RTU will operate in one of three modes, determined by a time schedule, a command from a Building Automation System (BAS), or a manual selection on the HMI:

*   **Off Mode:** The entire RTU is disabled. The supply fan, heating, and cooling are all off. The outside air damper is commanded closed.
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

A single PID loop (`TO_PID_DAT_Control`) manages all heating and cooling to maintain the active Discharge Air Temperature (DAT) Setpoint.

#### 1.3.1. Economizer / Free Cooling Mode (`EM-400`)

*   **Activation:** The system will enter Economizer mode if **ALL** of the following conditions are true:
    1.  There is a demand for cooling (DAT is above setpoint).
    2.  Outside Air Temperature (OAT) is below the Return Air Temperature (RAT) by a configurable amount (e.g., 2°F).
    3.  OAT is below a configurable high-limit (e.g., 65°F).
*   **Operation:**
    *   Mechanical cooling (`EM-200`) is **locked out** and cannot run.
    *   A dedicated economizer PID loop (`TO_PID_Econ_Control`) will modulate the outside air damper to bring in cool air and maintain the DAT setpoint.
*   **Deactivation:** If any of the activation conditions become false, the economizer will exit, and the damper will return to its minimum position.

#### 1.3.2. Mechanical Cooling Mode (`EM-200`)

*   **Activation:** The system will enable the DX compressor if **ALL** of the following conditions are true:
    1.  There is a demand for cooling.
    2.  The system is **NOT** in Economizer mode.
    3.  All cooling safeties (High/Low Pressure, Freeze Stat) are in a normal state.
    4.  The compressor minimum off-time (3 minutes) has elapsed.
*   **Operation:** The single-stage compressor is commanded ON. It will run until the cooling demand is met or for a minimum run-time of 3 minutes, whichever is longer.
*   **Safety Trips:** A trip on the High-Pressure, Low-Pressure, or Freeze-Stat inputs will immediately de-energize the compressor and generate a specific alarm.

#### 1.3.3. Heating Mode (`EM-300`)

*   **Activation:** The system will enable the heating stage if **ALL** of the following conditions are true:
    1.  There is a demand for heating (DAT is below setpoint).
    2.  The high-temperature limit safety is in a normal state.
*   **Operation:** The single-stage heating element is commanded ON and will run until the heating demand is met.
*   **Safety Trip:** A trip on the High-Temperature Limit input will immediately de-energize the heating element and generate a **"High-Temperature Limit Fault"** alarm.

### 1.4. Ventilation Control (`EM-400`)

*   Whenever the supply fan is running and the unit is not in Economizer mode, the outside air damper will be modulated to a minimum position (e.g., 20%) to ensure the space receives adequate fresh air.

### 1.5. System Monitoring & Alarms (`EM-500`)

*   **Dirty Filter:** A "Dirty Filter" alarm is generated if the filter differential pressure switch is active for more than 10 seconds. This is a maintenance alarm and does not shut down the unit.
*   **Safety Alarms:** All safety alarms (Fan Failure, VFD Fault, HP/LP Fault, Freeze Stat, High-Limit) are critical and will shut down the associated equipment.

## 2. Functional Test Procedure

This procedure is to be used by a commissioning technician to verify the correct operation of the RTU controller.

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
| 4 | Clear alarms. Set DAT Setpoint to 75°F. Force the DAT reading to 65°F. | Heating command turns ON. |
| 5 | Simulate a **High-Temperature Limit** fault. | Heating command turns OFF immediately. "High-Temperature Limit Fault" alarm appears. |
| **Cooling Control** |
| 6 | Clear alarms. Set DAT Setpoint to 55°F. Force DAT to 65°F. Force OAT to 80°F (to disable economizer). | After the 3-minute off-time delay, the Compressor command turns ON. |
| 7 | While running, simulate a **High-Pressure Switch** fault. | Compressor command turns OFF immediately. "High Pressure Fault" alarm appears. |
| **Economizer Control** |
| 8 | Clear alarms. Set DAT Setpoint to 55°F. Force DAT to 65°F. Force OAT to 50°F and RAT to 75°F. | The system enters Economizer mode. **The Compressor command must remain OFF.** The Damper command modulates to control the DAT. |
| 9 | Force OAT to 70°F (above the high-limit). | Economizer mode exits. The damper returns to its minimum position. The Compressor command turns ON (after its time delay). |
| **System Monitoring** |
| 10 | Simulate a **Dirty Filter Status** input. | After 10 seconds, a "Dirty Filter" alarm appears. The unit continues to run. |
