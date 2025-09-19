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
        self.fault_timer_start = None
        print("MockPLC: Initialized for Supply Fan Test.")

    def read_tag(self, tag_name):
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB100 logic based on current inputs."""
        # --- Read Inputs ---
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        vfd_fault_di = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"')
        run_fdbk_di = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Run_Fdbk_DI"')
        airflow_di = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Airflow_Status_DI"')
        fault_delay = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Fault_Delay_Sec"')

        # --- VFD Fault Logic (Highest Priority) ---
        if vfd_fault_di:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_Alm"'] = True

        # Alarms can only be reset when the fan is disabled
        if not enable:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"'] = False
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_Alm"'] = False

        vfd_fault_alm = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_Alm"')
        fan_fail_alm = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"')

        # --- Start/Stop Command Logic ---
        start_cmd = enable and not vfd_fault_alm and not fan_fail_alm
        self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"'] = start_cmd

        # --- Fan Failure Timer Logic ---
        proof_of_run = run_fdbk_di and airflow_di
        timer_active = start_cmd and not proof_of_run

        if timer_active:
            if self.fault_timer_start is None:
                self.fault_timer_start = time.time()
            if (time.time() - self.fault_timer_start) >= fault_delay:
                self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"'] = True
        else:
            self.fault_timer_start = None

        time.sleep(0.01)


def test_normal_start_stop(plc):
    """Test Case 1: Verifies a normal start/stop sequence."""
    print("\n--- Running Test: TC1_Normal_Start_Stop ---")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Run_Fdbk_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Airflow_Status_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False

    print("Step: Command ON")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is True

    print("Step: Command OFF")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False

    print("--- TC1_Normal_Start_Stop: PASSED ---")


def test_vfd_fault_condition(plc):
    """Test Case 2: Verifies a VFD fault immediately shuts down the fan."""
    print("\n--- Running Test: TC2_VFD_Fault ---")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is True

    print("Step: Simulate VFD Fault")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_Alm"') is True
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False

    print("--- TC2_VFD_Fault: PASSED ---")


def test_fan_failure_alarm(plc):
    """Test Case 3: Verifies the fan failure alarm logic."""
    print("\n--- Running Test: TC3_Fan_Failure_Alarm ---")
    delay = 1
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.VFD_Fault_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Fault_Delay_Sec"', delay)

    # Simulate normal running feedback first
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Run_Fdbk_DI"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Airflow_Status_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is True
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"') is False

    print("Step: Create failure condition (no airflow)")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Airflow_Status_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"') is False, "Failure alarm should be delayed"

    print(f"Step: Waiting for {delay}s delay...")
    time.sleep(delay)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"') is True, "Failure alarm should be active after delay"
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Start_Cmd_DO"') is False, "Start command should be off on failure"

    print("Step: Disable to reset alarm")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Fan_Failure_Alm"') is False, "Failure alarm should reset when disabled"

    print("--- TC3_Fan_Failure_Alarm: PASSED ---")


def main():
    """Main function to set up the environment and run all tests."""
    print("Initializing Test Environment...")
    plc = MockPLC()

    try:
        # Run all test cases
        test_normal_start_stop(plc)
        test_vfd_fault_condition(plc)
        test_fan_failure_alarm(plc)

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    except Exception as e:
        print(f"\n!!! AN UNEXPECTED ERROR OCCURRED: {e} !!!")
    finally:
        print("\nTesting complete.")


if __name__ == "__main__":
    main()
