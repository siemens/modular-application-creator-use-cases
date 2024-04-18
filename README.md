# Usecases for the SIMATIC Modular Application Creator **Module Builder**

This repository demonstrates how to use the comfort functions of the Modular Application Creator Module Builder. With those comfort functions the Openness programmer does not need to handle the xml file when e.g. calls in ladder logic should be generated. More information is available under [Modular Application Creator on SIOS](https://support.industry.siemens.com/cs/de/en/view/109762852).

## 🚀 Getting started Documentation

To see the documentation download html folder and start index.html in your browser.

## Prerequisites
- TIA-Portal 19 installed on Windows PC
- Download the [Modular Application Creator 19.1](https://support.industry.siemens.com/cs/attachments/109762852/Modular_Application_Creator_19.00.01_all_in_one.zip)
- Microsoft Visual Studio with [Modular Application Creator Module Builder](https://support.industry.siemens.com/cs/de/en/view/109762852) installed

## First steps
1. clone this repository in your Visual Studio
1. optional: go to the Properties of your project in Visual Studio --> Debug --> Start external program and select the ModularApplicationCreator.exe
1. Start your solution --> Modular Application Creator should show up
    1. Create new Modular Application Creator project
    1. Assign Equipment Module: modular-application-creator-use-cases to your PLC
    1. Configure your Module under the "Configure modules" tab
    1. Click generate under the "Generate" tab

    --> your TIA-Portal project is generated

## 📝 License

Copyright © 2024 [Siemens AG](https://www.siemens.com/).

This project is MIT licensed.
