module.exports = () => ({
    resolve: { extensions: [".ts", ".js"] },
    module: { rules: [{ test: /\.ts/, loader: "ts-loader" }] },
    externals: { backend: "commonjs backend" },
    entry: "./src/index.ts",
    output: {
        filename: "index.js",
        library: { type: "commonjs" },
        clean: true
    },
    devtool: "source-map"
});
