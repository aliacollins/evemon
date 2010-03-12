using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator
{
    public static class StringCleaningExtensions
    {
        public static string Clean(this string input)
        {
            string output;

            // Remove whitespace from the beginning and end of a string
            output = input.Trim();

            // Replace tab characters with spaces
            output = output.Replace('\t', ' ');

            // Remove markup
            Regex htmlClean = new Regex("<.+?>", RegexOptions.Singleline | RegexOptions.Compiled);
            output = htmlClean.Replace(output, String.Empty);

            // remove two space character next to eachother
            Regex collapseSpace = new Regex(@"[ ]{2,}", RegexOptions.Compiled);
            output = collapseSpace.Replace(output, @" ");
            
            return output;
        }

        public static string FormatPropertyValue(this DgmTypeAttribute property)
        {
            if (property.ValueInt.HasValue)
                return property.ValueInt.ToString();

            // is it actually an integer stored as a float?
            if (Math.Truncate(property.ValueFloat.Value) == property.ValueFloat.Value)
                return Convert.ToInt32(property.ValueFloat.Value).ToString();
            
            return property.ValueFloat.ToString();
        }

        public static string FormatDecimal(this decimal input)
        {
            // is it actually an integer stored as a double?
            if (Math.Truncate(input) == input)
                return Convert.ToInt64(input).ToString();

            return input.ToString();
        }
    }
}
