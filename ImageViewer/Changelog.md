# Tama's ImageViewer Changelog

## v2.4.2

- **Fixed Encoding Issues**: Resolved issues affecting image encoding in certain edge cases.
- **New Encoder Parameter**: Added the `-skipExisting` encoder parameter for `-encodeAll`, which skips re-encoding files if they already exist in the specified output directory.

---

## v2.4.1

- **Fixed Installer Issues**: Resolved unexpected installer behavior for a smoother installation process.
- **Shortcut Functionality**: Shortcuts now operate correctly.
- **Example Images Download**: Example images are now downloaded silently post-installation, leading to a smaller installer size compared to v2.4. This change is intentional, as example images are no longer included in the installer package.
- **Dynamic Theme Setting**: The default theme is no longer fixed to Dark; it now follows the theme selected in Windows. If you are upgrading from v2.4, the theme will still be set to Dark. To change it, navigate to the settings file located in `AppData/Roaming/TamaSuite` named `ImageViewerSettings.tiny`. Open it with your preferred text editor, locate the line starting with `Theme: Dark`, and change it to `Theme: Default`.

---

## v2.4

- **New Installer** for streamlined setup.
- **File Associations**: Easily open files directly with ImageViewer.
- **Brand New Icons** for better user experience
- **Updated GUI** for quality of life
- **Light and Dark Modes**: Added Light/Dark Themes.
  - Also includes custom colors.
- **Print Feature**: Print images directly from the app.
- **New Format Support**:
  - `.webp` files now supported.
  - `.obi` Open Book Image file support added.
- **Navigation Buttons**:
  - *Next* and *Previous* buttons are disabled if no other images are available in the directory.
  - *Rotate Left* and *Rotate Right* buttons are disabled for unsupported formats (.gif, .dds, .tif).
  - *Page Up* and *Page Down* buttons are now shown when a multipage document is opened.
  - *Print* button added to allow user easier printing experience.
  - *Save* button added to allow user to save rotated images.
  - *File* button added to allow the user to easily open images.
- **Keyboard Shortcuts**:
  - `Ctrl + P` added for printing files directly from the app.
  - `Up Arrow` added as an alternative to `Page Up`.
  - `Down Arrow` & `Space` added as an alternative to `Page Down`.
  - **Fullscreen Options**:
    - `F11` key added for fullscreen mode.
    - `Ctrl+Shift+F` added for fullscreen mode.
    - `Alt+Enter` retained for fullscreen mode.
- **Image Rotation**: *Save* button now appears instead of immediate overwrite when rotating.
- **Window Management**: Improved window state memory for a more seamless and consistent experience when going in and out of fullscreen and maximized modes.
- **Open File Dialog**: Added missing file extensions.
- **Drag&Drop** feature added as an alternate method to open images.
- **TINY Serialization**: Replaced built-in JSON with TINYSharp for optimized performance and code clarity.
- **Performance Improvements**: Optimized *Next* and *Previous* actions.
- **Commandline**: Removed outdated options, and added new ones.
	- `-saveFilePathBing` removed.
	- `-DesktopJoke.png` removed.
	- `-attemptToGetId3Tags` removed.
	- `-print <file>` added.
	- `-encode <sourcePath> -to <outputPath> [EncoderParameters]` added.
	- `-encodeAll <sourceDir> -to <destinationDir> <extension> [EncoderParameters]` added.
- **Countless Small Improvements and Bug Fixes**
- **TGA Thumbnails**: An attempt was make in this version to create ThumbnailProvider and register it to COM for .tga files. Unfortunately it didn't work and had to be scrapped.

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
