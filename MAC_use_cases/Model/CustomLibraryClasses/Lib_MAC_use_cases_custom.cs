using System.Linq;
using MAC_use_cases.Model;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Project = Siemens.Engineering.Project;

namespace MAC_use_cases
{
    /// <summary>
    /// This class is an additional extension for the automaticly created classes from the Folder TiaImports/GeneratedClasses. It has to be partial and can then be used with the ResourceManagement class.
    /// The .tiares file is not able to generate classes for HMI specific Mastercopies. So now the files for the HMI Mastercopies has to be defined by hand
    /// </summary>
    public partial class Lib_MAC_use_cases
    {
        /// <summary>
        /// This call returns a mastercopy for the defined path. Here the path has to be the same as it is in the library
        /// </summary>
        public virtual LibraryMasterCopy Lib_Screen_1
        {
            get
            {
                LibraryMasterCopy returnValue = MasterCopies["HMI.Screen_1"];
                return returnValue;
            }
        }

    }
}
