﻿using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using System.Diagnostics;

namespace TsGui
{
    internal static class SystemConnector
    {
        //check an xml element. check for environmental variable elements
        //return the first one that brings back a value. otherwise, return 
        //the value of the root xml
        public static string GetVariableValue(string Variable)
        {
            string s;

            //try ts env 
            //try process variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.Process);
            if (s != null) { return s; }

            //try computer variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.Machine);
            if (s != null) { return s; }

            //try user variables
            s = Environment.GetEnvironmentVariable(Variable, EnvironmentVariableTarget.User);
            if (s != null) { return s; }

            //not found. return null
            return null;
             
        }

        //get a value from WMI
        public static string GetWmiQuery(string WmiQuery)
        {
            string s = null;
            try
            {
                WqlObjectQuery wqlQuery = new WqlObjectQuery(WmiQuery);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wqlQuery);

                foreach (ManagementObject m in searcher.Get())
                {
                    foreach (PropertyData propdata in m.Properties)
                    {
                        s = s + propdata.Value;
                    }                    
                }

                //ManagementObject m = new ManagementObject(WmiQuery);
                //s = m.ToString();
                if (String.IsNullOrEmpty(s)) { return null; }
                else { return s; }
            }
            catch
            {
                Debug.WriteLine("Exception thrown in SystemConnector: GetWmiQuery");
                return null;
            }
            
        }
    }
}
