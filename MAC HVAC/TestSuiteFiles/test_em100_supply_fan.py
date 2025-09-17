# test_em100_supply_fan.py
#
# Automated unit test for the FB100_EM_SupplyFan module.
# This script uses the plcsim-adv-api to interact with a PLCSIM Advanced instance,
# providing a template for automated CI/CD testing.

import time

# Note: The 'plcsim-adv-api' library would need to be installed in the
# test environment (e.g., via pip install plcsim-adv-api).
# The following is a conceptual script that demonstrates the testing logic.
# A real implementation would require a connection object to a running PLCSIM Adv instance.

# --- Test Configuration ---
PLC_INSTANCE_NAME = "AHU_Controller_Sim"
INSTANCE_DB_NAME = "IDB_SupplyFan"  # The name of the instance DB for the fan module

class MockPLC:
    """
    A mock PLC class to simulate the plcsim-adv-api for demonstration purposes.
    In a real scenario, this would be replaced by the actual API object.
    """
    def __init__(self):
        self.tags = {}
        print("MockPLC: Initialized.")

    def read_tag(self, tag_name):
        print(f"READ:  '{tag_name}' -> {self.tags.get(tag_name, 'Not Found')}")
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        print(f"WRITE: '{tag_name}' <- {value}")
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB logic based on current inputs."""
        print("--- PLC CYCLE ---")
        # Simulate FB logic here based on self.tags
        # Example: If Enable is true and no faults, Start_Cmd_DO should be true.
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        vfd_fault = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"')

        if vfd_fault:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"'] = False
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_Alm"'] = True
        elif enable:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"'] = True
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"'] = False

        # ... add more logic for other alarms and statuses ...
        time.sleep(0.1)


def test_normal_start_stop(plc):
    """Test Case 1: Verifies a normal start/stop sequence."""
    print("\n--- Running Test: TC1_Normal_Start_Stop ---")

    # 1. Initial state: Ensure module is disabled
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False

    # 2. Command ON
    print("Step: Command ON")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is True

    # 3. Command OFF
    print("Step: Command OFF")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False

    print("--- TC1_Normal_Start_Stop: PASSED ---")


def test_vfd_fault_condition(plc):
    """Test Case 2: Verifies a VFD fault immediately shuts down the fan."""
    print("\n--- Running Test: TC2_VFD_Fault ---")

    # 1. Initial state: Fan is commanded to run, no faults
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is True

    # 2. Simulate VFD Fault
    print("Step: Simulate VFD Fault")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_Alm"') is True
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False

    print("--- TC2_VFD_Fault: PASSED ---")


def main():
    """Main function to set up the environment and run all tests."""
    print("Initializing Test Environment...")
    # In a real CI/CD pipeline, you would connect to a PLCSIM Adv instance here.
    # from plcsim_adv_api import PLCSIMAdvanced
    # plc = PLCSIMAdvanced()
    # plc.start_instance(PLC_INSTANCE_NAME)

    # For this example, we use the MockPLC
    plc = MockPLC()

    try:
        # Run all test cases
        test_normal_start_stop(plc)
        test_vfd_fault_condition(plc)
        # Add calls to other test functions (e.g., test_fan_failure) here.

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    except Exception as e:
        print(f"\n!!! AN UNEXPECTED ERROR OCCURRED: {e} !!!")
    finally:
        # plc.set_stop()
        # plc.close_instance()
        print("\nTesting complete.")


if __name__ == "__main__":
    main()
