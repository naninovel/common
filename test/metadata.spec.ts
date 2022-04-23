import { defaultMetadata, mergeWithDefaultMetadata } from "../src";
import { Metadata } from "bindings";

describe("project metadata", () => {
    it("can provide default", () => {
        expect(defaultMetadata).toBeTruthy();
    });
    it("can merge with default", () => {
        const custom = {
            actors: [{ id: "Ai" }],
            commands: [{ id: "foo", parameters: [{ id: "bar" }] }]
        } as Metadata.Project;
        const merged = mergeWithDefaultMetadata(custom);
        expect(merged.actors[0].id).toEqual("Ai");
        expect(merged.commands.find(c => c.id == "foo")!.parameters[0].id).toEqual("bar");
        expect(merged.commands.find(c => c.id == "PrintText")).toBeTruthy();
    });
    it("replaces overridden commands", () => {
        const params = defaultMetadata.commands.find(c => c.alias == "print")!.parameters;
        const custom = {
            commands: [{ id: "@", alias: "print", parameters: params }]
        } as Metadata.Project;
        const merged = mergeWithDefaultMetadata(custom);
        expect(merged.commands.filter(c => c.alias == "print")).toHaveLength(1);
    });
    it("transfers docs to the overridden commands", () => {
        const params = defaultMetadata.commands.find(c => c.alias == "print")!.parameters;
        const custom = {
            commands: [{ id: "@", alias: "print", parameters: params }]
        } as Metadata.Project;
        const merged = mergeWithDefaultMetadata(custom);
        const command = merged.commands.find(c => c.alias == "print")!;
        expect(command.summary).toBeTruthy();
        expect(command.remarks).toBeTruthy();
        expect(command.examples).toBeTruthy();
        expect(command.parameters.find(p => p.id == "Text")!.summary).toBeTruthy();
    });
    it("doesn't mutate original metadata", () => {
        const custom = { actors: [{ id: "Ai" }] } as Metadata.Project;
        const merged = mergeWithDefaultMetadata(custom);
        expect(merged.actors).not.toBe(custom.actors);
        expect(merged.actors[0]).not.toBe(custom.actors[0]);
        expect(merged.commands).not.toBe(defaultMetadata.commands);
        expect(merged.commands[0]).not.toBe(defaultMetadata.commands[0]);
        expect(merged.commands[0].parameters).not.toBe(defaultMetadata.commands[0].parameters);
        expect(merged.commands[0].parameters[0]).not.toBe(defaultMetadata.commands[0].parameters[0]);
    });
});
