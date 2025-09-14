# Operating Manual: Python Automated Tests

**Version:** 1.0
**Date:** September 14, 2025
**Purpose:** This document explains how to set up and run the Python-based automated tests for the Equipment Modules against a PLCSIM Advanced instance.

---

## 1. Overview

The Python scripts in this directory are designed to provide automated unit testing for the control logic within each Function Block (FB). They use the `plcsim-adv-api` library to programmatically interact with a running PLCSIM Advanced instance, allowing for rapid, repeatable, and scriptable testing without manual intervention in the TIA Portal environment.

The scripts currently provided use a `MockPLC` class for demonstration purposes. Minor modifications are required to connect them to a live simulation.

## 2. Prerequisites

Before running the tests, ensure the following are installed on your system:

*   **Python 3.x**
*   **pip** (Python package installer)
*   **Siemens PLCSIM Advanced:** A running instance is required to execute the tests against.
*   **plcsim-adv-api Library:** Install this library using pip:
    ```bash
    pip install plcsim-adv-api
    ```

## 3. Configuration

To switch from the `MockPLC` to a live simulation, you will need to edit the target Python script (e.g., `test_em200_cooling.py`):

1.  **Comment out the MockPLC:** Find the line `plc = MockPLC()` and comment it out or delete it.

2.  **Instantiate the Real API:** Uncomment or add the following lines to connect to your PLCSIM Advanced instance.

    ```python
    # Import the library
    from plcsim_adv_api import PLCSIMAdvanced

    # The name of your running PLCSIM Advanced instance
    PLC_INSTANCE_NAME = "Your_PLCSIM_Instance_Name"

    # Create the connection object
    plc = PLCSIMAdvanced(PLC_INSTANCE_NAME)
    plc.connect()
    ```
    *Note: Replace `"Your_PLCSIM_Instance_Name"` with the actual name of your virtual PLC instance.*

## 4. Execution

1.  **Open a terminal** or command prompt.
2.  **Navigate** to this directory (`MAC HVAC/TestSuiteFiles/`).
3.  **Run the desired test script** using Python:
    ```bash
    python test_em200_cooling.py
    ```

## 5. Interpreting Results

The script will print its progress to the console.

*   A **`PASSED`** message at the end of each test case indicates that all assertions within that test were successful.
*   An **`AssertionError`** message indicates a test failure. The message will specify which check failed (e.g., `AssertionError: Compressor should stop on HP fault`). This means the PLC logic did not behave as expected.
*   The script will print a final "Testing complete" message when it has finished running all test functions.

---

## 6. Troubleshooting

*   **Issue:** `ModuleNotFoundError: No module named 'plcsim_adv_api'`
    *   **Cause:** The required Python library for communicating with PLCSIM Advanced is not installed in your Python environment.
    *   **Solution:** Run `pip install plcsim-adv-api` from your terminal, as described in the "Prerequisites" section.

*   **Issue:** The script fails with a connection error or cannot find the PLC instance.
    *   **Cause 1:** The PLCSIM Advanced virtual PLC instance is not running.
    *   **Solution 1:** Open the "SIMATIC PLCSIM Advanced" control panel and start your virtual PLC.
    *   **Cause 2:** The `PLC_INSTANCE_NAME` variable in the Python script does not exactly match the name of the running virtual PLC instance.
    *   **Solution 2:** Ensure the name in the script matches the name in the PLCSIM Advanced control panel.

*   **Issue:** A test fails with an `AssertionError`.
    *   **Cause:** The logic in the PLC program is not behaving as the test expects.
    *   **Solution:** This is a genuine test failure. Use the error message to identify the failing test case. Review the SCL code in the corresponding Function Block and the logic in the test script to debug the issue.
