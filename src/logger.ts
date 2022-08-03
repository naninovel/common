import { Bindings } from "backend";

let injectedLog: (message: string) => void;
let injectedWarn: (message: string) => void;
let injectedError: (message: string) => void;

export function injectLogger(
    log: (message: string) => void,
    warn?: (message: string) => void,
    error?: (message: string) => void
) {
    Bindings.LogInfo = injectedLog = log;
    Bindings.LogWarning = injectedWarn = warn ?? log;
    Bindings.LogError = injectedError = error ?? log;
}

export function log(message: string) {
    if (injectedLog == null) console.log(message);
    else injectedLog(message);
}

export function warn(message: string) {
    if (injectedWarn == null) console.warn(message);
    else injectedWarn(message);
}

export function error(message: string) {
    if (injectedError == null) console.error(message);
    else injectedError(message);
}
