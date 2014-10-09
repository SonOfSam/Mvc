// <auto-generated />
namespace Microsoft.AspNet.Mvc
{
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.AspNet.Mvc.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// Unable to find the required services. Please add all the required services by calling AddMvc() before calling UseMvc()/UsePerRequestServices() in the Application Startup.
        /// </summary>
        internal static string UnableToFindServices
        {
            get { return GetString("UnableToFindServices"); }
        }

        /// <summary>
        /// Unable to find the required services. Please add all the required services by calling AddMvc() before calling UseMvc()/UsePerRequestServices() in the Application Startup.
        /// </summary>
        internal static string FormatUnableToFindServices()
        {
            return GetString("UnableToFindServices");
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
