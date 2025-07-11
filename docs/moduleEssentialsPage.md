# Module Essentials  

## Overview  
The **Module Essentials** is a control module designed to streamline the development process. It serves as a NuGet package that helps developers quickly build user interfaces and efficiently manage dependencies between UI controls.  

## Key Features  
- **Rapid UI Development**: Simplifies the creation of user interfaces, reducing development time.  
- **Dependency Management**: Handles dependencies between UI controls with ease, ensuring smooth integration and functionality.  
- **NuGet Integration**: Easily accessible as a NuGet package, making it simple to add to your projects.  

## Getting Started  
To use the **Module Essentials** in your project:  
1. Install the NuGet package: "Siemens.ModularApplicationCreator.ControlModules.ModuleEssentials"  

    ### Versioning  
    - The versioning system of **Module Essentials** ensures compatibility with TIA Portal.  
    - The **first digit** of the version number corresponds to the supported TIA Portal version.  

    ### Parameter Types  
    - In the context of **Module Essentials**, any UI control is considered a parameter.  
      - Example: A parameter like `Velocity` must have a corresponding class defined for it, typically inheriting from the `BaseParameter` class.  
    - Types of parameters:  
      - **Simple Parameters**:  
        - Hold a single value.  
        - Example: A text box or numeric input field for entering a single value.  
        - These parameters are often implemented using classes derived from `BaseParameter`.  
      - **Collection Parameters**:  
        - Designed for collections and often displayed as ComboBoxes.  
        - Example: A dropdown menu for selecting an item from a **predefined** list.  
        - These parameters can also be implemented using `BaseParameter` as the base class.  
    - UI controls are optional:  
      - Developers can choose whether or not to implement UI controls based on their specific requirements. ✅