using Telemetry.Api.Application.Interfaces;

namespace Telemetry.Api.Application.Services
{
    /// <summary>
    /// Represents information about a service, encapsulating details
    /// such as the service name, description, status, and other relevant metadata.
    /// </summary>
    public class ServiceInfo : IServiceInfo
    {
        /// <summary>
        /// Represents information about a service, including its name,
        /// description, and any related metadata.
        /// </summary>
        public ServiceInfo()
        {
            ServiceId = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the unique identifier for the service.
        /// </summary>
        /// <remarks>
        /// The ServiceId property is used to uniquely identify a specific service within the system.
        /// This identifier is typically assigned automatically and is immutable once set.
        /// </remarks>
        public Guid ServiceId { get; }
    }
}