const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');
module.exports = {
    optimization: {
        minimizer: [
            new TerserPlugin({
                cache: true,
                parallel: true,
                sourceMap: false
            }),
        ],
    },
    entry: {
        index: ['./src/index.tsx'],
    },
    output: {
        filename: '[name].js',
        path: path.join(__dirname, 'dist'),
    },
    resolve: {
        extensions: ['.js', '.ts', '.tsx']
    },
    externals: {
        'react': 'React',
        'react-dom': 'ReactDOM',
        'axios': 'axios'
    },
    module: {
        rules: [{
            test: /\.less$/,
            use: [{
                loader: 'style-loader'
            }, {
                loader: 'css-loader',
                options: {
                    modules: true,
                    localIdentName: '[hash:base64:5]'
                }
            }, {
                loader: 'less-loader'
            }]
        }, {
            test: /\.tsx?$/,
            use: "ts-loader"
        }]
    }
};