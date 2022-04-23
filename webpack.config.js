module.exports = () => ({
    resolve: { extensions: [".ts", ".js"] },
    module: { rules: [{ test: /\.ts/, loader: "ts-loader" }] },
    externals: { bindings: "commonjs bindings" },
    entry: "./src/index.ts",
    output: {
        filename: "index.js",
        library: { type: "commonjs" },
        clean: true
    }
});
