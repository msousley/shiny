﻿using System;
using Shiny.Infrastructure;
using Shiny.IO;
using Shiny.Jobs;
using Shiny.Net;
using Shiny.Power;
using Shiny.Settings;
using Shiny.Support.Uwp;
using Microsoft.Extensions.DependencyInjection;


namespace Shiny
{
    public class UwpShinyHost : ShinyHost
    {
        public static void Init(IShinyStartup startup = null, Action<IServiceCollection> platformBuild = null)
            => InitPlatform(startup, services =>
            {
                services.AddSingleton<IEnvironment, EnvironmentImpl>();
                services.AddSingleton<IConnectivity, ConnectivityImpl>();
                services.AddSingleton<IPowerManager, PowerManagerImpl>();
                services.AddSingleton<IFileSystem, FileSystemImpl>();
                services.AddSingleton<ISettings, SettingsImpl>();
                services.AddSingleton<UwpContext>();

                services.AddSingleton<IJobManager, JobManager>();
                services.AddSingleton<IBackgroundTaskProcessor, JobBackgroundTaskProcessor>();
                services.RegisterPostBuildAction(sp =>
                    ShinyBackgroundTask.Bridge = sp.ResolveOrInstantiate<IUwpBridge>()
                );

                platformBuild?.Invoke(services);
            });
    }
}
