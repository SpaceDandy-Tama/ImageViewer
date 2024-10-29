# Tama's ImageViewer

### What is it?
Tama's ImageViewer is an extremely lightweight image viewing application completely written in C#

### Creator
Oğuz Can Soyselçuk aka Tama

### Honorable Mentions
Çağatay Öz aka Cagatus

### List of Supported Image File Extensions
- .bmp
- .jpg
- .jpeg
- .png
- .gif
- .tiff
- .tif
- .ico
- .tga
- .dds
- .webp

### Installation
- Download the Latest Release
- Double click the downloaded setup file

### File Association
The setup will take care of file associations

### Inputs
- Mouse Left Button: Shows/Hides UI
- Mouse Right Button: Closes Application
- Mouse Middle Button: Zoom to 100%
- Mouse X1 Button: Previous Image
- Mouse X2 Button: Next Image
- Mouse Scroll: Zoom to Cursor Location
- Keyboard Escape: Closes Application
- Keyboard Left: Previous Image
- Keyboard Right: Next Image
- Keyboard Add: Zoom In
- Keyboard Sub: Zoom Out
- Keyboard Numpad 1-6: Quickly Switch Display in Fullscreen
- Keyboard PageUp: Previous Page (Multipage Image Only)
- Keyboard PageDown: Next Page (Multipage Image Only)
- Keyboard B: Download Bing Image of the Day
- Keyboard Ctrl + B: Download Bing Image of the Day for the last 8 days
- Keyboard Ctrl + O: Open File Dialog
- Keyboard Alt + Enter: Toggle Fullscreen
- Keyboard Ctrl + Shift + F: Toggle Fullscreen
- Keyboard F11: Toggle Fullscreen

### Command Line Arguments
- -silentBing
	Downloads image of the day from bing and closes
- -silentBingMultiple
	Downloads images of the day from the last eight days and closes
- -print <path>
	Prints image and closes