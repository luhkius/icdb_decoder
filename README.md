# icdb_decoder
Decode ICDB files for the Bakutsuri Bar Hunter 3ds game

# Build
This code is reasonably framework-agnostic, and should compile with any of the latest C# .net toolchains
Tested with visual studio using the latest .net framework on windows, and .net 5 on linux

# Usage
Once compiled, simply drag your .icdb file onto the exe (or linux binary).
You can also specify the .icdb file to be parsed, as a command line argument.
The information will be parsed and converted to a more human-friendly .csv file.
The .csv file will be generated with the same filename, in the same directory as the input .icdb file.
