using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.Library.MasterCopies;

namespace MAC_use_cases.Model.UseCases;

/// <summary>
///     All the functions to configure and generate parts of the library are defined here.
/// </summary>
public class IntegrateLibraries
{
    /// <summary>
    ///     This function creates an instance DB in the target folder (folder under program blocks)
    ///     \image html CreateInstanceDB.png
    /// </summary>
    /// <param name="module">The Module</param>
    /// <param name="masterCopy">A master copy of the block you want to create an instance DB of</param>
    /// <param name="instanceName">Name of the instance</param>
    /// <param name="target">The folder under program blocks in which the DB is created</param>
    public static DataBlock CreateInstanceDataBlock(MAC_use_casesEM module, FBMasterCopy masterCopy,
        string instanceName, BlockGroup target)
    {
        return module.ResourceManagement.CreateInstanceDb(masterCopy, instanceName, target.Blocks);
    }

    public static DataBlock CreateInstanceDataBlock(MAC_use_casesEM module, FBLibraryType libraryType,
        string instanceName, BlockGroup target)
    {
        return module.ResourceManagement.CreateInstanceDb(libraryType, instanceName, target.Blocks);
    }


    /// <summary>
    ///     This function can also creates an instance DB in the target folder (folder under program blocks)
    ///     \image html CreateInstanceDB_via_XmlInstDB.png
    /// </summary>
    /// <param name="module">The Module</param>
    /// <param name="masterCopy">A master copy of the block you want to create an instance DB of</param>
    /// <param name="instanceName">Name of the instance</param>
    /// <param name="target">The folder under program blocks in which the DB is created</param>
    public static void CreateInstanceDB_via_XmlInstDB(MAC_use_casesEM module, FBMasterCopy masterCopy,
        string instanceName, BlockGroup target, PlcDevice m_plcDevice)
    {
        var instance_DB_of_MAC_use_casesFB = new XmlInstDB(instanceName, masterCopy.Name);
        instance_DB_of_MAC_use_casesFB.GenerateXmlBlock(m_plcDevice, target.Blocks);
    }

    /// <summary>
    ///     This function creates the screen from a Mastercopy
    ///     \image html CreateScreenFromMastercopy.png
    /// </summary>
    /// <param name="hmiSoftware">The openness object of the HMI software</param>
    /// <param name="screen">A master copy of the screen you want to create</param>
    public static void GenerateScreenFromMastercopy(HmiTarget hmiSoftware, LibraryMasterCopy screen)
    {
        hmiSoftware.ScreenFolder.Screens.CreateFrom((MasterCopy)screen);
    }
}
