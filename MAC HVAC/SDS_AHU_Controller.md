# Software Design Specification (SDS) - Constant Volume Air Handler Unit (AHU) Controller

**Project:** Constant Volume AHU Controller
**PLC Platform:** Siemens TIA Portal
**Target CPU:** Siemens S7-1500 Family
**Author:** Jules
**Date:** 2025-09-17
**Version:** 2.0

**References:** This document's requirements are based on the corrected `AGENTS.md` file. The design incorporates details from existing project documents (`Software Design Specification.md`, `AHU_Sequence_of_Operations.md`) and user feedback to create a specification for a Constant Volume AHU with full feedback on modulating devices.

---

### **1. System Architecture and Hardware Specification**

This section defines the core hardware and software architecture for the AHU controller.

#### **1.1. PLC Hardware**

*   **Controller:** Siemens SIMATIC S7-1500, **CPU 1511C-1 PN**.
    *   **Rationale:** The compact model provides a solid foundation. Its integrated I/O is supplemented by analog I/O modules to meet all project requirements.

#### **1.2. I/O Signal Modules (SMs)**

*   **Total Project I/O Requirements:**
    *   6 Digital Inputs
    *   1 Digital Output
    *   6 Analog Inputs (3x Temp, 3x Feedback)
    *   4 Analog Outputs (1x Fan Speed, 2x Valve, 1x Damper)
*   **CPU Integrated I/O:**
    *   16x DI, 16x DO, 5x AI, 2x AO
*   **Additional Signal Modules:**
    *   1x **SM 531, AI 8xU/I/RTD/TC ST** (Provides 8 additional Analog Inputs)
    *   1x **SM 532, AQ 4xU/I ST** (Provides 4 additional Analog Outputs)
*   **Conclusion:** The CPU's integrated I/O combined with one analog input module and one analog output module is required.

#### **1.3. Technology Objects (TOs)**

*   **Instance 1: `TO_PID_DAT_Control`**
    *   **Purpose:** Main Discharge Air Temperature (DAT) Control. A bipolar PID that outputs -100% (full heat) to +100% (full cool).
    *   **Process Variable (PV):** `AHU1_DAT_Temp`
    *   **Manipulated Variable (MV):** The bipolar output is mapped to the heating and cooling valve commands.
*   **Instance 2: `TO_PID_Econ_Control`**
    *   **Purpose:** Economizer Free Cooling Control.
    *   **Process Variable (PV):** `AHU1_DAT_Temp`
    *   **Manipulated Variable (MV):** The output (0-100%) directly controls the `Damper Position Command`.

---

### **2. Detailed Equipment Module (EM) Specifications**

#### **EM-100: Supply Fan Control**
*   **Purpose:** To control and monitor the VFD for the main supply fan.
*   **Logic:** Controls fan start/stop and speed. Monitors run feedback, airflow, and VFD fault status.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Start/Stop Command | Digital | Output | AHU1_SF_StartCmd |
| Speed Reference | Analog | Output | AHU1_SF_SpeedRef |
| Run Feedback/Status | Digital | Input | AHU1_SF_RunFdbk |
| Airflow Switch Status | Digital | Input | AHU1_SF_AirflowSw |
| VFD Fault Status | Digital | Input | AHU1_SF_VfdFault |

#### **EM-200: Cooling Control (Chilled Water)**
*   **Purpose:** To control a modulating chilled water valve.
*   **Logic:** Accepts an analog demand (0-100%). Monitors a freeze-stat. Generates a "Valve Failure" alarm if the commanded position and feedback do not match.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Chilled Water Valve Cmd | Analog | Output | AHU1_CW_VlvCmd |
| Chilled Water Valve Fdbk | Analog | Input | AHU1_CW_VlvFdbk |
| Chilled Water Freeze Stat | Digital | Input | AHU1_CW_FreezeStat |

#### **EM-300: Heating Control (Hot Water)**
*   **Purpose:** To control a modulating hot water valve.
*   **Logic:** Accepts an analog demand (0-100%). Monitors a freeze-stat. Generates a "Valve Failure" alarm if the commanded position and feedback do not match.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Hot Water Valve Cmd | Analog | Output | AHU1_HW_VlvCmd |
| Hot Water Valve Fdbk | Analog | Input | AHU1_HW_VlvFdbk |
| Hot Water Freeze Stat | Digital | Input | AHU1_HW_FreezeStat |

#### **EM-400: Damper/Economizer Control**
*   **Purpose:** To manage dampers for ventilation and economizer cooling.
*   **Logic:** Controls a modulating damper actuator. Generates a "Damper Failure" alarm if the commanded position and feedback do not match.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Damper Position Cmd | Analog | Output | AHU1_DMP_PosCmd |
| Damper Position Fdbk | Analog | Input | AHU1_DMP_PosFdbk |
| Return Air Temp | Analog | Input | AHU1_RAT_Temp |
| Outside Air Temp | Analog | Input | AHU1_OAT_Temp |
| Discharge Air Temp | Analog | Input | AHU1_DAT_Temp |

#### **EM-500: System Monitoring**
*   **Purpose:** To monitor system-wide components.
*   **Logic:** Monitors a differential pressure switch for a "Dirty Filter" alarm.
*   **Parameter Set:**
| Parameter Name | Signal Type | I/O Type | TIA Portal Tag Name Convention |
| :--- | :--- | :--- | :--- |
| Dirty Filter Status | Digital | Input | AHU1_SYS_DirtyFilter |

---

### **3. Main Control Program (OB1 Logic)**
The main logic coordinates the EMs based on operating mode and thermal demand.
*   **Control Sequence:** The `TO_PID_DAT_Control` output (-100% to +100%) determines the heating/cooling demand. A positive output modulates the EM-200 chilled water valve. A negative output modulates the EM-300 hot water valve. The `TO_PID_Econ_Control` modulates the EM-400 damper for free cooling when conditions are favorable. All modulating devices will be checked against their feedback signals.

---

### **4. HMI Strategy and Library Recommendation**
*   **Library:** **Library of General Functions (LGF)**
*   **Key HMI Screens:**
    *   **Main Overview:** Graphical screen showing the AHU status, including commanded vs. actual feedback for all valves and dampers.
    *   **Alarms Screen:** List of all active and historical alarms, including feedback failure alarms.
    *   **Settings Screen:** Password-protected screen for setpoints, PID tuning, and delays.
    *   **Trend Screen:** For trending temperatures, setpoints, and feedback signals.

---

### **5. Glossary & FAQ**
*   **AHU:** Air Handler Unit
*   **EM:** Equipment Module
*   **VFD:** Variable Frequency Drive
*   **CHW / HW:** Chilled Water / Hot Water
*   **Fdbk:** Feedback (a signal representing the actual state of a device)
*   **Q: Why add an extra AI and AO module?**
    *   **A:** To support position feedback from all modulating devices (2 valves, 1 damper) and to provide enough analog outputs to command them, the CPU's integrated I/O was not sufficient.
