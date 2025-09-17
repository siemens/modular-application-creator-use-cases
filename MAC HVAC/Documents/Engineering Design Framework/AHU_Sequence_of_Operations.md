# Sequence of Operations & Functional Test Procedure

**Project:** Constant Volume Air Handler Unit (AHU) Controller
**Reference:** Software Design Specification, Version 2.0

## 1. Sequence of Operations (SOO)

### 1.1. General Operation & Modes
The AHU will operate in one of three modes: Off, Occupied, or Unoccupied, determined by a schedule or BAS command.
*   **Occupied Mode:** The supply fan runs continuously to provide ventilation and the system maintains the occupied DAT setpoint.
*   **Unoccupied Mode:** The fan cycles on only when there is a demand for heating or cooling to maintain wider temperature setpoints.

### 1.2. Supply Fan Control
The Supply Fan (`EM-100`) is commanded to start in Occupied mode or during a call for heat/cool in Unoccupied mode. The system monitors run feedback and airflow status, generating a "Fan Failure" or "VFD Fault" alarm if proof of operation is not established.

### 1.3. Temperature Control
A single bipolar PID loop (`TO_PID_DAT_Control`) manages all heating and cooling.

#### 1.3.1. Economizer / Free Cooling Mode (`EM-400`)
*   **Activation:** Occurs on a call for cooling if Outside Air Temperature (OAT) is favorable compared to Return Air Temperature (RAT).
*   **Operation:**
    *   Mechanical cooling (`EM-200`) is locked out.
    *   A dedicated economizer PID (`TO_PID_Econ_Control`) modulates the outside air damper to maintain the DAT setpoint.
    *   **Feedback Monitoring:** The damper command is continuously compared to the `Damper Position Fdbk` signal. If the command and feedback differ by more than a configurable tolerance (e.g., 10%) for a set delay (e.g., 60 seconds), a **"Damper Failure"** alarm is generated.

#### 1.3.2. Chilled Water Cooling Mode (`EM-200`)
*   **Activation:** Enabled when there is a cooling demand and the system is not in Economizer mode.
*   **Operation:**
    *   The positive output of the `TO_PID_DAT_Control` (0-100%) modulates the `CHW_Valve_Cmd_AO`.
    *   **Feedback Monitoring:** The valve command is continuously compared to the `Chilled Water Valve Fdbk` signal. If the command and feedback differ by more than a configurable tolerance (e.g., 10%) for a set delay (e.g., 60 seconds), a **"Chilled Water Valve Failure"** alarm is generated.
*   **Safety Trip:** A `CHW_Freeze_Stat_DI` trip immediately closes the valve and generates a **"Chilled Water Freeze Alarm"**.

#### 1.3.3. Hot Water Heating Mode (`EM-300`)
*   **Activation:** Enabled when there is a heating demand.
*   **Operation:**
    *   The negative output of the `TO_PID_DAT_Control` (-100% to 0) is inverted and scaled to modulate the `HW_Valve_Cmd_AO`.
    *   **Feedback Monitoring:** The valve command is continuously compared to the `Hot Water Valve Fdbk` signal. If the command and feedback differ by more than a configurable tolerance (e.g., 10%) for a set delay (e.g., 60 seconds), a **"Hot Water Valve Failure"** alarm is generated.
*   **Safety Trip:** A `HW_Freeze_Stat_DI` trip immediately closes the valve and generates a **"Hot Water Freeze Alarm"**.

### 1.4. System Monitoring & Alarms (`EM-500`)
A "Dirty Filter" maintenance alarm is generated if the filter differential pressure switch is active for a configurable delay. All other failure and safety alarms are critical and will shut down associated equipment.

## 2. Functional Test Procedure
*(Use the HMI or TIA Portal to override values and observe results)*

| Step | Action | Expected Result |
| :--- | :--- | :--- |
| **Fan Control** |
| 1 | Command unit to **Occupied** mode. | Supply Fan starts. |
| 2 | While fan is running, disable **Run Feedback**. | After 5s, "Fan Failure" alarm appears. |
| **Heating Control** |
| 3 | Set DAT Setpoint to 75°F. Force DAT to 65°F. | `HW_Valve_Cmd_AO` modulates open. `HW_Valve_Fdbk` should track the command. |
| 4 | While heating, force `HW_Valve_Fdbk` to 0%. | After 60s, a "Hot Water Valve Failure" alarm appears. |
| 5 | Simulate a **HW Freeze Stat** fault. | `HW_Valve_Cmd_AO` goes to 0 immediately. A "Hot Water Freeze Alarm" appears. |
| **Cooling Control** |
| 6 | Set DAT Setpoint to 55°F. Force DAT to 65°F. Force OAT to 80°F. | `CHW_Valve_Cmd_AO` modulates open. `CHW_Valve_Fdbk` should track the command. |
| 7 | While cooling, force `CHW_Valve_Fdbk` to 0%. | After 60s, a "Chilled Water Valve Failure" alarm appears. |
| 8 | Simulate a **CHW Freeze Stat** fault. | `CHW_Valve_Cmd_AO` goes to 0 immediately. A "Chilled Water Freeze Alarm" appears. |
| **Economizer Control** |
| 9 | Set DAT Setpoint to 55°F. Force DAT to 65°F. Force OAT to 50°F. | System enters Economizer mode. Damper command modulates open. `Damper Position Fdbk` should track. |
| 10 | While in econ mode, force `Damper Position Fdbk` to 0%. | After 60s, a "Damper Failure" alarm appears. |
| **System Monitoring** |
| 11 | Simulate a **Dirty Filter Status** input. | After 10s, a "Dirty Filter" alarm appears. Unit continues to run. |
