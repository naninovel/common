let injectedLog: ((message: string) => void) | undefined;
let injectedWarn: ((message: string) => void) | undefined;
let injectedError: ((message: string) => void) | undefined;

export type Logger = {
    logInfo?: (message: string) => void;
    logWarning?: (message: string) => void;
    logError?: (message: string) => void;
}

export function injectLogger(
    logger: Logger,
    log?: (message: string) => void,
    warn?: (message: string) => void,
    error?: (message: string) => void
) {
    logger.logInfo = injectedLog = log;
    logger.logWarning = injectedWarn = warn ?? log;
    logger.logError = injectedError = error ?? log;
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
