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
  - **Page Up**, **Up Arrow**: Previous Page (Multipage Image Only)
  - **Page Down**, **Down Arrow**, **Space**: Next Page (Multipage Image Only)
- **Bing Image of the Day**:
  - **B**: Download today's Bing Image
  - **Ctrl + B**: Download Bing Images of the Day from the past 8 days
- **File Operations**:
  - **Ctrl + O**: Open File Dialog
- **Fullscreen Toggle**:
  - **Alt + Enter**, **Ctrl + Shift + F**, or **F11**: Toggle Fullscreen
- **Print**:
  - **Ctrl + P**: Print Image

---

### Command Line Arguments

- `-silentBing`  
  Downloads the Bing image of the day and exits.

- `-silentBingMultiple`  
  Downloads images of the day from the last eight days and exits.

- `-print <path>`  
  Prints the specified image and exits.

- `-encode <sourcePath> -to <outputPath> [EncoderParameters]`  
  Encodes a single image file located at `<sourcePath>` and saves the encoded version to `<outputPath>`. This command is useful for converting images into different formats or applying specific encoding settings.

  **Parameters:**
  - `<sourcePath>`: The path to the image file that you want to encode.
  - `<outputPath>`: The path where the encoded image will be saved.
  - `[EncoderParameters]`: Optional parameters to customize the encoding process, including:
    - `-quality <quality 0-100>`: Sets the quality of the output image (0 = lowest, 100 = highest).
    - `-simple`: Applies a simple encoding strategy that prioritizes speed over quality.
    - `-advanced <speed 1-9>`: Uses an advanced encoding method, where lower values prioritize speed and higher values prioritize quality.
    - `-nearLossless <speed 1-9>`: Aims for nearly lossless encoding, balancing compression and image fidelity.
    - `-lossless`: Enables lossless encoding, ensuring no information is lost in the output image.

- `-encodeAll <sourceDir> -to <destinationDir> <extension> [EncoderParameters]`  
  Encodes all image files within the specified directory `<sourceDir>` and saves the encoded versions to `<destinationDir>`. This command is ideal for batch processing multiple images with consistent encoding parameters.

  **Parameters:**
  - `<sourceDir>`: The path to the directory containing the images to encode.
  - `<destinationDir>`: The path to the directory where the encoded images will be saved.
  - `<extension>`: The file extension for the output images (e.g., `.jpg`, `.png`, `.webp`).
  - `[EncoderParameters]`: Optional parameters for controlling the encoding process, same as those in the `-encode` command.
