using KronoGeo_Api.Applications.MediatR.Queries.Identity;
using KronoGeo_Api.Infrastructure.Service.Telemetry;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Microsoft.Maui.Devices.Sensors;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.TestUnitaire
{
    public class TestUnitaireTelemetry
    {

        /// <summary>
        /// Test unitaire pour le calcul d'élévation positive
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CalculPositiveElevation()
        {
            // - Arrange
            Mock<IServiceSaveParametrage> _mockService = new();

            var loc1 = new Localisation()
            {
                Latitude = 0,
                Longitude = 0,
                OrderIndex = 1,
                Altitude = 360,
                Timestamp = DateTimeOffset.Now,
            };


            var loc2 = new Localisation()
            {
                Latitude = 0,
                Longitude = 0,
                OrderIndex = 2,
                Altitude = 365,
                Timestamp = DateTimeOffset.Now,
            };

            // - Act
            var serviveTelemetry = new ServiceTelemetry(_mockService.Object);
            var telemetry = new RouteTelemetry();
            serviveTelemetry.CalculateTelemetry(loc1, loc2, ref telemetry);

            // - Assert
            Assert.False(telemetry.PositiveElevationGain == 0);
            Assert.True(telemetry.PositiveElevationGain >= 5);
            Assert.True(telemetry.NegativeElevationGain == 0);
        }

        /// <summary>
        /// Test unitaire pour le calcul d'élévation négative
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CalculNegativeElevation()
        {
            // - Arrange
            Mock<IServiceSaveParametrage> _mockService = new();

            var loc1 = new Localisation()
            {
                Latitude = 0,
                Longitude = 0,
                OrderIndex = 1,
                Altitude = 365,
                Timestamp = DateTimeOffset.Now,
            };


            var loc2 = new Localisation()
            {
                Latitude = 0,
                Longitude = 0,
                OrderIndex = 2,
                Altitude = 360,
                Timestamp = DateTimeOffset.Now,
            };

            // - Act
            var serviveTelemetry = new ServiceTelemetry(_mockService.Object);
            var telemetry = new RouteTelemetry();
            serviveTelemetry.CalculateTelemetry(loc1, loc2, ref telemetry);

            // - Assert
            Assert.True(telemetry.PositiveElevationGain == 0);
            Assert.True(telemetry.NegativeElevationGain >= 5);
        }
    }
}
