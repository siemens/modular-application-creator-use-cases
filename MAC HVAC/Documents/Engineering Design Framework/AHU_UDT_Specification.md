# AHU Controller - UDT Specification

**Version:** 2.0
**Date:** September 17, 2025
**Reference:** `AHU_Controller_Hardware_Software_Specification.md`

This document defines the structure of the User Data Types (UDTs) for each Equipment Module (EM). These UDTs serve as the standardized data interface for the control logic blocks (FBs) and HMI faceplates.

---

## `UDT100_EM_SupplyFan`
**Description:** Holds all parameters for the Supply Fan Equipment Module (`EM-100`).
| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| `Run_Fdbk_DI` | `Bool` | Digital Input: Fan run feedback. |
| `Airflow_Status_DI` | `Bool` | Digital Input: Airflow switch status. |
| `VFD_Fault_DI` | `Bool` | Digital Input: VFD fault status. |
| `Start_Cmd_DO` | `Bool` | Digital Output: Command to start/stop the fan. |
| `Speed_Ref_AO` | `Real` | Analog Output: Speed reference to VFD (0.0 - 100.0 %). |
| `Fault_Delay_Sec` | `Time` | Time delay for fan failure detection. |
| `Fan_Failure_Alm` | `Bool` | Alarm: `True` if run/airflow feedback is missing. |
| `VFD_Fault_Alm` | `Bool` | Alarm: `True` if the VFD hardware fault input is active. |

---

## `UDT200_EM_Cooling`
**Description:** Holds all parameters for the Chilled Water Cooling Equipment Module (`EM-200`).
| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| `CHW_Freeze_Stat_DI` | `Bool` | Digital Input: Freeze-stat on chilled water coil. |
| `CHW_Valve_Fdbk_AI` | `Real` | Analog Input: Chilled water valve position feedback (0-100%). |
| `CHW_Valve_Cmd_AO`| `Real` | Analog Output: Chilled water valve position command (0-100%). |
| `CHW_Freeze_Alm`| `Bool` | Alarm: Chilled water coil freeze-stat tripped. |
| `Valve_Failure_Alm` | `Bool` | Alarm: `True` if command and feedback do not match. |

---

## `UDT300_EM_Heating`
**Description:** Holds all parameters for the Hot Water Heating Equipment Module (`EM-300`).
| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| `HW_Freeze_Stat_DI` | `Bool` | Digital Input: Freeze-stat on hot water coil. |
| `HW_Valve_Fdbk_AI` | `Real` | Analog Input: Hot water valve position feedback (0-100%). |
| `HW_Valve_Cmd_AO`| `Real` | Analog Output: Hot water valve position command (0-100%). |
| `HW_Freeze_Alm` | `Bool` | Alarm: Hot water coil freeze-stat tripped. |
| `Valve_Failure_Alm` | `Bool` | Alarm: `True` if command and feedback do not match. |

---

## `UDT400_EM_Damper`
**Description:** Holds all parameters for the Damper/Economizer Equipment Module (`EM-400`).
| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| `Return_Air_Temp_AI`| `Real` | Analog Input: Return air temperature sensor. |
| `Outside_Air_Temp_AI`|`Real` | Analog Input: Outside air temperature sensor. |
| `Discharge_Air_Temp_AI`|`Real`| Analog Input: Discharge air temperature sensor. |
| `Damper_Pos_Fdbk_AI` | `Real` | Analog Input: Damper position feedback (0-100%). |
| `Damper_Pos_Cmd_AO`| `Real` | Analog Output: Damper position command (0-100%). |
| `Damper_Failure_Alm` | `Bool` | Alarm: `True` if command and feedback do not match. |
| `Econ_Mode_Active`| `Bool` | Status: `True` when economizer mode is active. |

---

## `UDT500_EM_Monitoring`
**Description:** Holds all parameters for the System Monitoring Equipment Module (`EM-500`).
| Parameter Name | Data Type | Comment / Purpose |
| :--- | :--- | :--- |
| `Dirty_Filter_DI`| `Bool` | Digital Input: Dirty filter DP switch. |
| `Filter_Delay_Sec` | `Time` | Time delay for dirty filter alarm. |
| `Dirty_Filter_Alm` | `Bool` | Alarm: Dirty filter alarm. |
