# Developing Ahsoka.Platform Extensions 
This project / repo contains an example extension for use with the OpenPV Software Platform

# Getting Started


1. Pre-Requisites	
2. Project Layout
3. Extension Registration and Building 


# Prerequisites

- **C++ Prerequisites** - C++ Development takes place in a docker container called openview/openviewXXX_sdk_x0.  This container image is maintained by the Ahsoka Platform  development team (Emerging Platforms) and includes the dependencies and latest toolchain which matches the latest Distribution Image.

    - Podman - You should be familiar with Podman and have a Podman Engine running on your desktop when developing C++ components (or compiling C# Components). The Windows Podman Desktop application requires Windows Subsystem for Linux (WSL) feature enabled.
    - Visual Studio Code - VS Code is the preferred development tool for Linux Development (Ubuntu or STM)
 
- **C# Prerequisites** - C# Development takes place in Visual Studio 2022 for Windows.  Note that all C++ development prerequisites are needed here as well as the default compile requires the C++ components be built for Ahsoka.Core.

    - **Visual Studio 2022 Release 17.4+** - C# is currently building with DotNetCore 7.0 which is supported in 17.4 or later.

    
# Project Layout

**Visual Studio Solution Items**

- **Root** - The root folder contains the C# Solution, the top level CMakeLists file and other global files used in building
- **Ahsoka.BuildInterop** - This is a C# Project that contains the commands to build the C++ libraries / executables as part of building the main solution.  This will be executed in the build pipeline or local machine and will start the correct containers to execute the builds.
- **Ahsoka.CreateProto** - This is a C# Project that contains a small executable that will allow you to extract the a .proto file from the solution for use in other languages such as C++.  This is used when your C++ needs to interact with a service directly or when you need to share .proto definitions between c# and other languages.
- **Ahsoka.Test** - This is a C# test project used for executing Unit Tests to validate your project logic.   
- **Ahsoka.Extensions.Template** - This is an example project (Copied from Ahsoka.Media Extension).  This project shows how to run an Ahsoka Service as well as interact with the included "Ahsoka.VideoPlayer" C++ executable.    This project can easily be modified to be included as a native .so / .dll if your use case depends on this type of interop.
  
  Note that this project also contains the necessary metadata for deploying the OpenPV Extension which is explained further down.
- **Ahsoka.Extension.Template.UX** - This project shows how to build a simple Extension User Interface which can be deployed alongside your extension to support visual editing of the configurations.

**C++ / VSCode Items**

- **.devcontainer** - This folder contains the devcontainer.json files used by vscode (Dev Containers Extension) to launch the correct container for developing on your target platform.
- **Ahsoka.Interop** - This is the location of any C++ Build Code.  This folder is organized by C++ Project and uses CMAKE for build time orchestration. 

**Misc**

- **Demos** - This folder contains any demo applications you wish to deliver with your Extension.  These will be packages in the extension nuget package and made available for users in the OpenPV Developer Toolkit
- **Ahsoka.BuildOutputs** - This is a temporary folder where C++ Build artifacts are collected during builds.  Do not commit items in this folder.
- **LocalNugetFeed** - This folder is the output location for nuget packages build by the solution.  This folder is useful in testing as it allows you to side load an extension into the Developer Toolkit during your testing.




# Extension Registration and Building 

**Building**

Building the project is accomplished by simply building the entire solution in Visual Studio.  This will execute the BuildInterop to build any embedded builds as well as package any files (C++ binaries) into the resulting nuget package.  In this template, two nuget packages will be built. Ahsoka.Extensions.Template and Ahsoka.Extensions.Template.UX.   


**Testing**

Testing the extension can be accomplished by side loading the extension in the OpenPV Developer Toolkit. This is completeing by using the "Import Extension" button in the OpenPV Package Editor and selecting the base .nugpkg file typically located in the LocalNugetFeed folder.   This will automatically pick up the .UX package if found.



**Extension Configuration / Registration**

Located in the Ahsoka.Extensions.Template folder is file called "Ahsoka.Extensions.Template.extensionInfo.json".  THe first line of this file should not be edited as it contains the magic number used by OpenPV to find / filter extensions in nuget.org when loading your extension.   To be picked up by OpenPV, the nuget package must be named "Ahsoka.*" and include this magic number in the metadata.  

This file is where you will configure the various settings needed by OpenPV to load the extension.  The following properties are availble

- ExtensionName - This is the human readable name of the extension.
- PackageName - This is the name of the nuget package / library that contains the extension.
- UXPackageName (optional) - If your extension includes a UX, this should be the name of the UX Library / Nuget Package Name.
- UXPackageName (optional) - This is the name of the view model to load for your UX Extension.
- HasCommands - This property tells the Core if the extension contains commands that should be run as part of the build.
- HasUX - This property tells the Core if the extension has a UX that should be loaded.
- HasSDKGenerator - This property tells the Core if the extension contains any SDK Code Generators that shoudl be run when injecting the SDK into a users project.
- ServiceConfigurations - If your extension contains a service, you can place the default service setup here.  This will be automatically added to the customers service setup.



