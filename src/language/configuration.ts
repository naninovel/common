import { ServerCapabilities, TextDocumentSyncKind, InitializeResult } from "vscode-languageserver";
import { Language } from "bindings";

export function createConfiguration(): InitializeResult {
    return {
        serverInfo: { name: "Naninovel Language Server" },
        capabilities: createCapabilities()
    };
}

function createCapabilities(): ServerCapabilities {
    return {
        textDocumentSync: TextDocumentSyncKind.Incremental,
        completionProvider: { triggerCharacters: ["@", ":", "[", " ", ".", ",", "{"] },
        semanticTokensProvider: { legend: Language.GetTokenLegend(), full: { delta: false }, range: true },
        documentSymbolProvider: {},
        foldingRangeProvider: {},
        hoverProvider: {}
    };
}
