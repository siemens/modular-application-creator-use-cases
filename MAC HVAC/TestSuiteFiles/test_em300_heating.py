# test_em300_heating.py
#
# Automated unit test for the FB300_EM_Heating module.

import time

# Note: This script uses a MockPLC class for demonstration.

INSTANCE_DB_NAME = "IDB_Heating"

class MockPLC:
    """Mock PLC class to simulate the plcsim-adv-api."""
    def __init__(self):
        self.tags = {}
        print("MockPLC: Initialized for Heating Test.")

    def read_tag(self, tag_name):
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB logic."""
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        high_limit = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_DI"')

        # --- Alarm Logic ---
        self.tags[f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_Alm"'] = high_limit

        # --- Command Logic ---
        if enable and not high_limit:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"'] = True
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"'] = False

        time.sleep(0.1)

def test_normal_operation(plc):
    """Test Case 3.1: Verifies normal start/stop sequence."""
    print("\n--- Running Test: TC3.1_Normal_Operation ---")

    # 1. Initial state
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_DI"', False)
    plc.run_cycle()

    # 2. Command ON
    print("Step: Command ON")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"') is True, "Heater should start"

    # 3. Command OFF
    print("Step: Command OFF")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"') is False, "Heater should stop"

    print("--- TC3.1_Normal_Operation: PASSED ---")

def test_safety_trip(plc):
    """Test Case 3.2: Verifies the high-limit safety cutout."""
    print("\n--- Running Test: TC3.2_Safety_Trip ---")

    # 1. Initial state: running
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"') is True

    # 2. Test High Limit Fault
    print("Step: Test High Limit Fault")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"') is False, "Heater should stop on high limit fault"
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_Alm"') is True, "High Limit Alarm should be active"

    # Reset
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.High_Limit_Alm"') is False, "High Limit Alarm should reset"
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Heat_Stage1_Cmd_DO"') is True, "Heater should restart"

    print("--- TC3.2_Safety_Trip: PASSED ---")

def main():
    """Main function to set up and run all tests."""
    print("Initializing Heating Module Test Environment...")
    plc = MockPLC()

    try:
        test_normal_operation(plc)
        test_safety_trip(plc)

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    finally:
        print("\nTesting complete.")

if __name__ == "__main__":
    main()
