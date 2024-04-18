using Siemens.Automation.ModularApplicationCreator.Tia.Openness;

namespace UseCaseBasedDoku.Model.UseCases
{
    /// <summary>
    /// All the functions to configure and generate parts of the library are defined here.
    /// </summary>
    public class IntegrateLibraries
    {
        /// <summary>
        /// This function creates an instance DB in the target folder (folder under program blocks)
        /// \image html CreateInstanceDB.png
        /// </summary>
        /// <param name="module">The Module</param>
        /// <param name="masterCopy">A master copy of the block you want to create an instance DB of</param>
        /// <param name="instanceName">Name of the instance</param>
        /// <param name="target">The folder under program blocks in which the DB is created</param>
        public static void CreateInstanceDB(UseCaseBasedDokuEM module, FBMasterCopy masterCopy, string instanceName, BlockGroup target)
        {
            DataBlock instDB = module.ResourceManagement.CreateInstanceDb(masterCopy, instanceName, target.Blocks);
        }
    }
}
