using Naninovel.Metadata;

namespace Naninovel.Bridging;

/// <summary>
/// Sent by the server when project metadata is changed.
/// </summary>
public class UpdateMetadata : IServerMessage
{
    /// <summary>
    /// The actual project metadata.
    /// </summary>
    public Project Metadata { get; set; }
}
