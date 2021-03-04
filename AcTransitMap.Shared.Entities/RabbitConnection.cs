using System;
using System.Collections.Generic;
using System.Text;

namespace AcTransitMap.Shared.Entities
{
    /// <summary>
    /// An object to store RabbitMQ connection details.
    /// </summary>
    public class RabbitConnection
    {
        /// <summary>
        /// The RabbitMQ endpoint/URL.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// The username for the RabbitMQ user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password for the RabbitMQ user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Sets the virtual host to use for RabbitMQ.
        /// 
        /// Defaults to <cd>"/"</cd>
        /// </summary>
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// Returns whether or not the RabbitMQ connection details are valid.
        /// <br/>
        /// It's considered valid when the <see cref="Endpoint"/>, 
        /// <see cref="Username"/> and <see cref="Password"/> are not null
        /// or empty.
        /// </summary>
        /// <returns>Whether or not the RabbitMQ connection details are valid.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Endpoint) 
                && !string.IsNullOrEmpty(Username)
                && !string.IsNullOrEmpty(Password)
                && !string.IsNullOrEmpty(VirtualHost);
        }

    }
}
