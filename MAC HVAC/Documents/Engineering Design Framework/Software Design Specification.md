# **Software Design Specification (SDS)**

## **Basic Rooftop HVAC Unit (RTU) Controller**

Project: Basic RTU Controller  
PLC Platform: Siemens TIA Portal V18  
Target CPU: Siemens S7-1500 Family  
Author: Senior Controls Engineer  
Date: September 12, 2025  
Version: 1.1

### **1\. System Architecture and Hardware Specification**

This section defines the core hardware components and the software architecture for the RTU controller. The design emphasizes a centralized control strategy using a single Siemens S7-1500 PLC.

#### **1.1. PLC Hardware**

* **Controller:** Siemens SIMATIC S7-1500, **CPU 1511C-1 PN**.  
  * **Rationale:** The compact model provides an excellent balance of performance and value. It includes integrated I/O points suitable for a basic RTU, with processing power to spare for future enhancements.

#### **1.2. I/O Signal Modules (SMs)**

Based on the required I/O points from the Equipment Modules, the following hardware is specified. The integrated I/O of the 1511C will be utilized, supplemented by one signal module for analog I/O.

* **Integrated Digital I/O (on CPU):**  
  * 16x Digital Inputs (DI)  
  * 16x Digital Outputs (DO)  
* **Integrated Analog I/O (on CPU):**  
  * 5x Analog Inputs (AI)  
  * 2x Analog Outputs (AO)  
* **Total Project I/O Requirements:**  
  * 8 Digital Inputs  
  * 3 Digital Outputs  
  * 3 Analog Inputs  
  * 2 Analog Outputs  
* **Conclusion:** The CPU 1511C-1 PN's integrated I/O is sufficient for this application, providing room for future expansion without additional hardware.

#### **1.3. Technology Objects (TOs)**

To ensure robust and efficient process control, the PID\_Compact Technology Object will be used for temperature regulation.

* **Instance 1: PID\_Cooling**  
  * **Purpose:** Discharge Air Temperature Control (Cooling Mode). This loop modulates the economizer dampers and enables the DX cooling stage.  
  * **Process Variable (PV):** RTU1\_DAT\_Temp (Discharge Air Temperature).  
  * **Setpoint (SP):** RTU1\_Cooling\_Sp (Cooling Setpoint).  
  * **Manipulated Variable (MV):** The PID output will be directed to the economizer damper command and the cooling stage enable.  
* **Instance 2: PID\_Heating**  
  * **Purpose:** Discharge Air Temperature Control (Heating Mode).  
  * **Process Variable (PV):** RTU1\_DAT\_Temp (Discharge Air Temperature).  
  * **Setpoint (SP):** RTU1\_Heating\_Sp (Heating Setpoint).  
  * **Manipulated Variable (MV):** The PID output will enable the heating stage.

### **2\. Detailed Equipment Module (EM) Specifications**

The control logic is segmented into the following Equipment Modules (EMs). Each EM is a self-contained functional block designed for use in the Modular Application Creator (MAC).

#### **EM-100: Supply Fan Control**

* **Purpose:** To control and monitor the Variable Frequency Drive (VFD) for the main supply fan.  
* **Logic:**  
  * Accepts a digital Start/Stop command and an analog speed command (0-100%).  
  * Monitors VFD Run Feedback, VFD Fault status, and Airflow Switch status.  
  * A "Fan Failure" alarm is generated if the RunFeedback\_DI is not received within a configurable StartDelay (e.g., 5 seconds) of the Start\_DO being true, OR if the AirflowSw\_DI is not made when the fan is confirmed running.  
* Parameter Set:  
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Start/Stop Command | Digital | Output | RTU1\_SF\_StartCmd |  
  | Speed Reference | Analog | Output | RTU1\_SF\_SpeedRef |  
  | Run Feedback/Status | Digital | Input | RTU1\_SF\_RunFdbk |  
  | Airflow Switch Status | Digital | Input | RTU1\_SF\_AirflowSw |  
  | VFD Fault Status | Digital | Input | RTU1\_SF\_VfdFault |

#### **EM-200: Cooling Control (Single-Stage DX)**

* **Purpose:** To control and monitor a single-stage direct expansion (DX) cooling compressor and its safeties.  
* **Logic:**  
  * Accepts a cooling enable command from the main program logic.  
  * Monitors High-Pressure, Low-Pressure, and Freeze Stat safety switches. A trip on any safety generates a specific alarm and locks out the compressor until reset.  
  * Implements configurable minimum run-time and minimum off-time delays (e.g., 3 minutes) to prevent compressor short-cycling. The compressor command will not be turned off until the min run-time is met, and will not be turned on until the min off-time is met.  
* Parameter Set:  
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Compressor Stage 1 Cmd | Digital | Output | RTU1\_C\_Comp1Cmd |  
  | High-Pressure Switch | Digital | Input | RTU1\_C\_HighPressSw |  
  | Low-Pressure Switch | Digital | Input | RTU1\_C\_LowPressSw |  
  | Freeze Stat | Digital | Input | RTU1\_C\_FreezeStat |

#### **EM-300: Heating Control (Single-Stage Gas/Electric)**

* **Purpose:** To control a single-stage heating element and its primary safety.  
* **Logic:**  
  * Accepts a heating enable command from the main program logic.  
  * Monitors a High-Temperature Limit switch. If the switch trips, the module generates a "High-Temperature Limit Fault" alarm and locks out the heating stage until reset.  
* Parameter Set:  
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Heating Stage 1 Cmd | Digital | Output | RTU1\_H\_Heat1Cmd |  
  | High-Temp Limit Sw | Digital | Input | RTU1\_H\_HighTempSw |

#### **EM-400: Damper/Economizer Control**

* **Purpose:** To manage the fresh, return, and exhaust air dampers for ventilation and economizer-based free cooling.  
* **Logic:**  
  * In all occupied modes, the damper modulates to a MinFreshAirPosition setpoint (e.g., 20%) to ensure proper ventilation.  
  * "Economizer Mode" is enabled if Outside Air Temperature is lower than Return Air Temperature by a configurable differential (e.g., 2°C) and there is a call for cooling.  
  * In Economizer Mode, the PID\_Cooling TO output modulates the damper actuator to maintain the Discharge Air Temperature Setpoint.  
* Parameter Set:  
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Damper Position Cmd | Analog | Output | RTU1\_DMP\_PosCmd |  
  | Return Air Temp | Analog | Input | RTU1\_RAT\_Temp |  
  | Outside Air Temp | Analog | Input | RTU1\_OAT\_Temp |  
  | Discharge Air Temp | Analog | Input | RTU1\_DAT\_Temp |

#### **EM-500: System Monitoring**

* **Purpose:** To monitor system-wide components not tied to a specific process.  
* **Logic:**  
  * Monitors a differential pressure switch across the air filters.  
  * If the switch is made for a configurable delay (e.g., 10 seconds), a "Dirty Filter" alarm is generated. This is a maintenance alert and does not shut down the unit.  
* Parameter Set:  
  | Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |  
  | :--- | :--- | :--- | :--- |  
  | Dirty Filter Status | Digital | Input | RTU1\_SYS\_DirtyFilter |

### **3\. TIA Portal Project and Code Standards**

This section defines the mandatory software structure and standards to ensure consistency and maintainability, in alignment with the "Programming Best Practices & Design Framework."

#### **3.1. TIA Portal Project Structure**

All project elements shall be organized into the following logical groups within the TIA Portal project tree:

* **Program Blocks:**
  * `01_Main`: Contains OB1 and the main program cycle calls.
  * `02_Equipment_Modules`: Contains all Equipment Module FBs.
  * `03_Functions`: Contains all reusable FCs.
  * `04_Technology_Objects`: Contains the PID controller blocks (PID_Compact).
  * `05_Data_Blocks`: Contains all global DBs.
* **PLC Data Types (UDTs):**
  * All UDTs will be organized in a folder named `UDTs`.
* **PLC Tags:**
  * Organized into specific tag tables (e.g., `DI_Tags`, `DO_Tags`, `AI_Tags`, `AO_Tags`).

#### **3.2. Code Implementation Standards**

* **Guiding Principle:** "Openness and Clarity." Code should be written to be easily understood by another engineer.
* **Language Choice:**
  * **Structured Control Language (SCL):** **Preferred language** for all complex logic, state machines, and data manipulation.
  * **Ladder Logic (LAD):** To be used **only** for simple, visually intuitive boolean logic, such as safety chains or basic permissive circuits.
* **Naming Conventions:**
  * **Function Blocks (FBs):** `FB_EM<Number>_<Name>` (e.g., `FB_EM100_SupplyFan`).
  * **Functions (FCs):** `FC_<Verb><Noun>` (e.g., `FC_ScaleAnalogInput`).
  * **Data Blocks (DBs):** Global DBs: `DB_<Purpose>` (e.g., `DB_SystemSettings`). Instance DBs: `iDB_EM<Number>_<Name>` (e.g., `iDB_EM100_SupplyFan`).
  * **User-Defined Types (UDTs):** `UDT_EM<Number>_Interface` (e.g., `UDT_EM100_Interface`).
* **Addressing and Data:**
    * **100% Symbolic Addressing:** No absolute addressing is permitted within application blocks.
    * **No "Magic Numbers":** Configurable values like setpoints and delays must be defined as input parameters or sourced from a global settings DB.

#### **3.3. Global Configuration Data Block**
All system-wide configurable parameters that are not specific to a single piece of equipment are stored in a central data block.

* **DB Name:** `DB_SystemSettings`
* **Purpose:** To provide a single source for tuning and configuration values, accessible by all program blocks.
* **Parameter Set:**
  | Parameter Name | Data Type | Default | Description |
  | :--- | :--- | :--- | :--- |
  | `OccupiedCoolingSetpoint` | Real | 24.0 | Occupied mode cooling setpoint (°C) |
  | `OccupiedHeatingSetpoint` | Real | 21.0 | Occupied mode heating setpoint (°C) |
  | `UnoccupiedCoolingSetpoint` | Real | 28.0 | Unoccupied mode cooling setpoint (°C) |
  | `UnoccupiedHeatingSetpoint` | Real | 18.0 | Unoccupied mode heating setpoint (°C) |
  | `FanFailureDelay` | Time | T#5s | Delay before a fan failure is triggered. |
  | `CompressorMinRunTime` | Time | T#3m | Minimum compressor run time to prevent short-cycling. |
  | `CompressorMinOffTime` | Time | T#3m | Minimum compressor off time to prevent short-cycling. |
  | `DirtyFilterDelay` | Time | T#10s | Delay before a dirty filter alarm is triggered. |
  | `MinFreshAirPosition` | Real | 20.0 | Minimum fresh air damper position in occupied mode (%). |
  | `EconomizerTempDifferential`| Real | 2.0 | Temperature difference (OAT vs RAT) to enable economizer. |

### **4\. Main Control Program (OB1 Logic)**

The main program logic, executed in OB1, coordinates the EMs based on the unit's operating mode and thermal demand.

#### **4.1. Modes of Operation**

* **Off:** The unit is completely shut down. All outputs are de-energized.  
* **Occupied:** The unit is active. The Supply Fan runs continuously at a minimum speed, and the system will heat or cool as needed to maintain the occupied temperature setpoints.  
* **Unoccupied:** The unit operates in a setback/setup mode. The fan runs only when there is a demand for heating or cooling to maintain wider unoccupied temperature setpoints.

#### **4.2. Control Sequence**

1. **Mode Determination:** The system state (Off, Occupied, Unoccupied) is determined by a schedule or a Building Automation System (BAS) command.  
2. **Fan Control:**  
   * In Occupied mode, EM-100 is commanded to start.  
   * In Unoccupied mode, EM-100 is commanded to start only when there is a call for heat or cool.  
3. **Demand Calculation:** The system compares the Discharge Air Temperature to the active heating and cooling setpoints to determine the operating state (Heating, Cooling, Deadband).  
4. **Cooling Logic:**  
   * If a call for cooling exists, the PID\_Cooling TO is enabled.  
   * The system first attempts to cool using the EM-400 economizer if conditions are favorable.  
   * If economizer cooling is insufficient, EM-200 (DX Cooling) is enabled.  
5. **Heating Logic:**  
   * If a call for heating exists, the PID\_Heating TO is enabled.  
   * The TO's output enables EM-300 (Heating Control).  
6. **Safety Interlocks:** All EM outputs are interlocked with the EM-100 fan status. No heating or cooling can occur unless the fan is proven to be running via the Airflow Switch.

### **5\. HMI Strategy and Library Recommendation**

To ensure consistency and rapid development, a standardized HMI library is required.

#### **5.1. HMI Library Recommendation**

* **Library:** **Siemens HMI Library Suite**  
* **Justification:** This comprehensive library provides a wide range of pre-designed graphical objects and faceplates that are ideal for HVAC applications. Key benefits include:  
  * **Standardization:** Ensures all RTU projects have a consistent look and feel.  
  * **Efficiency:** Faceplates for motors, valves, and PIDs can be dragged and dropped and linked directly to the PLC data structures.
  * **Reduced Engineering:** Minimizes the time spent on HMI screen design and scripting.
* **Data Interface:** The HMI must interface with the PLC **primarily through the UDT created for each Equipment Module.** This ensures a clean, organized, and reusable connection between the front-end and back-end.

#### **5.2. Key HMI Screens**

* **Main Overview:** A graphical P\&ID-style screen showing the entire RTU. It will display the live status of the fan, compressor, and heating element, along with real-time temperatures (OA, RA, DA) and damper position.  
* **Alarms Screen:** A standard alarm viewer listing all active and historical alarms with timestamps, descriptions, and acknowledgment status.  
* **Settings Screen:** A password-protected screen for authorized personnel to adjust key operational parameters, including:  
  * Occupied/Unoccupied Setpoints  
  * PID Tuning Parameters (P, I, D)  
  * Configurable Time Delays (compressor cycle, fan failure, etc.)  
* **Trend Screen:** A multi-pen trend view for diagnostics and performance monitoring. Key trends will include:  
  * Discharge Air Temperature  
  * Return Air Temperature  
  * Outside Air Temperature  
  * Active Setpoint  
  * Damper Position Command

### **6\. Version Control and Automated Testing Strategy**

To align with modern development practices, a robust version control and automated testing pipeline will be implemented.

#### **6.1. Version Control**

* **System:** Git  
* **Platform:** GitHub  
* **Methodology:** The TIA Portal project will be managed using the siemens/tia-project-exporter tool. This utility exports project components like code blocks (FCs, FBs) and data types (UDTs) into a human-readable XML format. This allows for effective line-by-line change tracking and merging within Git.

#### **6.2. Automated Unit Testing (Continuous Integration)**

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