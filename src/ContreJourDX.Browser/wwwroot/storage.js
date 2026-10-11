// localStorage accessors. Synchronous by nature, which is exactly what Preferences needs:
// it reads and writes from ordinary game code with no await points available.

export function read(key) {
    return globalThis.localStorage.getItem(key);
}

export function write(key, value) {
    globalThis.localStorage.setItem(key, value);
}
