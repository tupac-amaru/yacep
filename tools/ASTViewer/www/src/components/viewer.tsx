import * as React from "react";
import Axios from 'axios';
const style = require('./viewer.less');

export class Viewer extends React.Component<{}, {
    resp?: any
}>{
    constructor() {
        super({}, {});
        this.state = { resp: null };
    }
    private async sendExpression(expr: string) {
        const self = this;
        if (expr.trim()) {
            const loadingContainer = document.getElementsByClassName("loading-cover")[0] as HTMLDivElement;
            loadingContainer.className = "loading-cover";
            loadingContainer.style.display = "block";
            loadingContainer.style.opacity = "0.8";
            const { data } = await Axios.post("/api", expr);
            self.setState({ resp: data }, () => {
                loadingContainer.style.display = "none";
            });
        } else {
            self.setState({ resp: null });
        }
    }

    private timer?: number;
    private keyDown() {
        const self = this;
        self.timer && window.clearTimeout(self.timer);
    }

    private keyUp(event: React.KeyboardEvent<HTMLInputElement>) {
        const self = this;
        const expr = event.currentTarget.value;
        self.timer && window.clearTimeout(self.timer);
        self.timer = window.setTimeout(() => self.sendExpression(expr), 1000);
    }

    render() {
        const self = this;
        const ast = self.state.resp ? (self.state.resp.str || self.state.resp.error || '') : '';
        return <div className={style["viewer-container"]}>
            <div className={style["left-container"]}>
                <div className={style["input-container"]}>
                    <div className={style["github"]}>
                        <iframe src="https://ghbtns.com/github-btn.html?user=tupac-amaru&repo=yacep&type=star&count=false&size=large"
                            frameBorder="0" scrolling="0" width="90px" height="30px">
                        </iframe>
                        <iframe src="https://ghbtns.com/github-btn.html?user=tupac-amaru&repo=yacep&type=watch&count=false&size=large&v=2"
                            frameBorder="0" scrolling="0" width="105px" height="30px">
                        </iframe>
                        <iframe src="https://ghbtns.com/github-btn.html?user=tupac-amaru&repo=yacep&type=fork&count=false&size=large"
                            frameBorder="0" scrolling="0" width="90px" height="30px">
                        </iframe>
                    </div>
                    <div className={style["title"]}>
                        yacep
                        <span className={style["intro"]}>  yet another csharp expression parser </span>
                    </div>
                    <input placeholder="input a expression, show it's abstract syntax tree"
                        onKeyUp={self.keyUp.bind(self)} onKeyDown={self.keyDown.bind(self)} />
                </div>
                <div className={style["text-graph-container"]}>
                    <h2 id="user-content-features">Features</h2>
                    <div>
                        <ul>
                            <li>
                                <p><strong>Out of the box</strong> - Zero-Configuration</p>
                            </li>
                            <li>
                                <p><strong>Cross platform</strong> - build with <a href="https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md">netstandard2.0</a></p>
                            </li>
                            <li>
                                <p><strong>Small and tiny</strong> - the core parser code is only 500+ lines</p>
                            </li>
                            <li>
                                <p><strong>Low consumption</strong> - use <a href="https://docs.microsoft.com/en-za/dotnet/api/system.readonlyspan-1?view=netcore-2.2" rel="nofollow">ReadOnlySpan&lt;T&gt; Struct</a> to read string</p>
                            </li>
                            <li>
                                <p><strong>Fast</strong> - good performance to access object’s public method, field, property value instance over using C# reflection</p>
                            </li>
                            <li>
                                <p><strong>Custom unary operator</strong> - support custom a string as an unary operator</p>
                            </li>
                            <li>
                                <p><strong>Custom binary operator</strong> - support custom a string as a binary operator and set an <a href="https://en.wikipedia.org/wiki/Order_of_operations#Programming_language" rel="nofollow">order</a> for it</p>
                            </li>
                            <li>
                                <p><strong>Custom literal</strong> - support custom a literal as a value</p>
                            </li>
                            <li>
                                <p><strong>Custom function</strong> - support custom function</p>
                            </li>
                            <li>
                                <p><strong>Conditional expression</strong> - like <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator" rel="nofollow">?:</a> operator in C#</p>
                            </li>
                            <li>
                                <p><strong>In expression</strong> - evaluates to true if it finds a variable in an array and false otherwise</p>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div className={style["right-container"]}>
                <pre dangerouslySetInnerHTML={{ __html: ast }}>
                </pre>
            </div>
        </div>
    }
}
