# VSAutomate

## Summary
VSAutomate is a sample project to help setup common conventions and configuration
used across all deveopment projects using nuget and VS envDTE. 


The sample tasks included in this package are

1. **Solutions folders** - set up those folders you use in every solution, defaults are .build and .docs
2. **Solution configurations** - set up solution build configurations, defaults are development, staging and release
3. **GlobalAssemblyInfo.cs** - adds a globalassemblyinfo.cs file and links all projects to it
2. **Stylecop** - adds a solution stylecop.settings file and links all projects to it
3. **Default.ruleset** - adds a solution code analysis ruleset and links all projects to it. By default always enabled codeanalysis to run on all projects for all configurations
4. **Custom Dictionary** - adds a solution custom dictionary and links all projects to it
5. **Config transforms** - adds config transform documents to all app.config files and configures the transforms to run on build

## Debugging

To debug the package, take the following steps.

1. build the solution in debug configuration
2. open the testsolution.sln in a second instance of visual studio
3. open the package manager console 
4. [click the cog] create a local package feed pointing to the VSAutomate.Nuget folder
5. > install-pckage VSAutomate

