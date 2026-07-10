using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;
using FlaUIRecorder.CodeProvider.Common;

namespace FlaUIRecorder.CodeProvider.Common.Internals
{
    public class VariableList : List<Variable>
    {
        /// <summary>
        /// Gets a variable by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Variable this[string name]
        {
            get
            {
                var result = this.FirstOrDefault(v => 0 == string.Compare(v.Name, name, StringComparison.OrdinalIgnoreCase));

                if (result == null)
                    throw new ArgumentException($"No variable of name '{name}' found!");

                return result;
            }
        }

        /// <summary>
        /// Adds a new variable
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <param name="element">The according ui element.</param>
        public Variable Add(string name, AutomationElement element)
        {
            return Add(name, element, null, null);
        }

        public Variable Add(string name, AutomationElement element, string parentVariableName, SelectorInfo selector)
        {
            var result = new Variable
            {
                Name = name,
                Element = element,
                ParentVariableName = parentVariableName,
                SelectorKey = selector?.GetCacheKey()
            };
            Add(result);

            return result;
        }

        /// <summary>
        /// Finds a variable by the given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Variable Find(AutomationElement element)
        {
            if (element == null)
                return null;

            try
            {
                if (element.Properties.AutomationId.TryGetValue(out var automationId) && !string.IsNullOrEmpty(automationId))
                {
                    var result = this.FirstOrDefault(v =>
                    {
                        try
                        {
                            return v.Element.Properties.AutomationId.TryGetValue(out var id)
                                && string.Equals(id, automationId, StringComparison.Ordinal);
                        }
                        catch (System.Runtime.InteropServices.COMException) { return false; }
                        catch (InvalidOperationException) { return false; }
                    });

                    if (result != null)
                        return result;
                }
            }
            catch (System.Runtime.InteropServices.COMException) { }
            catch (InvalidOperationException) { }

            return this.FirstOrDefault(v => ReferenceEquals(v.Element, element));
        }

        public Variable FindByParentAndSelector(string parentVariableName, SelectorInfo selector)
        {
            if (selector == null || string.IsNullOrEmpty(parentVariableName))
                return null;

            var selectorKey = selector.GetCacheKey();
            return this.FirstOrDefault(v =>
                string.Equals(v.ParentVariableName, parentVariableName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(v.SelectorKey, selectorKey, StringComparison.Ordinal));
        }
    }
}
