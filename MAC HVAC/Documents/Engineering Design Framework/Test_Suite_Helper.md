# TIA Portal Test Suite - Helper Guide

This guide provides a quick, step-by-step walkthrough for creating your first unit test for an Equipment Module using the project standards.

**Objective:** Create a simple test for the `FB100_EM_SupplyFan` to verify its normal start sequence.

**1. Open the Test Suite**
   - In the TIA Portal Project Tree, navigate to `Program Blocks > EMs`.
   - Right-click the `FB100_EM_SupplyFan` block.
   - Select **"Create/open test"**. The Test Suite editor will open.

**2. Create a New Test Case**
   - In the Test Suite editor window, click the **"New test case"** button in the toolbar.
   - A new test will appear in the list. Select it and rename it to something descriptive, like `TC1_Normal_Start_Sequence`.

**3. Define the Test Steps in the Table**
   - Use the table editor to add rows. Each row is either an action (setting an input) or an evaluation (checking an output).
   - Recreate the following sequence:

| Test ID | Test Step Name | Parameter | Value | Comment |
| :--- | :--- | :--- | :--- | :--- |
| 1.0 | **Set Initial State** | `#Instance_DB.Enable` | `FALSE` | Ensure the block is disabled initially. |
| 2.0 | **Command Fan ON** | `#Instance_DB.Enable` | `TRUE` | This is the action we are testing. |
| 2.1 | *Evaluate Output* | `#Instance_DB.UDT.Start_Cmd_DO` | `TRUE` | Check if the output command turns on. |
| 3.0 | **Simulate Feedback** | `#Instance_DB.UDT.Run_Fdbk_DI`| `TRUE` | Simulate the VFD confirming it is running. |
| 3.1 | *Evaluate Status* | `#Instance_DB.UDT.Is_Running` | `TRUE` | Check if the block's status updates correctly. |


**4. Run the Test**
   - Click the **"Run test(s)"** button (green play icon) in the Test Suite toolbar.
   - The test will execute in the background using PLCSIM (if configured).

**5. Check the Result**
   - A **green checkmark** next to your test case name indicates that all evaluation steps passed.
   - A **red 'X'** indicates a failure. The specific step that failed will be highlighted in red, showing the expected value vs. the actual value.
