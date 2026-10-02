import { test } from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import vm from "node:vm";

const root = new URL("../../src/ContreJour.Browser/wwwroot/", import.meta.url);
const flush = () => new Promise((resolve) => setImmediate(resolve));
function emitter(extra = {}) {
    const listeners = new Map();
    return Object.assign(
        {
            addEventListener(type, fn) {
                if (!listeners.has(type)) listeners.set(type, new Set());
                listeners.get(type).add(fn);
            },
            removeEventListener(type, fn) {
                listeners.get(type)?.delete(fn);
            },
            emit(type, event) {
                for (const fn of [...(listeners.get(type) ?? [])]) fn(event);
            },
        },
        extra,
    );
}
async function harness({
    waiting = null,
    installing = null,
    controller = {},
    registrationMissing = false,
    rejected = false,
} = {}) {
    let updates = 0,
        reloads = 0,
        persistence = 0,
        now = 0;
    const messages = [];
    const worker = emitter({
        state: "installing",
        postMessage(message) {
            messages.push(message.type);
        },
    });
    const dialog = emitter({
        open: false,
        showModal() {
            this.open = true;
        },
        close() {
            this.open = false;
        },
    });
    const elements = { update: dialog, "update-later": {}, "update-now": {} };
    const registration = emitter({
        active: worker,
        waiting: waiting ? worker : null,
        installing: installing ? worker : null,
        async update() {
            updates++;
        },
    });
    const sw = emitter({ controller, ready: Promise.resolve(registration) });
    const document = emitter({
        visibilityState: "visible",
        getElementById(id) {
            return elements[id] ?? null;
        },
    });
    const global = emitter({
        document,
        navigator: {
            serviceWorker: sw,
            storage: {
                async persisted() {
                    return false;
                },
                async persist() {
                    persistence++;
                },
            },
        },
        location: {
            reload() {
                reloads++;
            },
        },
        Date: {
            now() {
                return now;
            },
        },
        console: { warn() {} },
        cjServiceWorkerRegistration: rejected
            ? Promise.reject(new Error("blocked"))
            : Promise.resolve(registrationMissing ? undefined : registration),
    });
    vm.runInNewContext(readFileSync(new URL("pwa.js", root), "utf8"), global);
    await flush();
    return {
        global,
        document,
        sw,
        registration,
        worker,
        dialog,
        elements,
        messages,
        get updates() {
            return updates;
        },
        get reloads() {
            return reloads;
        },
        get persistence() {
            return persistence;
        },
        advance(ms) {
            now += ms;
        },
    };
}

test("waiting update stays waiting when Later is clicked", async () => {
    const h = await harness({ waiting: true });
    assert.equal(h.dialog.open, true);
    h.elements["update-later"].onclick();
    assert.equal(h.dialog.open, false);
    assert.deepEqual(h.messages, []);
    assert.equal(h.reloads, 0);
});

test("accepting update reloads once after controller takeover", async () => {
    const h = await harness({ waiting: true });
    h.elements["update-now"].onclick();
    assert.deepEqual(h.messages, ["skip-waiting"]);
    assert.equal(h.reloads, 0);
    h.sw.emit("controllerchange");
    h.sw.emit("controllerchange");
    assert.equal(h.reloads, 1);
});

test("already installing update is offered on installed state", async () => {
    const h = await harness({ installing: true });
    assert.equal(h.dialog.open, false);
    h.worker.state = "installed";
    h.worker.emit("statechange");
    assert.equal(h.dialog.open, true);
});

test("first install does not interrupt player with an update prompt", async () => {
    const h = await harness({ installing: true, controller: null });
    h.worker.state = "installed";
    h.worker.emit("statechange");
    assert.equal(h.dialog.open, false);
});

test("updates check immediately and on returning to the tab after fifteen minutes", async () => {
    const h = await harness();
    assert.equal(h.updates, 1);
    h.document.emit("visibilitychange");
    assert.equal(h.updates, 1);
    h.advance(16 * 60 * 1000);
    h.document.emit("visibilitychange");
    assert.equal(h.updates, 2);
});

test("persistent storage is requested on the first gesture only", async () => {
    const h = await harness();
    assert.equal(h.persistence, 0);
    h.global.emit("pointerdown");
    await flush();
    assert.equal(h.persistence, 1);
    h.global.emit("keydown");
    await flush();
    assert.equal(h.persistence, 1);
});

test("missing and rejected worker registrations do not break boot", async () => {
    assert.equal((await harness({ registrationMissing: true })).updates, 0);
    assert.equal((await harness({ rejected: true })).updates, 0);
});

test("installation metadata uses Contre Jour branding and files that exist", () => {
    const manifest = JSON.parse(
        readFileSync(new URL("manifest.webmanifest", root), "utf8"),
    );
    assert.equal(manifest.name, "Contre Jour DX");
    assert.equal(manifest.start_url, "./");
    assert.equal(manifest.scope, "./");
    assert.equal(manifest.display, "fullscreen");
    for (const icon of manifest.icons)
        assert.ok(readFileSync(new URL(icon.src, root)).length);
    const html = readFileSync(new URL("index.html", root), "utf8");
    assert.match(html, /rel="manifest"[^>]*href="\.\/manifest.webmanifest"/);
    assert.match(html, /<dialog id="update"[^>]*aria-labelledby=/);
    assert.match(html, /src="\.\/pwa.js"/);
});

test("post-boot callback asks the active worker to cache the selected runtime", async () => {
    const h = await harness();
    await h.global.cjCacheGame("_framework-single");
    assert.deepEqual(h.messages, ["cache-game"]);
});

test("dialog keeps keyboard events out of game handlers without preventing button defaults", async () => {
    const h = await harness();
    let stopped = 0,
        prevented = 0;
    const event = {
        stopPropagation() {
            stopped++;
        },
        preventDefault() {
            prevented++;
        },
    };
    h.dialog.emit("keydown", event);
    h.dialog.emit("keyup", event);
    assert.equal(stopped, 2);
    assert.equal(prevented, 0);
});
