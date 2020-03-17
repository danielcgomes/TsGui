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
using NUnit.Framework;
using TsGui.Connectors;

namespace TsGui.Tests
{
    [TestFixture]
    public class SystemConnectorTests
    {
        [Test]
        [TestCase("ComputerName", ExpectedResult = "ROG")]
        [TestCase(null, ExpectedResult = null)]
        public string GetVariableValueTest(string Variable)
        {
            return SystemConnector.GetVariableValue(Variable);
        }
    }
}
