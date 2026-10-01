import * as hostEvents from "./host-events.js";

// The page side of the browser host: canvas shape, screen, language, progress text, full screen and cursor.

const MAX_PIXEL_RATIO = 2;

function pixelRatio() {
    return Math.min(globalThis.devicePixelRatio || 1, MAX_PIXEL_RATIO);
}

// [cssWidth, cssHeight, backingWidth, backingHeight]
function measure(canvas) {
    const ratio = pixelRatio();
    const cssWidth = Math.max(1, Math.round(canvas.clientWidth));
    const cssHeight = Math.max(1, Math.round(canvas.clientHeight));
    return [
        cssWidth,
        cssHeight,
        Math.max(1, Math.round(cssWidth * ratio)),
        Math.max(1, Math.round(cssHeight * ratio)),
    ];
}

export function measureCanvas(canvasId) {
    const canvas = document.getElementById(canvasId);
    return canvas === null ? [] : measure(canvas);
}

// [width, height, devicePixelRatio] of the screen the page is on, for the game's logical canvas.
export function screenSize() {
    return [
        globalThis.screen?.width || 1,
        globalThis.screen?.height || 1,
        globalThis.devicePixelRatio || 1,
    ];
}

export function query() {
    return globalThis.location.search;
}

export function navigatorLanguage() {
    return globalThis.navigator?.language ?? "";
}

export function setLoadingProgress(type, loaded, total) {
    const progress = document.getElementById("splash-progress");
    if (progress !== null) {
        progress.textContent = `Loading ${type}: ${loaded} of ${total}…`;
    }
}

export function isFullScreen() {
    return document.fullscreenElement != null;
}

// Outside a user gesture the browser refuses; that is not an error worth reporting.
export function setFullScreen(on) {
    if (on && document.fullscreenElement == null) {
        void document.documentElement.requestFullscreen?.().catch(() => {});
    } else if (!on && document.fullscreenElement != null) {
        void document.exitFullscreen?.().catch(() => {});
    }
}

export function setCursorVisible(visible) {
    const canvas = document.getElementById("game");
    if (canvas !== null) {
        canvas.style.cursor = visible ? "" : "none";
    }
}

// A lost context cannot be rebuilt in place yet: the GPU objects the renderer holds outlive the context
// that made them. Reloading is what recovers, so the player is handed the reload rather than told to go
// and find it. The game saves as it notices the loss.
function reportContextLost() {
    document.getElementById("splash")?.classList.remove("hidden");
    for (const element of ["splash-spinner", "splash-progress", "start"]) {
        document.getElementById(element)?.setAttribute("hidden", "");
    }
    globalThis.cjStopHint?.();
    document.getElementById("context-lost-error")?.removeAttribute("hidden");

    const resume = document.getElementById("resume");
    if (resume === null) {
        return;
    }
    resume.hidden = false;
    resume.addEventListener("click", () => globalThis.location.reload(), {
        once: true,
    });
}

// cjhost.cpp's webglcontextlost listener calls this.
globalThis.cjReportContextLost = reportContextLost;

let watchedCanvas = null;
let devicePixelRatioQuery = null;
let canvasObserver = null;

function report(canvas) {
    hostEvents.resize(
        Math.max(1, canvas.clientWidth),
        Math.max(1, canvas.clientHeight),
        pixelRatio(),
    );
}

// A ResizeObserver reports the canvas box changing, but not the page moving to a display of a
// different pixel density. A media query pinned to the current ratio covers that: it stops
// matching the moment the ratio changes, and is re-armed against the new one.
function watchDevicePixelRatio() {
    devicePixelRatioQuery?.removeEventListener(
        "change",
        onDevicePixelRatioChange,
    );
    devicePixelRatioQuery = globalThis.matchMedia(
        `(resolution: ${globalThis.devicePixelRatio || 1}dppx)`,
    );
    devicePixelRatioQuery.addEventListener("change", onDevicePixelRatioChange);
}

function onDevicePixelRatioChange() {
    if (watchedCanvas !== null) {
        report(watchedCanvas);
        watchDevicePixelRatio();
    }
}

// The canvas fills the viewport through the stylesheet; the game letterboxes its fixed canvas into
// whatever box this reports.
export function watchCanvas(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (canvas === null) {
        return;
    }
    canvasObserver?.disconnect();
    watchedCanvas = canvas;
    canvasObserver = new ResizeObserver(() => report(canvas));
    canvasObserver.observe(canvas);
    watchDevicePixelRatio();
    report(canvas);
}
