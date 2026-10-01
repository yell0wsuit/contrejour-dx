import * as hostEvents from "./host-events.js";
import { unlock } from "./audio.js";
import { setLoadingProgress } from "./page.js";

// The failure seam is installed by the inline module in index.html, because this module's static imports are
// fetched before its first statement runs.
const fail = (id, detail) => globalThis.cjFail?.(id, detail);

try {
    const { dotnet } = await import("./_framework/dotnet.js");
    const runtime = await dotnet
        .withDiagnosticTracing(false)
        .withModuleConfig({
            onDownloadResourceProgress: (loaded, total) => setLoadingProgress("runtime", loaded, total),
        })
        .create();
    globalThis.cjWasmModule = runtime.Module;
    // Returns once boot is done: content preloaded, the frame loop running and waiting for Play.
    await runtime.runMain(runtime.getConfig().mainAssemblyName, []);
} catch (error) {
    fail("boot-error", error);
    throw error;
}

const canvas = document.getElementById("game");

// getBoundingClientRect forces the browser to settle layout before it answers, and a drag
// asks once per pointermove. The rectangle only moves when the canvas box does, so it is
// measured then and reused for every event in between.
let canvasRect = null;
const invalidateCanvasRect = () => {
    canvasRect = null;
};
new ResizeObserver(invalidateCanvasRect).observe(canvas);
globalThis.addEventListener("resize", invalidateCanvasRect);
globalThis.addEventListener("scroll", invalidateCanvasRect, {
    capture: true,
    passive: true,
});

const sendPointer = (event, phase) => {
    event.preventDefault();
    canvasRect ??= canvas.getBoundingClientRect();
    hostEvents.pointer(phase, event, event.clientX - canvasRect.left, event.clientY - canvasRect.top);
};

canvas.addEventListener("pointerdown", (event) => {
    canvas.setPointerCapture(event.pointerId);
    sendPointer(event, 0);
});
canvas.addEventListener("pointermove", (event) => sendPointer(event, 1));
canvas.addEventListener("pointerup", (event) => sendPointer(event, 2));
canvas.addEventListener("pointercancel", (event) => sendPointer(event, 3));
canvas.addEventListener("contextmenu", (event) => event.preventDefault());

// The game scrolls in the desktop's wheel units: one notch is 120 and positive scrolls up. A
// WheelEvent reports the opposite sign and, depending on deltaMode, counts lines or pages
// rather than pixels - so both are normalized here and the game sees what it does on desktop.
const PIXELS_PER_NOTCH = 100;
const UNITS_PER_NOTCH = 120;
// Firefox reports one notch as three lines where other browsers report ~100px, so a line is
// worth a third of a notch here rather than a text line's height.
const PIXELS_PER_LINE = PIXELS_PER_NOTCH / 3;

canvas.addEventListener(
    "wheel",
    (event) => {
        event.preventDefault();
        const scale =
            event.deltaMode === 1
                ? PIXELS_PER_LINE
                : event.deltaMode === 2
                  ? canvas.clientHeight
                  : 1;
        const units =
            (-event.deltaY * scale * UNITS_PER_NOTCH) / PIXELS_PER_NOTCH;
        const rounded = Math.round(units);
        if (rounded !== 0) {
            hostEvents.wheel(rounded);
        }
    },
    // preventDefault needs a non-passive listener, which wheel handlers default to.
    { passive: false },
);

const onKey = (event, down) => {
    if (!hostEvents.forwardsKey(event)) {
        return;
    }
    event.preventDefault();
    hostEvents.key(hostEvents.keyId(event.code), down);
};
globalThis.addEventListener("keydown", (event) => onKey(event, true));
globalThis.addEventListener("keyup", (event) => onKey(event, false));

// Focus and visibility are separate losses and either one must freeze the game: a hidden
// tab stops getting animation frames but keeps its audio, while a window merely pushed
// behind another stays visible and keeps ticking at full speed.
const syncActive = () => {
    const visible = document.visibilityState === "visible";
    hostEvents.active(visible && document.hasFocus(), !visible);
};
globalThis.addEventListener("focus", syncActive);
globalThis.addEventListener("blur", syncActive);
document.addEventListener("visibilitychange", syncActive);

// A mobile browser can discard a backgrounded page without giving it another
// visibilitychange, and pagehide is the last callback such a page receives. Treating it as a
// deactivation is a best-effort request for the pending save, not an assurance of one.
// pageshow covers the other direction, since a back/forward-cache restore fires no
// visibilitychange.
globalThis.addEventListener("pagehide", () => hostEvents.active(false, true));
globalThis.addEventListener("pageshow", syncActive);

syncActive();

// Pressing Play: unlock audio while the click still counts as a gesture, then let the game start.
globalThis.cjStart = () => {
    unlock();
    hostEvents.start();
};
globalThis.cjReady?.();

// A test aid: ?autostart presses Play as soon as boot is done, so a headless browser can run the game.
// Audio is refused without a gesture; the game runs anyway.
if (new URLSearchParams(globalThis.location.search).has("autostart")) {
    globalThis.cjStart();
    document.getElementById("splash")?.classList.add("hidden");
}

globalThis.cjBootComplete?.();
