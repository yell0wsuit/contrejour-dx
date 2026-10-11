// WebAudio for the game. Effects are decoded up front and played as buffer sources through one effects gain
// (the mute switch). Songs are kept as blob URLs of the preloaded bytes and streamed by a single <audio>
// element through a music gain (fades): decoding all seven (~15 minutes) would hold ~345 MB of PCM.

let context = null;
let effectsGain = null;
let musicGain = null;
let musicElement = null;
let fadeTimer = 0;
// The game wants the song heard: set by playSong, cleared when a fade ends.
let songWanted = false;
let songPaused = false;
let suspended = false;

const effects = new Map();
const songs = new Map();
const voices = new Set();

function ensureContext() {
    if (context === null) {
        context = new (
            globalThis.AudioContext || globalThis.webkitAudioContext
        )();
        effectsGain = context.createGain();
        effectsGain.connect(context.destination);
        musicGain = context.createGain();
        musicGain.connect(context.destination);
    }
    return context;
}

function music() {
    if (musicElement === null) {
        musicElement = new Audio();
        musicElement.loop = true;
        ensureContext()
            .createMediaElementSource(musicElement)
            .connect(musicGain);
    }
    return musicElement;
}

function playMusicElement() {
    if (songWanted && !songPaused && !suspended) {
        void music()
            .play()
            .catch(() => {});
    }
}

// Called from the Play click: a context may only start inside a user gesture.
export function unlock() {
    void ensureContext()
        .resume()
        .catch(() => {});
}

export async function decodeEffect(key, url) {
    const response = await fetch(url);
    if (!response.ok) {
        return 0;
    }
    effects.set(
        key,
        await ensureContext().decodeAudioData(await response.arrayBuffer()),
    );
    return 1;
}

export async function loadSong(key, url) {
    const response = await fetch(url);
    if (!response.ok) {
        return 0;
    }
    songs.set(key, URL.createObjectURL(await response.blob()));
    return 1;
}

export function playEffect(key, volume) {
    const buffer = effects.get(key);
    if (buffer === undefined || suspended) {
        return;
    }
    const ctx = ensureContext();
    const source = ctx.createBufferSource();
    const gain = ctx.createGain();
    source.buffer = buffer;
    gain.gain.value = volume;
    source.connect(gain).connect(effectsGain);
    voices.add(source);
    source.onended = () => voices.delete(source);
    source.start();
}

export function stopEffects() {
    for (const source of voices) {
        try {
            source.stop();
        } catch {
            // Already ended.
        }
    }
    voices.clear();
}

export function setEffectsMuted(muted) {
    ensureContext();
    effectsGain.gain.value = muted ? 0 : 1;
}

export function playSong(key) {
    const url = songs.get(key);
    if (url === undefined) {
        return;
    }
    const element = music();
    clearTimeout(fadeTimer);
    const now = context.currentTime;
    musicGain.gain.cancelScheduledValues(now);
    musicGain.gain.setValueAtTime(1, now);
    if (element.src !== url) {
        element.src = url;
    }
    element.currentTime = 0;
    songWanted = true;
    songPaused = false;
    playMusicElement();
}

export function fadeOutSong(seconds) {
    if (musicElement === null || !songWanted) {
        return;
    }
    const now = context.currentTime;
    musicGain.gain.cancelScheduledValues(now);
    musicGain.gain.setValueAtTime(musicGain.gain.value, now);
    musicGain.gain.linearRampToValueAtTime(0, now + seconds);
    clearTimeout(fadeTimer);
    fadeTimer = setTimeout(() => {
        songWanted = false;
        musicElement.pause();
    }, seconds * 1000);
}

export function setSongPaused(paused) {
    if (musicElement === null || !songWanted) {
        return;
    }
    songPaused = paused;
    if (paused) {
        musicElement.pause();
    } else {
        playMusicElement();
    }
}

// The page went away (or came back): everything holds still, as the desktop host suspends its audio device.
export function setSuspended(value) {
    suspended = value;
    if (context === null) {
        return;
    }
    if (value) {
        void context.suspend().catch(() => {});
        musicElement?.pause();
    } else {
        void context.resume().catch(() => {});
        playMusicElement();
        // resume() settles later, and Safari refuses it outside a gesture for a context it interrupted; until
        // the graph runs, the next tap or key retries it.
        if (context.state !== "running") {
            armGestureResume();
        }
    }
}

// Safari suspends the graph while the page is in the background, and has an "interrupted" state of its own
// that a phone call or another app taking the audio device puts it in. Either way the context can stay
// stopped after the game resumes it, so a gesture retries both the graph and the song until it runs.
function resumeFromGesture() {
    unlock();
    playMusicElement();
    if (context !== null && context.state === "running") {
        globalThis.removeEventListener("pointerdown", resumeFromGesture);
        globalThis.removeEventListener("keydown", resumeFromGesture);
    }
}

function armGestureResume() {
    globalThis.addEventListener("pointerdown", resumeFromGesture, {
        passive: true,
    });
    globalThis.addEventListener("keydown", resumeFromGesture, {
        passive: true,
    });
}

// An interruption while the page stays visible (and the game active) never reaches setSuspended.
document.addEventListener("visibilitychange", () => {
    if (
        document.visibilityState !== "visible" ||
        suspended ||
        context === null ||
        context.state === "running"
    ) {
        return;
    }
    unlock();
    armGestureResume();
});
