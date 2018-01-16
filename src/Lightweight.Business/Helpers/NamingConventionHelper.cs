using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lightweight.Business.Exceptions;

namespace Lightweight.Business.Helpers
{
    public class NamingConventionHelper
    {
        /// <summary>
        /// Parses a naming convention string.
        /// </summary>
        /// <param name="convention">The convention string like {[PropertyA.SubPropertyA...etc]}separator{[PropertyB.SubPropertyB...etc]}separator...{[PropertyN.SubPropertyN]}</param>
        /// <returns>Returns a naming convention structure.</returns>
        public static NamingConvention ParseNamingConvention(string convention)
        {
            var rx = new Regex(@"\{\[?(\w+\.?)*(?:\(\d\))?\]?\}");
            var rxname = new Regex(@"(\w+\.?)+");
            var rxoptional = new Regex(@"\[(\w+\.?)+(\(\d)?\)?\]");
            var rxpadding = new Regex(@"\w+\((?<padding>\d)\)", RegexOptions.ExplicitCapture);

            var matches = rx.Matches(convention);

            NamingConvention nc = new NamingConvention();

            // build format pattern from convention
            var pattern = convention;
            for (int index = 0; index < matches.Count; index++)
            {
                var match = matches[index].Value;

                // get match property
                NamingConventionProperty ncp = new NamingConventionProperty()
                {
                    Name = rxname.Match(match).Value,
                    IsOptional = rxoptional.IsMatch(match)
                };

                if (rxpadding.IsMatch(match)) // get padding
                {
                    Match paddingMatch = rxpadding.Match(match);
                    var paddingstr = paddingMatch.Groups["padding"] != null ? paddingMatch.Groups["padding"].Value : string.Empty;
                    int padding;
                    int.TryParse(paddingstr, out padding);
                    ncp.Padding = padding;
                }

                nc.Properties.Add(ncp);

                // add match placeholder to pattern
                var mrx = new Regex(Regex.Escape(match));
                pattern = mrx.Replace(pattern, string.Format("{{{0}}}", index), 1);
            }

            nc.Pattern = pattern;

            return nc;
        }

        private static string GetFormattedValue(object value, NamingConventionProperty property)
        {
            if (value != null)
            {
                if (property.Padding > 0) // apply padding if possible
                {
                    // check if number
                    if (!value.GetType().IsAssignableFrom(typeof(int)))
                        throw new BusinessException(
                            string.Format(
                                "Failed to apply padding for property {0} because it is not integer type. Property type:{1} ",
                                property.Name, value.GetType().Name));
                    return ((int)value).ToString("D" + property.Padding.ToString());
                }

                return value.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Given a list of objects a naming convention and a key property, extracts a dictionary of values using the naming convention specified.
        /// </summary>
        /// <typeparam name="T">The type of the objects list used to extract values from.</typeparam>
        /// <param name="objects">The objects list used to extract values from.</param>
        /// <param name="convention">The naming convention structure used for extracting the values.</param>
        /// <param name="key">The key property name used in the return dictionary.</param>
        /// <returns>Returns a dictionary of values extracted from the input objects list using the naming convention specified and as key the value of the key property from each object.</returns>
        public static IDictionary<dynamic, string> ExtractValues<T>(IEnumerable<T> objects, NamingConvention convention, string key)
        {

            Dictionary<dynamic, string> results = new Dictionary<dynamic, string>();
            var rootType = typeof(T); // remember root type

            foreach (var obj in objects)  // for each object in list
            {
                List<object> values = new List<object>(); // initialize list of extracted values

                // for each property from naming convention, get its value from the object
                foreach (var property in convention.Properties)
                    values.Add(GetFormattedValue(ReflectionHelper.GetPropertyValue(obj, property.Name, property.IsOptional), property));

                // concatenate the values
                var result = string.Format(convention.Pattern, values.ToArray());

                object keyValue = string.IsNullOrEmpty(key) ? obj : ReflectionHelper.GetPropertyValue(obj, key);

                if (keyValue == null)
                    throw new BusinessException("The specified key value is null on at least one object. Please specify another key property or make sure that none of the key property values are null.");

                // strip double slashes from pattern
                result = Regex.Replace(result, "/+", "/");

                if (results.ContainsKey(keyValue))
                    throw new BusinessException(string.Format("The key value {0} is present on more that one object and it cannot be added to the result dictionary.", keyValue));

                // add the result to the dictionary
                results.Add(keyValue, result);
            }

            return results;
        }
    }

    public class NamingConvention
    {
        public string Pattern { get; set; }
        public List<NamingConventionProperty> Properties { get; set; }

        public NamingConvention()
        {
            Properties = new List<NamingConventionProperty>();
        }
    }

    public class NamingConventionProperty
    {
        public string Name { get; set; }
        public bool IsOptional { get; set; }
        public int Padding { get; set; }
    }
}
