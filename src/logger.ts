import { Bindings } from "bindings";

let currentLogger: (message: string) => void;

injectLogger(console.log);

export function injectLogger(logger: (message: string) => void) {
    currentLogger = logger;
    Bindings.Log = logger;
}

export function log(message: string) {
    currentLogger(message);
}
