### **Installation Qualification (IQ) Protocol: HVAC System**

**Document ID:** IQ-HVAC-001
**Version:** 1.0
**Date:** 2025-09-19

| **Role** | **Name** | **Signature** | **Date** |
| :--- | :--- | :--- | :--- |
| **Author:** | (Senior Validation Specialist) | | |
| **Reviewed By:** | (Engineering Head) | | |
| **Approved By:** | (Quality Assurance Head) | | |

### **1.0 Purpose**
The purpose of this Installation Qualification (IQ) protocol is to verify and document that the HVAC system, its components, and the associated Building Automation System (BAS) have been installed correctly, in accordance with the approved design documents, P&IDs, and manufacturer's recommendations.

### **2.0 Scope**
This IQ protocol covers the physical installation of all components related to the HVAC system for the Grade A, B, and C areas, including:
*   Air Handling Units (AHUs)
*   Ductwork, piping, and insulation
*   Terminal HEPA filters
*   Instrumentation and sensors (Temperature, RH, Pressure)
*   BAS hardware (Siemens S7-PLC panels, I/O modules, wiring)

### **3.0 Installation Qualification Checklist**

#### **3.1 Documentation Verification**
| Check ID | Description | Acceptance Criteria | Verified (Y/N) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| IQ-DOC-01 | Approved Design Qualification Report (DQ-HVAC-001) | Report is approved and available. | | |
| IQ-DOC-02 | Final "As-Built" P&IDs and Electrical Schematics | Drawings are available and marked "As-Built". | | |
| IQ-DOC-03 | Major Equipment Manuals (AHUs, PLC) | Manuals are available on-site. | | |
| IQ-DOC-04 | Instrument Calibration Certificates | All cGMP-critical sensors have valid, traceable calibration certificates. | | |
| IQ-DOC-05 | Material Certificates for Ductwork | Certificates for Grade 316L SS are available. | | |

#### **3.1.1. Instrument to PLC Tag Cross-Reference**
*Note: This table provides a mapping from the P&ID instrument tag to the PLC software tag.*
| P&ID Instrument Tag | PLC Tag (from SDS) | Description |
| :--- | :--- | :--- |
| `T-101` | `AHU1_DAT_Temp` | Discharge Air Temperature |
| `T-102` | `AHU1_RAT_Temp` | Return Air Temperature |
| `T-103` | `AHU1_OAT_Temp` | Outside Air Temperature |
| `H-101` | `AHU1_DAR_RH` | Discharge Air Relative Humidity |
| `H-102` | `AHU1_RAR_RH` | Return Air Relative Humidity |
| `H-103` | `AHU1_OAR_RH` | Outside Air Relative Humidity |
| `DP-101` | `AHU1_PFL_DP` | Pre-Filter Differential Pressure |
| `DP-102` | `AHU1_AFL_DP` | Post-Filter Differential Pressure |
| `DP-103` | `AHU1_RM_DP` | Room Differential Pressure |

#### **3.2 Equipment Installation Verification (Physical Check)**
| Check ID | Equipment | Acceptance Criteria | Verified (Y/N) | Comments (e.g., Serial #) |
| :--- | :--- | :--- | :--- | :--- |
| IQ-EQP-01 | Air Handling Unit (AHU-01) | Model, make, and serial number match purchase order and datasheets. Installed level and on designated pad. | | |
| IQ-EQP-02 | Air Handling Unit (AHU-02) | Model, make, and serial number match purchase order and datasheets. Installed level and on designated pad. | | |
| IQ-EQP-03 | Temperature Sensors (T-101, T-102, T-103) | Installed sensors' model numbers match design specification. Location matches P&ID. | | |
| IQ-EQP-04 | RH Sensors (H-101, H-102, H-103) | Installed sensors' model numbers match design specification. Location matches P&ID. | | |
| IQ-EQP-05 | Differential Pressure Sensors (DP-101, DP-102, DP-103) | Installed sensors' model numbers match design specification. Location matches P&ID. | | |
| IQ-EQP-06 | Terminal HEPA Filters | Installed filters' model and efficiency (H14) match specification. No visible damage to media or seals. | | |
| IQ-EQP-07 | BAS Control Panel (PNL-BAS-01) | Panel is installed at the location shown on drawings. All components (PLC, I/O cards, power supply) match design. | | |

#### **3.3 Installation Verification**
| Check ID | Description | Acceptance Criteria | Verified (Y/N) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| IQ-INST-01 | Ductwork Installation | All duct joints are sealed per specification. Ductwork is properly supported and sloped where required. | | |
| IQ-INST-02 | Piping Installation | All piping (chilled water, hot water, steam) is installed, insulated, and pressure tested per design. | | |
| IQ-INST-03 | Electrical and Wiring | All field instruments are wired to the correct I/O terminals in the BAS panel as per electrical schematics. All wiring is labeled. | | |
| IQ-INST-04 | Utility Connections | Correct electrical voltage is supplied to AHUs and control panel. Water/steam utilities are connected. | | |

### **4.0 Deviation Log**
All deviations from the acceptance criteria listed above must be documented.

| Deviation # | Check ID | Description of Deviation | Corrective Action | Approved By (QA) |
| :--- | :--- | :--- | :--- | :--- |
| | | | | |
| | | | | |

### **5.0 Summary and Conclusion**
Upon completion of the checklist and resolution of all deviations, a final conclusion will be recorded.

**Conclusion:**
[ ] The HVAC and BAS systems are **INSTALLED** correctly and meet all requirements of this IQ protocol. The system is ready to proceed to Operational Qualification (OQ).
[ ] The HVAC and BAS systems are **NOT APPROVED** for OQ. The critical deviations noted in Section 4.0 must be resolved.

---
**Executed By:** _________________________ (Validation Specialist) **Date:** ___________

**Reviewed By:** _________________________ (Engineering Head) **Date:** ___________

**Approved By:** _________________________ (Quality Assurance) **Date:** ___________
