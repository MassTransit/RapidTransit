namespace RapidTransit.Core.Services
{
    using System;
    using System.Text.RegularExpressions;


    public static class ServiceExtensions
    {
        static readonly Regex _regex = new Regex(@"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))");

        public static string GetFriendlyDescription(this Type bootstrapperType)
        {
            string name = GetFriendlyName(bootstrapperType);

            return _regex.Replace(name, " $1");
        }

        public static string GetFriendlyName(this Type bootstrapperType)
        {
            string name = bootstrapperType.Name;
            if (name.EndsWith("Bootstrapper", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Bootstrapper".Length);
            if (name.EndsWith("Service", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "Service".Length);
            if (name.EndsWith("BusHost", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "BusHost".Length);
            return name;
        }
    }
}