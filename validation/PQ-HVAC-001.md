### **Performance Qualification (PQ) Protocol: HVAC System**

**Document ID:** PQ-HVAC-001
**Version:** 1.0
**Date:** 2025-09-19

| **Role** | **Name** | **Signature** | **Date** |
| :--- | :--- | :--- | :--- |
| **Author:** | (Senior Validation Specialist) | | |
| **Reviewed By:** | (Engineering Head) | | |
| **Approved By:** | (Quality Assurance Head) | | |

### **1.0 Purpose**
The purpose of this Performance Qualification (PQ) protocol is to provide final, documented verification that the HVAC system for the Grade A, B, and C cleanrooms can reliably and repeatedly maintain the required environmental conditions under normal, "at-rest" operational conditions over an extended period.

### **2.0 Scope**
This PQ protocol covers the performance testing of the HVAC system over a defined study period. The tests will confirm the consistent achievement of all critical process parameters as defined in the Functional Specification (FS-HVAC-001).

### **3.0 Prerequisites**
*   The Operational Qualification (OQ-HVAC-001) protocol must be successfully executed and approved.
*   All critical deviations from the OQ must be resolved and closed.
*   All personnel performing tests must be trained on this protocol.
*   All rooms must be cleaned and in their defined "at-rest" state.

### **4.0 Performance Qualification Test Plan**

#### **4.1 Long-Term Environmental Monitoring**
*   **Test Description:** This test will monitor the temperature, relative humidity, and differential pressure in all classified areas continuously over a period of **10 consecutive days**. Data will be logged by the validated BAS and verified by independent, calibrated data loggers placed in each room.
*   **Test Conditions:** The facility will be in a static, "at-rest" state (i.e., all equipment running, no personnel present).
*   **Acceptance Criteria:**
    1.  All monitored parameters (Temperature, RH, Differential Pressure) as recorded by the BAS must remain within their specified **Action Limits** (per FS 5.1) for 100% of the 10-day study period.
    2.  No parameter may exceed its specified **Alarm Limit** at any time.
    3.  The data from the independent loggers must not deviate from the corresponding BAS sensor readings by more than ±0.5°C for temperature and ±2.5% for RH.

#### **4.2 Cleanroom Classification and Recovery Testing**
These tests will be performed during the 10-day monitoring period.

| Test ID | Test Description | Acceptance Criteria | Result (Pass/Fail) | Comments |
| :--- | :--- | :--- | :--- | :--- |
| PQ-AIR-01 | **Airflow Visualization (Smoke Study):** Introduce smoke at critical locations in each room. | Airflow is demonstrated to be unidirectional and non-turbulent in the Grade A area. Air is shown to sweep effectively from clean areas to less-clean areas in Grades B and C. No "dead spots" are observed. | | |
| PQ-AIR-02 | **Room Recovery Rate:** After generating a high concentration of non-viable particles, measure the time required for the room to return to its specified classification level. | The room must recover to its classification level (e.g., ISO 7 for Grade B) within 15-20 minutes. | | |
| PQ-AIR-03 | **Non-Viable Particulate Count (At-Rest):** Perform particle counting at multiple locations within each room. | Particle counts for both ≥0.5 µm and ≥5.0 µm particles must meet the limits for the room's classification (e.g., ISO 5 for Grade A, ISO 7 for Grade B, ISO 8 for Grade C). | | |
| PQ-AIR-04 | **Viable Particulate Monitoring (At-Rest):** Perform microbial monitoring using active air samplers and settle plates. | Results must meet the cGMP limits for viable counts for the room's classification. | | |

### **5.0 Data Sheets**
(Attachment) - Raw data from the BAS, independent data loggers, and reports from the particle counting, airflow visualization, and microbial monitoring tests will be attached to the final report.

### **6.0 Deviation Log**
| Deviation # | Test ID | Description of Deviation | Root Cause and Impact Assessment | Corrective Action |
| :--- | :--- | :--- | :--- | :--- |
| | | | | |

### **7.0 Summary and Conclusion**
Upon completion of all tests and resolution of any deviations, a final conclusion will be recorded.

**Conclusion:**
[ ] The HVAC and BAS systems have **PERFORMED** successfully and consistently meet all requirements of this PQ protocol. The system is fully qualified and approved for operational use.
[ ] The system is **NOT APPROVED** for operational use. The critical deviations noted in Section 6.0 must be resolved and the affected tests must be repeated.

---
**Executed By:** _________________________ (Validation Specialist) **Date:** ___________

**Reviewed By:** _________________________ (Engineering Head) **Date:** ___________

**Approved By:** _________________________ (Quality Assurance) **Date:** ___________
