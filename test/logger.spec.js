import { test, expect, vi } from "vitest";
import { injectLogger, log, warn, error } from "../src";

test("can log without injecting", () => {
    expect(() => log("")).not.toThrow();
    expect(() => error("")).not.toThrow();
    expect(() => warn("")).not.toThrow();
});

test("can inject custom loggers", () => {
    const logger = {};
    let infoMsg, warnMsg, errMsg;
    injectLogger(logger, msg => infoMsg = msg, wrn => warnMsg = wrn, err => errMsg = err);
    log("foo");
    warn("bar");
    error("nya");
    expect(infoMsg).toEqual("foo");
    expect(warnMsg).toEqual("bar");
    expect(errMsg).toEqual("nya");
});

test("when only info logger is injected, others re-use it", () => {
    const logger = {};
    let infoMsg;
    injectLogger(logger, msg => infoMsg = msg);
    log("foo");
    expect(infoMsg).toEqual("foo");
    warn("bar");
    expect(infoMsg).toEqual("bar");
    error("nya");
    expect(infoMsg).toEqual("nya");
});

test("bindings are assigned when injected", () => {
    const logger = {};
    const log = vi.fn(), warn = vi.fn(), err = vi.fn();
    injectLogger(logger, log, warn, err);
    expect(logger.logInfo).toEqual(log);
    expect(logger.logWarning).toEqual(warn);
    expect(logger.logError).toEqual(err);
});
