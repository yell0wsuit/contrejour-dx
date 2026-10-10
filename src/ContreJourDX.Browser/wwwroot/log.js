// Per-run logging adapted from CutTheRopeDX. The single-threaded page and game share
// this module; their records stay separate. IndexedDB keeps ten runs of each kind.
// Keep the existing database name so upgrades retain log history.
const DB_NAME = "contrejour-logs";
const STORE = "sessions";
const MAX_SESSIONS = 10;
const MAX_RECORD_LENGTH = 1024 * 1024;
const FLUSH_DELAY_MS = 2000;
const started = new Date();
const runId =
    formatSessionId(started) +
    "-" +
    String(started.getMilliseconds()).padStart(3, "0") +
    "-" +
    crypto.randomUUID();
const gameRecord = createRecord("");
const browserRecord = createRecord("-browser");
let dbPromise = null;

function createRecord(suffix) {
    return {
        suffix,
        id: null,
        header: "",
        text: "",
        timer: null,
        chain: Promise.resolve(),
        version: 0,
        writtenVersion: -1,
    };
}
function openDatabase() {
    return (dbPromise ??= new Promise((resolve, reject) => {
        const operation = indexedDB.open(DB_NAME, 1);
        operation.onupgradeneeded = () =>
            operation.result.createObjectStore(STORE, { keyPath: "id" });
        operation.onsuccess = () => resolve(operation.result);
        operation.onerror = () => reject(operation.error);
        operation.onblocked = () =>
            reject(new Error("Log database upgrade blocked"));
    }));
}
function request(operation) {
    return new Promise((resolve, reject) => {
        operation.onsuccess = () => resolve(operation.result);
        operation.onerror = () => reject(operation.error);
    });
}
function store(db, mode) {
    return db.transaction(STORE, mode).objectStore(STORE);
}
// Resolve writes only when the transaction commits, so export includes the latest batch.
function commit(db, action) {
    return new Promise((resolve, reject) => {
        const tx = db.transaction(STORE, "readwrite");
        tx.oncomplete = resolve;
        tx.onerror = tx.onabort = () =>
            reject(tx.error ?? new Error("Log write failed"));
        action(tx.objectStore(STORE));
    });
}
export function formatSessionId(date) {
    const pad = (value) => String(value).padStart(2, "0");
    return `${date.getFullYear()}${pad(date.getMonth() + 1)}${pad(date.getDate())}-${pad(date.getHours())}${pad(date.getMinutes())}${pad(date.getSeconds())}`;
}
function describeDevice() {
    const device = globalThis.navigator;
    return `Browser: ${device?.userAgent ?? "unknown"}\nCPU: ${device?.hardwareConcurrency ?? "unknown"} cores`;
}
function beginRecord(record, header) {
    if (record.id !== null) return record.id;
    record.id = runId + record.suffix;
    record.header = (header + "\n" + describeDevice() + "\n").slice(0, 8192);
    record.text = record.header;
    record.version++;
    void flushRecord(record);
    return record.id;
}
function appendRecord(record, line, urgent) {
    if (record.id === null) beginRecord(record, "Contre Jour DX");
    record.text += line.slice(-MAX_RECORD_LENGTH) + "\n";
    record.version++;
    if (record.text.length > MAX_RECORD_LENGTH) {
        const marker = "[Earlier entries truncated]\n";
        const tail = record.text.slice(
            -(MAX_RECORD_LENGTH - record.header.length - marker.length),
        );
        record.text = record.header + marker + tail;
    }
    if (urgent) {
        void flushRecord(record);
    } else if (record.timer === null) {
        record.timer = setTimeout(() => {
            record.timer = null;
            void flushRecord(record);
        }, FLUSH_DELAY_MS);
    }
}
function flushRecord(record) {
    if (record.timer !== null) {
        clearTimeout(record.timer);
        record.timer = null;
    }
    if (record.id === null) return record.chain;
    const write = async () => {
        if (record.writtenVersion === record.version) return;
        const version = record.version;
        const text = record.text;
        try {
            const db = await openDatabase();
            await commit(db, (target) => target.put({ id: record.id, text }));
            record.writtenVersion = version;
            const ids = (await request(store(db, "readonly").getAllKeys()))
                .filter(
                    (id) =>
                        id !== record.id &&
                        id.endsWith("-browser") === (record === browserRecord),
                )
                .sort();
            const doomed = ids.slice(
                0,
                Math.max(0, ids.length + 1 - MAX_SESSIONS),
            );
            if (doomed.length > 0)
                await commit(db, (target) =>
                    doomed.forEach((id) => target.delete(id)),
                );
        } catch {
            // Keep the bounded current record in memory for export when storage is denied.
        }
    };
    record.chain = record.chain.then(write, write);
    return record.chain;
}
export function begin(header) {
    return beginRecord(gameRecord, header);
}
export function beginBrowser(header) {
    return beginRecord(browserRecord, header);
}
export function append(line, urgent) {
    appendRecord(gameRecord, line, urgent);
}
export function appendBrowser(line, urgent) {
    appendRecord(browserRecord, line, urgent);
}
export function flush() {
    return Promise.all([flushRecord(gameRecord), flushRecord(browserRecord)]);
}
export async function readAll() {
    await flush();
    let sessions = [];
    try {
        sessions = await request(
            store(await openDatabase(), "readonly").getAll(),
        );
    } catch {}
    // Prefer the live copy, including entries whose storage transaction failed.
    const byId = new Map(sessions.map((session) => [session.id, session]));
    for (const record of [gameRecord, browserRecord]) {
        if (record.id !== null)
            byId.set(record.id, { id: record.id, text: record.text });
    }
    const ordered = [...byId.values()].sort((a, b) => a.id.localeCompare(b.id));
    // A previously pruned live tab can still export its own run without exceeding retention.
    const retained = [];
    for (const record of [gameRecord, browserRecord]) {
        const others = ordered.filter(
            (row) =>
                row.id !== record.id &&
                row.id.endsWith("-browser") === (record === browserRecord),
        );
        retained.push(
            ...others.slice(-(MAX_SESSIONS - (record.id === null ? 0 : 1))),
        );
        if (record.id !== null) retained.push(byId.get(record.id));
    }
    return retained.sort((a, b) => a.id.localeCompare(b.id));
}

const CRC_TABLE = (() => {
    const table = new Uint32Array(256);
    for (let i = 0; i < 256; i++) {
        let value = i;
        for (let bit = 0; bit < 8; bit++) {
            value = value & 1 ? 0xedb88320 ^ (value >>> 1) : value >>> 1;
        }
        table[i] = value >>> 0;
    }
    return table;
})();

export function crc32(bytes) {
    let crc = 0xffffffff;
    for (let i = 0; i < bytes.length; i++) {
        crc = CRC_TABLE[(crc ^ bytes[i]) & 0xff] ^ (crc >>> 8);
    }
    return (crc ^ 0xffffffff) >>> 0;
}

async function deflate(bytes) {
    // Every browser this build supports has compression streams; the check is for the one that
    // turns it off rather than for an old engine. Stored entries still make a valid archive.
    if (typeof CompressionStream !== "function") {
        return null;
    }

    try {
        const stream = new Blob([bytes])
            .stream()
            .pipeThrough(new CompressionStream("deflate-raw"));
        return new Uint8Array(await new Response(stream).arrayBuffer());
    } catch {
        return null;
    }
}

function dosTime(date) {
    const time =
        (date.getHours() << 11) |
        (date.getMinutes() << 5) |
        (date.getSeconds() >> 1);
    const day =
        ((date.getFullYear() - 1980) << 9) |
        ((date.getMonth() + 1) << 5) |
        date.getDate();
    return { time, day };
}

/**
 * Builds a zip holding one .log per stored run.
 *
 * @param {{name: string, bytes: Uint8Array}[]} files Entries to archive.
 * @returns {Promise<Blob>} The archive.
 */
export async function buildZip(files) {
    const encoder = new TextEncoder();
    const parts = [];
    const central = [];
    const stamp = dosTime(new Date());
    let offset = 0;

    for (const file of files) {
        const name = encoder.encode(file.name);
        const compressed = await deflate(file.bytes);
        const body = compressed ?? file.bytes;
        const method = compressed === null ? 0 : 8;
        const crc = crc32(file.bytes);

        const local = new DataView(new ArrayBuffer(30));
        local.setUint32(0, 0x04034b50, true);
        local.setUint16(4, 20, true);
        local.setUint16(6, 0, true);
        local.setUint16(8, method, true);
        local.setUint16(10, stamp.time, true);
        local.setUint16(12, stamp.day, true);
        local.setUint32(14, crc, true);
        local.setUint32(18, body.length, true);
        local.setUint32(22, file.bytes.length, true);
        local.setUint16(26, name.length, true);
        local.setUint16(28, 0, true);
        parts.push(new Uint8Array(local.buffer), name, body);

        const entry = new DataView(new ArrayBuffer(46));
        entry.setUint32(0, 0x02014b50, true);
        entry.setUint16(4, 20, true);
        entry.setUint16(6, 20, true);
        entry.setUint16(8, 0, true);
        entry.setUint16(10, method, true);
        entry.setUint16(12, stamp.time, true);
        entry.setUint16(14, stamp.day, true);
        entry.setUint32(16, crc, true);
        entry.setUint32(20, body.length, true);
        entry.setUint32(24, file.bytes.length, true);
        entry.setUint16(28, name.length, true);
        entry.setUint32(42, offset, true);
        central.push(new Uint8Array(entry.buffer), name);

        offset += 30 + name.length + body.length;
    }

    const centralSize = central.reduce((total, part) => total + part.length, 0);
    const end = new DataView(new ArrayBuffer(22));
    end.setUint32(0, 0x06054b50, true);
    end.setUint16(8, files.length, true);
    end.setUint16(10, files.length, true);
    end.setUint32(12, centralSize, true);
    end.setUint32(16, offset, true);

    return new Blob([...parts, ...central, new Uint8Array(end.buffer)], {
        type: "application/zip",
    });
}

export async function exportZip() {
    await globalThis.cjCaptureSettled?.();
    const sessions = await readAll();
    if (sessions.length === 0) return 0;
    const encoder = new TextEncoder();
    const archive = await buildZip(
        sessions.map((session) => ({
            name: `contrejour-dx-${session.id}.log`,
            bytes: encoder.encode(session.text),
        })),
    );
    const url = URL.createObjectURL(archive);
    const link = document.createElement("a");
    link.href = url;
    link.download = `contrejour-dx-logs-${formatSessionId(new Date())}.zip`;
    link.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
    return sessions.length;
}

globalThis.addEventListener?.("pagehide", () => {
    void flush();
});
globalThis.document?.addEventListener("visibilitychange", () => {
    if (document.visibilityState === "hidden") void flush();
});
