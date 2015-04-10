# Orc.NuGetExplorer

Provides NuGet packages support for your any application. Very useful to create support remote plugin updating for your program.

## Features

* explore installed and available for installation packages
* checking for updates for all packages
* support package sources, which requires authentication
* smart update (checking for recommended updates for your plugins)
* verification of package source
* ability to work in background

## Downloads

library consists of two NuGet packages:

*  **[Orc.NuGetExplorer](http://www.nuget.org/packages/Orc.NuGetExplorer/)** => logic of this library. It can be used for working in background.
*  **[Orc.NuGetExplorer.Xaml](http://www.nuget.org/packages/Orc.NuGetExplorer.Xaml)** => the version of Ui for **Orc.NuGetExplorer**

## Screenshots

Packages available for installing 

![NuGetExplorer 01](doc/images/NuGetExplorer_01.png)

Available updates

![NuGetExplorer 02](doc/images/NuGetExplorer_02.png)

Recommended updates

![NuGetExplorer 03](doc/images/NuGetExplorer_03.png)

## How to use

to use Orc.NuGetExplorer, you must add it to your project (look **Downloads** section) and use:

services:

* **INuGetConfigurationService** => configure NuGetExplorer
* **INuGetFeedVerificationService** => ferify package source feed
* **IPackageBatchService** => show window with list of package to make spacific operation on this packages (for all at the same time or individually for each package)
* **IPackageOperationService** => execute operation with the package (Install, Uninstall or Update)
* **IPackageQueryService** => can be used to search packages in background
* **IPackagesUIService** => show NuGetExplorer window
* **IPackagesUpdatesSearcherService** => used for searching updates with the options release/prerelease/recommended
* **IRepositoryService** => allow ro get acces to repositories. could be used for searching with the *IPackageQueryService*

watcher:
* **PackageManagerWatcherBase** => abstract class, which used to create watcher for package management operations