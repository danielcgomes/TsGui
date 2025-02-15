﻿#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core.Diagnostics;

namespace TsGui.Config
{
    public static class ConfigBuilder
    {

        /// <summary>
        /// Load the configured config. This will reset any existing config
        /// </summary>
        public static async Task<XElement> LoadConfigAsync(string configpath)
        {
            XElement rootconfig =  await GetXml(configpath);
            await Expand(rootconfig).ConfigureAwait(false);
            return rootconfig;
        }

        /// <summary>
        /// Traverse the XML and expand any found Import elements in place
        /// </summary>
        /// <param name="importxml"></param>
        /// <returns></returns>
        private static async Task Expand(XElement importxml)
        {
            foreach (XElement element in importxml.Elements())
            {
                if (element.Name == "Import")
                {
                    string path = XmlHandler.GetStringFromXml(element, "Path", string.Empty);
                    path = XmlHandler.GetStringFromXml(element, "Path", path);
                    XElement partx = await GetXml(path);

                    element.AddBeforeSelf(new XComment(element.ToString()));

                    foreach (XElement partelement in partx.Elements())
                    {
                        element.AddBeforeSelf(partelement);
                    }

                    element.AddBeforeSelf(new XComment(element.ToString()));
                    element.Remove();
                } 
                else
                {
                    await Expand(element);
                }
            }
        }

        /// <summary>
        /// Get XElement from uri, auto-detect web vs local based on path
        /// </summary>
        /// <param name="configpath"></param>
        /// <returns></returns>
        private static async Task<XElement> GetXml(string configpath)
        {
            string lowerpath = configpath.ToLower();
            XElement results = null;
            if (lowerpath.StartsWith("http://") || lowerpath.StartsWith("https://") || lowerpath.StartsWith("ftp://"))
            {
                results = await XmlHandler.ReadWebAsync(configpath);
            }
            else
            {
                results = XmlHandler.Read(configpath);
            }
            return results;
        }
    }
}
