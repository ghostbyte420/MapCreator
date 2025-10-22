## Welcome to MapCreator!

This software is for use with all custom Ultima Online™ emulation servers; whether you are using **UOX**, **POL**, **RunUO**, **ServUO**, or **Sphere**, using this software with a paint program will enable you to compile your own custom worlds!


### Software License

> This software is provided as-is, it is completely open source, and the only ask is that if you improve it then pay it forward and send us a copy so we can evaluate the changes and possibly integrate them with the next release. This includes, but is not limited to, plugins you might make and changes to the core code.

### [MapCreator.Engine](https://github.com/ghostbyte420/MapCreator.Engine)

> This class library is required for the compiler to run. Without it, the program will not work.

### How To Compile

> *Requirements*
Visual Studio 2022 Community Edition

> To compile load up the ${\color{blue}MapCreator.csproj}$ file and then save the solution file to **MapCreator**. Then add the ${\color{blue}MapCreator.Engine.csproj}$ as an existing project to the solution you just created and save. Now that the project is set up link the two projects by removing and re-adding the ${\color{blue}MapCreator.Engine.csproj}$ project as a reference to the ${\color{blue}MapCreator.csproj}$ project. Now build your solution and run the program. 

> Two Required Folders have been deliberately left out **for now** because the software is not ready for deployment yet. When it comes time to release MapCreator we will add the *Development* and the *MapCompiler* directories into the repository. These two folders are required for the MapCreator.Engine.dll to work.