# test_em500_monitoring.py
#
# Automated unit test for the FB500_EM_Monitoring module.

import time

# Note: This script uses a MockPLC class for demonstration.

INSTANCE_DB_NAME = "IDB_Monitoring"

class MockPLC:
    """Mock PLC class to simulate the plcsim-adv-api."""
    def __init__(self):
        self.tags = {}
        self.timer_start_time = None
        print("MockPLC: Initialized for Monitoring Test.")

    def read_tag(self, tag_name):
        return self.tags.get(tag_name)

    def write_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def run_cycle(self):
        """Simulates the execution of the FB logic."""
        enable = self.tags.get(f'"{INSTANCE_DB_NAME}"."Enable"')
        dirty_di = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_DI"')
        delay_sec = self.tags.get(f'"{INSTANCE_DB_NAME}"."UDT.Filter_Delay_Sec"', 0) # T#10s -> 10

        if enable:
            if dirty_di:
                if self.timer_start_time is None:
                    self.timer_start_time = time.time()

                if (time.time() - self.timer_start_time) >= delay_sec:
                    self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"'] = True
                else:
                    self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"'] = False
            else:
                self.timer_start_time = None
                self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"'] = False
        else:
            self.tags[f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"'] = False

        time.sleep(0.1)

def test_dirty_filter_alarm(plc):
    """Test Case 5.1: Verifies the dirty filter alarm and time delay."""
    print("\n--- Running Test: TC5.1_Dirty_Filter_Alarm ---")

    delay = 2 # Using a shorter delay for the mock test

    # 1. Initial state
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."Enable"', True)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_DI"', False)
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Filter_Delay_Sec"', delay)
    plc.run_cycle()

    # 2. Simulate Dirty Filter
    print("Step: Simulate Dirty Filter")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_DI"', True)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"') is False, "Alarm should not be immediate"

    # 3. Wait for delay
    print(f"Step: Waiting for {delay} second delay...")
    time.sleep(delay)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"') is True, "Alarm should be active after delay"

    # 4. Reset condition
    print("Step: Resetting filter")
    plc.write_tag(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_DI"', False)
    plc.run_cycle()
    assert plc.read_tag(f'"{INSTANCE_DB_NAME}"."UDT.Dirty_Filter_Alm"') is False, "Alarm should reset"

    print("--- TC5.1_Dirty_Filter_Alarm: PASSED ---")


def main():
    """Main function to set up and run all tests."""
    print("Initializing Monitoring Module Test Environment...")
    plc = MockPLC()

    try:
        test_dirty_filter_alarm(plc)

    except AssertionError as e:
        print(f"\n!!! A TEST FAILED: {e} !!!")
    finally:
        print("\nTesting complete.")

if __name__ == "__main__":
    main()
