# Tama's ImageViewer

## Overview
**Tama's ImageViewer** is an ultra-lightweight image viewing application, fully developed in C#.

### Creator
- **Oğuz Can Soyselçuk** (aka Tama)

### Honorable Mentions
- **Çağatay Öz** (aka Cagatus)

---

## Supported Image Formats
Tama's ImageViewer supports a wide range of image file formats, including:

- `.bmp`
- `.jpg`, `.jpeg`
- `.png`
- `.gif`
- `.tiff`, `.tif`
- `.ico`
- `.tga`
- `.dds`
- `.webp`

---

## Installation Instructions
1. **Download the Latest Release**
2. **Run the Setup**: Double-click the downloaded setup file to install.

> **Note:** File associations will be configured automatically during installation.

---

## User Controls

### Mouse Controls
- **Left Button**: Show/Hide UI
- **Right Button**: Close Application
- **Middle Button**: Zoom to 100%
- **X1 Button**: Previous Image
- **X2 Button**: Next Image
- **Scroll Wheel**: Zoom to Cursor Location

### Keyboard Controls
- **Escape**: Close Application
- **Arrow Keys**:
  - **Left**: Previous Image
  - **Right**: Next Image
- **Zoom**:
  - **Add (+)**: Zoom In
  - **Subtract (-)**: Zoom Out
- **Display Switch**:
  - **Numpad 1-6**: Quickly switch display in Fullscreen mode
- **Multipage Navigation**:
  - **Page Up**: Previous Page (Multipage Image Only)
  - **Page Down**: Next Page (Multipage Image Only)
- **Bing Image of the Day**:
  - **B**: Download today's Bing Image
  - **Ctrl + B**: Download Bing Images of the Day from the past 8 days
- **File Operations**:
  - **Ctrl + O**: Open File Dialog
- **Fullscreen Toggle**:
  - **Alt + Enter**, **Ctrl + Shift + F**, or **F11**: Toggle Fullscreen

---

### Command Line Arguments

- `-silentBing`  
  Downloads the Bing image of the day and exits.

- `-silentBingMultiple`  
  Downloads images of the day from the last eight days and exits.

- `-print <path>`  
  Prints the specified image and exits.
  
  ---