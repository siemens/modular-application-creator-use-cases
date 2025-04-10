using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    ///     All the functions for non TIA Portal Project based operations are defined here.
    /// </summary>
    public class NonTIAProjectBased
    {
        /// <summary>
        ///     List of models for serialization
        /// </summary>
        public List<ModelToSerialize> ModelList = new List<ModelToSerialize>();

        /// <summary>
        ///     Creates a new object of the class, create a new object of ModelToSerialize and add it to the ModelList. Start the
        ///     method CopyModel()
        /// </summary>
        /// <param name="name">The Name of the model</param>
        public NonTIAProjectBased(string name)
        {
            ModelList.Add(new ModelToSerialize(name));
            CopyModel();
        }

        /// <summary>
        ///     This is the string which will be displayed in the View
        /// </summary>
        public string StringOfAllNames
        {
            get
            {
                var stringOfAllNames = "";
                ModelList.ForEach(x => stringOfAllNames += x.Name);
                return stringOfAllNames;
            }
        }

        /// <summary>
        ///     This function gets the first element of the ModelList. Serialize the object, deserialize an object of the specific
        ///     type and add it to ModelList
        ///     This function is called to display the process of a typical MAC serialization
        /// </summary>
        public void CopyModel()
        {
            var currentModel = ModelList.FirstOrDefault();
            var jsonOfChannel = JsonConvert.SerializeObject(currentModel, new JsonSerializerSettings());
            var copyOfChannel =
                JsonConvert.DeserializeObject<ModelToSerialize>(jsonOfChannel, new JsonSerializerSettings());
            ModelList.Add(copyOfChannel);
        }
    }

    /// <summary>
    ///     This class is an exemplary model, which is used to demonstrate the MAC serialization process
    /// </summary>
    public class ModelToSerialize
    {
        /// <summary>
        ///     [JsonProperty] means that this attribute will be serialized with its value to get the value again after
        ///     serialization.
        ///     This attribute is en exemplary string which gets serialized
        /// </summary>
        [JsonProperty] private string _name;

        /// <summary>
        ///     Creates a new object of the class and set the name
        /// </summary>
        /// <param name="name">The name of the model</param>
        public ModelToSerialize(string name) => Name = name;

        /// <summary>
        ///     [JsonIgnore] means that this attribute will not be serialized.
        ///     This property gets and sets the "_name" attribute which gets serialized
        /// </summary>
        [JsonIgnore]
        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
