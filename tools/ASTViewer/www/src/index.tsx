import * as React from "react";
import * as ReactDOM from "react-dom";
import { Viewer } from './components/viewer';

(() => {
    const container = document.getElementById("viewer");
    if (container) {
        ReactDOM.render(<Viewer />, container, () => {
            const loadingContainer = document.getElementsByClassName("loading-cover")[0] as HTMLDivElement;
            if (loadingContainer) {
                loadingContainer.className = "loading-cover fade-out";
                setTimeout(() => { loadingContainer.style.display = "none"; }, 1000);
            }
        });
    }
})();