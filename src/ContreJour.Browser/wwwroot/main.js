import * as hostEvents from "./host-events.js";
import { setFullScreen, setLoadingProgress } from "./page.js";

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

// Pressing Play: full screen first, while the click still counts as a gesture, then let the game start.
globalThis.cjStart = () => {
    setFullScreen(true);
    hostEvents.start();
};
globalThis.cjReady?.();

// A test aid: ?autostart presses Play as soon as boot is done, so a headless browser can run the game.
// Full screen and audio are refused without a gesture; the game runs anyway.
if (new URLSearchParams(globalThis.location.search).has("autostart")) {
    globalThis.cjStart();
    document.getElementById("splash")?.classList.add("hidden");
}

globalThis.cjBootComplete?.();
