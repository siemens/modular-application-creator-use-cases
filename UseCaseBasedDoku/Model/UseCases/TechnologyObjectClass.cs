using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.TO;

namespace UseCaseBasedDoku.Model.UseCases
{
    /// <summary>
    /// All the functions to configure and generate Technology Objects are defined here.
    /// </summary>
    public class TechnologyObjectClass
    {
        /// <summary>
        /// This attribute is the used MotionControlVersion
        /// </summary>
        public const string MotionControlVersion = "7.0";

        /// <summary>
        /// This attribute is the used MainTelegramNumber
        /// </summary>
        public const int MainTelegramNumber = 105;

        /// <summary>
        /// This attribute is the used name of the TechnologyObject
        /// </summary>
        public string Name = "myTO";

        /// <summary>
        /// This object property is the used instance of the class TechnologyObjectInfo
        /// </summary>
        public TechnologicalObjectInfo TechnologicalObject { get; set; }

        /// <summary>
        /// This attribute is the used Type of the TechnologyObject
        /// </summary>
        public const string TOType = OpennessConstants.TO_POSITIONING_AXIS;

        /// <summary>
        /// This attribute is the used MaximumVelocity of the TechnologyObject
        /// </summary>
        public string MaximumVelocity = "999.0";

        /// <summary>
        /// This attribute is the used VirtualAxisMode of the TechnologyObject
        /// </summary>
        public string VirtualAxisMode = "1";

        /// <summary>
        /// Creates a new object of the class, sets the MainTelegramNumber of the TechnologyObject add it to myModule.ProvidedTechnologicalObjects
        /// </summary>
        /// <param name="myModule">The module</param>
        public TechnologyObjectClass(UseCaseBasedDokuEM myModule)
        {
            TechnologicalObject = new TechnologicalObjectInfo(Name, TOType, MotionControlVersion, "");

            //Add the main telegram number to the TechnologicalObjectInfo Object
            TechnologicalObject.MainTelegramNumber = MainTelegramNumber;

            //Add the TechnologicalObjectInfo Object to the list of ProvidedTechnologicalObjects of the module
            myModule.ProvidedTechnologicalObjects.Add(TechnologicalObject);
        }

        /// <summary>
        /// constructor for deserialization
        /// </summary>
        /// <param name="myModule">The module</param>
        [JsonConstructor]
        public TechnologyObjectClass()
        {

        }

        /// <summary>
        /// This function creates all TOs (toInfo objects) that the user added to module.ProvidedTechnologicalObjects
        /// Call this method after adding all your TOs in myModule to myModule.ProvidedTechnologicalObjects
        /// \image html CreateTO.png
        /// </summary>
        /// <param name="plcDevice">The PLC on which the equipment module is implemented</param>
        /// <param name="module">The corresponding equipment module</param>
        public static void CreateTOs(PlcDevice plcDevice, UseCaseBasedDokuEM module)
        {
            foreach (var toInfo in module.ProvidedTechnologicalObjects)
            {
                //GetOrCreateRootTO will create a TO in the TO root folder. If a TO with the same name already exists, this TO is returned.
                var generatedTO = plcDevice.TechnologicalObjects.GetOrCreateRootTO(toInfo, module);
            }

        }

        /// <summary>
        /// This function configures the TechnologyObject (toInfo objects) and add it to the myModule.ProvidedTechnologicalObjects
        /// \image html ConfigureTO.png
        /// </summary>
        /// <param name="TechnologicalObject">The TechnologyObject that should be configured</param>
        /// <param name="myModule">The corresponding equipment module</param>
        public void ConfigureTO(TechnologicalObjectInfo TechnologicalObject, UseCaseBasedDokuEM myModule)
        {
            //Add the additional parameters to the TechnologicalObjectInfo Object
            string toPath = "DynamicLimits.MaxVelocity";
            TechnologicalObject.AdditionalParameter[toPath] = MaximumVelocity;

            toPath = "VirtualAxis.Mode";
            TechnologicalObject.AdditionalParameter[toPath] = VirtualAxisMode;

            //Change parameter of TO
            toPath = "DynamicDefaults.Velocity";
            TechnologicalObject.AdditionalParameter[toPath] = "50.0";

            //Add the TechnologicalObjectInfo Object to the list of ProvidedTechnologicalObjects of the module
            //myModule.ProvidedTechnologicalObjects.Add(TechnologicalObject);
        }
    }

}
