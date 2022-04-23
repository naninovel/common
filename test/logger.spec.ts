import { log, injectLogger } from "../src";

describe("logger", () => {
    it("can inject and log", () => {
        let message = "";
        injectLogger(msg => message = msg);
        log("foo");
        expect(message).toEqual("foo");
    });
});
