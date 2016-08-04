# File Formats

This project has been made to open, read, edit and/or create files from uncommon formats found within various games or software.

## Getting started modding Megadimension Neptunia Victory II

I'll give you the instructions for modding any texture or picture present within the game. It'll be the same for modding other things.  
So, you need to :
* Extract the tools placed in the Tools.zip archive
* Use the Multi-Extractor to extract a .PAC (GAME00000.pac contains most of the textures).
* Use the Multi-Extractor again to extract a particular .CL3 (for the first MegaNep VII mod I chose to edit GAME00000\event\ma\0001.cl3).
* Use the Multi-Extractor again to convert the tex_00.tid to a PNG.
* Edit the PNG as you wish (for the first MegaNep VII mod I just made the picture black and white), you can even change the format if that's what you desire (my tools supports transparency, which was not the case of nr1_tidtool !).
* Convert the picture back to a TID named tex_00.tid using the Tid Maker.
* Repack the TID inside of the CL3 using the Cl3 Editor (just follow the instructions inside of it)
* Repack the CL3 inside of the PAC using the Pac Editor (just add the new CL3 you've made, otherwise you'll be left with a fully uncompressed .PAC, and it'll be too big for the game to handle if it is GAME0000X.pac).
* Place your brand new pack inside of the game folder (do not forget to rename the original one so you will not have to redownload it again).
* Start the game and... ENJOY !

## What we can do for now with the library

We cannot do much, but here's what's available.

Idea Factory's File Formats :
* Read, extract and create (uncompressed) .PAC
* Read, extract and create .CL3
* Convert .TID to common picture formats and common picture format back to .TID

## Side-tools

Since not everyone is able to create and compile C# projects from a library, I decided to make a few tools for these people :
* Multi Extractor, a tool able to extract/convert .PAC, .CL3 and .TID.
* Tid Maker, a tool able to convert any .NET-readable picture format to an uncompressed .TID (version 0x90).
* Pac Editor, a tool able to edit existing .PAC.
* Cl3 Editor, a tool able to edit existing .CL3.

## What has yet to be done

We still have a long way to go ! Here's what we need to do :
* Add compression support for .PAC.
* Fix DXT1/5 compression for .TID which isn't working for an unknown reason.
* Find an alternative to Neptoolia.Datalayer.Decompressor.
* Test everything.
* Remove the bin folders (those DLLs are taking quite a bit of space).
* Add icons to the tools with a GUI.

## About the author

My name is Alexandre, I'm a 17 years old French guy who likes to tweak things around me (that's what I am spending most of my time for), and that's how this project was born.

## Contributing

If you are willing to contribute, please add your name to the list of contributors below and send a pull request with what you made.

## Contributors

The following people are contributors of this project :
- [MysteryDash](https://github.com/MysteryDash)
- NepIsLife (Neptoolia.DataLayer.Decompressor - Decompressor.cs)
- ps-auxw (Neptoolia.DataLayer.Decompressor - Decompressor.cs)

## Versioning

We use [Semantic Versioning 2.0.0](http://semver.org/) for versioning.  
The current version of this project is the 2.5.0-alpha.

## License

This project is licensed under the Simple Non Code License 2.0.2.  
See the [License.txt](License.txt) file for details.