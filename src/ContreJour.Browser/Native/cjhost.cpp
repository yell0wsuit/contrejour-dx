// Bodies are EM_ASM so they run in the page's JavaScript scope, where the WebGL context and the animation
// frame live. Ported from cuttherope-dx's ctrdxhost.cpp (its single-threaded half).
//
// This is C++ rather than C only because of the emcc command line: SkiaSharp's WebAssembly native assets
// link Dawn's emdawnwebgpu port, which adds -std=c++20 to the same emcc invocation that compiles this
// file. Hence extern "C" below: the exported names stay unmangled for DirectPInvoke to bind them.

#include <emscripten.h>
#include <stdlib.h>

extern "C"
{

static void (*frame_callback)(double) = NULL;
static void *event_buffer = NULL;

EMSCRIPTEN_KEEPALIVE
void cj_set_frame_callback(void (*callback)(double))
{
    frame_callback = callback;
}

EMSCRIPTEN_KEEPALIVE
void cj_frame_entry(double timestamp)
{
    if (frame_callback != NULL)
    {
        frame_callback(timestamp);
    }
}

// Runs a frame now and abandons the one the browser still owes. A hidden page stops being given animation
// frames, so the loop cannot notice its own pause; host-events.js calls this on focus and visibility changes.
EMSCRIPTEN_KEEPALIVE
void cj_wake(void)
{
    EM_ASM({
        globalThis.cjFrameToken = (globalThis.cjFrameToken | 0) + 1;
        _cj_frame_entry(performance.now());
    });
}

EMSCRIPTEN_KEEPALIVE
void cj_request_frame(void)
{
    EM_ASM({
        var token = (globalThis.cjFrameToken | 0) + 1;
        globalThis.cjFrameToken = token;
        requestAnimationFrame(function (timestamp) {
            if (globalThis.cjFrameToken !== token) {
                return;
            }
            _cj_frame_entry(timestamp);
        });
    });
}

EMSCRIPTEN_KEEPALIVE
int cj_acquire_canvas(void)
{
    return EM_ASM_INT({
        globalThis.cjCanvas = document.getElementById('game');
        return globalThis.cjCanvas ? 1 : 0;
    });
}

EMSCRIPTEN_KEEPALIVE
int cj_create_context(int width, int height)
{
    return EM_ASM_INT({
        var surface = globalThis.cjCanvas;
        if (!surface) {
            return 0;
        }
        surface.width = $0;
        surface.height = $1;
        var context = surface.getContext('webgl2', {
            alpha: true,
            depth: true,
            stencil: true,
            antialias: false,
            premultipliedAlpha: true,
            preserveDrawingBuffer: false
        });
        if (!context) {
            return 0;
        }
        var handle = GL.registerContext(context, {
            majorVersion: 2,
            minorVersion: 0,
            enableExtensionsByDefault: 1,
            alpha: 1,
            depth: 1,
            stencil: 8,
            antialias: 0,
            premultipliedAlpha: 1,
            preserveDrawingBuffer: 0
        });
        if (!handle) {
            return 0;
        }
        GL.makeContextCurrent(handle);
        globalThis.cjContextLost = 0;
        surface.addEventListener('webglcontextlost', function (event) {
            event.preventDefault();
            globalThis.cjContextLost = 1;
            if (globalThis.cjReportContextLost) {
                globalThis.cjReportContextLost();
            }
        });
        return handle;
    }, width, height);
}

EMSCRIPTEN_KEEPALIVE
int cj_resize_canvas(int width, int height)
{
    return EM_ASM_INT({
        var surface = globalThis.cjCanvas;
        if (!surface) {
            return 0;
        }
        surface.width = $0;
        surface.height = $1;
        return 1;
    }, width, height);
}

EMSCRIPTEN_KEEPALIVE
int cj_context_lost(void)
{
    return EM_ASM_INT({
        return globalThis.cjContextLost | 0;
    });
}

EMSCRIPTEN_KEEPALIVE
void *cj_event_buffer(int bytes)
{
    if (event_buffer == NULL)
    {
        event_buffer = calloc(1, (size_t)bytes);
    }
    return event_buffer;
}

} // extern "C"
