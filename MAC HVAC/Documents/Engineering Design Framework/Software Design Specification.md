# **Software Design Specification (SDS)**

## **Constant Volume Air Handler Unit (AHU) Controller**

Project: Constant Volume AHU Controller
PLC Platform: Siemens TIA Portal V20
Target CPU: Siemens S7-1500 Family  
Author: Dallas Levine
Date: September 15, 2025
Version: 2.1

### **1\. System Architecture and Hardware Specification**

This section defines the core hardware components and the software architecture for the AHU controller. The design emphasizes a centralized control strategy using a single Siemens S7-1500 PLC.

#### **1.1. PLC Hardware**

* **Controller:** Siemens SIMATIC S7-1500, **CPU 1511C-1 PN**.  
  * **Rationale:** The compact model provides an excellent balance of performance and value. Its integrated I/O is supplemented by an analog output module to meet all project requirements.

#### **1.2. I/O Signal Modules (SMs)**

Based on the required I/O points from the Equipment Modules, the integrated I/O of the CPU will be supplemented by one analog output signal module.

* **CPU Integrated I/O:**
  * 16x Digital Inputs (DI)  
  * 16x Digital Outputs (DO)  
  * 5x Analog Inputs (AI)  
  * 2x Analog Outputs (AO)
* **Additional Signal Modules:**
  * 1x **SM 532, AQ 4xU/I ST** (4x Analog Outputs)
* **Total Project I/O Requirements:**  
  * 6 Digital Inputs
  * 1 Digital Output
  * 5 Analog Inputs
  * 4 Analog Outputs
* **Conclusion:** The CPU 1511C-1 PN's integrated I/O (which includes 5 AIs) combined with one 4-channel analog output module is sufficient for this application. No additional analog input module is required.

#### **1.3. Technology Objects (TOs)**

To ensure robust and efficient process control, a combination of PID Technology Objects will be used. The main temperature control will use the more advanced `PID_Temp` object for its bipolar output capabilities, while the economizer will use the simpler `PID_Compact` for its single-direction control.

* **Instance 1: `TO_PID_DAT_Control`**
  * **Purpose:** Main Discharge Air Temperature (DAT) Control. This is the primary PID loop responsible for maintaining the DAT setpoint by enabling mechanical heating or cooling.
  * **Process Variable (PV):** `AHU1_DAT_Temp` (Discharge Air Temperature).
  * **Setpoint (SP):** The active DAT Setpoint (either Occupied or Unoccupied).
  * **Manipulated Variable (MV):** The output of this PID will be mapped to a bipolar range (e.g., -100% to +100%). Negative output signifies a demand for heating, and positive output signifies a demand for cooling. This demand signal will enable `EM-200` (Cooling) or `EM-300` (Heating).
  * **Tuning Considerations:** To ensure stable control of the slow-reacting water valves, the PID's integral time (`Ti`) should be set to a relatively high value. This will prevent rapid oscillation of the valve commands. Derivative action (`Td`) is likely not required for this thermal process.

* **Instance 2: `TO_PID_Econ_Control`**
  * **Purpose:** Economizer Free Cooling Control. This loop is active only when the system is in Economizer Mode.
  * **Process Variable (PV):** `AHU1_DAT_Temp` (Discharge Air Temperature).
  * **Setpoint (SP):** The active Cooling Setpoint.
  * **Manipulated Variable (MV):** The output (0-100%) directly controls the `Damper Position Command` of `EM-400` to modulate the amount of free cooling provided by outside air.

### **2\. Detailed Equipment Module (EM) Specifications**

The control logic is segmented into the following Equipment Modules (EMs). Each EM is a self-contained functional block designed for use in the Modular Application Creator (MAC).

#### **EM-100: Supply Fan Control**

* **Purpose:** To control and monitor the Variable Frequency Drive (VFD) for the main supply fan.  
* **Logic:**  
  * Accepts a digital Start/Stop command and an analog speed command (0-100%).  
  * Monitors VFD Run Feedback, VFD Fault status, and Airflow Switch status.  
  * A "Fan Failure" alarm is generated if the RunFeedback\_DI is not received within a configurable StartDelay (e.g., 5 seconds) of the Start\_DO being true, OR if the AirflowSw\_DI is not made when the fan is confirmed running.  
* **Parameter Set:**
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Start/Stop Command | Digital | Output | AHU1\_SF\_StartCmd |
  | Speed Reference | Analog | Output | AHU1\_SF\_SpeedRef |
  | Run Feedback/Status | Digital | Input | AHU1\_SF\_RunFdbk |
  | Airflow Switch Status | Digital | Input | AHU1\_SF\_AirflowSw |
  | VFD Fault Status | Digital | Input | AHU1\_SF\_VfdFault |

#### **EM-200: Cooling Control (Chilled Water)**

* **Purpose:** To control a modulating chilled water valve for cooling the discharge air.
* **Logic:**  
  * Accepts an analog cooling demand (0-100%) from the main PID controller.
  * Scales the demand signal to the `CHW_Valve_Cmd_AO` analog output.
  * Monitors a `CHW_Freeze_Stat_DI` on the coil. If the freeze stat trips, the valve is commanded fully closed (0%) and a `CHW_Freeze_Alm` is generated.
* **Parameter Set:**
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Chilled Water Valve Cmd | Analog | Output | AHU1\_CW\_VlvCmd |
  | Chilled Water Freeze Stat | Digital | Input | AHU1\_CW\_FreezeStat |
  | Chilled Water Valve Fdbk | Analog | Input | AHU1\_CW\_VlvFdbk |

#### **EM-300: Heating Control (Hot Water)**

* **Purpose:** To control a modulating hot water valve for heating the discharge air.
* **Logic:**  
  * Accepts an analog heating demand (0-100%) from the main PID controller.
  * Scales the demand signal to the `HW_Valve_Cmd_AO` analog output.
  * Monitors a `HW_Freeze_Stat_DI` on the coil. If the freeze stat trips, the valve is commanded fully closed (0%) and a `HW_Freeze_Alm` is generated.
* **Parameter Set:**
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Hot Water Valve Cmd | Analog | Output | AHU1\_HW\_VlvCmd |
  | Hot Water Freeze Stat | Digital | Input | AHU1\_HW\_FreezeStat |
  | Hot Water Valve Fdbk | Analog | Input | AHU1\_HW\_VlvFdbk |

#### **EM-400: Damper/Economizer Control**

* **Purpose:** To manage the fresh, return, and exhaust air dampers for ventilation and economizer-based free cooling.  
* **Logic:**  
  * In all occupied modes, the damper modulates to a MinFreshAirPosition setpoint (e.g., 20%) to ensure proper ventilation.  
  * "Economizer Mode" is enabled if Outside Air Temperature is lower than Return Air Temperature by a configurable differential (e.g., 2°C) and there is a call for cooling.  
  * In Economizer Mode, the `TO_PID_Econ_Control` TO output modulates the damper actuator to maintain the Discharge Air Temperature Setpoint.
* **Parameter Set:**
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Damper Position Cmd | Analog | Output | AHU1\_DMP\_PosCmd |
  | Return Air Temp | Analog | Input | AHU1\_RAT\_Temp |
  | Outside Air Temp | Analog | Input | AHU1\_OAT\_Temp |
  | Discharge Air Temp | Analog | Input | AHU1\_DAT\_Temp |

#### **EM-500: System Monitoring**

* **Purpose:** To monitor system-wide components not tied to a specific process.  
* **Logic:**  
  * Monitors a differential pressure switch across the air filters.  
  * If the switch is made for a configurable delay (e.g., 10 seconds), a "Dirty Filter" alarm is generated. This is a maintenance alert and does not shut down the unit.  
* **Parameter Set:**
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Dirty Filter Status | Digital | Input | AHU1\_SYS\_DirtyFilter |

### **3\. Main Control Program (OB1 Logic)**

The main program logic, executed in OB1, coordinates the EMs based on the unit's operating mode and thermal demand.

#### **3.1. Modes of Operation**

* **Off:** The unit is completely shut down. All outputs are de-energized.  
* **Occupied:** The unit is active. The Supply Fan runs continuously, and the system will heat or cool as needed to maintain the occupied temperature setpoints.
* **Unoccupied:** The unit operates in a setback/setup mode. The fan runs only when there is a demand for heating or cooling to maintain wider unoccupied temperature setpoints.

#### **3.2. Control Sequence**

1. **Mode Determination:** The system state (Off, Occupied, Unoccupied) is determined by a schedule or a Building Automation System (BAS) command.  
2. **Fan Control:**  
   * In Occupied mode, EM-100 is commanded to start.  
   * In Unoccupied mode, EM-100 is commanded to start only when there is a call for heat or cool.  
3. **Demand Calculation:** The `TO_PID_DAT_Control` PID determines the operating state (Heating, Cooling, Deadband).
4. **Cooling Logic:**  
   * If the `TO_PID_DAT_Control` output is positive (indicating a cooling demand), the system evaluates the cooling strategy.
   * The system first attempts to cool using the EM-400 economizer. If conditions are favorable, the `TO_PID_Econ_Control` is enabled to modulate the damper.
   * If economizer cooling is not available or insufficient, the positive output of `TO_PID_DAT_Control` (0-100%) is passed to EM-200 to modulate the chilled water valve.
5. **Heating Logic:**  
   * If the `TO_PID_DAT_Control` output is negative (indicating a heating demand), the inverted output (0-100%) is passed to EM-300 to modulate the hot water valve.
6. **Safety Interlocks:** All EM outputs are interlocked with the EM-100 fan status. No heating or cooling can occur unless the fan is proven to be running via the Airflow Switch.

### **4\. HMI Strategy and Library Recommendation**

To ensure consistency and rapid development, a standardized HMI library is required.

#### **4.1. HMI Library Recommendation**

* **Library:** **Library of General Functions (LGF)**
* **Justification:** The LGF is selected to align with the project's software bill of materials and to maintain an open, flexible, and cost-effective design. As a free and universally available library from Siemens, the LGF provides a comprehensive set of basic functions (e.g., scaling, timers, signal processing) that can be used to build robust control logic without introducing licensing costs or dependencies on more complex, feature-heavy libraries. This approach gives developers the flexibility to create highly customized HMI screens tailored specifically to the AHU application, rather than being constrained by pre-defined faceplates.

#### **4.2. Key HMI Screens**

* **Main Overview:** A graphical P&ID-style screen showing the entire AHU. It will display the live status of the fan, hot water valve position, and chilled water valve position, along with real-time temperatures (OA, RA, DA) and damper position.
* **Alarms Screen:** A standard alarm viewer listing all active and historical alarms with timestamps, descriptions, and acknowledgment status.  
* **Settings Screen:** A password-protected screen for authorized personnel to adjust key operational parameters, including:  
  * Occupied/Unoccupied Setpoints  
  * PID Tuning Parameters (P, I, D)  
  * Configurable Time Delays (fan failure, etc.)
* **Trend Screen:** A multi-pen trend view for diagnostics and performance monitoring. Key trends will include:  
  * Discharge Air Temperature  
  * Return Air Temperature  
  * Outside Air Temperature  
  * Active Setpoint  
  * Damper Position Command
  * HW Valve Position Command
  * CW Valve Position Command

### **5\. Version Control and Automated Testing Strategy**

To align with modern development practices, a robust version control and automated testing pipeline will be implemented.

#### **5.1. Version Control**

* **System:** Git  
* **Platform:** GitHub  
* **Methodology:** The TIA Portal project will be managed using the siemens/tia-project-exporter tool. This utility exports project components like code blocks (FCs, FBs) and data types (UDTs) into a human-readable XML format. This allows for effective line-by-line change tracking and merging within Git.

#### **5.2. Automated Unit Testing (Continuous Integration)**

A GitHub Actions workflow will be created to automatically test Equipment Modules upon a push to a development or main branch.

* **Trigger:** on: push for branches: \[ develop, main \]  
* **Workflow:**  
  1. The job starts on a self-hosted runner with TIA Portal and PLCSIM Advanced installed.  
  2. The workflow checks out the repository code.  
  3. A Python script using the plcsim-adv-api library is executed.  
  4. The script programmatically starts a PLCSIM Advanced virtual PLC instance.  
  5. It downloads the latest compiled project code to the virtual PLC.  
  6. A series of test cases are executed against each EM. For example, for EM-100, it will:  
     * Force the Start\_DO tag high.  
     * Wait for the configured StartDelay.  
     * Verify the Fan\_Failure alarm is NOT active.  
     * Force the RunFeedback\_DI low and verify the Fan\_Failure alarm becomes active.  
  7. The script concludes and reports a pass/fail status.  
  8. The GitHub Action run will reflect this status, blocking faulty code from being merged into the main branch.

### **6\. Glossary & FAQ**

#### **6.1. Glossary of Acronyms**
*   **PLC:** Programmable Logic Controller
*   **AHU:** Air Handler Unit
*   **EM:** Equipment Module
*   **FB:** Function Block
*   **UDT:** User Data Type (a structured data type)
*   **TIA Portal:** Totally Integrated Automation Portal (Siemens engineering software)
*   **SCL:** Structured Control Language (a high-level programming language for PLCs)
*   **VFD:** Variable Frequency Drive
*   **I/O:** Input/Output

#### **6.2. Frequently Asked Questions (FAQ)**

*   **Q: Why are the generated source files in XML format?**
    *   **A:** The XML format is used by the TIA Portal Openness interface. It provides a text-based, human-readable representation of PLC blocks that is ideal for version control systems like Git and allows for automated generation.

*   **Q: What is the correct order to import the XML files into TIA Portal?**
    *   **A:** The import order is critical due to dependencies. The correct order is:
        1.  User Data Types (`UDT*.xml`)
        2.  PLC Tag Tables (`TagTable_*.xml`)
        3.  Function Blocks (`FB*.xml`)

*   **Q: Why are the Function Blocks (FBs) missing their internal SCL logic?**
    *   **A:** The project is being developed using a 'breadth-first' approach. We first create the entire structure (all UDTs, Tag Tables, and FB interfaces) to ensure the framework is solid. The next phase of work will be to populate these FB skeletons with the detailed control logic.