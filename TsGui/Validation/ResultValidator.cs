﻿//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Text.RegularExpressions;
//using System.Diagnostics;
using System.Xml.Linq;

namespace TsGui.Validation
{
    public static class ResultValidator
    {
        /// <summary>
        /// Check xml value against ignore strings. Return true if the value should be ignored
        /// </summary>
        /// <param name="InputXml"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool ShouldIgnore(XElement InputXml, string Value)
        {
            if (Value == null) { return true; }
            foreach (XElement xignore in InputXml.Elements("Ignore"))
            {
                //first check for empty value in the ignore entry i.e. it has been left in 
                //the file but has no use
                if (string.IsNullOrEmpty(xignore.Value.Trim())) { continue; }

                bool toignore = false;
                XAttribute attrib = xignore.Attribute("SearchType");
                
                if (attrib != null)
                {
                    string s = xignore.Attribute("SearchType").Value;
                    //run the correct search type. default is startswith
                    if (s == "EndsWith")
                    { toignore = Value.EndsWith(xignore.Value, StringComparison.OrdinalIgnoreCase); }
                    else if (s == "Contains")
                    { toignore = Value.ToUpper().Contains(xignore.Value.ToUpper()); }
                    else if (s == "Equals")
                    { toignore = Value.Equals(xignore.Value, StringComparison.OrdinalIgnoreCase); }
                    else
                    { toignore = Value.StartsWith(xignore.Value, StringComparison.OrdinalIgnoreCase); }
                }
                else
                { toignore = Value.StartsWith(xignore.Value, StringComparison.OrdinalIgnoreCase); }

                //Debug.WriteLine("toignore value: " + toignore);
                if (toignore) { return true; }
            }

            //match hasn't been found. Return false i.e. don't ignore
            return false;
        }

        /// <summary>
        /// Truncate a string to the specified length
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string Truncate (string StringValue, int Length)
        {
            if (StringValue == null ) { return null; }
            if (StringValue.Length > Length) { return StringValue.Substring(0, Length); }
            else { return StringValue; }
        }

        /// <summary>
        /// Remove invalid characters from a string
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="InvalidChars"></param>
        /// <returns></returns>
        public static string RemoveInvalid (string StringValue, string InvalidChars)
        {
            if ((string.IsNullOrEmpty(StringValue)) || (string.IsNullOrEmpty(InvalidChars)))
                { return StringValue; }

            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = InvalidChars.ToCharArray();
            string newstring = StringValue;

            foreach (char c in invalidchars)
            {
                newstring = newstring.Replace(c.ToString(),string.Empty);
            }

            return newstring;
        }

        /// <summary>
        /// Find if StringValue contains specified characters. 
        /// Returns false if StringValue contains no invalid characters
        /// Returns true if String value is null, or if contains invalid characters
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Characters"></param>
        /// <param name="CaseSensitive"></param>
        /// <returns></returns>
        public static bool DoesStringContainCharacters(string SourceString, string Characters, bool CaseSensitive)
        {
            if (SourceString == null) { return false; }
            if (string.IsNullOrEmpty(Characters)) { return true; }
            
            //Debug.WriteLine("RemoveInvalid called");
            char[] invalidchars = Characters.ToCharArray();

            foreach (char c in invalidchars)
            {
                if (CaseSensitive == false)
                {
                    if (SourceString.ToUpper().Contains(c.ToString().ToUpper())) { return true; }
                }
                else
                {
                    if (SourceString.Contains(c.ToString())) { return true; }
                }             
            }

            return false;
        }

        /// <summary>
        /// Check if StringValue is too long. If StringValue is null,
        /// will return false. If MaxLength is zero, will return true.
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool ValidMaxLength(string StringValue,int MaxLength)
        {
            if (StringValue == null) { return false; }

            //if max length is 0, maxlength is undefined 
            if (MaxLength == 0 ) { return true; }
            if (StringValue.Length <= MaxLength) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Check if StringValue is too short. Will return false if StringValue
        /// is null.
        /// </summary>
        /// <param name="StringValue"></param>
        /// <param name="MinLength"></param>
        /// <returns></returns>
        public static bool ValidMinLength(string StringValue, int MinLength)
        {
            if (StringValue == null) { return false; }
            //Debug.WriteLine(StringValue.Length);
            if (StringValue.Length >= MinLength) { return true; }
            else { return false; }
        } 

        public static bool DoesRegexMatch(string Input, string Pattern, bool IsCaseSensitvie)
        {
            if (IsCaseSensitvie == true) { return Regex.IsMatch(Input, Pattern); }
            else { return Regex.IsMatch(Input, Pattern, RegexOptions.IgnoreCase); }
        }

        public static bool DoesStringMatchRule(StringValidationRule Rule, string Input)
        {
            bool result = false;
            string rulestring = Rule.Content;
            string inputstring = Input;
            
            if (Rule.IsCaseSensitive == false)
            {
                rulestring = rulestring.ToUpper();
                inputstring = inputstring.ToUpper();
            }

            switch (Rule.Type)
            {
                case StringValidationRuleType.Characters:
                    result = DoesStringContainCharacters(Input, Rule.Content, Rule.IsCaseSensitive);
                    break;
                case StringValidationRuleType.Contains:
                    result = inputstring.Contains(rulestring); 
                    break;
                case StringValidationRuleType.EndsWith:
                    result = inputstring.EndsWith(rulestring);          
                    break;
                case StringValidationRuleType.StartsWith:
                    result = inputstring.StartsWith(rulestring);
                    break;
                case StringValidationRuleType.RegEx:
                    result = DoesRegexMatch(Input, Rule.Content, Rule.IsCaseSensitive);
                    break;
                case StringValidationRuleType.Equals:
                    result = inputstring.Equals(rulestring);
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
