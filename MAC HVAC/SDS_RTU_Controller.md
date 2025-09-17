# Software Design Specification (SDS) - Basic Rooftop HVAC Unit (RTU) Controller

**Project:** Basic Rooftop HVAC Unit (RTU) Controller
**PLC Platform:** Siemens TIA Portal
**Target CPU:** Siemens S7-1500 Family
**Author:** Jules
**Date:** 2025-09-17
**Version:** 1.0

**References:** This document was created to fulfill the requirements outlined in `AGENTS.md`. The content of this SDS is a direct implementation of the specifications provided in that file.

---

### **1. System Architecture and Hardware Specification**

This section defines the core hardware components and the software architecture for the RTU controller. The design emphasizes a centralized control strategy using a single Siemens S7-1500 PLC.

#### **1.1. PLC Hardware**

*   **Controller:** Siemens SIMATIC S7-1500, **CPU 1511C-1 PN**.
    *   **Rationale:** The compact model provides an excellent balance of performance and value, and its integrated I/O is sufficient to meet all project requirements without additional signal modules.

#### **1.2. I/O Signal Modules (SMs)**

Based on the required I/O points from the Equipment Modules, the integrated I/O of the CPU is sufficient.

*   **Total Project I/O Requirements:**
    *   8 Digital Inputs
    *   3 Digital Outputs
    *   3 Analog Inputs
    *   2 Analog Outputs
*   **CPU Integrated I/O:**
    *   16x Digital Inputs (DI)
    *   16x Digital Outputs (DO)
    *   5x Analog Inputs (AI)
    *   2x Analog Outputs (AO)
*   **Conclusion:** The CPU 1511C-1 PN's integrated I/O is sufficient for this application. No additional signal modules are required.

#### **1.3. Technology Objects (TOs)**

To ensure robust and efficient process control, `PID_Compact` Technology Objects will be used for temperature control.

*   **Instance 1: `TO_PID_Cooling`**
    *   **Purpose:** Discharge Air Temperature Control (Cooling). This PID loop is responsible for maintaining the cooling setpoint by enabling the DX cooling stage.
    *   **Process Variable (PV):** `EM400_Discharge_Air_Temp` (Discharge Air Temperature).
    *   **Setpoint (SP):** The active Cooling Setpoint.
    *   **Manipulated Variable (MV):** The boolean output of this PID will enable the `EM-200` Cooling Control module.

*   **Instance 2: `TO_PID_Heating`**
    *   **Purpose:** Discharge Air Temperature Control (Heating). This PID loop is responsible for maintaining the heating setpoint by enabling the heating stage.
    *   **Process Variable (PV):** `EM400_Discharge_Air_Temp` (Discharge Air Temperature).
    *   **Setpoint (SP):** The active Heating Setpoint.
    *   **Manipulated Variable (MV):** The boolean output of this PID will enable the `EM-300` Heating Control module.

---

### **2. Detailed Equipment Module (EM) Specifications**

The control logic is segmented into the following Equipment Modules (EMs). Each EM is a self-contained functional block designed for use in the Modular Application Creator (MAC).

#### **EM-100: Supply Fan Control**

*   **Purpose:** To control and monitor the main supply fan, which is equipped with a Variable Frequency Drive (VFD).
*   **Logic:**
    *   Accepts a Start/Stop command.
    *   Accepts a speed command (0-100%).
    *   Monitors fan status/run feedback and airflow status (from a differential pressure switch).
    *   Generates a "Fan Failure" alarm if run feedback is missing after a configurable delay (e.g., 5 seconds) or if the airflow switch is not made.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Start/Stop Command | Digital | Output | RTU1_SF_StartCmd |
| Speed Reference | Analog | Output | RTU1_SF_SpeedRef |
| Run Feedback/Status | Digital | Input | RTU1_SF_RunFdbk |
| Airflow Switch Status | Digital | Input | RTU1_SF_AirflowSw |
| VFD Fault Status | Digital | Input | RTU1_SF_VfdFault |

#### **EM-200: Cooling Control (Single-Stage DX)**

*   **Purpose:** To control a single-stage direct expansion (DX) cooling coil and its associated safety devices.
*   **Logic:**
    *   Accepts a cooling enable command.
    *   Monitors high and low refrigerant pressure safety switches.
    *   Monitors a freeze-stat on the coil.
    *   Implements a minimum run-time and minimum off-time delay (e.g., 3 minutes) to prevent compressor short-cycling.
    *   Generates alarms for "High Pressure Fault," "Low Pressure Fault," and "Freeze Stat Tripped."
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Compressor Stage 1 Command | Digital | Output | RTU1_CLG_CompCmd |
| High-Pressure Switch Input | Digital | Input | RTU1_CLG_HiPressSw |
| Low-Pressure Switch Input | Digital | Input | RTU1_CLG_LoPressSw |
| Freeze Stat Input | Digital | Input | RTU1_CLG_FreezeStat |

#### **EM-300: Heating Control (Single-Stage Gas/Electric)**

*   **Purpose:** To control a single-stage heating element.
*   **Logic:**
    *   Accepts a heating enable command.
    *   Monitors a high-temperature limit safety switch.
    *   Generates an alarm for "High-Temperature Limit Fault."
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Heating Stage 1 Command | Digital | Output | RTU1_HTG_HeatCmd |
| High-Temperature Limit Switch Input | Digital | Input | RTU1_HTG_HiTempLim |

#### **EM-400: Damper/Economizer Control**

*   **Purpose:** To control the fresh air, return air, and exhaust air dampers for ventilation and economizer free cooling.
*   **Logic:**
    *   Controls a modulating damper actuator (0-10V) to maintain a minimum fresh air position.
    *   When in "Economizer Mode," the module will modulate the dampers to maintain the discharge air temperature setpoint.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Damper Position Command | Analog | Output | RTU1_DMP_PosCmd |
| Return Air Temperature | Analog | Input | RTU1_RAT_Temp |
| Outside Air Temperature | Analog | Input | RTU1_OAT_Temp |
| Discharge Air Temperature | Analog | Input | RTU1_DAT_Temp |

#### **EM-500: System Monitoring**

*   **Purpose:** To monitor common system-wide parameters.
*   **Logic:**
    *   Monitors the status of the air filters via a differential pressure switch.
    *   Generates a "Dirty Filter" alarm.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Dirty Filter Status | Digital | Input | RTU1_SYS_DirtyFilter |

---

### **3. Main Control Program (OB1 Logic)**

The main program logic, executed in OB1, coordinates the EMs based on the unit's operating mode and thermal demand.

#### **3.1. Modes of Operation**

*   **Off:** The unit is completely shut down. All outputs are de-energized.
*   **Occupied:** The unit is active. The Supply Fan runs continuously, and the system will heat or cool as needed to maintain the occupied temperature setpoints.
*   **Unoccupied:** The unit operates in a setback/setup mode. The fan runs only when there is a demand for heating or cooling to maintain wider unoccupied temperature setpoints.

#### **3.2. Control Sequence**

1.  **Mode Determination:** The system state (Off, Occupied, Unoccupied) is determined by a schedule or a Building Automation System (BAS) command.
2.  **Fan Control:** The `EM-100 Supply Fan` will be enabled whenever there is a demand for heating, cooling, or ventilation (i.e., during Occupied mode).
3.  **Demand Calculation:**
    *   The `TO_PID_Cooling` PID controller determines if there is a call for cooling.
    *   The `TO_PID_Heating` PID controller determines if there is a call for heating.
4.  **Cooling Logic:**
    *   When a call for cooling is active, the system first checks if the `EM-400 Economizer` can be used.
    *   The Economizer is enabled if the Outside Air Temperature is suitable for free cooling.
    *   If the Economizer is not available or not sufficient, the `TO_PID_Cooling` output will enable the `EM-200 Cooling Control` module.
5.  **Heating Logic:**
    *   When a call for heating is active, the `TO_PID_Heating` output will enable the `EM-300 Heating Control` module.
6.  **Safety Interlocks:** All heating and cooling operations are interlocked with the fan status from EM-100. No heating or cooling can occur unless the fan is proven to be running.

---

### **4. HMI Strategy and Library Recommendation**

To ensure consistency and rapid development, a standardized HMI library is required.

#### **4.1. HMI Library Recommendation**

*   **Library:** **Library of General Functions (LGF)**
*   **Justification:** The LGF is selected to align with the project's goal of creating an open, flexible, and cost-effective design. As a free and universally available library from Siemens, the LGF provides a comprehensive set of basic functions that can be used to build robust control logic without introducing licensing costs or dependencies on more complex, feature-heavy libraries. This approach gives developers the flexibility to create highly customized HMI screens tailored specifically to the RTU application.

#### **4.2. Key HMI Screens**

*   **Main Overview:** A graphical representation of the RTU, showing the status of all major components (fan, compressor, heating), and key temperatures.
*   **Alarms Screen:** A list of active and historical alarms.
*   **Settings Screen:** A password-protected screen for adjusting setpoints, PID parameters, and time delays.
*   **Trend Screen:** A screen for trending key process values like temperatures and setpoints.

---

### **5. Glossary & FAQ**

#### **5.1. Glossary of Acronyms**
*   **PLC:** Programmable Logic Controller
*   **RTU:** Rooftop Unit
*   **EM:** Equipment Module
*   **FB:** Function Block
*   **UDT:** User Data Type (a structured data type)
*   **TIA Portal:** Totally Integrated Automation Portal (Siemens engineering software)
*   **VFD:** Variable Frequency Drive
*   **DX:** Direct Expansion (a type of cooling system)
*   **PID:** Proportional-Integral-Derivative (a control loop mechanism)
*   **I/O:** Input/Output

#### **5.2. Frequently Asked Questions (FAQ)**

*   **Q: Why was the CPU 1511C-1 PN chosen?**
    *   **A:** Its integrated I/O capabilities perfectly match the project's requirements, eliminating the need for additional, costly signal modules.

*   **Q: What is the purpose of the minimum run-time and off-time for the compressor?**
    *   **A:** This logic (known as anti-short-cycling) prevents the compressor from starting and stopping too rapidly, which can cause mechanical damage and reduce the lifespan of the equipment.
