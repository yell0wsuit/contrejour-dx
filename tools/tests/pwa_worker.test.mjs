import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import vm from 'node:vm';

const root = new URL('../../src/ContreJour.Browser/wwwroot/', import.meta.url);
const scope = 'https://example.test/contre-jour/';
const assets = [
    ['index.html', 'shell'], ['main.js', 'script'],
    ['coi.js', 'bootstrap'], ['coi-sw.js', 'worker'],
    ['manifest.webmanifest', 'manifest'],
    ['_framework/a.wasm', 'threaded'], ['_framework-single/a.wasm', 'single'],
    ['content/audio.ogg', 'audio'],
].map(([url, hash]) => ({ url, hash }));

function harness(version = 'v1', entries = assets, stores = new Map()) {
    const handlers = new Map(), fetched = [];
    let offline = false, skipped = 0, claimed = 0;
    const key = (request) => typeof request === 'string' ? request : request.url ?? request.href;
    const cache = (name) => {
        if (!stores.has(name)) stores.set(name, new Map());
        const data = stores.get(name);
        return {
            async match(r) { return data.get(key(r))?.clone(); },
            async put(r, response) { data.set(key(r), response.clone()); },
            async add(r) { await this.put(r, await fetcher(r)); },
            async keys() { return [...data.keys()].map(url => new Request(url)); },
            async delete(r) { return data.delete(key(r)); },
        };
    };
    const fetcher = async (request) => {
        fetched.push(key(request));
        if (offline) throw new Error('offline');
        return new Response(key(request).endsWith('.ogg') ? '0123456789' : 'shell');
    };
    const self = {
        assetsManifest: {version, assets: entries}, location: {href: scope + 'coi-sw.js'},
        importScripts() {}, addEventListener(type, fn) { handlers.set(type, fn); },
        skipWaiting() { skipped++; }, clients: {async claim() { claimed++; }},
    };
    vm.runInNewContext(readFileSync(new URL('service-worker.published.js', root), 'utf8'), {
        self, caches: {async open(name) {return cache(name);}, async keys() {return [...stores.keys()];},
            async delete(name) {return stores.delete(name);}, async match(r) {
                for (const name of stores.keys()) { const response = await cache(name).match(r); if (response) return response; }
            }}, fetch: fetcher, URL, Request, Response, Headers, setTimeout,
    });
    return {
        stores, fetched, handlers, cache, get skipped() {return skipped;}, get claimed() {return claimed;},
        offline() {offline = true;},
        async lifecycle(type) { let promise; handlers.get(type)({waitUntil(p) {promise = p;}}); await promise; },
        async request(path, options = {}) {
            let promise;
            handlers.get('fetch')({request: new Request(scope + path, options), respondWith(p) {promise = p;}});
            return promise;
        },
    };
}

test('install precaches shell and leaves both runtime trees and content for demand', async () => {
    const h = harness(); await h.lifecycle('install');
    assert.deepEqual(h.fetched.sort(), [scope + 'index.html', scope + 'main.js', scope + 'coi.js'].sort());
    assert.equal(h.skipped, 0);
    h.handlers.get('message')({data: {type: 'skip-waiting'}});
    assert.equal(h.skipped, 1);
});

test('selected runtime and content remain available offline with isolation headers', async () => {
    const h = harness(); await h.lifecycle('install'); await h.lifecycle('activate');
    await h.request('_framework-single/a.wasm'); await h.request('content/audio.ogg'); h.offline();
    const runtime = await h.request('_framework-single/a.wasm');
    assert.equal(runtime.headers.get('Cross-Origin-Opener-Policy'), 'same-origin');
    assert.equal(runtime.headers.get('Cross-Origin-Embedder-Policy'), 'require-corp');
    assert.equal(await runtime.text(), 'shell');
    assert.equal(await (await h.request('content/audio.ogg')).text(), '0123456789');
    assert.equal(h.fetched.includes(scope + '_framework/a.wasm'), false);
    assert.equal(h.claimed, 1);
});

test('offline audio ranges return 206 and invalid ranges return 416', async () => {
    const h = harness(); await h.request('content/audio.ogg'); h.offline();
    const response = await h.request('content/audio.ogg', {headers: {range: 'bytes=2-4'}});
    assert.equal(response.status, 206); assert.equal(await response.text(), '234');
    assert.equal(response.headers.get('content-range'), 'bytes 2-4/10');
    const suffix = await h.request('content/audio.ogg', {headers: {range: 'bytes=-3'}});
    assert.equal(await suffix.text(), '789');
    assert.equal((await h.request('content/audio.ogg', {headers: {range: 'bytes=20-'}})).status, 416);
});

test('activation keeps unchanged content, prunes changed content and retires only cj shell caches', async () => {
    const h = harness(); await h.lifecycle('install'); await h.request('content/audio.ogg');
    h.stores.set('other-app-shell', new Map());
    const same = harness('v2', assets, h.stores); await same.lifecycle('activate');
    assert.equal(h.stores.has('cj-shell-v1'), false);
    assert.equal(h.stores.has('other-app-shell'), true);
    same.offline(); assert.equal(await (await same.request('content/audio.ogg')).text(), '0123456789');
    const changed = harness('v3', assets.map(a => a.url.startsWith('content/') ? {...a, hash: 'changed'} : a), h.stores);
    await changed.lifecycle('activate'); changed.offline();
    await assert.rejects(changed.request('content/audio.ogg'), /offline/);
});

test('isolation bootstrap is available offline to attach PWA update watching', async () => {
    const h = harness(); await h.lifecycle('install'); h.offline();
    assert.equal(await (await h.request('coi.js')).text(), 'shell');
});

test('post-boot warming caches only selected runtime and all content after deferred registration', async () => {
    const h = harness(); await h.lifecycle('install'); await h.lifecycle('activate');
    let warming;
    h.handlers.get('message')({data: {type: 'cache-game', runtime: '_framework-single'}, waitUntil(p) {warming = p;}});
    await warming; h.offline();
    assert.equal(await (await h.request('_framework-single/a.wasm')).text(), 'shell');
    assert.equal(await (await h.request('content/audio.ogg')).text(), '0123456789');
    assert.equal(h.fetched.includes(scope + '_framework/a.wasm'), false);
});
