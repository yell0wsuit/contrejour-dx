// The splash's floating planet, ported from the web version's loader (cj.js): two planet textures
// turning in opposite directions behind the foreground planet, the whole planet drifting on a slow
// sine, and sparkles rising from below it. The web version advanced it 60 times a second; this keeps
// that rate whatever the display refreshes at, and stops while the splash is hidden.

const TICK_MS = 1000 / 60;
const ACTIVE_PARTICLES = 80;

const loadImage = (src) => {
    const image = new Image();
    image.decoding = "async";
    image.src = src;
    return image;
};

const planetBackground1 = loadImage("./loader/McPlanetVBackground1-hd.png");
const planetBackground2 = loadImage("./loader/McPlanetVBackground2-hd.png");
const planetForeground = loadImage("./loader/LoaderPlanetFg.png");
const particleImage = loadImage("./loader/siteLoadingParticle.png");

const ready = (image) => {
    return image.complete && image.naturalWidth > 0;
};

class Particle {
    constructor(minX, maxX, y) {
        this.velX = (0.5 - Math.random()) / 2.5;
        this.velY = 0.1 + Math.random();
        this.velO = 0.001 + 0.004 * Math.random();
        this.posY = y;
        this.opacity = 0.5 + 0.5 * Math.random();
        this.posX = minX + Math.random() * (maxX - minX);
        this.scale = 0.45 + 0.5 * Math.random();
        this.isActive = true;
    }

    update() {
        this.posX -= this.velX;
        this.posY -= this.velY;
        this.opacity -= this.velO;
        this.isActive =
            this.scale > 0.05 && this.opacity > 0.01 && this.posY > 0;
    }

    draw(ctx) {
        if (!this.isActive) {
            return;
        }
        // Fade out over the top 50 pixels.
        ctx.globalAlpha =
            this.posY < 50 ? this.opacity * (this.posY / 50) : this.opacity;
        ctx.save();
        ctx.translate(this.posX, this.posY);
        ctx.scale(this.scale, this.scale);
        ctx.drawImage(particleImage, 0, 0);
        ctx.restore();
    }
}

const drawSpinning = (ctx, image, x, y, degrees) => {
    ctx.save();
    ctx.translate(x, y);
    ctx.rotate((degrees * Math.PI) / 180);
    ctx.scale(1.25, 1.25);
    ctx.drawImage(image, image.width / -2, image.height / -2);
    ctx.restore();
};

export function startLoaderAnimation(canvas, splash) {
    const ctx = canvas?.getContext("2d");
    if (!ctx || !splash) {
        return;
    }
    let particles = [];
    let rotation1 = 0;
    let rotation2 = 30;
    let frame = 0;
    let last = 0;
    let lag = 0;

    function tick() {
        rotation1 += 0.35;
        rotation2 -= 0.65;
        if (particles.length === 0) {
            while (particles.length < 20) {
                particles.push(
                    new Particle(100, 360, 100 + 300 * Math.random()),
                );
            }
        } else if (particles.length > 100) {
            particles = particles.filter((particle) => particle.isActive);
        }
        let active = 0;
        for (const particle of particles) {
            particle.update();
            if (particle.isActive) {
                active++;
            }
        }
        for (let i = active; i < ACTIVE_PARTICLES; i++) {
            particles.push(new Particle(100, 360, 400));
        }
    }

    function draw(now) {
        const time = 0.002 * now;
        const driftX = 15 * Math.sin(time);
        const driftY = 10 * Math.cos(time);
        const { width, height } = canvas;
        ctx.clearRect(0, 0, width, height);
        ctx.save();
        ctx.scale(0.99, 0.99);
        if (ready(particleImage)) {
            for (const particle of particles) {
                particle.draw(ctx);
            }
        }
        ctx.globalAlpha = 1;
        const centerX = width / 2 + driftX;
        const centerY = height / 2 + (driftY - 10);
        if (ready(planetBackground1)) {
            drawSpinning(ctx, planetBackground1, centerX, centerY, rotation1);
        }
        if (ready(planetBackground2)) {
            drawSpinning(ctx, planetBackground2, centerX, centerY, rotation2);
        }
        if (ready(planetForeground)) {
            ctx.drawImage(
                planetForeground,
                (width - planetForeground.width) / 2 + (driftX + 10),
                (height - planetForeground.height) / 2 + (driftY - 10),
            );
        }
        ctx.restore();
    }

    // Hidden behind the game, or replaced by sad Petit after a failure.
    function stopped() {
        return splash.classList.contains("hidden") || splash.classList.contains("failed");
    }

    function loop(now) {
        if (stopped()) {
            frame = 0;
            return;
        }
        // A hidden tab or a long main-thread stall must not replay minutes of ticks at once.
        lag = Math.min(lag + (last === 0 ? TICK_MS : now - last), 10 * TICK_MS);
        last = now;
        while (lag >= TICK_MS) {
            tick();
            lag -= TICK_MS;
        }
        draw(Date.now());
        frame = requestAnimationFrame(loop);
    }

    function resume() {
        if (frame === 0 && !stopped()) {
            last = 0;
            lag = 0;
            frame = requestAnimationFrame(loop);
        }
    }

    // The splash comes back over the game when the graphics context is lost or the boot fails.
    new MutationObserver(resume).observe(splash, {
        attributes: true,
        attributeFilter: ["class"],
    });
    resume();
}
