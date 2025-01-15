# Usecases for the SIMATIC Modular Application Creator **Module Builder**

This repository demonstrates how to use the comfort functions of the Modular Application Creator Module Builder. With those comfort functions the Openness programmer does not need to handle the xml file when e.g. calls in ladder logic should be generated. More information is available under [Modular Application Creator on SIOS](https://support.industry.siemens.com/cs/de/en/view/109762852).

## 🚀 Getting started Documentation

We just updated our [Modular Application Creator Use Case Based Documentation](https://siemens.github.io/modular-application-creator-use-cases/html/index.html).

## Prerequisites
- TIA-Portal installed on Windows PC
- Download the same major version of the [Modular Application Creator](https://support.industry.siemens.com/cs/de/en/view/109762852)
- Microsoft Visual Studio with [Modular Application Creator Module Builder](https://support.industry.siemens.com/cs/de/en/view/109762852) installed

## First steps
1. clone this repository in your Visual Studio (avoid OneDrive folder)
1. optional: go to the Properties of your project in Visual Studio --> Debug --> Start external program and select the ModularApplicationCreator.exe
1. Start your solution --> Modular Application Creator should show up
    1. Go to settings and add the the location of the nuget packages to the sources (xxx\modular-application-creator-use-cases\UseCaseBasedDoku\bin\Debug)
    1. Create new Modular Application Creator project
    1. Assign Equipment Module: MAC_use_cases.xxx-prexxx to your PLC
    1. Configure your Module under the "Configure modules" tab
    1. Click generate under the "Generate" tab

    --> your TIA-Portal project is generated
   
## Contact us
If you have problems or suggestions, please send an email to [modular.application.creator.industry@siemens.com](mailto:modular.application.creator.industry@siemens.com)

## 📝 License

Copyright © 2024 [Siemens AG](https://www.siemens.com/).

This project is MIT licensed.
