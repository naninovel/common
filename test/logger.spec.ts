import { log, injectLogger } from "../src";

describe("logger", () => {
    it("can log without injecting", () => {
        expect(() => log("")).not.toThrow();
    });
    it("can inject custom logger", () => {
        let message = "";
        injectLogger(msg => message = msg);
        log("foo");
        expect(message).toEqual("foo");
    });
});
