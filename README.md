The files in this directory are the source files used to generate a set of
debugging textures. Also included are the generated debugging textures at a
resolution of 2048 by 2048 pixels (other resolutions, e.g., 1024 by 1024 or
4096 by 4096 can easily be exported from the SVG files).

Here are previews of the texures:

<img src="tex_DebugUVTiles.png" alt="UV Tiles" width="256"/>
<img src="tex_DebugAlignment.png" alt="Alignment" width="256"/>
<img src="tex_DebugGrid.png" alt="DebugGrid" width="256"/>

These files are being released under the CC0 license, which puts them in the
public domain to the maximum extent possible before copyrights expire (laws in
some regions prevent simply putting them in the public domain everywhere). See
LICENSE.md for details.

The program files are simple C# programs. A C# compiler is required to compile
and execute them. The recommended way to modify the SVG files is by editing the
C# program files and then regenerating the SVG files using the modified
programs. This makes it easier to maintain consistency among the various SVG
elements.

The SVG files can then be exported to image files using an SVG editor (e.g.,
InkScape or Adobe Illustrator). Note that while the SVG files nominally use a
resolution of 2048 by 2048 pixels, it is possible using an SVG editor to create
any resolution image required. It is not necessary to modify the C# programs
just to export the image at a different resolution.

The sRGB values in the images range from 39 to 232, which should make the images
suitable for use as the base-color of non-metallic materials in most PBR scenes.
