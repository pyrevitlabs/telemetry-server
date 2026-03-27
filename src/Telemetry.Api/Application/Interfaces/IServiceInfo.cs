namespace Telemetry.Api.Application.Interfaces
{
    /// <summary>
    /// Represents a service information interface that provides details
    /// or metadata about a specific service.
    /// </summary>
    public interface IServiceInfo
    {
        /// <summary>
        /// Gets or sets the unique identifier for the service.
        /// </summary>
        /// <remarks>
        /// The ServiceId property is used to uniquely identify a specific service
        /// within the system. This value is typically generated or assigned
        /// during service creation and should remain constant to ensure
        /// consistent identification.
        /// </remarks>
        Guid ServiceId { get; }
    }
}