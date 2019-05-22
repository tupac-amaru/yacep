 const path = require('path');
 module.exports = {
     entry: {
         index: ['./src/index.tsx']
     },
     output: {
         filename: '[name].js',
         path: path.join(__dirname, 'dist'),
     },
     resolve: {
         extensions: ['.js', '.ts', '.tsx']
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
                     localIdentName: '[name]-[local]-[hash:base64:5]'
                 }
             }, {
                 loader: 'less-loader'
             }]
         }, {
             test: /\.tsx?$/,
             use: "ts-loader"
         }]
     },
     externals: {
         'react': 'React',
         'react-dom': 'ReactDOM',
         'axios': 'axios'
     },
     devtool: 'source-map',
     devServer: {
         historyApiFallback: true,
         hot: true,
         inline: true,
         disableHostCheck: true,
         contentBase: './dist',
         compress: true,
         port: 8082,
         stats: {
             colors: true
         },
         watchOptions: {
             aggregateTimeout: 300,
             poll: 1000
         },
         proxy: {
             '/api': {
                 target: "http://localhost:5000/",
                 changeOrigin: true
             }
         }
     }
 };