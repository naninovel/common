namespace Naninovel.Metadata;

/// <summary>
/// Describes script playback flow branching caused by a <see cref="Command"/> execution.
/// </summary>
public class Branch
{
    /// <summary>
    /// Traits of the branching; may have multiple flags (bitmask).
    /// </summary>
    public BranchTraits Traits { get; set; }
    /// <summary>
    /// Indicates that the command is a part of a switch block, which starts at the command
    /// with the specified ID. For example, @else command has 'if' as switch root, indicating
    /// that the playback, once returned from either nested @if or other @else blocks will
    /// skip the consequent @else blocks.
    /// </summary>
    public string? SwitchRoot { get; set; }
}