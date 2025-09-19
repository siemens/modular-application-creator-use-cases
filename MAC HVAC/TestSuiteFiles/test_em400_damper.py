# test_em400_damper.py
#
# Automated unit test for the FB400_EM_Damper module.

import time

# Note: This script uses a MockPLC class for demonstration.
# The logic for economizer mode is simplified as it depends on a main control program.

INSTANCE_DB_NAME = "IDB_Damper"

class MockPLC:
    """Mock PLC class to simulate the plcsim-adv-api."""
    def __init__(self):
        self.tags = {}
        print("MockPLC: Initialized for Damper/Economizer Test.")

    def read_tag(self, tag_name):
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB logic."""
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        econ_active = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"')
        min_pos = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Min_Fresh_Air_Pos"')

        if enable:
            if econ_active:
                # In a real scenario, a PID would calculate this value.
                # We'll simulate a modulating value.
                self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"'] = 75.5
            else:
                self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"'] = min_pos
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"'] = 0.0

        time.sleep(0.1)

def test_minimum_ventilation(plc):
    """Test Case 4.1: Verifies damper goes to minimum position."""
    print("\n--- Running Test: TC4.1_Minimum_Ventilation ---")

    # 1. Initial state
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Min_Fresh_Air_Pos"', 20.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"', False)
    plc.run_cycle()

    # 2. Evaluate
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"') == 20.0, "Damper should be at min position"

    print("--- TC4.1_Minimum_Ventilation: PASSED ---")

def test_economizer_activation(plc):
    """Test Case 4.2: Verifies damper modulates in economizer mode."""
    print("\n--- Running Test: TC4.2_Economizer_Activation ---")

    # 1. Initial state
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Min_Fresh_Air_Pos"', 20.0)
    # Note: In the real system, other logic sets Econ_Mode_Active. Here we force it.
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"', True)
    plc.run_cycle()

    # 2. Evaluate
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"') > 20.0, "Damper should be modulating above min"

    # 3. Deactivate economizer
    print("Step: Deactivate Economizer")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"') == 20.0, "Damper should return to min"

    print("--- TC4.2_Economizer_Activation: PASSED ---")


def main():
    """Main function to set up and run all tests."""
    print("Initializing Damper/Economizer Module Test Environment...")
    plc = MockPLC()

    try:
        test_minimum_ventilation(plc)
        test_economizer_activation(plc)

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    finally:
        print("\nTesting complete.")

if __name__ == "__main__":
    main()
