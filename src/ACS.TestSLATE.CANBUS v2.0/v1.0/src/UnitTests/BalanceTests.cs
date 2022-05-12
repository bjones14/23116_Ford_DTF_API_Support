// <copyright file="ModelTests.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> Unit Tests for the Balance Class.</summary>

using System;
using Customer.TestSLATE.Mnemonic.Models;
using Customer.TestSLATE.Mnemonic.Properties;
using NUnit.Framework;

namespace Customer.TestSLATE.Mnemonic.UnitTests
{
    /// <summary>
    /// Unit Tests for the Model Class.
    /// </summary>
    [TestFixture]
    public class ModelTests
    {
        /// <summary>
        /// Model Constructor Unit Test
        /// </summary>
        [Test]
        [ExpectedException(typeof(TypeInitializationException))]
        public void BalanceConstructor()
        {
            // We actually expect this test to throw the defined exception because we're not running Test SLATE.
            // The CellEnvironment will throw the exception.
            var balance = Balance.CreateNewBalance();
            Assert.AreEqual(Resources.DefaultBalName, balance.BalanceName);
            Assert.AreEqual(Resources.DefaultDescription, balance.BalanceDescr);
            Assert.AreEqual(Resources.DefaultFilePath, balance.AeroTareFilePath);
            Assert.AreEqual(Resources.DefaultFilePath, balance.CalibrationFilePath);
            Assert.AreEqual(Resources.DefaultFilePath, balance.WeightTareFilePath);
            Assert.AreEqual(0, balance.BalanceOrder);
            Assert.AreEqual(6, balance.NumberOfReadings);
            Assert.AreEqual(0, balance.BalanceReference);
        }
    }
}
