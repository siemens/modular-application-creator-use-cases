# TIA Portal Export Files

### Purpose of These Files

This directory contains the structural definitions for the RTU Controller's PLC program, exported in the TIA Portal Openness XML format. Each file represents a component that can be imported into a TIA Portal project.

*   **`UDT*.xml` files:** These define the custom **User Data Types** (also known as PLC Data Types or structs) that form the data foundation for our Equipment Modules.
*   **`FB*.xml` files:** These define the **Function Blocks** for each Equipment Module. They contain the block's interface (Inputs, Outputs, Statics) which uses the corresponding UDT.

**Current Status:** These files represent the **skeletons** of the blocks. They do not yet contain the final SCL control logic.

### How to Use These Files

To import these components into your TIA Portal project, follow these steps:

1.  **Add External Source Files:** In the TIA Portal Project Tree, right-click on the `Program blocks` folder inside your PLC.
2.  Select **"Add new external source file"**.
3.  Navigate to this directory and select all the `.xml` files. Click **Open**. This will link the source files to your project.
4.  **Generate Blocks from Source:** Right-click on one of the newly added source files (e.g., `UDT100_EM_SupplyFan.xml`) and select **"Generate block from source"**.
5.  Repeat the generation for all source files. TIA Portal will process the XML and create the corresponding UDTs and FBs in your project.
