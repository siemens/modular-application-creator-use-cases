# Technology Object & Control Loop Configuration

**Version:** 1.0
**Date:** September 18, 2025
**Reference:** `SDS_AHU_Controller.md`

This document provides a detailed guide for the configuration and setup of the advanced Technology Objects (TOs) and control loop structures used in the AHU Controller project.

---

## 1. Introduction

The core control of the AHU relies on several interconnected PID loops and psychrometric calculations to manage temperature, humidity, and energy efficiency. This guide outlines the recommended setup for these control strategies, ensuring proper operation and safety.

---

## 2. Enthalpy and Humidity Control Strategy

This section details the advanced control strategy for managing the air properties based on enthalpy and dew point. This strategy is designed to be more energy-efficient than simple dry-bulb temperature control, especially when an economizer is involved.

### 2.1. Configuration and Logic Flow

The following steps outline the sequential logic flow required for this control strategy.

#### **Step 1: Read Physical Sensor Inputs**

The control loop requires real-time data from the primary air sensors. These tags should be mapped from the physical I/O to the program.

*   `Sensor_Tdb_F`: Dry Bulb Temperature (°F)
*   `Sensor_RH_Pct`: Relative Humidity (%)
*   `Sensor_Patm_hPa`: Atmospheric Pressure (hPa)

#### **Step 2: Perform Psychrometric Calculations**

The raw sensor data must be processed to calculate the key air properties. This is achieved by calling the dedicated calculation blocks.

*   **Enthalpy Calculation:** Call the `FB_CalculateAirEnthalpy` function block to get the enthalpy, humidity ratio, and vapor pressure.

```scl
// Call the Enthalpy FB
"FB_CalculateAirEnthalpy_DB"(
    DryBulbTemp_F := "Sensor_Tdb_F",
    RelativeHumidity_Pct := "Sensor_RH_Pct",
    AtmosphericPressure_hPa := "Sensor_Patm_hPa"
);
```

*   **Dew Point Calculation:** Call a dedicated function (`FC`) to calculate the current dew point from the partial vapor pressure.

```scl
// Call the Dew Point calculation FC
"FC_CalculateDewPoint"(
    PartialVaporPressure_Pa := "FB_CalculateAirEnthalpy_DB".PartialVaporPressure_Pa,
    DewPoint_F := "Current_Tdp_F" // Output to a global tag
);
```

#### **Step 3: Enthalpy Control PID Loop (`PID_Enthalpy`)**

This is the primary PID loop for managing the overall energy in the air (both sensible and latent heat).

*   **Setup:**
    *   **SP (Setpoint):** `Enthalpy_SP` - The desired enthalpy setpoint (BTU/lb).
    *   **PV (Process Variable):** `FB_CalculateAirEnthalpy_DB.Enthalpy_BTU_lb` - The calculated real-time enthalpy.
    *   **Output:** `HeatingCooling_Output` - A bipolar output (e.g., -100% to +100%) that is mapped to the heating and cooling EMs.

#### **Step 4: Humidity / Dew Point Control PID Loop (`PID_Humidity`)**

This PID loop specifically manages the moisture content of the air.

*   **Setup:**
    *   **SP (Setpoint):** `DewPoint_SP` - The desired dew point setpoint (°F).
    *   **PV (Process Variable):** `Current_Tdp_F` - The calculated real-time dew point.
    *   **Output:** `HumidifierDehumidifier_Output` - An output that controls the humidification or dehumidification process.

#### **Step 5: Condensation Prevention Override**

This is a critical safety feature to prevent condensation from forming on cold surfaces within the AHU plenum. This logic overrides the output of the humidity PID.

*   **Logic:** The current dew point is compared against the coldest measured surface temperature. If the dew point gets too close to the surface temperature (within a defined safety margin), humidification is disabled.

```scl
// Condensation Prevention Safety Logic
IF "Current_Tdp_F" >= ("Plenum_Surface_Temp" - "Tdp_Safety_Margin") THEN
    // Override and limit humidification to prevent condensation
    "HumidifierDehumidifier_Output" := MIN(in1 := "HumidifierDehumidifier_Output", in2 := 0.0);
END_IF;
```

#### **Step 6: Command Physical Actuators**

The final calculated outputs from the PID loops (after the safety override) are sent to the respective equipment modules to command the physical actuators (valves, dampers, etc.).
