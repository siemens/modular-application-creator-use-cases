using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using MAC_use_cases.Model;
using MAC_use_cases.Model.UseCases;
using MAC_use_cases.ViewModel;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;

namespace MAC_use_cases.Serialization
{
    public static class MAC_use_casesSerializer
    {
        public static string DefaultFileExtension = ".json";

        public static bool SerializeModuleToFile(MAC_use_casesEM module, string filePath, JsonSerializer serializer = null)
        {
            bool success = false;
            if (serializer == null)
            {
                serializer = GetSerializer();
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, module);
                        writer.Flush();
                    }
                }
                success = true;
            }
            catch (Exception e)
            {
                MacManagement.LoggingService.LogMessage(LogTypes.GenerationError, e.Message, nameof(MAC_use_casesSerializer));
            }

            return success;
        }

        public static MAC_use_casesEM DeserializeModuleFromFile(string filePath, JsonSerializer serializer = null)
        {
            MAC_use_casesEM result = null;

            try
            {
                if (serializer == null)
                {
                    serializer = GetSerializer();
                }

                using (StreamReader sr = File.OpenText(filePath))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    result = serializer.Deserialize(reader, typeof(MAC_use_casesEM)) as MAC_use_casesEM;
                }
            }
            catch (Exception e)
            {
                MacManagement.LoggingService.LogMessage(LogTypes.GenerationError, e.Message, nameof(MAC_use_casesSerializer));
            }

            return result;
        }

        // Properties owned by the MAC framework that must never be overwritten during import.
        // ControlModules is wired up by CreateControlModules() in the constructor and its
        // serialised shape can differ from the live object, causing a JSON parse error.
        private static readonly HashSet<string> _importExcludedProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "ControlModules",
            "ModuleStart",
        };

        public static bool ImportModuleFromFile(MAC_use_casesEM module, string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);

                JObject json = JObject.Parse(fileContent);
                foreach (string prop in _importExcludedProperties)
                {
                    json.Remove(prop);
                }

                ResolveImportSourceOnImport(json, Path.GetDirectoryName(Path.GetFullPath(filePath)));

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    TypeNameHandling = TypeNameHandling.Auto,
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                };

                JsonConvert.PopulateObject(json.ToString(Formatting.None), module, settings);
                return true;
            }
            catch (Exception e)
            {
                MacManagement.LoggingService.LogMessage(LogTypes.GenerationError, e.Message, nameof(MAC_use_casesSerializer));
                return false;
            }
        }

        public static void ExportModule(MAC_use_casesEM module, string filePath)
        {
            if (module == null)
            {
                return;
            }

            if (module.HardwareGenerationExcelBasedViewModel?.ExportRelativeImportSource != true)
            {
                SerializeModuleToFile(module, filePath);
                return;
            }

            try
            {
                var serializer = GetSerializer();
                var stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, module);

                JObject json;
                using (var reader = new JsonTextReader(new StringReader(stringWriter.ToString()))
                       { DateParseHandling = DateParseHandling.None })
                {
                    json = JObject.Load(reader);
                }

                MakeImportSourceRelativeOnExport(json, Path.GetDirectoryName(Path.GetFullPath(filePath)));
                File.WriteAllText(filePath, json.ToString(serializer.Formatting));
            }
            catch (Exception e)
            {
                MacManagement.LoggingService.LogMessage(LogTypes.GenerationError, e.Message, nameof(MAC_use_casesSerializer));
            }
        }

        /// <summary>
        ///     Resolves a relative ImportSource of the CSV based hardware generation in a module configuration
        ///     against the folder of the imported .json, so the module stores and shows the absolute path.
        /// </summary>
        /// <param name="json">The module configuration.</param>
        /// <param name="configDirectory">The folder of the imported .json.</param>
        public static void ResolveImportSourceOnImport(JObject json, string configDirectory)
        {
            UpdateImportSource(json,
                importSource => HardwareGenerationFileBased.ResolveImportSource(importSource, configDirectory));
        }

        /// <summary>
        ///     Writes the ImportSource of the CSV based hardware generation in a module configuration relative
        ///     to the folder of the exported .json.
        /// </summary>
        /// <param name="json">The module configuration.</param>
        /// <param name="configDirectory">The folder of the exported .json.</param>
        public static void MakeImportSourceRelativeOnExport(JObject json, string configDirectory)
        {
            UpdateImportSource(json,
                importSource => HardwareGenerationFileBased.MakeRelativeImportSource(importSource, configDirectory));
        }

        private static void UpdateImportSource(JObject json, Func<string, string> update)
        {
            var importSource = json?[nameof(MAC_use_casesEM.HardwareGenerationExcelBasedViewModel)]?
                [nameof(HardwareGenerationExcelBasedViewModel.ImportSource)] as JValue;
            if (importSource?.Type == JTokenType.String)
            {
                importSource.Value = update((string)importSource.Value);
            }
        }

        private static void TestModuleSerialization(MAC_use_casesEM module, string filePath)
        {
            foreach (var serializer in _serializerList)
            {
                try
                {
                    SerializeModuleToFile(module, filePath, serializer);
                    var deserializedModule = DeserializeModuleFromFile(filePath, serializer);
                    File.Delete(filePath);
                }
                catch (Exception)
                {
                }
            }
        }

        private static readonly List<JsonSerializer> _serializerList = new List<JsonSerializer>
        {
            GetSerializer(),
            GetSerializer2(),
            GetSerializer3()
        };

        private static JsonSerializer GetSerializer()
        {
            return new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                Formatting = Formatting.Indented
            };
        }

        private static JsonSerializer GetSerializer2()
        {
            return new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                Formatting = Formatting.Indented
            };
        }

        private static JsonSerializer GetSerializer3()
        {
            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All,
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                MissingMemberHandling = MissingMemberHandling.Error,
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
        }
    }
}
