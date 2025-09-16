# Operating Manual: TIA Portal XML Source Files

**Version:** 1.1
**Date:** September 14, 2025
**Purpose:** This document provides a formal procedure for importing the XML source files from this directory into a TIA Portal V20 project to generate the required PLC data types, tag tables, and program blocks.

---

## 1. Introduction to TIA Portal Openness

The XML files in this directory are generated using the **TIA Portal Openness** interface. Openness is an API provided by Siemens that allows for the programmatic creation and management of TIA Portal projects. Using XML as an intermediate format allows us to automate code creation and use text-based version control systems like Git.

Each `.xml` file in this directory represents an "external source file" that TIA Portal can use to generate a corresponding project element.

## 2. File Descriptions

This directory contains the structural definitions for the AHU Controller's PLC program.

*   **`UDT*.xml` files:** Define the custom **User Data Types** (structs) that form the data foundation for our Equipment Modules.
*   **`TagTable_*.xml` files:** Define the **PLC Tag Tables**, separated by I/O type (DI, DO, AI, AO). They map symbolic tag names to physical PLC addresses.
*   **`FB*.xml` files:** Define the **Function Blocks** for each Equipment Module. They contain the block's interface which uses the corresponding UDT.

**Current Status:** The FB files in this directory represent the **skeletons** of the blocks. They contain the complete interface but do not yet contain the final SCL control logic.

## 3. Import and Generation Procedure

Follow these steps precisely to ensure the project components are generated correctly. The generation order is critical due to dependencies (FBs depend on Tag Tables and UDTs, etc.).

1.  **Open TIA Portal Project:** Launch TIA Portal V20 and open the target PLC project.

2.  **Navigate to External Source Files:** In the Project Tree, find your PLC station. There is a folder named **"External source files"**.

3.  **Add External Source Files:** Right-click on the "External source files" folder and select **"Add new external source file"**.

4.  **Select All XML Files:** In the file browser, navigate to this `TiaPortal_Export_Files` directory. Select **all** of the `.xml` files. Click **Open**. The files will now appear under the "External source files" folder.

5.  **Generate in Correct Order:** Right-click on the source files and generate them in the following order:
    1.  **Generate UDTs:** Select all `UDT*.xml` files, right-click, and select "Generate blocks from source". Verify their creation under `PLC data types`.
    2.  **Generate Tag Tables:** Select all `TagTable_*.xml` files, right-click, and select "Generate blocks from source". Verify their creation under `PLC tags`.
    3.  **Generate FBs:** Select all `FB*.xml` files, right-click, and select "Generate blocks from source". Verify their creation under `Program blocks`.

You have now successfully imported the complete structural framework of the AHU controller into your TIA Portal project.

---

## 4. Troubleshooting

*   **Issue:** When generating an FB, you get a compile error like "Data type `UDT...` not found."
    *   **Cause:** The required UDT or Tag Table was not generated before the FB that depends on it.
    *   **Solution:** Follow the generation order in Section 3 precisely. Ensure all `UDT*.xml` and `TagTable_*.xml` files are generated successfully *before* attempting to generate the `FB*.xml` files.

*   **Issue:** A source file has a red 'X' next to it after adding it, or you get a generic error during generation.
    *   **Cause:** The XML file may be malformed, or there is a syntax error.
    *   **Solution:** Check the "Info > Compile" window in TIA Portal. It will provide detailed error messages that can help pinpoint the exact problem in the XML source file.
