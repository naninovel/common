import { Bindings } from "backend";
import { test, expect, vi } from "vitest";
import { injectLogger, log, warn, error } from "../src";

test("can log without injecting", () => {
    expect(() => log("")).not.toThrow();
    expect(() => error("")).not.toThrow();
    expect(() => warn("")).not.toThrow();
});

test("can inject custom loggers", () => {
    let infoMsg, warnMsg, errMsg;
    injectLogger(msg => infoMsg = msg, wrn => warnMsg = wrn, err => errMsg = err);
    log("foo");
    warn("bar");
    error("nya");
    expect(infoMsg).toEqual("foo");
    expect(warnMsg).toEqual("bar");
    expect(errMsg).toEqual("nya");
});

test("when only info logger is injected, others re-use it", () => {
    let infoMsg;
    injectLogger(msg => infoMsg = msg);
    log("foo");
    expect(infoMsg).toEqual("foo");
    warn("bar");
    expect(infoMsg).toEqual("bar");
    error("nya");
    expect(infoMsg).toEqual("nya");
});

test("bindings are assigned when injected", () => {
    const log = vi.fn(), warn = vi.fn(), err = vi.fn();
    injectLogger(log, warn, err);
    expect(Bindings.logInfo).toEqual(log);
    expect(Bindings.logWarning).toEqual(warn);
    expect(Bindings.logError).toEqual(err);
});
