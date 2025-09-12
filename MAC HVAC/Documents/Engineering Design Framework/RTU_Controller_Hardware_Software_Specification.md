# Hardware and Software Specification

**Project:** Basic Rooftop Unit (RTU) Controller
**Reference:** Software Design Specification, Version 1.0

## 1. Hardware Bill of Materials (BOM)

The following hardware components are required for a single RTU controller panel. The selection is based on the I/O requirements defined in the SDS and the use of the integrated I/O on the CPU 1511C-1 PN.

| Item | Manufacturer | Part Number | Quantity | Description |
| :--- | :--- | :--- | :--- | :--- |
| **1** | Siemens | **6ES7511-1CK01-0AB0** | 1 | SIMATIC S7-1500 Compact CPU, CPU 1511C-1 PN. Includes 16 DI, 16 DO, 5 AI, 2 AO. |
| **2** | Siemens | **6ES7954-8LC03-0AA0** | 1 | SIMATIC S7 Memory Card, 4 MB. Required for CPU operation. |
| **3** | Siemens | **6ES7505-0KA00-0AB0** | 1 | SIMATIC PM 70W Power Supply. Provides 24V DC power to the PLC rack. |
| **4** | Siemens | **6ES7590-1AE80-0AA0** | 1 | SIMATIC S7-1500 Mounting Rail, 483 mm (19"). |
| **5** | - | - | 1 | NEMA 1 Enclosure (or as required by installation environment). |
| **6** | - | - | 1 | Terminal blocks, wiring, and circuit protection as required for field I/O connections. |
| **7** | - | - | 1 | HMI Panel (e.g., Siemens Comfort Panel TP700, 7") if local control is required. |

## 2. Software Bill of Materials (BOM)

The following software components form the basis of the TIA Portal project.

| Item | Product Name | Version | Description |
| :--- | :--- | :--- | :--- |
| **1** | Siemens TIA Portal | **V20** | The core engineering environment for programming the S7-1500 PLC and HMI. |
| **2** | Library of General Functions (LGF) | **V20 Compatible** | Siemens library for standard functions. Version must be compatible with TIA Portal V20. |

### 2.1. Custom Software Components (To Be Created)

These are the specific software objects that will be created within the TIA Portal project, following the Modular Application Creator (MAC) philosophy.

#### **User Data Types (UDTs)**
Standardized data structures will be created for each Equipment Module to hold all parameters, commands, and status signals.

*   `UDT100_EM_SupplyFan`
*   `UDT200_EM_Cooling`
*   `UDT300_EM_Heating`
*   `UDT400_EM_Damper`
*   `UDT500_EM_Monitoring`

#### **Function Blocks (FBs)**
Each Equipment Module will be encapsulated in its own reusable Function Block.

*   `FB100_EM_SupplyFan`: Contains all logic for the supply fan.
*   `FB200_EM_Cooling`: Contains all logic for the cooling stage.
*   `FB300_EM_Heating`: Contains all logic for the heating stage.
*   `FB400_EM_Damper`: Contains all logic for damper control.
*   `FB500_EM_Monitoring`: Contains all logic for system monitoring.

#### **Data Blocks (DBs)**
Instance Data Blocks (IDBs) will be generated for each FB instance. A global DB will hold system-wide parameters.

*   `IDB_EM_SupplyFan`: Instance DB for the Supply Fan FB.
*   `IDB_EM_Cooling`: Instance DB for the Cooling FB.
*   `IDB_EM_Heating`: Instance DB for the Heating FB.
*   `IDB_EM_Damper`: Instance DB for the Damper FB.
*   `IDB_EM_Monitoring`: Instance DB for the Monitoring FB.
*   `DB_Global_Params`: Global DB for setpoints, modes, and system-wide configuration.

#### **Technology Objects (TOs)**
As defined in the SDS, the following TOs will be configured in the project.

*   `TO_PID_DAT_Control`: PID controller for main temperature control.
*   `TO_PID_Econ_Control`: PID controller for economizer damper modulation.
