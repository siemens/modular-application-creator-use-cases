using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("UseCaseBasedDoku")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Siemens AG")]
[assembly: AssemblyProduct("UseCaseBasedDoku")]
[assembly: AssemblyCopyright("Copyright © Siemens AG 2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("095c4c93-44fa-4e9a-a2a5-e9d015355826")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
// Make sure that  the version of the assembly is incremented by every build in debug mode too!
// Otherwise the module update will not work.
#if DEBUG
[assembly: AssemblyVersion("1.0.*")]
#else
[assembly: AssemblyVersion("1.0.0")]
#endif

// InformationalVersion / ProductVersion used for nuget semantic versioning - Major.Minor.Patch
// Major : Breaking changes
// Minor : New features, but backwards compatible
// Patch : Backwards compatible bug fixes only
/// <remarks><see cref="https://docs.microsoft.com/en-us/nuget/create-packages/prerelease-packages"/></remarks>
[assembly: AssemblyInformationalVersion("1.0.0")]
//[assembly: AssemblyFileVersion("1.0.0.0")]