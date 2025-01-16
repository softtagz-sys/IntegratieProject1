const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");


module.exports = {
    entry: {
        site: './src/ts/site.ts',
        validation: './src/ts/validation.ts',
        range: './src/ts/views/flow/questions/range.ts',
        redirectedQuestionId: './src/ts/views/flow/questions/redirectedQuestionId.ts',
        goingToNextQuestionCircularFlow: './src/ts/views/flow/goingToNextQuestionCircularFlow.ts',
        showPopUp: './src/ts/showPopUp.ts',
        createScroll: './src/ts/views/flow/createScroll.ts',
        analytics: './src/ts/analytics/analytics.ts',
        controlQuestions: './src/ts/views/flow/questions/controlQuestions.ts',
        editQuestion: './src/ts/views/question/editQuestionPresenter.ts',
        editProject: './src/ts/views/project/projectPresenter.ts',
        editFlow: './src/ts/views/flow/flowPresenter.ts',
        createQuestion: './src/ts/views/question/createQuestionPresenter.ts',
        manageFacilitators: './src/ts/views/project/facilitatorsPresenter.ts',
        orderQuestions: './src/ts/views/flow/editFlowPresenter.ts',
        playerCountUpdater: './src/ts/views/flow/playerCountUpdater.ts',
    },
    output: {
        filename: '[name].entry.js',
        path: path.resolve(__dirname, '..', 'wwwroot', 'dist'),
        clean: true
    },
    devtool: 'source-map',
    mode: 'development',
    resolve: {
      extensions: [".ts", ".js"],
      extensionAlias: {'.js': ['.js', '.ts']}
    },
    module: {
        rules: [
            {
                test: /\.ts$/i,
                use: ['ts-loader'],
                exclude: /node_modules/
            },
            {
                test: /\.s?css$/,
                use: [{loader: MiniCssExtractPlugin.loader}, 'css-loader', 'sass-loader']
            },
            {
                test: /\.(png|svg|jpg|jpeg|gif|webp)$/i,
                type: 'asset'
            },
            {
                test: /\.(eot|woff(2)?|ttf|otf|svg)$/i,
                type: 'asset'
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css"
        })
    ]
};