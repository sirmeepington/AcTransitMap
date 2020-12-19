using GtfsConsumer.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageProcessor
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Checks the validity of the <see cref="IVehiclePosition"/> message and
        /// returns whether or not it is valid.
        /// </summary>
        /// <param name="vehicle">The <see cref="IVehiclePosition"/> to check the validity
        /// of.</param>
        /// <param name="message">A message that explains what is wrong with the message
        /// for logging purposes.</param>
        /// <returns>True if the message is valid, false otherwise.</returns>
        public static bool IsValid(this IVehiclePosition vehicle, out string message)
        {
            if (string.IsNullOrEmpty(vehicle.VehicleId))
            {
                message = "Vehicle id cannot be null";
                return false;
            }

            if (vehicle.Latitude < -90 || vehicle.Latitude > 90)
            {
                message = "Vehicle latitude is outside of acceptable range.";
                return false;
            }

            if (vehicle.Longitude < -180 || vehicle.Longitude > 180)
            {
                message = "Vehicle longitude is outside of acceptable range.";
                return false;
            }

            message = null;
            return true;
        }

    }
}
