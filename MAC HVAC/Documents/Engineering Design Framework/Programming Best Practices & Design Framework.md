# **Programming Best Practices & Design Framework**

## **AHU Controller Project**

**Document Purpose:** This document provides a unified set of programming standards and best practices for all engineers developing PLC (back-end) and HMI (front-end) software for the Constant Volume AHU Controller project.

**Guiding Principle:** "Openness and Clarity." Code should be written to be easily understood by another engineer with no prior knowledge of the specific block. Avoid overly complex or "clever" solutions in favor of straightforward, readable logic.

### **1\. General Principles**

* **Clarity Over Cleverness:** Write code that is self-explanatory. If you have to spend five minutes deciphering a line of logic, it is too complex.  
* **Single Responsibility:** Each Function Block (FB) and Function (FC) must have one, and only one, well-defined purpose. For example, EM-100: Supply Fan Control is responsible only for the fan; it should not contain logic for dampers or heating coils.  
* **Do Not Repeat Yourself (DRY):** If you find yourself writing the same sequence of logic in multiple places, encapsulate it into a reusable Function (FC).

### **2\. TIA Portal Project Structure**

A standardized project structure is essential for navigation and maintainability. All project elements shall be organized into logical groups.

* **Program Blocks:**  
  * 01\_Main: Contains OB1 and the main program cycle calls.  
  * 02\_Equipment\_Modules: Contains all Equipment Module FBs (e.g., FB\_EM100\_SupplyFan).  
  * 03\_Functions: Contains all reusable FCs.  
  * 04\_Technology\_Objects: Contains the PID controller blocks (PID\_Compact).  
  * 05\_Data\_Blocks: Contains all global DBs.  
* **PLC Data Types (UDTs):**  
  * All UDTs will be organized in a folder named UDTs.  
* **PLC Tags:**  
  * Organized into specific tag tables (e.g., DI\_Tags, DO\_Tags, AI\_Tags, AO\_Tags).

### **3\. Code Implementation Standards (Back-End)**

#### **3.1. Language Choice**

* **Structured Control Language (SCL):** **Preferred language** for all complex logic, including calculations, state machines, data manipulation, and conditional statements. Its readability is paramount.  
* **Ladder Logic (LAD):** To be used **only** for simple, visually intuitive boolean logic, such as safety chains or basic start/stop permissive circuits where a visual representation is clearer.

#### **3.2. Naming Conventions**

* **Function Blocks (FBs):** FB\_EM\<Number\>\_\<Name\> (e.g., FB\_EM100\_SupplyFan).  
* **Functions (FCs):** FC\_\<Verb\>\<Noun\> (e.g., FC\_ScaleAnalogInput).  
* **Data Blocks (DBs):** Instance DBs are auto-named. Global DBs: DB\_\<Purpose\> (e.g., DB\_SystemSettings).  
* **User-Defined Types (UDTs):** UDT\_EM\<Number\>\_Interface (e.g., UDT\_EM100\_Interface).

#### **3.3. Commenting and Documentation**

* **Block Header:** Every FB and FC must have a completed header comment explaining its **Purpose**, **Functionality**, **Author**, and **Version History**.  
* **Network/Line Comments:** Comments must explain the **"why,"** not the "what." The code itself shows what it is doing. The comment should explain the reasoning or intent behind the logic.  
  * **Bad:** // Add 5 to the variable  
  * **Good:** // Add 5-second offset to account for network latency  
* **Tag Comments:** Every parameter in an FB/FC interface and every tag in a DB must have a clear, descriptive comment.

#### **3.4. Addressing and Data Management**

* **100% Symbolic Addressing:** **No absolute addressing** (e.g., %I0.0, %Q0.1, %M100.0) is permitted within application blocks (FCs/FBs). All I/O must be mapped to descriptive symbolic tags in a central tag table, which are then passed into the appropriate EM.  
* **No "Magic Numbers":** Hardcoded numerical constants are forbidden. All configurable values (setpoints, delays, counts) must be defined as input parameters to the FB, sourced from a global settings DB (e.g., DB\_SystemSettings), or defined as a local Constant if it is truly a fixed, non-configurable value.  
* **Isolate State:** All data and state information related to a specific instance of an EM (e.g., the Supply Fan's run timer) must be stored within its own Instance DB. **Avoid using global memory (M-flags) for instance-specific data.**

### **4\. HMI Development Standards (Front-End)**

#### **4.1. Library and Data Interface**

* **Mandatory Library:** All HMI screens will be built using objects from the **Siemens HMI Library Suite** as specified in the SDS. No custom, non-library objects are to be created without formal review.  
* **Structured Tag Interface:** The HMI must interface with the PLC primarily through the UDT created for each Equipment Module. This ensures a clean, organized, and reusable connection between the front-end and back-end.

#### **4.2. UI/UX Guidelines**

* **Consistency:** The placement of navigation buttons, alarm banners, and status indicators must be identical across all screens. Use the project library's color scheme and object styles without modification.  
* **User Feedback:** All interactive elements (buttons, input fields) must provide clear visual feedback to the user (e.g., a button changes appearance when pressed).  
* **Alarming:** All alarms must be generated from the PLC using the Program\_Alarm instruction. HMI-based alarming is not permitted. This centralizes alarm logic and ensures proper timestamping.

### **5\. Version Control and CI/CD**

#### **5.1. Git Workflow**

* **Branching:** All new work must be done on a feature branch (e.g., feature/EM200-short-cycle-logic).  
* **Commits:** Commits should be small, logical, and atomic. A single commit should represent a single piece of completed work.  
* **Commit Messages:** Messages must follow the Conventional Commits standard (e.g., feat:, fix:, docs:, chore:).  
  * **Example:** fix(EM-100): Correct fan failure alarm logic for airflow switch

#### **5.2. Automated Testing**

* As defined in the SDS, all code pushed to develop or main branches will trigger the automated unit testing workflow via GitHub Actions.  
* **A failing test will block the code from being merged.** It is the programmer's responsibility to ensure their code passes all unit tests before creating a pull request.  
* Before committing, developers must run the siemens/tia-project-exporter tool to convert the proprietary TIA Portal files into their XML representation for effective version control.