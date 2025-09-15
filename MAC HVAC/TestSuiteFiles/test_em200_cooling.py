# test_em200_cooling.py
#
# Automated unit test for the FB200_EM_Cooling module.

import time

# Note: This script uses a MockPLC class for demonstration.
# A real implementation would connect to a running PLCSIM Advanced instance.

INSTANCE_DB_NAME = "IDB_Cooling"

class MockPLC:
    """Mock PLC class to simulate the plcsim-adv-api."""
    def __init__(self):
        self.tags = {}
        print("MockPLC: Initialized for Cooling Test.")

    def read_tag(self, tag_name):
        # print(f"READ:  '{tag_name}' -> {self.tags.get(tag_name, 'Not Found')}")
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        # print(f"WRITE: '{tag_name}' <- {value}")
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB logic based on current inputs."""
        # This mock logic is simplified. A real test would rely on the PLC's execution.
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        hp_fault = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.HP_Switch_DI"')
        lp_fault = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.LP_Switch_DI"')
        freeze_fault = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Freeze_Stat_DI"')

        # --- Alarm Logic ---
        self.tags[f'"{INSTANCE_DB_NAME}"."UDT.HP_Fault_Alm"'] = hp_fault
        self.tags[f'"{INSTANCE_DB_NAME}"."UDT.LP_Fault_Alm"'] = lp_fault
        self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Freeze_Stat_Alm"'] = freeze_fault

        any_fault = hp_fault or lp_fault or freeze_fault

        # --- Command Logic ---
        if enable and not any_fault:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"'] = True
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"'] = False

        time.sleep(0.1)

def test_normal_operation(plc):
    """Test Case 2.1: Verifies normal start/stop sequence."""
    print("\n--- Running Test: TC2.1_Normal_Operation ---")

    # 1. Initial state
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.HP_Switch_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.LP_Switch_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Freeze_Stat_DI"', False)
    plc.run_cycle()

    # 2. Command ON
    print("Step: Command ON")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"') is True, "Compressor should start"

    # 3. Command OFF
    print("Step: Command OFF")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"') is False, "Compressor should stop"

    print("--- TC2.1_Normal_Operation: PASSED ---")

def test_safety_trips(plc):
    """Test Case 2.3: Verifies safety cutouts."""
    print("\n--- Running Test: TC2.3_Safety_Trips ---")

    # 1. Initial state: running
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.HP_Switch_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.LP_Switch_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Freeze_Stat_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"') is True

    # 2. Test High Pressure Fault
    print("Step: Test HP Fault")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.HP_Switch_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"') is False, "Compressor should stop on HP fault"
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.HP_Fault_Alm"') is True, "HP Alarm should be active"

    # Reset
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.HP_Switch_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.HP_Fault_Alm"') is False, "HP Alarm should reset"
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Compressor_Cmd_DO"') is True, "Compressor should restart"

    print("--- TC2.3_Safety_Trips: PASSED ---")


def main():
    """Main function to set up and run all tests."""
    print("Initializing Cooling Module Test Environment...")
    plc = MockPLC()

    try:
        test_normal_operation(plc)
        test_safety_trips(plc)
        # Note: Testing time delays accurately requires a more advanced mock or real PLC.
        print("\nNOTE: Short-cycle prevention test (TC2.2) was not implemented in this mock script.")

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    finally:
        print("\nTesting complete.")

if __name__ == "__main__":
    main()
