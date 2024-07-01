namespace Naninovel.Metadata;

/// <summary>
/// Describes nature of branching caused by a <see cref="Command"/> execution;
/// may have multiple flags specified via bitmask.
/// </summary>
[Flags]
public enum BranchTraits
{
    /// <summary>
    /// Playback is navigated to a specific <see cref="Naninovel.Metadata.Endpoint"/>.
    /// </summary>
    /// <remarks>
    /// Command is expected to have a parameter with <see cref="ValueContextType.Endpoint"/>
    /// context which specifies the endpoint to which the playback is redirected.
    /// </remarks>
    Endpoint = 1 << 0,
    /// <summary>
    /// Playback is navigated to the commands nested under the command.
    /// </summary>
    /// <remarks>
    /// Command is expected to have <see cref="Command.Nest"/> specified.
    /// </remarks>
    Nest = 1 << 1,
    /// <summary>
    /// Branching depends on user interaction, eg picking a choice.
    /// </summary>
    Interactive = 1 << 2,
    /// <summary>
    /// Playback flow, once branched, is expected, at some point, to return
    /// to the next command with the same nesting level.
    /// </summary>
    Return = 1 << 3,
    /// <summary>
    /// Denotes that the command is a switch root and starting a switch block, ie the playback
    /// may end up at one of the consequent commands which have <see cref="Branch.SwitchRoot"/>
    /// equal to the ID of this command and skip all the others in the same block.
    /// </summary>
    Switch = 1 << 4
}

public static class BranchFlagsExtensions
{
    public static bool HasFlag (this BranchTraits value, BranchTraits trait)
    {
        return (value & trait) != 0;
    }
}
