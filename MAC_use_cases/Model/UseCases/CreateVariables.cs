using System;
using System.Collections.Generic;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.SoftwareUnit;
using Siemens.Engineering.Library.Types;
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
            tag.SetComment("en-US", tagComment);
        }
        /// <summary>
        ///     This Function creates a User Constant in a Tag Table
        /// </summary>
        /// <param name="tagTable">An existing tag table in which you want to create a user constant</param>
        /// <param name="value"></param> value of the user constant
        /// <param name="tagName">Name of the user constant</param>
        /// <param name="dataType">Date type of the user constant</param>
        /// <param name="tagComment">Comment of the user constant</param>
        public static void CreateUserConstantInTagTable(ControllerTags tagTable,
            string value, string tagName, string dataType, string tagComment)
        {
            var tag = tagTable[tagName];
            tag?.Delete();
            tag = tagTable.AddUserConstant(tagName, dataType, value);
            tag.SetComment("en-US", tagComment);
        }
        /// <summary>
        ///     Writes a UDT definition in SCL source format to a <c>.udt</c> file.
        /// </summary>
        /// <param name="plcdevice">The PLC Device where the UDT shall be created.</param>
        /// <param name="udtName">The name of the UDT type, e.g. <c>MyDataType1</c>.</param>
        /// <param name="version">The version string, e.g. <c>0.2</c>.</param>
        /// <param name="members">Dictionary of member names and their data types, e.g. ("Int", "Int").</param>
        public static void createUDT(
            PlcDevice plcdevice,
            string udtName,
            string version,
            Dictionary<string, string> members)
        {
            var structMembers = new System.Text.StringBuilder();
            foreach (var member in members)
            {
                structMembers.AppendLine($"         \"{member.Key}\" : {member.Value};");
            }

            var udtContent = $@"TYPE ""{udtName}""
            VERSION : {version}
            STRUCT
            myStruct : Struct
            {structMembers}      END_STRUCT;
             END_STRUCT;

            END_TYPE
            ";
            var filePath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".udt");
            try
            {
                System.IO.File.WriteAllText(filePath, udtContent, System.Text.Encoding.UTF8);
                OpennessFuncs.ImportToPlc(filePath, plcdevice);
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
