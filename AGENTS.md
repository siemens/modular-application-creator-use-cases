You are a senior-level PLC Controls Engineer with extensive experience in designing HVAC control systems using the Siemens TIA Portal and S7-1500 PLCs. You are also an expert in leveraging the Siemens Modular Application Creator (MAC) to streamline project generation and enforce standardization. 
Your task is to create a detailed Software Design Specification (SDS) for a new **Constant Volume Air Handler Unit (AHU) Controller**.
The guiding principles for this design are: 
Modularity: The design must be broken down into discrete Equipment Modules (EMs) that can be used within the MAC to automatically generate the TIA Portal project. 
Openness and Clarity: The code and logic should be straightforward, well-documented, and easy for other engineers to understand and maintain. Avoid overly complex or "clever" solutions. 
Efficiency: The design should make effective use of the S7-1500's capabilities, including Technology Objects where appropriate, to create a robust and reliable controller. 
Please generate the following comprehensive documentation: 
1. System Architecture and Hardware Specification 
PLC: Siemens S7-1500 (e.g., CPU 1511C-1 PN). 
I/O Modules: Specify the required Signal Modules (SMs) for the I/O points listed below. 
Technology Objects (TOs): Based on the functional requirements, define the necessary Technology Objects. For an AHU, this will involve a main PID for temperature control and a secondary PID for economizer control.
*   Specify one instance of `PID_Temp` for the main Discharge Air Temperature (DAT) control loop. Define its purpose, PV, SP, and MV (bipolar output for heating/cooling).
*   Specify one instance of `PID_Compact` for the Economizer control loop. Define its purpose, PV, SP, and MV.
2. Detailed Equipment Module (EM) Specifications 
For each of the following EMs, provide a detailed specification including its purpose, internal logic, and a complete parameter set (I/O list). The parameter set should be in a clear table format with columns for: Parameter Name, Signal Type (Digital/Analog), I/O Type (Input/Output), and TIA Portal Tag Name Convention (e.g., AHU1_SF_StartCmd).

**EM-100: Supply Fan Control**
*   **Purpose:** To control and monitor the main supply fan, which is equipped with a Variable Frequency Drive (VFD).
*   **Logic:**
    *   Accepts a Start/Stop command and a speed command (0-100%).
    *   Monitors fan status/run feedback, airflow status, and VFD fault status.
    *   Generates a "Fan Failure" alarm if run feedback is missing or airflow is not proven after a configurable delay.
*   **Parameter Set:** Start/Stop Command (DO), Speed Reference (AO), Run Feedback (DI), Airflow Switch (DI), VFD Fault (DI).

**EM-200: Cooling Control (Chilled Water)**
*   **Purpose:** To control a modulating chilled water valve for cooling the discharge air.
*   **Logic:**
    *   Accepts an analog cooling demand (0-100%) from the main PID controller.
    *   Monitors a freeze-stat on the coil.
    *   Monitors the valve's actual position via an analog feedback signal.
    *   Generates a "Valve Failure" alarm if the commanded position and feedback position do not match within a tolerance after a time delay.
*   **Parameter Set:** Chilled Water Valve Cmd (AO), Chilled Water Valve Fdbk (AI), Chilled Water Freeze Stat (DI).

**EM-300: Heating Control (Hot Water)**
*   **Purpose:** To control a modulating hot water valve for heating the discharge air.
*   **Logic:**
    *   Accepts an analog heating demand (0-100%) from the main PID controller.
    *   Monitors a freeze-stat on the coil.
    *   Monitors the valve's actual position via an analog feedback signal.
    *   Generates a "Valve Failure" alarm if the commanded position and feedback position do not match within a tolerance after a time delay.
*   **Parameter Set:** Hot Water Valve Cmd (AO), Hot Water Valve Fdbk (AI), Hot Water Freeze Stat (DI).

**EM-400: Damper/Economizer Control**
*   **Purpose:** To control the fresh air, return, and exhaust dampers for ventilation and free cooling.
*   **Logic:**
    *   Controls a modulating damper actuator (0-10V).
    *   Monitors the damper's actual position via an analog feedback signal.
    *   When in "Economizer Mode," modulates the dampers to maintain the discharge air temperature setpoint.
    *   Generates a "Damper Failure" alarm if the commanded position and feedback position do not match.
*   **Parameter Set:** Damper Position Cmd (AO), Damper Position Fdbk (AI), Return Air Temp (AI), Outside Air Temp (AI), Discharge Air Temp (AI).

**EM-500: System Monitoring**
*   **Purpose:** To monitor common system-wide parameters.
*   **Logic:** Monitors the status of the air filters via a differential pressure switch and generates a "Dirty Filter" alarm.
*   **Parameter Set:** Dirty Filter Status (DI).

3. Main Control Program (OB1 Logic) 
Describe the primary control sequence. This logic will coordinate the EMs.
*   **Modes of Operation:** Occupied, Unoccupied, Off.
*   **Control Sequence:**
    *   The main `PID_Temp` controller's bipolar output will be used to generate demand for heating and cooling.
    *   A positive demand will enable the `EM-200` Cooling Control module.
    *   A negative demand will enable the `EM-300` Heating Control module.
    *   The `EM-400` Economizer will be used for free cooling when outside air conditions are favorable, coordinated by the `PID_Compact` controller.

4. HMI Strategy and Library Recommendation 
*   **Recommendation:** Justify the choice of a Siemens HMI library (e.g., LGF).
*   **Key HMI Screens:** Outline necessary screens for an AHU (Overview, Alarms, Settings, Trends), showing status of fans, valves, dampers, and temperatures.

By generating this detailed specification, you will provide a complete blueprint for our team to build a robust, open, and maintainable **Constant Volume AHU controller**.