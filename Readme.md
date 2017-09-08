# File Formats (aka Dash.FileFormats) [![License](https://img.shields.io/:license-SNCL%202.1.0-blue.svg)](https://raw.githubusercontent.com/MysteryDash/Simple-Non-Code-License/master/License.txt)  

This project has been made to open, read, edit and create files from uncommon formats found within various games and software.

## Do you like my project ? [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](http://paypal.me/MysteryDash/5)  

## What we can do for now with the library

We cannot do much, but here's what's available.

Idea Factory's File Formats :
* Read, extract and create (uncompressed) .PAC
* Read, extract and create .CL3
* Convert .TID to common picture formats and common picture format back to .TID

Valve's Formats :
* Valve Data Format, also known as [KeyValues](https://developer.valvesoftware.com/wiki/KeyValues)

## Side-tools

The side-tools are gone. Not forever, but I don't know how long they won't be available for.
You can still download them from my old commits though.

## What has yet to be done

We still have a long way to go ! Here's what we need to do :
* Add compression support for .PAC.
* Fix DXT1/5 compression for .TID which isn't working for an unknown reason (almost done).
* Find an alternative to Neptoolia.Datalayer.Decompressor.
* Test everything.
* Add XML comments.

## About the author

My name is Alexandre, I'm a French guy who likes to tweak things around me (that's what I am spending most of my time for), and that's how this project was born.

## Contributing

If you are willing to contribute, send a pull request with what you made and don't forget to add your name in the list of contributors below.

## Contributors

The following people are contributors of this project :
- [MysteryDash](https://github.com/MysteryDash)
- NepIsLife (Neptoolia.DataLayer.Decompressor - Decompressor.cs)
- ps-auxw (Neptoolia.DataLayer.Decompressor - Decompressor.cs)

## Versioning

We use [Semantic Versioning 2.0.0](http://semver.org/) for versioning.  
The current version of this project is the 3.0.0-alpha.2.

## License

This project is licensed under the Simple Non Code License 2.1.0.  
See the [License.txt](License.txt) file for details.

## Links

- [DXT decompression written in assembly & C++ by Matej Tomčík](http://www.matejtomcik.com/Public/KnowHow/DXTDecompression/)
- [rygDXT real-time DXT compressor by Fabian "ryg" Giesen](https://github.com/nothings/stb/blob/master/stb_dxt.h)