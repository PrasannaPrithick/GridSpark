#!/usr/bin/env python3
from pathlib import Path
import math
from PIL import Image, ImageDraw, ImageFilter

OUT = Path(__file__).resolve().parents[1] / "GridAura" / "Assets" / "Resources" / "Art"
SCALE = 4
SIZE = 256
HI = SIZE * SCALE
WHITE = (255, 255, 255, 255)


def blank(size=HI):
    return Image.new("RGBA", (size, size), (0, 0, 0, 0))


def down(img):
    return img.resize((SIZE, SIZE), Image.Resampling.LANCZOS)


def save(img, name):
    path = OUT / name
    down(img).save(path)
    print("wrote", path)


def circle(draw, x, y, r, fill=WHITE):
    draw.ellipse((x - r, y - r, x + r, y + r), fill=fill)


def line(draw, a, b, w, fill=WHITE):
    draw.line((a, b), fill=fill, width=w)
    circle(draw, a[0], a[1], w / 2, fill)
    circle(draw, b[0], b[1], w / 2, fill)


def make_speaker():
    img = blank()
    d = ImageDraw.Draw(img)
    # speaker body
    d.rounded_rectangle((220, 380, 380, 644), radius=48, fill=WHITE)
    d.polygon([(360, 300), (620, 160), (620, 864), (360, 724)], fill=WHITE)
    # sound waves
    for r, w in ((120, 48), (210, 48)):
        box = (520 - r, 512 - r * 1.15, 520 + r * 1.7, 512 + r * 1.15)
        d.arc(box, start=-48, end=48, fill=WHITE, width=w)
    save(img, "IconSound.png")


def make_music():
    img = blank()
    d = ImageDraw.Draw(img)
    # note head
    d.ellipse((250, 620, 560, 860), fill=WHITE)
    # stem
    d.rounded_rectangle((500, 180, 575, 740), radius=36, fill=WHITE)
    # flag
    d.polygon([(575, 170), (820, 310), (820, 430), (575, 330)], fill=WHITE)
    save(img, "IconMusic.png")


def make_phone():
    img = blank()
    d = ImageDraw.Draw(img)
    w = 72
    d.rounded_rectangle((300, 120, 724, 904), radius=150, outline=WHITE, width=w)
    d.rounded_rectangle((430, 780, 594, 830), radius=24, fill=WHITE)
    save(img, "IconHaptics.png")


def make_palette():
    img = blank()
    d = ImageDraw.Draw(img)
    d.ellipse((150, 170, 900, 900), fill=WHITE)
    d.ellipse((330, 520, 560, 750), fill=(0, 0, 0, 0))
    # punch hole by compositing
    hole = Image.new("RGBA", img.size, (0, 0, 0, 0))
    hd = ImageDraw.Draw(hole)
    hd.ellipse((330, 520, 560, 750), fill=(0, 0, 0, 255))
    base = Image.new("RGBA", img.size, (0, 0, 0, 0))
    base.paste(img, (0, 0))
    clear = Image.new("RGBA", img.size, (0, 0, 0, 0))
    img = Image.composite(clear, base, hole.convert("L"))
    d = ImageDraw.Draw(img)
    dots = [
        ((430, 280), (255, 90, 80, 255)),
        ((620, 250), (80, 170, 255, 255)),
        ((760, 400), (255, 205, 40, 255)),
        ((720, 590), (90, 210, 120, 255)),
    ]
    for (x, y), color in dots:
        d.ellipse((x, y, x + 130, y + 130), fill=color)
    save(img, "IconTheme.png")


def make_cart():
    img = blank()
    d = ImageDraw.Draw(img)
    w = 68
    line(d, (250, 430), (250, 210), w)
    line(d, (250, 210), (500, 210), w)
    line(d, (180, 430), (840, 430), w)
    line(d, (840, 430), (760, 720), w)
    line(d, (760, 720), (300, 720), w)
    line(d, (300, 720), (180, 430), w)
    circle(d, 360, 840, 72)
    circle(d, 680, 840, 72)
    save(img, "IconCart.png")


def make_shield():
    img = blank()
    d = ImageDraw.Draw(img)
    w = 72
    outer = [
        (512, 120),
        (830, 250),
        (800, 540),
        (512, 900),
        (224, 540),
        (194, 250),
    ]
    d.line(outer + [outer[0]], fill=WHITE, width=w)
    for p in outer:
        circle(d, p[0], p[1], w / 2)
    save(img, "IconShield.png")


def make_star():
    img = blank()
    d = ImageDraw.Draw(img)
    cx, cy, r = 512, 512, 390
    pts = []
    for i in range(10):
        ang = math.radians(-90 + i * 36)
        rr = r if i % 2 == 0 else r * 0.40
        pts.append((cx + rr * math.cos(ang), cy + rr * math.sin(ang)))
    d.polygon(pts, fill=(255, 210, 40, 255))
    save(img, "IconStar.png")


def make_back():
    img = blank()
    d = ImageDraw.Draw(img)
    w = 88
    line(d, (280, 512), (780, 512), w)
    line(d, (280, 512), (540, 250), w)
    line(d, (280, 512), (540, 774), w)
    save(img, "IconBack.png")


def make_chevron():
    img = blank()
    d = ImageDraw.Draw(img)
    w = 80
    line(d, (380, 250), (680, 512), w)
    line(d, (680, 512), (380, 774), w)
    save(img, "IconChevron.png")


def make_toggle_track():
    img = Image.new("RGBA", (400 * SCALE, 200 * SCALE), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.rounded_rectangle((0, 0, img.size[0] - 1, img.size[1] - 1), radius=img.size[1] // 2, fill=WHITE)
    out = img.resize((200, 100), Image.Resampling.LANCZOS)
    path = OUT / "IconToggleTrack.png"
    out.save(path)
    print("wrote", path)


def make_toggle_knob():
    img = blank(400)
    d = ImageDraw.Draw(img)
    d.ellipse((8, 8, 392, 392), fill=WHITE)
    out = img.resize((100, 100), Image.Resampling.LANCZOS)
    path = OUT / "IconToggleKnob.png"
    out.save(path)
    print("wrote", path)


if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    make_speaker()
    make_music()
    make_phone()
    make_palette()
    make_cart()
    make_shield()
    make_star()
    make_back()
    make_chevron()
    make_toggle_track()
    make_toggle_knob()
