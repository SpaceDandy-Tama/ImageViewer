# Tama's ImageViewer Changelog

## v2.4

- **New Installer** for streamlined setup.
- **File Associations**: Easily open files directly with ImageViewer.
- **Thumbnails**: Quick preview of images.
- **Light and Dark Modes**: Added Light/Dark Themes.
- **Print Feature**: Print images directly from the app.
	- `-print <file>` prints the image from command line.
	- `Ctrl+P` added for printing from app.
- **New Format Support**: `.webp` files now supported.
- **Fullscreen Options**:
  - `F11` key added for fullscreen mode.
  - `Ctrl+Shift+F` added for fullscreen mode.
  - `Alt+Enter` retained for fullscreen mode.
- **Navigation Buttons**:
  - *Next* and *Previous* buttons are disabled if no other images are available in the directory.
  - *Rotate Left* and *Rotate Right* buttons are disabled for unsupported formats (.gif, .dds, .tif).
  - *Page Up* and *Page Down* buttons are now shown when a multipage document is opened.
  - *Print* button added.
  - *Save* button added.
- **Image Rotation**: *Save* button now appears instead of immediate overwrite when rotating.
- **Window Management**: Improved window state memory for a more seamless and consistent experience when going in and out of fullscreen and maximized modes.
- **Open File Dialog**: Added missing file extensions.
- **TINY Serialization**: Replaced built-in JSON with TINYSharp for optimized performance and code clarity.
- **Performance Improvements**: Optimized *Next* and *Previous* actions.
- **Commandline**: Removed some outdated commandline options.
	- `-saveFilePathBing`
	- `-DesktopJoke.png`
	- `-attemptToGetId3Tags`

---

## v2.3

- **ID3 Tag Support**: Save and display image information as ID3 tags.

---

## v2.2

- **App Rename**: Renamed to *ImageViewer*.

---

## v2.1

- **ArdaBing Integration**: Enhanced Bing functionality.
  - `Ctrl+B` downloads the last 8 Bing images of the day.
  - `-silentBingMultiple` command added for multiple silent Bing downloads.
  - *Default Save Path*: Updated to `"My Pictures/Bing Images"` from `"Desktop"`.
  - New `-saveFilePathBing` command (e.g., `./ArdaViewer.exe -saveFilePathBing="C:\My Images"`).
  - `Ctrl+O` opens the File Dialog.
- **Documentation**: Added `Documentation.htm`.

---

## v2.0

- **Bug Fixes**:
  - Fixed *Next* and *Previous* button functionality.
  - Addressed memory leaks.
- **AppSettings**: Now saved to roaming AppData.
- **Multipage TIFF Support**:
  - Use `PageUp` and `PageDown` for navigation.
  - Displays the current page and page count for `.tiff` files.
- **File Information**:
  - Displays PixelFormat for `.dds` files.
  - Displays Bits Per Pixel and Compression Type for `.tga` files.

---

## v1.9

- **Zoom Feature**: Added zooming capabilities.

---

## v1.8

- **New Format Compatibility**: Added support for `.dds` and `.tga` file formats.
- **64-Bit Support**: Enhanced compatibility and performance.

---

*Note: Changelog wasn't maintained for earlier versions.*
