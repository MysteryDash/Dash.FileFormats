# File Formats

This project has been made to open, read and/or create files from uncommon formats found within various games or software.

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
* Pac Builder, a tool able to merge one or multiple folders into a single .PAC.
* Cl3 Builder, a tool able to create .CL3 files from folders. Note that the file link will stay empty, and it has not been tested with the game.

## What has yet to be done

We still have a long way to go ! Here's what we need to do :
* Add compression support for .PAC.
* Fix DXT1/5 compression for .TID which isn't working for an unknown reason.
* Find an alternative to Neptoolia.Datalayer.Decompressor.
* Test everything.
* Remove the bin folders (those DLLs are taking quite a bit of space).

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
The current version of this project is the 2.3.0-alpha.

## License

This project is licensed under the Simple Non Code License 2.0.2.  
See the [License.txt](License.txt) file for details.