### **Design Qualification (DQ) Protocol: HVAC System**

**Document ID:** DQ-HVAC-001
**Version:** 1.0
**Date:** 2025-09-19

| **Role** | **Name** | **Signature** | **Date** |
| :--- | :--- | :--- | :--- |
| **Author:** | (Senior Validation Specialist) | | |
| **Reviewed By:** | (Engineering Head) | | |
| **Approved By:** | (Quality Assurance Head) | | |

### **1.0 Purpose**
The purpose of this Design Qualification (DQ) protocol is to ensure that the design of the HVAC system and its associated Building Automation System (BAS) for the new sterile manufacturing facility is fully compliant with cGMP requirements and the pre-defined user and functional specifications (URS/FS). Successful execution of this protocol will confirm that the design is adequate and approved for procurement and installation.

### **2.0 Scope**
This DQ protocol applies to the design of the HVAC system serving the Grade A, B, and C cleanrooms. The review will encompass the following design documentation:
*   Piping & Instrumentation Diagrams (P&IDs)
*   HVAC Airflow Schematics and Room Layouts
*   Control System Architecture Diagrams
*   Equipment and Instrumentation Datasheets (AHUs, sensors, filters)
*   Electrical Schematics

### **3.0 Responsibilities**
*   **Validation Team:** Responsible for authoring and executing this DQ protocol.
*   **Engineering Department:** Responsible for providing all required design documentation and clarifying design queries.
*   **Quality Assurance (QA):** Responsible for reviewing and approving this protocol, the executed results, and any documented deviations.

### **4.0 Design Qualification Checklist**
The following checklist items must be verified against the design documents. All specifications must be confirmed to meet or exceed the requirements stated in the Functional Specification (FS-HVAC-001).

| Requirement ID | Requirement Description | Design Document(s) & Specification (Example) | Verified (Y/N) | Comments / Deviations |
| :--- | :--- | :--- | :--- | :--- |
| **FS 5.1** | **Grade A: Temp Control** | AHU-01 Datasheet: Cooling/Heating Coil Capacity. P&ID-101: Temp Sensor T-101 specified. | | |
| | **Grade A: Temp Setpoint:** 72°F ± 2°F | BAS Control Logic Diagram: Setpoint specified. | | |
| **FS 5.1** | **Grade B: RH Control** | AHU-02 Datasheet: Humidifier/Dehumidifier Capacity. P&ID-102: RH Sensor H-102 specified. | | |
| | **Grade B: RH Setpoint:** 45% ± 5% | BAS Control Logic Diagram: Setpoint specified. | | |
| **FS 5.1** | **Grade C: Pressure Control** | Airflow Schematic AFS-103: Air balance calculation shows +15 Pa cascade. P&ID-103: DP Sensor DP-103 specified. | | |
| **FS 5.1** | **Grade B/C: Air Changes (ACH)** | Airflow Schematic AFS-102/103: Supply/return airflow rates calculated to > 20 ACH. | | |
| **General** | **HEPA Filtration** | Filter Datasheet F-01: Specified as H14, 99.995% efficiency at MPPS. Room Layouts: Terminal HEPA filters shown in all graded areas. | | |
| **General** | **Materials of Construction** | Ductwork Specification DW-001: Grade 316L Stainless Steel for all process-contacting ducts. AHU-01/02 Datasheets: Smooth, non-shedding internal surfaces. | | |
| **FS 5.2** | **Alarm Management Design** | BAS Control Logic Diagram: Alarm setpoints match FS table. P&ID-101/102/103: Critical alarm outputs specified for beacons/dialer. | | |
| **FS 5.3** | **21 CFR Part 11 Compliance** | Control System Architecture Diagram CS-001: Shows secure server, non-editable database, and audit trail functionality. | | |
| **FS 5.3** | **BAS Hardware Specification** | Equipment List EL-001: Specifies Siemens S7-PLC series controller. | | |
| **FS 5.4** | **HMI/SCADA Screens** | HMI Screen Mockups HM-01 to HM-04: Design includes required Overview, Alarm, Trend, and Diagnostic screens. | | |

### **5.0 Deviation Log**
Any "No" answer or discrepancy noted in the checklist above must be documented below. Each deviation must be assessed for its impact and a corrective action plan must be approved by Engineering and QA before this DQ can be closed.

| Deviation # | Checklist Item # | Description of Deviation | Impact Assessment | Corrective Action Plan | Approved By (QA) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| | | | | | |
| | | | | | |

### **6.0 Summary and Conclusion**
Upon completion of the checklist and resolution of all deviations, a final conclusion will be recorded.

**Conclusion:**
[ ] The design of the HVAC and BAS systems is **APPROVED** and meets all specified requirements. Procurement and installation may proceed.
[ ] The design is **NOT APPROVED**. The design must be revised to address the critical deviations noted in Section 5.0.

---
**Executed By:** _________________________ (Validation Specialist) **Date:** ___________

**Reviewed By:** _________________________ (Engineering Head) **Date:** ___________

**Approved By:** _________________________ (Quality Assurance) **Date:** ___________
