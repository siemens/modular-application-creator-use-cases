# **Helper File: A Practical Guide to Building Your First Application**

This guide walks through the steps to create a simple application using the Modular Application Creator, based on the examples in the videos.

## **Objective: Create a "Hello, World" Name Formatter**

We will build an application that:

1. Asks the user for their name.  
2. Formats the name into a greeting.  
3. Prints the greeting to the console.

We will need three modules: GetUserInput, FormatGreeting, and PrintToConsole.

### **Step 1: Create the GetUserInput Module**

This module's job is to prompt the user for input and output the text they enter.

**File: modules/get\_user\_input.js**

// A conceptual module in JavaScript

class GetUserInput {  
  // The configuration might specify the question to ask.  
  constructor(config) {  
    this.promptMessage \= config.promptMessage || "Enter a value: ";  
  }

  // The execute method performs the module's action.  
  execute() {  
    // In a real scenario, this would use Node.js's readline or a similar library.  
    // For this example, we'll simulate it.  
    console.log(this.promptMessage);  
    // Let's pretend the user types "Alice".  
    const userInput \= "Alice";  
    console.log(\`(User entered: ${userInput})\`);  
    return userInput;  
  }  
}

// Modules need to be exportable so the application loader can find them.  
module.exports \= GetUserInput;

### **Step 2: Create the FormatGreeting Module**

This module takes text as input (the user's name) and formats it into a new string.

**File: modules/format\_greeting.js**

class FormatGreeting {  
  constructor(config) {  
    // The greeting pattern can be customized via the pipeline config.  
    this.greetingPattern \= config.pattern || "Hello, %s\!";  
  }

  // The 'data' parameter is the output from the previous module.  
  execute(data) {  
    if (typeof data \!== 'string' || data.length \=== 0\) {  
      return "Hello, stranger\!";  
    }  
    // Replace the placeholder '%s' with the user's name.  
    return this.greetingPattern.replace('%s', data);  
  }  
}

module.exports \= FormatGreeting;

### **Step 3: Create the PrintToConsole Module**

This is a very simple, reusable module that prints whatever data it receives.

**File: modules/print\_to\_console.js**

class PrintToConsole {  
  // This module needs no configuration.  
  constructor(config) {}

  execute(data) {  
    console.log("--- Application Output \---");  
    console.log(data);  
    console.log("--------------------------");  
    return data; // It's good practice to pass data through.  
  }  
}

module.exports \= PrintToConsole;

### **Step 4: Define the Pipeline**

Now we connect these modules in a pipeline.yaml file. This is the blueprint for our application.

**File: pipeline.yaml**

name: "Hello World Greeter"  
description: "A simple app to ask for a name and print a greeting."

modules:  
  \- module: GetUserInput  
    config:  
      promptMessage: "Please enter your name: "

  \- module: FormatGreeting  
    config:  
      pattern: "Welcome to the application, %s\!"

  \- module: PrintToConsole

### **Step 5: Run the Application**

When the application is run, the framework will perform the following actions:

1. Load GetUserInput, see its config, and execute it. It will ask "Please enter your name: " and output the name (e.g., "Alice").  
2. Load FormatGreeting, passing "Alice" as its input data. It will process it using the custom pattern and output "Welcome to the application, Alice\!".  
3. Load PrintToConsole, passing the greeting string as its input data. It will execute and print the final message to the terminal.

This demonstrates how to build a complete application by simply defining and connecting small, independent modules.