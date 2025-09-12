# **Helper File: Common Patterns and Best Practices**

As you build more complex applications, you'll encounter common challenges. This file outlines patterns and best practices for solving them.

### **Pattern 1: Conditional Execution (Branching)**

**Problem:** You want to run different modules based on the output of a previous module. For example, if a validation check passes, run Module A; if it fails, run Module B.

**Solution:** Use a special **Router** or **Branch** module.

A Router module's job isn't to process data, but to direct it. It executes and, based on the input, tells the pipeline executor which module to run next.

**Conceptual ValidationRouter Module:**

* **Input:** User data (e.g., an email address).  
* **Process:**  
  1. Check if the email address is in a valid format.  
  2. If valid, return a "success" signal.  
  3. If invalid, return a "failure" signal.  
* **Output:** A signal ('success' or 'failure').

**Pipeline with a Router:**

modules:  
  \- module: GetUserInput  
  \- module: ValidationRouter  
    \# The pipeline executor understands these special keys.  
    on\_success:  
      \# This is a nested pipeline that runs on success.  
      \- module: SaveToDatabase  
      \- module: SendConfirmationEmail  
    on\_failure:  
      \- module: LogError  
      \- module: DisplayErrorMessage

### **Pattern 2: Data Transformation**

**Problem:** Module A outputs data in a format that Module B cannot understand. For example, Module A outputs a complex object, but Module B expects a simple string.

**Solution:** Create a small, single-purpose **Transformer** (or "Adapter") module.

A Transformer module sits between two other modules and does nothing but reformat data.

**Example Scenario:**

* GetUserFromDB outputs: { id: 123, fName: 'John', lName: 'Doe' }  
* SendWelcomeEmail expects a full name string like: "John Doe"

**Pipeline with a Transformer:**

modules:  
  \- module: GetUserFromDB  
    config:  
      userId: 123  
  \- module: TransformUserObjectToFullName  
    \# This module takes the user object and returns "John Doe".  
  \- module: SendWelcomeEmail  
    \# This module now gets the simple string it expects.

This keeps the core modules (GetUserFromDB, SendWelcomeEmail) clean and reusable. You don't have to add special formatting logic inside them.

### **Best Practices**

1. **Keep Modules Small and Focused:** A module should do one thing and do it well. If a module is doing too much (e.g., fetching data, transforming it, *and* saving it), break it into smaller modules.  
2. **Use Configuration for Flexibility:** Avoid hardcoding values like file paths, API keys, or magic strings inside a module. Pass them in via the config section of the pipeline. This makes your modules far more reusable.  
3. **Handle Errors Gracefully:** A module should be able to handle bad input. If a module is likely to fail (e.g., a network request), it should catch the error and output a clear error object that downstream modules can act upon.  
4. **Document Module Inputs/Outputs:** Every module should have clear documentation explaining what kind of data it expects as input and what kind of data it produces as output. This makes it much easier for others (and your future self) to build pipelines.