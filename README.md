# icdb_decoder
Decode ICDB files for bakutsuri 3ds

# Build
This code is reasonably framework-agnostic, and should compile with any of the latest C# .net toolchains
Tested with visual studio using the latest .net framework on windows, and .net 5 on linux

# Usage
Once compiled, simply drag your .icdb file onto the exe.
You can also specify a .icdb file to open as a commandline argument.
A .csv file will be generated with the same filename, in the same directory as the input .icdb file.
