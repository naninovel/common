import { Bindings } from "bindings";

let thisLogger: (message: string) => void;

export function injectLogger(logger: (message: string) => void) {
    thisLogger = logger;
    Bindings.Log = logger;
}

export function log(message: string) {
    if (thisLogger == null)
        injectLogger(console.log);
    thisLogger(message);
}
