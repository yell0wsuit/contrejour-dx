# Contre Jour DX

<p align="center">
  <img alt="Logo of Contre Jour DX" src="./distribution/icons/ContreJourDXIcon_512.png" width="256"/>
</p>

## About

_Contre Jour DX (Decompiled Extra)_ is a fan-made enhancement of the Windows 8 version of _Contre Jour_. This project aims to improve the original game's codebase, restore content from other versions of the game, and enhance the overall gaming experience.

The game's source code is decompiled from the Windows 8 version, which serves as the foundation for development and feature expansion.

This project is led by [yell0wsuit](https://github.com/yell0wsuit), with help from [contributors](https://github.com/yell0wsuit/contrejour-dx/graphs/contributors).

> [!NOTE]
> Contre Jour DX is an **unofficial, non-commercial fan project.** It is not affiliated with, endorsed by, or officially associated with Mokus or any publisher of _Contre Jour_. All rights to the original game, characters, artwork, audio, and other assets belong to their respective rights holders.
>
> We contacted Maksym Hryniv, the author of _Contre Jour_, regarding this decompilation project. He told us that he does not mind the project as long as it remains non-commercial.
>
> _Contre Jour DX_ is therefore distributed free of charge and is intended to remain strictly non-commercial. **Do not sell this project, monetize it, or use _Contre Jour_ assets from this repository for commercial purposes.**

### Related projects

- [Contre Jour (web)](https://github.com/yell0wsuit/ContreJour): the HTML5 edition of _Contre Jour_. The _New Friend_ bonus chapter in this project comes from it.
- [Contre Jour Rekindled](https://github.com/yell0wsuit/contrejour-rekindled): a port of the Windows Phone version. It is the reference for the _Mango_ bonus chapter in this project.

## Download

Download the latest release from the [Releases page](https://github.com/yell0wsuit/contrejour-dx/releases).

## Play online

Play _Contre Jour DX_ in your browser:

<https://yell0wsuit.github.io/contrejour-dx/>

You can also install the game as an app from your browser. After it loads once, the game plays offline. Your progress is saved in your browser, separately from the desktop version's save files.

## Features

- Two bonus chapters:
    - _New Friend_, ported from the HTML5 edition: 10 levels with Amie, a companion who lifts Petit with her balloon.
    - _Mango_, ported from the Windows Phone version: 20 levels.
- Sharper art: sprites that the Windows 8 version shipped only at low resolution use the high-resolution art from the iOS version, such as backgrounds, the spikes flower and the end rose.
- Restored details from the iOS version, such as the outline of the ground's rest shape while you drag it.
- Support for sprites and animations from [TexturePacker](https://www.codeandweb.com/texturepacker) in JSON array format, for easier modding and new assets.
- The music from the official soundtrack album, and an ending theme for the final level.
- Open-source fonts for each language, so text renders smoothly at any size.
- Runs on Windows, macOS and Linux through SDL3 and Skia. The game uses Metal, Vulkan or OpenGL, and falls back to software rendering if no GPU renderer works. It recovers when the graphics device is lost.
- Runs in the browser: a WebAssembly build installs as a PWA. Saves live in `localStorage` rather than a file.
- Better save file format. The save files (`contrejour_preferences.json` and `contrejour_gamesave.json`) are stored in a `ContreJourDX_SaveData` folder, with the following fallback priority:
    - Next to the game executable (preferred for portability). A macOS `.app` bundle skips this location, because the bundle is read-only.
    - `Documents/ContreJourDX_SaveData`, if the above is not writable.
    - `%LOCALAPPDATA%/ContreJourDX_SaveData` (Windows) or the equivalent on other platforms.

## Development & contributing

The development of _Contre Jour DX_ is an ongoing process, and contributions are welcome! If you'd like to help out, please consider the following:

- **Reporting issues**: If you encounter any bugs or issues, please report them on the [GitHub Issues page](https://github.com/yell0wsuit/contrejour-dx/issues).
- **Feature requests**: If you have ideas for new features or improvements, feel free to submit a feature request through Issues.
- **Contributing code**: If you're a developer and want to contribute code, please fork the repository and submit a pull request.

### Testing the code

Do these steps to test the game while you develop it.

1. Install [.NET 10 or higher](https://dotnet.microsoft.com/en-us/download/dotnet/).

> [!NOTE]
> The `global.json` file sets the minimum SDK version. It uses `rollForward: latestFeature`. Thus a newer 10.0.x SDK also works.
> If your SDK is older than the minimum, each `dotnet` command stops with a version-mismatch error. Install a newer SDK to correct this.

2. Clone the repository to your computer:

    ```bash
    git clone https://github.com/yell0wsuit/contrejour-dx.git
    cd contrejour-dx
    ```

    You can also use [GitHub Desktop](https://desktop.github.com/) to clone the repository.

3. Build the game with one of these commands.

> [!NOTE]
> The `PublishAot` option has prerequisites. Obey the [AOT prerequisites](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet8#prerequisites) for your operating system.

a. Windows

```bash
dotnet publish src\ContreJour.Desktop\ContreJour.Desktop.csproj -c Release -r win-x64 -p:PublishAot=true -o .\src\ContreJour.Desktop\bin\Publish\win-x64
```

b. macOS

```bash
dotnet publish src/ContreJour.Desktop/ContreJour.Desktop.csproj -c Release -r osx-arm64 -p:PublishAot=true -o ./src/ContreJour.Desktop/bin/Publish/osx-arm64
```

To make a `.app` bundle, run `./distribution/bundle_macos.sh <version>` instead.

> [!NOTE]
> Change `osx-arm64` to `osx-x64` to build the game for an Intel Mac. We do not know if the game operates correctly on an Intel Mac.

c. Linux

```bash
dotnet publish src/ContreJour.Desktop/ContreJour.Desktop.csproj -c Release -r linux-x64 -p:PublishAot=true -o ./src/ContreJour.Desktop/bin/Publish/linux-x64
```

To make an AppImage, run `./distribution/build_appimage.sh <version>` instead.

> [!WARNING]
> A native AOT binary from Linux operates only on the same Linux version, or on a newer Linux version.

If native AOT causes a problem, remove the `-p:PublishAot=true` option. Then build the game again.

d. Browser (WebAssembly)

The browser build needs the WebAssembly workload. It also needs its own content. A Python script converts the desktop assets to WebP images, Ogg Opus audio and subset fonts. Do this conversion before you build the game.

```bash
dotnet workload install wasm-tools
python3 -m pip install -r tools/requirements.txt
python3 tools/build_web_content.py
```

> [!NOTE]
> The audio conversion needs an FFmpeg with the `libopus` encoder, from the `PATH` or from the `CJ_FFMPEG` environment variable. Many FFmpeg builds do not have this encoder, and the script then stops before it converts anything. Add `--skip-audio` to build without audio; the game then runs silent.

The conversion is incremental. Do the conversion again only after you change an asset.

Start the game in your browser. This quick build runs on the interpreter:

```bash
dotnet run --project src/ContreJour.Browser
```

Or publish the AOT-compiled site, which plays at full speed, and serve it at `http://127.0.0.1:8080`:

```bash
python3 tools/publish_browser.py --serve
```

The [Deploy Browser to GitHub Pages](.github/workflows/deploy-pages.yml) workflow publishes this site to GitHub Pages. You must start this workflow manually.

4. Run the unit tests:

    ```bash
    dotnet test --solution ContreJour.slnx
    ```

## Credits

_Contre Jour_, as credited in the game's ending:

- Graphic artists: Mihai Tymoshenko, Andriy Shvyrov
- Composer: David Ari Leon
- SFX: Ihor Pryshliak
- Producer: Tom Kinniburgh
- Idea: Anton Mykhaylets
- Everything else: Maksym Hryniv

The HTML5 edition of _Contre Jour_, which the _New Friend_ chapter comes from:

- HTML5 sponsor: Microsoft Internet Explorer Team
- HTML5 development: Clarity Consulting, Inc. (Jerry Brunning, Erik Klimczak, Nathan Gonzalez, Ryan Skurkis, Akin Owolabi, Levi Beaver, Ryan Mott)

## Special thanks

- Maksym Hryniv for creating _Contre Jour_ and allowing this fan project to exist.
- David Ari Leon for composing the music of _Contre Jour_.
- The original developer team of _Contre Jour_ for their work on the game.
- @TheAwesomeBlue for finding the original Windows 8 version of _Contre Jour_.
- Laxii for providing the lossless soundtrack of _Contre Jour_.
