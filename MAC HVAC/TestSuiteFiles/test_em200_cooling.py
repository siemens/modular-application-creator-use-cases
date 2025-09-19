# test_em200_cooling.py
#
# Automated unit test for the FB200_EM_Cooling module.
# This script has been completely rewritten to correctly test the
# chilled water valve logic as specified in the SDS.

import time

# Note: This script uses a MockPLC class for demonstration.
# A real implementation would connect to a running PLCSIM Advanced instance.

INSTANCE_DB_NAME = "IDB_Cooling"

class MockPLC:
    """Mock PLC class to simulate the plcsim-adv-api for the CHW Valve."""
    def __init__(self):
        self.tags = {}
        self.failure_timer_start = None
        print("MockPLC: Initialized for Chilled Water Cooling Test.")

    def read_tag(self, tag_name):
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB200 logic based on current inputs."""
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        valve_demand = self.tags.get(f'"{INSTANCE_DB_NAME}"."Valve_Demand_In"')
        freeze_di = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Stat_DI"')

        if not enable:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"'] = 0.0
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Alm"'] = False
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Failure_Alm"'] = False
            return

        if freeze_di:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"'] = 0.0
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Alm"'] = True
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Alm"'] = False
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"'] = valve_demand

            # Simulate Valve Failure Logic
            cmd_ao = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"')
            fdbk_ai = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Fdbk_AI"')
            tolerance = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Valve_Fdbk_Tolerance"')
            delay = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Valve_Failure_Delay_Sec"')

            error = abs(cmd_ao - fdbk_ai) > tolerance
            is_active = cmd_ao > 5.0 and error

            if is_active:
                if self.failure_timer_start is None:
                    self.failure_timer_start = time.time()
                if (time.time() - self.failure_timer_start) >= delay:
                    self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Failure_Alm"'] = True
            else:
                self.failure_timer_start = None
                self.tags[f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Failure_Alm"'] = False

        time.sleep(0.01) # Short delay to simulate cycle

def test_normal_operation(plc):
    """Test Case 2.1: Verifies normal valve modulation."""
    print("\n--- Running Test: TC2.1_Normal_Operation ---")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Stat_DI"', False)

    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Valve_Demand_In"', 50.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"') == 50.0, "Valve command should match demand"

    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Valve_Demand_In"', 0.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"') == 0.0, "Valve command should be 0"

    print("--- TC2.1_Normal_Operation: PASSED ---")

def test_freeze_stat_safety(plc):
    """Test Case 2.2: Verifies freeze stat safety trip."""
    print("\n--- Running Test: TC2.2_Freeze_Stat_Safety ---")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Valve_Demand_In"', 100.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Stat_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"') == 100.0

    print("Step: Activate Freeze Stat")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Stat_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Cmd_AO"') == 0.0, "Valve should close on freeze stat"
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Alm"') is True, "Freeze alarm should be active"

    print("--- TC2.2_Freeze_Stat_Safety: PASSED ---")

def test_valve_failure_alarm(plc):
    """Test Case 2.3: Verifies the valve failure alarm logic."""
    print("\n--- Running Test: TC2.3_Valve_Failure_Alarm ---")
    delay = 1 # seconds
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Freeze_Stat_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Valve_Fdbk_Tolerance"', 5.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Valve_Failure_Delay_Sec"', delay)

    print("Step: Create failure condition")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Valve_Demand_In"', 80.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Fdbk_AI"', 50.0) # Feedback mismatch
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Failure_Alm"') is False, "Failure alarm should be delayed"

    print(f"Step: Waiting for {delay}s delay...")
    time.sleep(delay)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Failure_Alm"') is True, "Failure alarm should be active after delay"

    print("Step: Resolve failure condition")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Fdbk_AI"', 80.0) # Feedback matches
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.CHW_Valve_Failure_Alm"') is False, "Failure alarm should reset"

    print("--- TC2.3_Valve_Failure_Alarm: PASSED ---")

def main():
    """Main function to set up and run all tests."""
    print("Initializing Chilled Water Cooling Module Test Environment...")
    plc = MockPLC()

    try:
        test_normal_operation(plc)
        test_freeze_stat_safety(plc)
        test_valve_failure_alarm(plc)

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    finally:
        print("\nTesting complete.")

if __name__ == "__main__":
    main()
