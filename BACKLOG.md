# Project Backlog

This file tracks known issues, technical debt, and environmental limitations that need to be addressed.

---

### **Issue ID: 001**

*   **Date Logged:** 2025-09-12
*   **Status:** Open
*   **Issue Description:** The project's code generation process relies on a GUI-based application (`ModularApplicationCreator.exe`) for which the development environment does not have access.
*   **Root Cause Analysis:** The `README.md` specifies that this repository is a "Module Builder" project, which acts as a plugin. The intended workflow requires an engineer to manually interact with the main application's GUI to configure and trigger the code generation. The current text-based, command-line environment cannot support this GUI-dependent workflow.
*   **Affected Components:** The entire automated code generation process.
*   **Workaround / Mitigation:** Development will proceed by manually generating the TIA Portal XML files. The C# source code of the module will be used as a reference to ensure the manually created XML matches the structure that the tool *would* have created. This allows development of the control logic to continue, but the final integration test of the generator plugin itself cannot be performed in this environment.
