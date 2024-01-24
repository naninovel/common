import { Metadata } from "backend";

export function getDefaultMetadata(): Metadata.Project {
    const json = require("../assets/default-metadata.json");
    return typeof json === "string" ? JSON.parse(json) : json;
}

export function mergeMetadata(...metadata: Metadata.Project[]) {
    const mergedMetadata: Metadata.Project = <never>{};
    for (const meta of metadata)
        mergeObject(deepClone(meta), <never>mergedMetadata);
    return mergedMetadata;
}

function deepClone(object: object) {
    return JSON.parse(JSON.stringify(object));
}

function mergeObject(source: Record<string, unknown>, destination: Record<string, unknown>) {
    for (const key in source)
        if (source.hasOwnProperty(key))
            mergeKey(key, source[key], destination);
}

function mergeKey(key: string, value: unknown, destination: Record<string, unknown>) {
    if (!destination.hasOwnProperty(key))
        destination[key] = value;
    else if (Array.isArray(value))
        if (key === "commands") destination[key] = mergeCommands(value, <Array<Metadata.Command>>destination[key]);
        else destination[key] = (<unknown[]>destination[key]).concat(value);
    else if (typeof value === "object")
        mergeObject(<never>value, <never>destination[key]);
}

function mergeCommands(custom: Array<Metadata.Command>, builtin: Array<Metadata.Command>) {
    const commands: Array<Metadata.Command> = [];
    for (const command of builtin) {
        const overridden = custom.find(c => c.alias != null && c.alias === command.alias);
        if (overridden == null) commands.push(command);
        else commands.push(mergeOverriddenCommand(overridden!, command));
    }
    return commands.concat(custom.filter(c => !commands.includes(c)));
}

function mergeOverriddenCommand(overridden: Metadata.Command, builtin: Metadata.Command) {
    overridden.summary ??= builtin.summary;
    overridden.remarks ??= builtin.remarks;
    overridden.examples ??= builtin.examples;
    overridden.parameters = mergeOverriddenParams(overridden.parameters, builtin.parameters);
    return overridden;
}

function mergeOverriddenParams(overridden: Array<Metadata.Parameter>, builtin: Array<Metadata.Parameter>) {
    const lengthDelta = overridden.length - builtin.length;
    for (let i = lengthDelta; i < overridden.length; i++)
        overridden[i].summary = builtin[i - lengthDelta].summary;
    return overridden;
}
