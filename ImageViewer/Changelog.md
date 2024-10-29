# Tama's ImageViewer Changelog

### v2.4

- Installer
- File Associations
- Thumbnails
- Print Feature!
- .webp file format support
- F11 keys added for fullscreen*
- Ctrl+Shift+F added for fullscreen*
- Alt+Enter retained for fullscreen*
- Next and Previous buttons are disabled when there are no other images in the directory!
- Rotate Left and Right buttons are disabled when the image format isn't supported (.gif, .dds, .tif)!
- Image isn't immediately overwriten when it's rotated anymore, instead Save button appears
- Maximize button disabled*
- Improved WindowState memory*
- Added missing extensions to Open File Dialog*
- Replaced built-in JSON Serializer with TINYSharp*
- Optimized Next and Previous Actions*
- Settings is not saved in Appdata/Roaming/TamaSuite/

---

### v2.3

- Added Functionality to Save Image Information as ID3 Tags, and the ability to display them when opened.

---

### v2.2

- Changed Name to ImageViewer

---

### v2.1

- ArdaBing Created (Improved Bing Functionality)
- Ctrl + B downloads last 8 images of the day from Bing
- -silentBingMultiple command added as a multiple equivalent of -silentBing command
- default SaveFilePathBing is now "My Pictures/Bing Images" instead of "Desktop"
- -saveFilePathBing command added (example: ./ArdaViewer.exe -saveFilePathBing="C:\My Images")
- Ctrl + O pops up Open File Dialog
- Documentation.htm added

---

### v2.0

- BUGFIX: next&previous image button functionality fixed
- BUGFIX: memory leaks
- AppSettings now saves to roaming appdata
- Tiff file multipage support added (PageUp & PageDn)
- Current page and page count of multipage .tiff is now displayed
- PixelFormat of .dds is now displayed
- Bits Per Pixel and Compression Type of .tga is now displayed

---

### v1.9

- Zoom feature added

---

### v1.8

- .dds and .tga file format compatibility added
- 64 bit