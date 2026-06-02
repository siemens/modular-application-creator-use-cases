using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.SoftwareUnit;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    ///     All the functions to configure and generate Tags and Tag Tables are defined here.
    /// </summary>
    public class CreateVariables
    {
        /// <summary>
        ///     Creates a new tag table or retrieves an existing one in the specified PLC device.
        ///     \image html CreateTagTable.png
        /// </summary>
        /// <param name="plcDevice">The PLC device where the tag table should be created or retrieved</param>
        /// <param name="tableName">The name for the tag table</param>
        /// <returns>A ControllerTags object representing the new or existing tag table</returns>
        /// <remarks>
        ///     If a tag table with the specified name already exists, it will be returned instead of creating a new one.
        ///     The returned tag table can be used to add, modify, or remove tags.
        /// </remarks>
        /// <example>
        ///     <code>
        /// var tagTable = CreateTagTable(myPlcDevice, "ProcessTags");
        /// </code>
        /// </example>
        public static ControllerTags CreateTagTable(PlcDevice plcDevice, string tableName)
        {
            return plcDevice.Tags.GetOrCreateGlobalTagTable(tableName);
        }

        /// <summary>
        ///     Creates a new tag table or retrieves an existing one in the specified software unit.
        /// </summary>
        /// <param name="softwareUnit">The software unit where the tag table should be created or retrieved</param>
        /// <param name="tableName">The name for the tag table</param>
        /// <returns>A ControllerTags object representing the new or existing tag table</returns>
        /// <remarks>
        ///     This overload creates/retrieves a tag table within a specific software unit rather than at the PLC device level.
        ///     If a tag table with the specified name already exists in the software unit, it will be returned instead of creating
        ///     a new one.
        /// </remarks>
        /// <example>
        ///     <code>
        /// var tagTable = CreateTagTable(mySoftwareUnit, "ModuleSpecificTags");
        /// </code>
        /// </example>
        public static ControllerTags CreateTagTable(ISoftwareUnitBase softwareUnit, string tableName)
        {
            return softwareUnit.Tags.GetOrCreateGlobalTagTable(tableName);
        }


        /// <summary>
        ///     This Function creates a Tag in a Tag Table
        ///     \image html CreateTag.png
        /// </summary>
        /// <param name="tagTable">An existing tag table in which you want to create a tag</param>
        /// <param name="addressType">Type of the address. (Input, Output)</param>
        /// <param name="addressByte">The Byte of the address</param>
        /// <param name="addressBit">The Bit of the address</param>
        /// <param name="tagName">Name of the tag</param>
        /// <param name="dataType">Date type of the tag</param>
        /// <param name="tagComment">Comment of the tag</param>
        public static void CreateTagInTagTable(ControllerTags tagTable, string addressType, string addressByte,
            string addressBit, string tagName, string dataType, string tagComment)
        {
            var tagAddress = addressType + addressByte + "." + addressBit;

            var tag = tagTable[tagName];

            tag?.Delete();

            tag = tagTable.AddTag(tagName, dataType, tagAddress);
            tag.SetComment("en-EN", tagComment);
        }
    }
}
