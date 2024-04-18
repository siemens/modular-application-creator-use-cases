using Siemens.Automation.ModularApplicationCreator.Tia.Openness;


namespace UseCaseBasedDoku.Model.UseCases
{
    /// <summary>
    /// All the functions to configure and generate Tags and Tag Tables are defined here.
    /// </summary>
    public class CreateVariables
    {
        /// <summary>
        /// This Function creates a Tag Table
        /// \image html CreateTagTable.png
        /// </summary>
        /// <param name="plcDevice">The PLC on which the equipment module is implemented</param>
        /// <param name="tableName">The Name of the tag table</param>
        /// <returns>newTable -> the created tag table (empty)</returns>
        ///

        public static ControllerTags CreateTagTable(PlcDevice plcDevice, string tableName)
        {
            var newTable = plcDevice.Tags.GetOrCreateGlobalTagTable(tableName);

            return newTable;
        }


        /// <summary>
        /// This Function creates a Tag in a Tag Table
        /// \image html CreateTag.png
        /// </summary>
        /// <param name="tagTable">An existing tag table in which you want to create a tag</param>
        /// <param name="addressType">Type of the address. (Input, Output)</param>
        /// <param name="addressByte">The Byte of the address</param>
        /// <param name="addressBit">The Bit of the address</param>
        /// <param name="tagName">Name of the tag</param>
        /// <param name="dataType">Date type of the tag</param>
        /// <param name="tagComment">Comment of the tag</param>
        public static void CreateTagInTagTable(ControllerTags tagTable, string addressType, string addressByte, string addressBit, string tagName, string dataType, string tagComment)
        {
            string tagAddress = addressType + addressByte.ToString() + "." + addressBit.ToString();

            var tag = tagTable[tagName];

            if (tag != null)
            {
                tag.Delete();
            }

            tag = tagTable.AddTag(tagName, dataType, tagAddress);
            tag.SetComment("en-EN", tagComment);
        }

    }
}
