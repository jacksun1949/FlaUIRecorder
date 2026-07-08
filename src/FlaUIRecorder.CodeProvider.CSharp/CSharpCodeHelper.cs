using FlaUI.Core.AutomationElements.Infrastructure;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Text.RegularExpressions;



namespace FlaUIRecorder.CodeProvider.CSharp

{

    public static class CSharpCodeHelper

    {

        public static string GetVariableName(AutomationElement element, AutomationElement parent = null)

        {

            var parts = new List<string>();



            if (parent != null && parent.Properties.Name.TryGetValue(out var parentName) && !string.IsNullOrEmpty(parentName))

            {

                var sanitizedParent = SanitizeNamePart(parentName);

                if (!string.IsNullOrEmpty(sanitizedParent))

                    parts.Add(sanitizedParent);

            }



            if (element.Properties.Name.TryGetValue(out var rawName) && !string.IsNullOrEmpty(rawName))

            {

                var sanitized = SanitizeNamePart(rawName);

                if (!string.IsNullOrEmpty(sanitized))

                    parts.Add(sanitized);

            }



            if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))

            {

                var sanitizedId = SanitizeNamePart(automationId);

                if (!string.IsNullOrEmpty(sanitizedId) && !parts.Any(p => string.Equals(p, sanitizedId, StringComparison.OrdinalIgnoreCase)))

                    parts.Add(sanitizedId);

            }



            string controlTypeName;

            if (element.Properties.ControlType.TryGetValue(out var controlType))

                controlTypeName = controlType.ToString();

            else

                controlTypeName = "Item";



            if (parts.Count > 0)

            {

                var combined = string.Join(string.Empty, parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));

                return char.ToLower(combined[0]) + combined.Substring(1) + controlTypeName;

            }



            return char.ToLower(controlTypeName[0]) + controlTypeName.Substring(1);

        }



        public static string SanitizeIdentifier(string value)

        {

            if (string.IsNullOrEmpty(value))

                return string.Empty;



            return Regex.Replace(value, "[^a-zA-Z0-9_]", string.Empty);

        }



        private static string SanitizeNamePart(string value)

        {

            if (string.IsNullOrEmpty(value))

                return string.Empty;



            var sanitized = Regex.Replace(value, "[^a-zA-Z0-9_]", string.Empty);

            if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))

                sanitized = "_" + sanitized;



            return sanitized.Length > 0 ? char.ToLower(sanitized[0]) + sanitized.Substring(1) : string.Empty;

        }

    }

}


