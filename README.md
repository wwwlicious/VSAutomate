# VSAutomateFTW

## Summary
The VSAutomateFTW nuget package is intended to provide a template for the common setup across 
all deveopment projects. The following items are managed by this package

1. **Common folders** .build and .docs
2. **Solution configurations** - development, staging and release
3. **GlobalAssemblyInfo.cs** - A single globalassemblyinfo.cs file is added to the repo route. All projects use this file as a linked item and can override
the name. The versioning will always be consistent across all repo projects by using this global assemblyinfo file
2. **Stylecop** - each project folder has a settings.stylecop link document added which links back to a single config file per repo.
It will be added if it doesn't exist and can be overridden as required.
3. **Default.ruleset** - A code analysis ruleset that each project is configured to use. Overrides for errors can be placed in a globalsuppressions.cs file
for each project. The ruleset will be added if it doesn't exist and can be overridden as required.  
**By default this is always set to run in every build configuration and enforced, change as required**
4. **Custom Dictionary** - Provides a user dictionary linked to each project to store common spellings and acronyms that otherwise would appear as spelling errors.
5. **Config transforms** - Will create a nested config transform for each build configuration that exists in the solution if it doesn't exist. It also adds a transformation
msbuild target that will tranform the config on build and copy it to the output directory. This is primarily used when running tests or debugging different configurations

## Location
Any files that are customisable are stored in the **.build** folder which is created if it doesn't exist


## Debugging

To debug the package, take the following steps.

1. build the solution in debug configuration
2. open the testsolution in a second instance of visual studio
3. open the package manager console 
4. create a local package feed pointing to the VSAutomate.Nugety folder
5. install packe VSAutomate

