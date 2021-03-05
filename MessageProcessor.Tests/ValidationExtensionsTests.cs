using NUnit.Framework;
using MessageProcessor;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using GtfsConsumer.Entities.Interfaces;

namespace MessageProcessor.Tests
{
    [TestFixture]
    public class ValidationExtensionsTests
    {
        [TestCase("Test",-10,-10,ExpectedResult = true)]
        [TestCase(null,-10,-10,ExpectedResult = false)]
        [TestCase("Test",-9999,-10,ExpectedResult = false)]
        [TestCase("Test",-10,-9999,ExpectedResult = false)]
        [TestCase(null,9999,9999,ExpectedResult = false)]
        public bool IsValidTest(string vehicleId, float longitude, float latitude)
        {

            var mock = new Mock<IVehiclePosition>();
            mock.Setup(x => x.VehicleId).Returns(vehicleId);
            mock.Setup(x => x.Longitude).Returns(longitude);
            mock.Setup(x => x.Latitude).Returns(latitude);

            var result = mock.Object.IsValid(out string message);
            return result;
        }
    }
}