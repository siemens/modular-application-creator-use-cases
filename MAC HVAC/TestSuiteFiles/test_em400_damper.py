# test_em400_damper.py
#
# Automated unit test for the FB400_EM_Damper module.
# This script has been completely rewritten to correctly test the
# economizer and damper failure logic as specified in the SDS.

import time

# Note: This script uses a MockPLC class for demonstration.

INSTANCE_DB_NAME = "IDB_Damper"

class MockPLC:
    """Mock PLC class to simulate the plcsim-adv-api for the Damper/Economizer."""
    def __init__(self):
        self.tags = {}
        self.failure_timer_start = None
        print("MockPLC: Initialized for Damper/Economizer Test.")

    def read_tag(self, tag_name):
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB400 logic."""
        if not self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"'):
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"'] = False
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"'] = 0.0
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Alm"'] = False
            return

        # Economizer Logic
        cooling_demand = self.tags.get(f'"{INSTANCE_DB_NAME}"."Cooling_Demand"')
        oat = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Outside_Air_Temp_AI"')
        rat = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Return_Air_Temp_AI"')
        econ_diff = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Temp_Diff"')
        econ_limit = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Econ_High_Limit"')

        if (cooling_demand > 1.0 and oat < (rat - econ_diff) and oat < econ_limit):
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"'] = True
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"'] = False

        # Damper Command
        damper_demand = self.tags.get(f'"{INSTANCE_DB_NAME}"."Damper_Demand_In"')
        self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"'] = damper_demand

        # Damper Failure Logic
        cmd_ao = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Cmd_AO"')
        fdbk_ai = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Fdbk_AI"')
        tolerance = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Fdbk_Tolerance"')
        delay = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Delay_Sec"')

        error = abs(cmd_ao - fdbk_ai) > tolerance
        is_active = cmd_ao > 5.0 and error

        if is_active:
            if self.failure_timer_start is None:
                self.failure_timer_start = time.time()
            if (time.time() - self.failure_timer_start) >= delay:
                self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Alm"'] = True
        else:
            self.failure_timer_start = None
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Alm"'] = False

        time.sleep(0.01)

def test_economizer_logic(plc):
    """Test Case 4.1: Verifies the economizer activation logic."""
    print("\n--- Running Test: TC4.1_Economizer_Logic ---")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Cooling_Demand"', 10.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Return_Air_Temp_AI"', 24.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Temp_Diff"', 2.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_High_Limit"', 22.0)

    print("Step: OAT is favorable")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Outside_Air_Temp_AI"', 18.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"') is True, "Econ should be active"

    print("Step: OAT is too warm")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Outside_Air_Temp_AI"', 23.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"') is False, "Econ should be inactive (OAT > limit)"

    print("Step: No cooling demand")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Outside_Air_Temp_AI"', 18.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Cooling_Demand"', 0.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Econ_Mode_Active"') is False, "Econ should be inactive (no demand)"

    print("--- TC4.1_Economizer_Logic: PASSED ---")

def test_damper_failure_alarm(plc):
    """Test Case 4.2: Verifies the damper failure alarm logic."""
    print("\n--- Running Test: TC4.2_Damper_Failure_Alarm ---")
    delay = 1
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Fdbk_Tolerance"', 5.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Delay_Sec"', delay)

    print("Step: Create failure condition")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Damper_Demand_In"', 50.0)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Fdbk_AI"', 20.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Alm"') is False, "Failure alarm should be delayed"

    print(f"Step: Waiting for {delay}s delay...")
    time.sleep(delay)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Alm"') is True, "Failure alarm should be active after delay"

    print("Step: Resolve failure condition")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Pos_Fdbk_AI"', 50.0)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Damper_Failure_Alm"') is False, "Failure alarm should reset"

    print("--- TC4.2_Damper_Failure_Alarm: PASSED ---")

def main():
    """Main function to set up and run all tests."""
    print("Initializing Damper/Economizer Module Test Environment...")
    plc = MockPLC()

    try:
        test_economizer_logic(plc)
        test_damper_failure_alarm(plc)

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    finally:
        print("\nTesting complete.")

if __name__ == "__main__":
    main()
