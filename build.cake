var projectName = "Orc.NuGetExplorer";
var projectsToPackage = new [] { "Orc.NuGetExplorer", "Orc.NuGetExplorer.Xaml" };
var company = "WildGums";
var startYear = 2010;
var defaultRepositoryUrl = string.Format("https://github.com/{0}/{1}", company, projectName);

#l "./deployment/cake/variables.cake"
#l "./deployment/cake/tasks.cake"
