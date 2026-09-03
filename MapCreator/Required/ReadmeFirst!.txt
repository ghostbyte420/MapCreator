REQUIRED FOR MAPCREATOR PLUGINS TO RUN:

[!] Place the Development, ClientFileData, UserXMLFiles, and runtimes directories into the .bin directory where MapCreator is compiled.

The best way to do this is to start up MapCreator in VisualStudio 2026. Then clean the solution and build it as a debug release. After 
that initial build the binary files will be in the .bin directory. Place the folders above into the .bin directory where the MapCreator
executable (.exe) is located. Once that is done, users can then start creating their map templates and move forward from there.