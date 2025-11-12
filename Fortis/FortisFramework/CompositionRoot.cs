using Autofac;
using FortisDeviceCenter;
using FortisFramework.Entity;
using FortisFramework.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FortisFramework;

internal static class CompositionRoot
{
    internal static IContainer Configure()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<SystemSetting>().As<ISystemSetting>().SingleInstance();
        builder.RegisterType<Terminal>().As<ITerminal>().SingleInstance();
        builder.RegisterType<DeviceCenter>().As<IDeviceCenter>().SingleInstance();
        builder.RegisterType<Datastore>().As<IDatastore>().SingleInstance();
        builder.RegisterType<SystemLogger>().As<ISystemLogger>().SingleInstance();

        builder.RegisterType<FortisFrameworkDbContext>();
        builder.RegisterType<DbContext>();

        var container = builder.Build();
        return container;
    }
}
