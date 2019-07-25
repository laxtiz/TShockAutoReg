var target = Argument<string>("target", "Build");
var configuration = Argument<string>("configuration", "Release");

var tshockRepo = "https://github.com/Pryaxis/TShock";
var tshockDownloadUrl = "https://github.com/Pryaxis/TShock/releases/download/v4.3.24/tshock_4.3.24.zip";

Setup(ctx =>
{
    Information("Check tshock.");
    if (!FileExists("./tshock/TerrariaServer.exe"))
    {
        Warning($"Download tshock from {tshockRepo}");
        var zipFile = DownloadFile(tshockDownloadUrl);
        Unzip(zipFile, "./tshock");
    }
});

Task("Build")
    .Does(() =>
    {
        DotNetCoreBuild("./AutoReg.sln", new DotNetCoreBuildSettings { Configuration = configuration });
    });

Task("Clean")
    .Does(() =>
    {
        DotNetCoreClean("./AutoReg.sln", new DotNetCoreCleanSettings { Configuration = configuration });
    });

Task("Run")
    .Does(() =>
    {
        DotNetCorePublish("./AutoReg.sln", new DotNetCorePublishSettings { OutputDirectory = "./tshock/ServerPlugins" });

        var dir = Directory("./tshock");
        var exe = "./tshock/TerrariaServer.exe";
        var args = "-ip 127.0.0.1";
        if (IsRunningOnUnix())
        {
            args = $"{exe} {args}";
            exe = "mono";
        }
        StartProcess(exe, new ProcessSettings { Arguments = args, WorkingDirectory = dir });
    });

RunTarget(target)
