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
        else if (key === "constants") destination[key] = mergeConstants(value, <Array<Metadata.Constant>>destination[key]);
        else destination[key] = (<unknown[]>destination[key]).concat(value);
    else if (typeof value === "object")
        mergeObject(<never>value, <never>destination[key]);
    else destination[key] = value;
}

function mergeCommands(custom: Array<Metadata.Command>, builtins: Array<Metadata.Command>) {
    const commands: Array<Metadata.Command> = [];
    for (const builtin of builtins) {
        const overridden = custom.find(c => c.id === builtin.id || c.alias != null && c.alias === builtin.alias);
        if (overridden == null) commands.push(builtin);
        else commands.push(mergeOverriddenCommand(overridden!, builtin));
    }
    return commands.concat(custom.filter(c => !commands.includes(c)));
}

function mergeConstants(custom: Array<Metadata.Constant>, builtins: Array<Metadata.Constant>) {
    const constants: Array<Metadata.Constant> = [];
    for (const builtin of builtins) {
        const overridden = custom.find(c => c.name === builtin.name);
        if (overridden == null) constants.push(builtin);
        else constants.push(overridden);
    }
    return constants.concat(custom.filter(c => !constants.includes(c)));
}

function mergeOverriddenCommand(overridden: Metadata.Command, builtin: Metadata.Command) {
    if (overridden.summary == null || overridden.summary.length === 0)
        overridden.summary = builtin.summary;
    if (overridden.remarks == null || overridden.remarks.length === 0)
        overridden.remarks = builtin.remarks;
    if (overridden.examples == null || overridden.examples.length === 0)
        overridden.examples = builtin.examples;
    overridden.parameters = mergeOverriddenParams(overridden.parameters, builtin.parameters);
    return overridden;
}

function mergeOverriddenParams(overridden: Array<Metadata.Parameter>, builtin: Array<Metadata.Parameter>) {
    const lengthDelta = overridden.length - builtin.length;
    for (let i = lengthDelta; i < overridden.length; i++)
        if (overridden[i].summary == null || overridden[i].summary!.length === 0)
            overridden[i].summary = builtin[i - lengthDelta].summary;
    return overridden;
}
