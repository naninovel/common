import { log, injectLogger } from "../src";

test("can log without injecting", () => {
    expect(() => log("")).not.toThrow();
});

test("can inject custom logger", () => {
    let message = "";
    injectLogger(msg => message = msg);
    log("foo");
    expect(message).toEqual("foo");
});
