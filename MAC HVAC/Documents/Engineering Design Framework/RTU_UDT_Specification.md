# RTU Controller - UDT Specification

**Version:** 1.0
**Date:** September 12, 2025
**Reference:** `RTU_Controller_Hardware_Software_Specification.md`

This document defines the structure of the User Data Types (UDTs) for each Equipment Module (EM). These UDTs serve as the standardized data interface for the control logic blocks (FBs) and HMI faceplates.

---

## `UDT100_EM_SupplyFan`

**Description:** Holds all parameters, commands, and status signals for the Supply Fan Equipment Module (`EM-100`).

| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| **// -- Inputs (from I/O) --** | | |
| `Run_Fdbk_DI` | `Bool` | Digital Input: Fan run feedback from VFD/contactor. |
| `Airflow_Status_DI` | `Bool` | Digital Input: Airflow switch status (`True` = Airflow OK). |
| `VFD_Fault_DI` | `Bool` | Digital Input: VFD fault status (`True` = Fault). |
| **// -- Outputs (to I/O) --** | | |
| `Start_Cmd_DO` | `Bool` | Digital Output: Command to start/stop the fan. |
| `Speed_Ref_AO` | `Real` | Analog Output: Speed reference to VFD (0.0 - 100.0 %). |
| **// -- Configuration --** | | |
| `Fault_Delay_Sec` | `Time` | Time delay for fan failure detection (e.g., `T#5s`). |
| **// -- Status & Alarms --** | | |
| `Is_Running` | `Bool` | Status: `True` when the fan is commanded on and all feedback is normal. |
| `Fan_Failure_Alm` | `Bool` | Alarm: `True` if run/airflow feedback is missing after delay. |
| `VFD_Fault_Alm` | `Bool` | Alarm: `True` if the VFD hardware fault input is active. |

---

## `UDT200_EM_Cooling`

**Description:** Holds all parameters for the single-stage Cooling Control Equipment Module (`EM-200`).

| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| **// -- Inputs (from I/O) --** | | |
| `HP_Switch_DI` | `Bool` | Digital Input: High-pressure refrigerant switch (`True` = Fault). |
| `LP_Switch_DI` | `Bool` | Digital Input: Low-pressure refrigerant switch (`True` = Fault). |
| `Freeze_Stat_DI` | `Bool` | Digital Input: Freeze-stat on coil (`True` = Fault). |
| **// -- Outputs (to I/O) --** | | |
| `Compressor_Cmd_DO`| `Bool` | Digital Output: Command to start/stop the compressor. |
| **// -- Configuration --** | | |
| `Min_Run_Time_Sec` | `Time` | Minimum compressor run time to prevent short-cycling (e.g., `T#3m`). |
| `Min_Off_Time_Sec` | `Time` | Minimum compressor off time to prevent short-cycling (e.g., `T#3m`). |
| **// -- Status & Alarms --** | | |
| `HP_Fault_Alm` | `Bool` | Alarm: High-pressure fault. |
| `LP_Fault_Alm` | `Bool` | Alarm: Low-pressure fault. |
| `Freeze_Stat_Alm`| `Bool` | Alarm: Freeze-stat tripped. |

---

## `UDT300_EM_Heating`

**Description:** Holds all parameters for the single-stage Heating Control Equipment Module (`EM-300`).

| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| **// -- Inputs (from I/O) --** | | |
| `High_Limit_DI` | `Bool` | Digital Input: High-temperature limit switch (`True` = Fault). |
| **// -- Outputs (to I/O) --** | | |
| `Heat_Stage1_Cmd_DO`| `Bool` | Digital Output: Command for heating stage 1. |
| **// -- Status & Alarms --** | | |
| `High_Limit_Alm` | `Bool` | Alarm: High-temperature limit fault. |

---

## `UDT400_EM_Damper`

**Description:** Holds all parameters for the Damper/Economizer Equipment Module (`EM-400`).

| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| **// -- Inputs (from I/O) --** | | |
| `Return_Air_Temp_AI`| `Real` | Analog Input: Return air temperature sensor. |
| `Outside_Air_Temp_AI`|`Real` | Analog Input: Outside air temperature sensor. |
| `Discharge_Air_Temp_AI`|`Real`| Analog Input: Discharge air temperature sensor. |
| **// -- Outputs (to I/O) --** | | |
| `Damper_Pos_Cmd_AO`| `Real` | Analog Output: Damper position command (0.0 - 100.0 %). |
| **// -- Configuration --** | | |
| `Min_Fresh_Air_Pos`| `Real` | Minimum damper position for ventilation (e.g., 20.0 %). |
| `Econ_Temp_Diff` | `Real` | Temp diff (RAT - OAT) to enable economizer (e.g., 2.0). |
| `Econ_High_Limit` | `Real` | High OAT limit to disable economizer (e.g., 65.0). |
| **// -- Status --** | | |
| `Econ_Mode_Active`| `Bool` | Status: `True` when economizer mode is active. |

---

## `UDT500_EM_Monitoring`

**Description:** Holds all parameters for the System Monitoring Equipment Module (`EM-500`).

| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| **// -- Inputs (from I/O) --** | | |
| `Dirty_Filter_DI`| `Bool` | Digital Input: Dirty filter DP switch (`True` = Dirty). |
| **// -- Configuration --** | | |
| `Filter_Delay_Sec` | `Time` | Time delay for dirty filter alarm (e.g., `T#10s`). |
| **// -- Status & Alarms --** | | |
| `Dirty_Filter_Alm` | `Bool` | Alarm: Dirty filter alarm. |
