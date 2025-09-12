# TIA Portal - Project Styleguide Standard

**Version:** 1.0
**Date:** September 12, 2025
**Purpose:** To define the ruleset for the TIA Portal Styleguide Check. Adherence to this standard is mandatory to ensure code consistency, readability, and long-term maintainability across the project.

---

## 1. Naming Conventions

The Styleguide Check shall enforce the following naming conventions.

### 1.1. Logic Blocks
*   **Function Blocks (FBs):** `FB<Number>_<Name>` (e.g., `FB100_EM_SupplyFan`)
*   **Functions (FCs):** `FC<Number>_<Name>` (e.g., `FC50_Calculate_Average`)
*   **Organization Blocks (OBs):** Default names are acceptable (e.g., `Main`, `CycleInterrupt`).

### 1.2. Data Blocks
*   **Instance DBs:** `IDB_<InstanceName>` (e.g., `IDB_SupplyFan`)
*   **Global DBs:** `DB_<Purpose>` (e.g., `DB_Global_Parameters`)

### 1.3. User Data Types (UDTs)
*   **Prefix:** `UDT`
*   **Numbering:** Should match the corresponding FB number where applicable.
*   **Name:** PascalCase, descriptive name.
*   **Example:** `UDT100_EM_SupplyFan`

### 1.4. PLC Tags & UDT Parameters
*   **Case:** PascalCase (e.g., `StartCmd`, `RunFeedback`).
*   **Suffixes:** Use standard suffixes to indicate signal type and source. This improves readability and clarifies intent.
    *   `_DI`: Digital Input
    *   `_DO`: Digital Output
    *   `_AI`: Analog Input
    *   `_AO`: Analog Output
    *   `_Alm`: Alarm (`Bool`)
    *   `_Flt`: Fault (`Bool`)
    *   `_SP`: Setpoint
    *   `_PV`: Process Variable
    *   `_Sec`: Time value in seconds
*   **Example:** `Run_Fdbk_DI`, `Min_Off_Time_Sec`, `DAT_Cooling_SP`

## 2. Block Properties

### 2.1. Required Fields
*   **Author:** The "Author" property of every block must be set to **"Dallas Levine"**.
*   **Header Comment:** Every block (FB, FC) must have a header comment in the "Comment" property that briefly explains its purpose.

### 2.2. Programming Language
*   All new logic blocks must be programmed in **SCL (Structured Control Language)**. LAD/FBD are not permitted for new control logic to ensure consistency and improve readability for complex operations.

## 3. Programming Rules

### 3.1. Prohibited Practices
*   **Absolute Addressing:** All access to PLC tags and DB variables must be symbolic. Absolute addressing (e.g., `%I0.0`, `%Q0.1`, `DB10.DBX0.0`) is strictly forbidden within logic blocks.
*   **Magic Numbers:** Do not use hard-coded numerical values directly in the code. Use named constants or UDT configuration parameters instead.
    *   **Incorrect:** `IF #temperature > 30.0 THEN ...`
    *   **Correct:** `IF #temperature > #High_Temp_SP THEN ...`

### 3.2. Code Structure
*   **Code Comments:** Complex algorithms or non-obvious logic steps must be commented to explain the 'why', not just the 'what'.
*   **Regions:** Use `#region` and `#endregion` pragmas to structure longer SCL blocks into logical sections (e.g., "Fault Handling", "State Machine", "Outputs").

## 4. Styleguide Check Execution

The TIA Portal Styleguide Check tool must be run on all new or modified blocks before committing code. A "Pass" result against this defined ruleset is required for a pull request to be approved.
