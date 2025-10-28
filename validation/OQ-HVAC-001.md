### **Operational Qualification (OQ) Protocol: HVAC System**

**Document ID:** OQ-HVAC-001
**Version:** 1.0
**Date:** 2025-09-19

| **Role** | **Name** | **Signature** | **Date** |
| :--- | :--- | :--- | :--- |
| **Author:** | (Senior Validation Specialist) | | |
| **Reviewed By:** | (Engineering Head) | | |
| **Approved By:** | (Quality Assurance Head) | | |

### **1.0 Purpose**
The purpose of this Operational Qualification (OQ) protocol is to challenge and verify that the HVAC system and its Building Automation System (BAS) function correctly and consistently according to the approved Functional Specification (FS-HVAC-001).

### **2.0 Scope**
This OQ protocol covers the functional testing of the HVAC system serving the Grade A, B, and C areas. This includes:
*   Control loop performance (Temperature, RH, Pressure).
*   Alarm and interlock functionality.
*   BAS security, data logging, and audit trail functions.
*   Power failure and recovery scenarios.

### **3.0 Prerequisites**
*   The Installation Qualification (IQ-HVAC-001) protocol must be successfully executed and approved.
*   All critical deviations from the IQ must be resolved and closed.
*   All test equipment must have valid calibration certificates.

### **4.0 Test Equipment**
| Equipment | Model / Serial # | Calibration Due Date |
| :--- | :--- | :--- |
| Certified Digital Thermometer/Hygrometer | | |
| Certified Differential Pressure Manometer | | |
| Laptop with BAS Engineering Software | | |

### **5.0 Operational Qualification Test Plan**

#### **5.1 Control Loop Verification**
| Test ID | Test Description | Acceptance Criteria | Result (Pass/Fail) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| OQ-CL-01 | **Grade B Temp Control:** Set BAS temp to 70°F. | Room temperature stabilizes at 70°F ± 2°F within 30 minutes. | | |
| OQ-CL-02 | **Grade B Temp Control:** Set BAS temp to 73°F. | Room temperature stabilizes at 73°F ± 2°F within 30 minutes. | | |
| OQ-CL-03 | **Grade C RH Control:** Set BAS RH to 40%. | Room RH stabilizes at 40% ± 5% within 30 minutes. | | |
| OQ-CL-04 | **Grade C RH Control:** Set BAS RH to 50%. | Room RH stabilizes at 50% ± 5% within 30 minutes. | | |
| OQ-CL-05 | **Pressure Cascade Test:** Open/close door between Grade C and Hallway. | Grade C pressure returns to +15 Pa (±3 Pa) relative to Hallway within 60 seconds. | | |

#### **5.2 Alarm and Interlock Verification**
| Test ID | Test Description | Acceptance Criteria | Result (Pass/Fail) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| OQ-ALM-01 | **High Temp Alarm:** Artificially force Grade B temperature reading to 77°F. | A "Major" alarm activates on the HMI. Email is sent to Supervisor. | | |
| OQ-ALM-02 | **Low Pressure Alarm:** Isolate DP sensor for Grade A and vent to atmosphere. | A "Critical" alarm activates. Audible/visual alarms trigger. Auto-dialer is activated. | | |
| OQ-ALM-03 | **HEPA Filter Alarm:** Simulate high differential pressure across a terminal HEPA filter. | A "Critical" alarm activates on the HMI. | | |
| OQ-ALM-04 | **Sensor Failure:** Disconnect the RH sensor for Grade C. | A "Major" alarm for sensor failure/communication loss activates on the HMI. | | |

#### **5.3 Security and Data Integrity Verification**
| Test ID | Test Description | Acceptance Criteria | Result (Pass/Fail) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| OQ-SEC-01 | **Operator Access:** Log in as an Operator. Attempt to change a setpoint. | Access is denied. Operator can view values and acknowledge alarms. | | |
| OQ-SEC-02 | **Supervisor Access:** Log in as a Supervisor. Change Grade C temperature setpoint from 72°F to 71°F. | Change is successful. An entry is created in the audit trail requiring a reason for the change. | | |
| OQ-SEC-03 | **Data Logging:** Verify BAS data log for the duration of the OQ. | All critical parameters (Temp, RH, DP) are logged every 60 seconds without interruption. | | |

#### **5.4 Power Failure and Recovery**
| Test ID | Test Description | Acceptance Criteria | Result (Pass/Fail) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| OQ-PF-01 | **Power Outage Test:** Disconnect main power to the HVAC/BAS control panel. Wait 5 minutes. | System shuts down safely. No data corruption occurs. | | |
| OQ-PF-02 | **Power Recovery Test:** Restore main power to the panel. | PLC and HMI restart automatically. System returns to the last valid control state within 10 minutes without manual intervention. | | |

### **6.0 Deviation Log**
| Deviation # | Test ID | Description of Deviation | Corrective Action | Approved By (QA) |
| :--- | :--- | :--- | :--- | :--- |
| | | | | |

### **7.0 Summary and Conclusion**
**Conclusion:**
[ ] The HVAC and BAS systems **OPERATE** correctly and meet all requirements of this OQ protocol. The system is ready to proceed to Performance Qualification (PQ).
[ ] The system is **NOT APPROVED** for PQ. The critical deviations noted in Section 6.0 must be resolved.

---
**Executed By:** _________________________ (Validation Specialist) **Date:** ___________

**Reviewed By:** _________________________ (Engineering Head) **Date:** ___________

**Approved By:** _________________________ (Quality Assurance) **Date:** ___________
