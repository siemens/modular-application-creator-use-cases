# Hardware and Software Specification

**Project:** Constant Volume Air Handler Unit (AHU) Controller
**Reference:** Software Design Specification, Version 2.0

## 1. Hardware Bill of Materials (BOM)

The following hardware components are required for a single AHU controller panel. The selection is based on the I/O requirements defined in the SDS.

| Item | Manufacturer | Part Number | Quantity | Description |
| :--- | :--- | :--- | :--- | :--- |
| **1** | Siemens | **6ES7511-1CK01-0AB0** | 1 | SIMATIC S7-1500 Compact CPU, CPU 1511C-1 PN. Includes 16 DI, 16 DO, 5 AI, 2 AO. |
| **2** | Siemens | **6ES7531-7KF00-0AB0** | 1 | SIMATIC S7-1500 Analog Input Module, SM 531, AI 8xU/I/RTD/TC ST. Required for temperature and position feedback. |
| **3** | Siemens | **6ES7532-5HD00-0AB0** | 1 | SIMATIC S7-1500 Analog Output Module, SM 532, AQ 4xU/I ST. Required for fan, valve, and damper control. |
| **4** | Siemens | **6ES7954-8LC03-0AA0** | 1 | SIMATIC S7 Memory Card, 4 MB. Required for CPU operation. |
| **5** | Siemens | **6ES7505-0KA00-0AB0** | 1 | SIMATIC PM 70W Power Supply. Provides 24V DC power to the PLC rack. |
| **6** | Siemens | **6ES7590-1AE80-0AA0** | 1 | SIMATIC S7-1500 Mounting Rail, 483 mm (19"). |
| **7** | - | - | 1 | NEMA 1 Enclosure (or as required by installation environment). |
| **8** | - | - | 1 | HMI Panel (e.g., Siemens Comfort Panel TP700, 7") if local control is required. |


## 2. Software Bill of Materials (BOM)

The following software components form the basis of the TIA Portal project.

| Item | Product Name | Version | Description |
| :--- | :--- | :--- | :--- |
| **1** | Siemens TIA Portal | **V20** | The core engineering environment for programming the S7-1500 PLC and HMI. |
| **2** | Library of General Functions (LGF) | **V20 Compatible** | Siemens library for standard functions. Version must be compatible with TIA Portal V20. |

### 2.1. Custom Software Components (To Be Created)

All custom software components (UDTs, FBs, DBs) and Technology Objects (TOs) will be created as defined in the main **Software Design Specification** document. This ensures a single source of truth for the detailed software design.
