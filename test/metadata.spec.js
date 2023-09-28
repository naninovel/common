import { expect, test } from "vitest";
import { getDefaultMetadata, mergeMetadata } from "../src";
import { Module } from "node:module";

test("can provide default when object", async () => {
    mock("../assets/default-metadata.json", { variables: ["foo"] });
    expect(getDefaultMetadata().variables).toContain("foo");
});

test("can provide default when json", async () => {
    mock("../assets/default-metadata.json", "{ \"variables\": [\"bar\"] }");
    expect(getDefaultMetadata().variables).toContain("bar");
});

test("can merge with default", () => {
    const def = { commands: [{ id: "PrintText" }] };
    const custom = { actors: [{ id: "Ai" }], commands: [{ id: "foo", parameters: [{ id: "bar" }] }] };
    const merged = merge(def, custom);
    expect(merged.actors[0].id).toEqual("Ai");
    expect(merged.commands.find(c => c.id === "foo").parameters[0].id).toEqual("bar");
    expect(merged.commands.find(c => c.id === "PrintText")).toBeTruthy();
});

test("can merge with object", () => {
    const merged = merge({ object: { foo: "foo" } }, { object: { bar: "bar" } });
    expect(merged.object.foo).toEqual("foo");
    expect(merged.object.bar).toEqual("bar");
});

test("replaces overridden commands by alias", () => {
    const def = { commands: [{ id: "def", alias: "foo", parameters: [] }] };
    const custom = { commands: [{ id: "custom", alias: "foo", parameters: [] }] };
    const merged = merge(def, custom);
    expect(merged.commands.filter(c => c.alias === "foo").length).toEqual(1);
    expect(merged.commands.filter(c => c.alias === "foo")[0].id).toEqual("custom");
});

test("transfers docs to the overridden commands and parameters", () => {
    const def = { id: "*", alias: "foo", summary: "", remarks: "", examples: "", parameters: [{ id: "*", summary: "" }] };
    const custom = { id: "*", alias: "foo", parameters: [{ id: "*" }] };
    const merged = merge({ commands: [def] }, { commands: [custom] });
    const command = merged.commands.find(c => c.alias === "foo");
    expect(command.summary).toStrictEqual("");
    expect(command.remarks).toStrictEqual("");
    expect(command.examples).toStrictEqual("");
    expect(command.parameters.length).toEqual(1);
    expect(command.parameters[0].summary).toStrictEqual("");
});

test("doesn't mutate original metadata", async () => {
    mock("../assets/default-metadata.json", {
        actors: [{ id: "foo" }],
        commands: [{ id: "cmd", parameters: [{ id: "bar" }] }]
    });
    const def = getDefaultMetadata();
    const custom = { actors: [{ id: "baz" }] };
    const merged = merge(def, custom);
    expect(merged.actors).not.toBe(custom.actors);
    expect(merged.actors[0]).not.toBe(custom.actors[0]);
    expect(merged.commands).not.toBe(def.commands);
    expect(merged.commands[0]).not.toBe(def.commands[0]);
    expect(merged.commands[0].parameters).not.toBe(def.commands[0].parameters);
    expect(merged.commands[0].parameters[0]).not.toBe(def.commands[0].parameters[0]);
    expect(def.actors.length).toStrictEqual(1);
    expect(def.actors[0].id).toStrictEqual("foo");
    expect(def.commands.length).toStrictEqual(1);
    expect(def.commands[0].id).toStrictEqual("cmd");
    expect(def.commands[0].parameters.length).toStrictEqual(1);
    expect(def.commands[0].parameters[0].id).toStrictEqual("bar");
});

function merge(...metas) {
    return mergeMetadata(...metas);
}

// vitest can't mock "require": https://github.com/vitest-dev/vitest/discussions/3134
function mock(mockedUri, stub) {
    Module._load_original = Module._load;
    Module._load = function (uri, parent) {
        if (uri === mockedUri) return stub;
        return Module._load_original(uri, parent);
    };
}
