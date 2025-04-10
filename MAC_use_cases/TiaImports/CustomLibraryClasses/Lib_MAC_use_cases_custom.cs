using Siemens.Automation.ModularApplicationCreator.Tia.Openness;

namespace MAC_use_cases
{
    /// <summary>
    ///     This class is an additional extension for the automatically created classes from the Folder
    ///     TiaImports/GeneratedClasses. It has to be partial and can then be used with the ResourceManagement class.
    ///     The .tiares file is not able to generate classes for HMI specific Mastercopies. So now the files for the HMI
    ///     Mastercopies has to be defined by hand
    /// </summary>
    public partial class Lib_MAC_use_cases
    {
        /// <summary>
        ///     This call returns a mastercopy for the defined path. Here the path has to be the same as it is in the library
        /// </summary>
        public virtual LibraryMasterCopy Lib_Screen_1
        {
            get
            {
                var returnValue = MasterCopies["HMI.Screen_1"];
                return returnValue;
            }
        }
    }
}
