import { test } from "node:test";
import assert from "node:assert/strict";
import { readFile, writeFile } from "node:fs/promises";
import { pathToFileURL } from "node:url";

// A small asynchronous IndexedDB substitute exercises this module's batching and retention.
const records = new Map();
let denied = false;
const request = (action) => {
    const operation = {};
    queueMicrotask(() => {
        try {
            operation.result = action();
            operation.onsuccess?.();
        } catch (error) {
            operation.error = error;
            operation.onerror?.();
        }
    });
    return operation;
};
globalThis.indexedDB = {
    open() {
        return request(() => {
            if (denied) throw new Error("storage disabled");
            return {
                transaction() {
                    const tx = {
                        objectStore() {
                            return {
                                get: (id) => request(() => records.get(id)),
                                put: (record) =>
                                    request(() => {
                                        records.set(record.id, record);
                                        queueMicrotask(() => tx.oncomplete?.());
                                    }),
                                delete: (id) =>
                                    request(() => {
                                        records.delete(id);
                                        queueMicrotask(() => tx.oncomplete?.());
                                    }),
                                getAll: () =>
                                    request(() => [...records.values()]),
                                getAllKeys: () =>
                                    request(() => [...records.keys()]),
                            };
                        },
                    };
                    return tx;
                },
            };
        });
    },
};
const moduleUrl = pathToFileURL(
    new URL("../../src/ContreJour.Browser/wwwroot/log.js", import.meta.url)
        .pathname,
);
const load = (suffix) => import(`${moduleUrl}?test=${suffix}`);

test("ordered batches, page isolation, unique runs, and bounded retention", async () => {
    records.clear();
    const log = await load("ordered");
    const id = log.begin("game header");
    log.beginBrowser("page header");
    log.append("one", false);
    log.append("two", true);
    log.append("three", true);
    log.appendBrowser("page error", true);
    await log.flush();
    const sessions = await log.readAll();
    const game = sessions.find((row) => row.id === id);
    assert.match(game.text, /game header/);
    assert.match(game.text, /one\ntwo\nthree/);
    assert.doesNotMatch(game.text, /page error/);
    assert.match(
        sessions.find((row) => row.id.endsWith("-browser")).text,
        /page error/,
    );
    const next = await load("next");
    assert.notEqual(next.begin("next run"), id);
    await next.flush();
    for (let run = 0; run < 12; run++) {
        const instance = await load(`run-${run}`);
        instance.begin(`run ${run}`);
        await instance.flush();
    }
    assert.equal(
        (await next.readAll()).filter((row) => !row.id.endsWith("-browser"))
            .length,
        10,
    );
});

test("storage denial still permits an in-memory log export", async () => {
    denied = true;
    try {
        const log = await load("denied");
        log.begin("offline header");
        log.append("important error", true);
        await log.flush();
        const sessions = await log.readAll();
        assert.equal(sessions.length, 1);
        assert.match(sessions[0].text, /important error/);
    } finally {
        denied = false;
    }
});

test("records stay bounded and retain recent failure details", async () => {
    const log = await load("bounded");
    const id = log.begin("header");
    log.append("x".repeat(2 * 1024 * 1024), false);
    log.append("last failure", true);
    await log.flush();
    const session = (await log.readAll()).find((row) => row.id === id);
    assert.ok(session.text.length <= 1024 * 1024);
    assert.match(session.text, /last failure/);
});

test("export produces a valid ZIP with its UTF-8 log content", async () => {
    const log = await load("zip");
    const bytes = new TextEncoder().encode("Contre Jour\n[Error] test boom\n");
    assert.equal(log.crc32(new TextEncoder().encode("123456789")), 0xcbf43926);
    const zip = await log.buildZip([{ name: "contrejour-test.log", bytes }]);
    await writeFile(
        "/tmp/contrejour-logging-test.zip",
        new Uint8Array(await zip.arrayBuffer()),
    );
});

test("early page capture and the export control are wired before game boot", async () => {
    const html = await readFile(
        new URL(
            "../../src/ContreJour.Browser/wwwroot/index.html",
            import.meta.url,
        ),
        "utf8",
    );
    assert.ok(
        html.indexOf('src="./console-capture.js"') <
            html.indexOf('import("./main.js")'),
    );
    assert.match(html, /id="download-logs"/);
    assert.match(html, /exportZip/);
});

test("retention protects the current session even when its random identifier sorts first", async () => {
    records.clear();
    const log = await load("current-protected");
    const id = log.begin("current session");
    for (let index = 0; index < 10; index++) {
        records.set(`99999999-${index}`, {
            id: `99999999-${index}`,
            text: "older session",
        });
    }
    await log.flush();
    assert.ok(records.has(id));
    assert.equal(records.size, 10);
});

test("page capture keeps original console output, stacks, resource failures, and avoids recursion", async () => {
    const { runInNewContext } = await import("node:vm");
    const capture = await readFile(
        new URL(
            "../../src/ContreJour.Browser/wwwroot/console-capture.js",
            import.meta.url,
        ),
        "utf8",
    );
    const lines = [];
    const printed = [];
    const listeners = new Map();
    const page = {
        console: {
            warn: (...args) => printed.push(args),
            error: (...args) => printed.push(args),
        },
        addEventListener: (name, handler) => listeners.set(name, handler),
        sinkModule: {
            beginBrowser() {},
            appendBrowser: (line) => lines.push(line),
        },
    };
    // Replace only the module loader to exercise the classic script without a browser runtime.
    runInNewContext(
        capture.replace('import("./log.js")', "Promise.resolve(sinkModule)"),
        page,
    );
    page.console.warn("warning marker");
    page.console.error(new Error("error marker"));
    page.console.warn("cj-log: storage failure");
    listeners.get("unhandledrejection")({
        reason: new Error("rejected marker"),
    });
    listeners.get("error")({
        target: { tagName: "SCRIPT", src: "missing.js" },
    });
    await page.cjCaptureSettled();
    assert.equal(printed.length, 3);
    assert.equal(lines.length, 4);
    assert.ok(lines.some((line) => line.includes("Error: error marker")));
    assert.ok(lines.some((line) => line.includes("Browser.Rejection")));
    assert.ok(
        lines.some(
            (line) =>
                line.includes("Browser.Resource") &&
                line.includes("missing.js"),
        ),
    );
    assert.ok(lines.every((line) => !line.includes("storage failure")));
});
