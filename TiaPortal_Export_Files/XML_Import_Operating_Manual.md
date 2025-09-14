# Operating Manual: TIA Portal XML Import

**Version:** 1.0
**Date:** September 14, 2025
**Purpose:** This document provides a formal procedure for importing the XML source files from this directory into a TIA Portal V20 project to generate the required PLC data types and program blocks.

---

## 1. Introduction to TIA Portal Openness

The XML files in this directory are generated using the **TIA Portal Openness** interface. Openness is an API (Application Programming Interface) provided by Siemens that allows for the programmatic creation, modification, and management of TIA Portal projects and their components.

Using XML as an intermediate format allows us to:
*   **Automate Creation:** Generate code from external tools or scripts.
*   **Version Control:** Store human-readable, text-based representations of PLC code in version control systems like Git.
*   **Ensure Standardization:** Programmatically create blocks that conform to a defined standard.

Each `.xml` file in this directory represents an "external source file" that TIA Portal can use to generate a corresponding project element.

## 2. File Descriptions

*   **`UDT*.xml` files:** These files define the custom **User Data Types** (also known as PLC Data Types or `structs`). These are the foundational data structures for our Equipment Modules and must be imported and generated first.
*   **`FB*.xml` files:** These files define the **Function Blocks** for each Equipment Module. They contain the block's interface (Inputs, Outputs, Statics) which uses the corresponding UDT.

**Current Status:** The FB files in this directory represent the **skeletons** of the blocks. They contain the complete interface but do not yet contain the final SCL control logic.

## 3. Import and Generation Procedure

Follow these steps precisely to ensure the project components are generated correctly.

1.  **Open TIA Portal Project:** Launch TIA Portal V20 and open the target PLC project.

2.  **Navigate to External Source Files:** In the Project Tree, find your PLC station. There is a folder named **"External source files"**.

3.  **Add External Source Files:** Right-click on the "External source files" folder and select **"Add new external source file"** from the context menu.

4.  **Select All XML Files:** In the file browser, navigate to this `TiaPortal_Export_Files` directory. Select **all** of the `.xml` files (`UDT*` and `FB*`). Click **Open**. The files will now appear under the "External source files" folder.

5.  **Generate UDTs First:** It is critical to generate the data types *before* the blocks that use them.
    *   Select all the `UDT*.xml` source files.
    *   Right-click on the selection.
    *   Select **"Generate blocks from source"**.
    *   TIA Portal will compile the UDTs. Check the "Info > Compile" window for any errors. The UDTs will now appear in your project under `PLC data types`.

6.  **Generate FBs:**
    *   Select all the `FB*.xml` source files.
    *   Right-click on the selection.
    *   Select **"Generate blocks from source"**.
    *   TIA Portal will compile the FBs. They will now appear in your project under `Program blocks`.

You have now successfully imported the complete structural framework of the RTU controller into your TIA Portal project.
