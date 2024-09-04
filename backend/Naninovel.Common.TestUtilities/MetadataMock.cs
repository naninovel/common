using Naninovel.Metadata;
using Naninovel.Parsing;

namespace Naninovel.TestUtilities;

public class MetadataMock : IMetadata
{
    public List<Actor> Actors { get; set; } = [];
    public List<Metadata.Command> Commands { get; set; } = [];
    public List<Resource> Resources { get; set; } = [];
    public List<Constant> Constants { get; set; } = [];
    public List<string> Variables { get; set; } = [];
    public List<Function> Functions { get; set; } = [];
    public Syntax Syntax { get; set; } = new();

    IReadOnlyCollection<Actor> IMetadata.Actors => Actors;
    IReadOnlyCollection<Metadata.Command> IMetadata.Commands => Commands;
    IReadOnlyCollection<Constant> IMetadata.Constants => Constants;
    IReadOnlyCollection<Resource> IMetadata.Resources => Resources;
    IReadOnlyCollection<string> IMetadata.Variables => Variables;
    IReadOnlyCollection<Function> IMetadata.Functions => Functions;
    ISyntax IMetadata.Syntax => Syntax;

    public Project AsProject () => new() {
        Actors = Actors.ToArray(),
        Commands = Commands.ToArray(),
        Resources = Resources.ToArray(),
        Constants = Constants.ToArray(),
        Variables = Variables.ToArray(),
        Functions = Functions.ToArray(),
        Syntax = Syntax
    };

    public Metadata.Command FindCommand (string aliasOrId)
    {
        return new MetadataProvider(AsProject()).FindCommand(aliasOrId);
    }

    public Metadata.Parameter FindParameter (string commandAliasOrId, string paramAliasOrId)
    {
        return new MetadataProvider(AsProject()).FindParameter(commandAliasOrId, paramAliasOrId);
    }

    public bool FindFunctions (string name, ICollection<Function> result)
    {
        return new MetadataProvider(AsProject()).FindFunctions(name, result);
    }

    public void SetupCommandWithEndpoint (string commandId)
    {
        var parameter = new Metadata.Parameter {
            Id = "Path",
            Nameless = true,
            ValueType = Metadata.ValueType.String,
            ValueContainerType = ValueContainerType.Named,
            ValueContext = [
                new ValueContext { Type = ValueContextType.Endpoint, SubType = Metadata.Constants.EndpointScript },
                new ValueContext { Type = ValueContextType.Endpoint, SubType = Metadata.Constants.EndpointLabel }
            ]
        };
        var command = new Metadata.Command { Id = commandId, Parameters = [parameter], Branch = new() { Traits = BranchTraits.Endpoint } };
        Commands = Commands.Append(command).ToList();
    }
}
