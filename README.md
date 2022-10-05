# ImageViewer

## What is it?
ImageViewer is an extremely lightweight image viewing application completely written in C#.

![screenshot](https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/imageviewer.png)

## List of Supported Image File Extensions
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

## Installation
- Download the Latest release
- Unzip/Uncompress into "C:\ImageSuite\ImageViewer" and never move again.

It is important to not move the application or change file names or folder names especially because of file association.

## File Association
There is no easy or automatic method to associate image files with ImageViewer. You have to manually associate each image extension with ImageViewer manually.

### How?
- Right click any image file that is supported by ImageViewer
- "Open With" > "Choose Another App"
- Scroll Down
- Click "More Apps"
- Scroll Down Again
- Click "Look for another app on this PC"
- Select ImageViewer.exe

*Note: After doing this for the first time ImageViewer might show on "Choose Another App" making it possible to omit browsing steps*

## Updating from Version 2.2 or Below
### (Only relevant if you saved up Bing Images in a folder somewhere)
If you have a folder where you saved up daily Bing Images with earlier versions, you can now add additional image information to each and every image.
Run the application once, after updating and setting saveFilePathBing, like this, "./ImageViewer.exe -attemptToGetId3Tags", without the quotes. And wait several minutes.
When it's finished, the app will automaticly exit. You can enjoy the updated, old Bing Images with the additional information they originally came with.

*Note: See Documentation.htm for more information such as hotkeys and command line arguments that aren't listed here*