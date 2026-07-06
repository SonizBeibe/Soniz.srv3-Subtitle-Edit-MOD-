# SonizSRV3

The subtitle editor but better :) 
A specialized, high-performance fork designed specifically for advanced ASS typesetting and typographic animation.

> **⚠️ Acknowledgement & Credits:**
> SonizSRV3 is a custom modification built upon the incredible foundation of [Subtitle Edit](https://github.com/SubtitleEdit/subtitleedit), created and maintained by Nikolaj Olsson (niksedk) and contributors. All core subtitle processing, format support, and baseline Avalonia UI elements are proudly credited to the original Subtitle Edit team.

---

## ✨ Features Exclusive to SonizSRV3

Unlike standard subtitle editors, SonizSRV3 is tailored for visual editors and typesetters:
- **Aegisub-Style Typesetting UI:** A completely refactored main editor area featuring quick-access 4-layer color pickers (`\1c` to `\4c`), style managers, and formatting toggles.
- **Total Temporal Freedom:** Internal timeline limits have been removed, allowing infinite overlapping and precise concurrent text animations without auto-shifting.
- **Visual Positioning:** Click directly on the video player overlay to generate `\pos` tags accurately based on native video resolution.
- **Karaoke Engine Integration:** Advanced native parsing for `\k`, `\kf`, and `\ko` tags with centisecond-to-millisecond conversion.
- **YTT / SRV3 Bridge:** Seamless backend integration that automatically triggers `ytsubconverter.exe` upon saving your `.ass` file to generate YouTube-ready stylized subtitles instantly.
- **Optimized Formats:** Stripped of legacy bloatware, restricting formats strictly to what high-end editors need (ASS, SRT, TXT, and Adobe After Effects).

---

## 🌐 Documentation & FAQ
For general usage regarding the core software, refer to the original documentation:
http://subtitleedit.github.io/subtitleedit/

---

## 🚀 Automated Builds
You can find the latest cross-platform builds of the **SonizSRV3 Mod** here:  
👉 [Releases](https://github.com/SonizBeibe/Soniz.srv3-Subtitle-Edit-MOD-/releases)

---

# 💻 System Requirements

## SonizSRV3 Specific Requirements

### Windows
- Minimum: Windows 10 version 22H2 (build 19045) or newer, fully updated. Older Windows 10 builds (2004/20H2/21H1/21H2) are end-of-life and may fail to start with a .NET runtime error (`0x80131506`).

### macOS

- **Minimum macOS version**: 12 (Monterey) or newer
- The `.dmg` is self-contained: `libmpv` and `ffmpeg` are bundled inside `Subtitle Edit.app`, so no MacPorts or Homebrew install is required.

#### Installing on macOS (Unsigned App)

Because this custom fork is not signed with an Apple developer certificate, macOS will block it by default. You can still install and run it by following these steps:

1. **Download** and **double-click** the `.dmg` file to mount it.
2. In the window that appears, **drag `Subtitle Edit.app` into your `Applications` folder**.
3. Open the **Terminal** app (you can find it via Spotlight or in `/Applications/Utilities/`).
4. In Terminal, run the following commands to remove macOS’s security quarantine flag and add adhoc code signature:
   ```bash
   sudo xattr -rd com.apple.quarantine "/Applications/Subtitle Edit.app"
