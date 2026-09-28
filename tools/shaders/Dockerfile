FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0
ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y --no-install-recommends wine wine64 p7zip-full curl wget ca-certificates && rm -rf /var/lib/apt/lists/*
RUN curl -sL https://monogame.net/downloads/net9_mgfxc_wine_setup.sh | bash
ENV MGFXC_WINE_PATH=/root/.winemonogame
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install -g dotnet-mgcb --version 3.8.5.1
WORKDIR /work
# wineboot finishes asynchronously and can overwrite the extracted d3dcompiler_47.dll with
# Wine's own builtin stub; re-extract the real one once wineserver has settled.
RUN WINEPREFIX=/root/.winemonogame wineserver -w; \
    curl -sL "https://download-installer.cdn.mozilla.net/pub/firefox/releases/62.0.3/win64/ach/Firefox%20Setup%2062.0.3.exe" -o /tmp/ff.exe && \
    7z e /tmp/ff.exe "core/d3dcompiler_47.dll" -o/root/.winemonogame/drive_c/windows/system32/ -aoa && \
    rm /tmp/ff.exe && \
    ! grep -q "Wine builtin" /root/.winemonogame/drive_c/windows/system32/d3dcompiler_47.dll
ENV WINEPREFIX=/root/.winemonogame WINEDEBUG=-all
