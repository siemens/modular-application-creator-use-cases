using System;
using System.Collections.Generic;
using System.Reflection;

using Siemens.Automation.ModularApplicationCreatorBasics.IOC;
using Siemens.ModularApplicationCreator.Testenvironment.ReflectionHelpClasses;

namespace MAC_use_cases.Tests.TestEnvironment
{
    internal static class MacPackageResolverTestSetup
    {
        public static void Initialize()
        {
            var resolverInterface = typeof(MacPkgAssemblyResolving).GetInterfaces()[0];
            var resolver = ServiceContainer.StatDef.GetService(resolverInterface);

            if (resolver == null)
            {
                throw new InvalidOperationException("The MAC package assembly resolver service is not registered.");
            }

            InitializeDictionary(resolver, "SpecificAssemblyPathForModulePackageDictionary");
            InitializeDictionary(resolver, "AssemblyDirectoryForNuGetPackageDictionary");
        }

        private static void InitializeDictionary(object resolver, string propertyName)
        {
            var property = resolver.GetType().GetProperty(propertyName);
            var field = resolver.GetType().GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (property == null || field == null)
            {
                throw new InvalidOperationException(
                    $"The MAC package assembly resolver does not expose the expected '{propertyName}' member.");
            }

            if (field.GetValue(resolver) != null)
            {
                return;
            }

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(property.PropertyType.GetGenericArguments());
            field.SetValue(resolver, Activator.CreateInstance(dictionaryType));
        }
    }
}
