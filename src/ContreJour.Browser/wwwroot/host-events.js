// The page's write side of the event ring. Wasm linear memory is a SharedArrayBuffer in
// the threaded build, so events reach the game thread without a message, a structured
// clone, or an allocation per pointer move.
//
// This is the single-threaded build, which still writes through the ring rather than calling
// managed code directly, so the threaded build can reuse it. Atomics.load, .store and .add
// accept an ordinary ArrayBuffer - only Atomics.wait insists on a shared one, and nothing
// here waits.

const HEADER_BYTES = 16;
const RECORD_BYTES = 24;
const CAPACITY = 1024;
// Pointer moves are the only high-volume event. Stop admitting them before the
// ring is full so releases, lifecycle transitions, and resizes retain space.
const CONTROL_RESERVE = 32;

const WRITE_INDEX = 0;
const READ_INDEX = 1;
const DROPPED = 2;

const KIND_POINTER = 1;
const KIND_KEY = 2;
const KIND_WHEEL = 3;
const KIND_ACTIVE = 4;
const KIND_RESIZE = 5;
const KIND_START = 6;

// KeyboardEvent.code -> BrowserKeys id (src/ContreJour.Browser.Platform/BrowserKeys.cs); the tables must agree.
// Q and R stand in for Escape and F5, which a browser keeps for itself.
const KEY_IDS = {
    Escape: 1,
    KeyQ: 2,
    KeyR: 3,
    Backspace: 4,
    Enter: 5,
    NumpadEnter: 5,
    CapsLock: 6,
    ArrowLeft: 7,
    ArrowUp: 8,
    ArrowRight: 9,
    ArrowDown: 10,
    Delete: 11,
    KeyA: 12,
    KeyD: 13,
    KeyS: 14,
    KeyW: 15,
    ShiftLeft: 16,
    ShiftRight: 17,
};

const POINTER_KINDS = { mouse: 0, pen: 1, touch: 2 };

let baseWord = 0;
let view = null;
let viewBuffer = null;

// A grown wasm memory replaces the buffer behind every typed-array view, so the
// view is rebuilt whenever the buffer identity changes. The ring's address does
// not move, only the window onto it.
function heap() {
    // The runtime exports heap views but not wasmMemory, and a debug build turns any
    // read of an unexported symbol - even an optional one - into an abort. Read the
    // current view each time: Emscripten replaces it when memory grows.
    const buffer = globalThis.cjWasmModule.HEAPU8.buffer;
    if (buffer !== viewBuffer) {
        viewBuffer = buffer;
        view = new Int32Array(buffer);
    }
    return view;
}

function write(kind, word0, word1, word2, word3, word4, droppable = false) {
    if (baseWord === 0) {
        return false;
    }

    const words = heap();
    const writeIndex = words[baseWord + WRITE_INDEX];
    const readIndex = Atomics.load(words, baseWord + READ_INDEX);
    const limit = droppable ? CAPACITY - CONTROL_RESERVE : CAPACITY;
    if (writeIndex - readIndex >= limit) {
        Atomics.add(words, baseWord + DROPPED, 1);
        return false;
    }

    const slot =
        baseWord +
        HEADER_BYTES / 4 +
        ((writeIndex >>> 0) & (CAPACITY - 1)) * (RECORD_BYTES / 4);
    words[slot] = kind;
    words[slot + 1] = word0;
    words[slot + 2] = word1;
    words[slot + 3] = word2;
    words[slot + 4] = word3;
    words[slot + 5] = word4;

    // Published last, so the reader never sees a slot before it is filled.
    Atomics.store(words, baseWord + WRITE_INDEX, writeIndex + 1);
    return true;
}

const floatBits = new DataView(new ArrayBuffer(4));
function bits(value) {
    floatBits.setFloat32(0, value, true);
    return floatBits.getInt32(0, true);
}

export function attach(address) {
    baseWord = address / 4;
}

// A hidden page stops being given animation frames, so the loop that would have noticed
// the change is the loop the change put to sleep. Waking it is what lets a pause be
// acted on and a resume re-arm the frame.
function wake() {
    globalThis.cjWasmModule?._cj_wake?.();
}

export function keyId(code) {
    return KEY_IDS[code];
}

// The keys the game takes are kept from the page (no scrolling, no back navigation) unless a modifier says
// they are meant for the browser - Cmd+R must still reload.
export function forwardsKey(event) {
    return (
        KEY_IDS[event.code] !== undefined &&
        !event.ctrlKey &&
        !event.metaKey &&
        !event.altKey
    );
}

export function pointer(phase, event, x, y) {
    const kind = POINTER_KINDS[event.pointerType] ?? 0;
    write(
        KIND_POINTER,
        phase | (kind << 8),
        event.pointerId,
        bits(x),
        bits(y),
        event.buttons,
        phase === 1,
    );
}

export function key(id, down) {
    write(KIND_KEY, down ? 1 : 0, id, 0, 0, 0);
}

export function wheel(delta) {
    write(KIND_WHEEL, delta, 0, 0, 0, 0);
}

export function active(isActive, isHidden) {
    // Hidden travels separately from active rather than being folded into it. A window merely
    // pushed behind another is inactive but still composited and still given frames, while a
    // hidden page is neither, and the two call for different responses.
    //
    // Woken on both edges rather than only the pause. Going inactive needs a wake because the
    // page stops being given animation frames before the loop would notice; coming back needs
    // one because the frame scheduled before the page was hidden is not reliably redelivered,
    // and nothing else re-arms the loop. A wake on a running loop costs one extra frame entry,
    // which is cheaper than a session that never resumes.
    if (write(KIND_ACTIVE, isActive ? 1 : 0, isHidden ? 1 : 0, 0, 0, 0)) {
        wake();
    }
}

export function resize(cssWidth, cssHeight, devicePixelRatio) {
    write(
        KIND_RESIZE,
        bits(cssWidth),
        bits(cssHeight),
        bits(devicePixelRatio),
        0,
        0,
    );
}

// The game holds still until this arrives. Boot has to finish before the player can press
// Play, so the loop is already running by now - the wake is for the case where the page was
// hidden and put it back to sleep in between.
export function start() {
    if (write(KIND_START, 0, 0, 0, 0, 0)) {
        wake();
    }
}
