using System.Drawing;
using Autofac;
using TagCloud.ColorSelectors;
using TagCloud.Excluders;
using TagCloud.TextHandlers;
using TagCloud.WordFilters;
using TagCloudApplication;
using TagCloudTests;
using ColorConverter = TagCloud.Extensions.ColorConverter;
using System.Linq;
using System.Reflection;

namespace TagCloud;

public static class TagCloudServicesFactory
{
    public static IContainer ConfigureService(Options options)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(new ConstantColorSelector(Color.Black));

        var colorSelectorTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAssignableTo<IColorSelector>())
            .Where(type => type
                .GetMethod(nameof(IColorSelector.CreateFromOptions), BindingFlags.Static | BindingFlags.Public)
                ?.Invoke(null, new object[] { options }) is IColorSelector)
            .ToList();

        colorSelectorTypes.ForEach(type =>
        {
            if (type.GetMethod(nameof(IColorSelector.CreateFromOptions),
                        BindingFlags.Static | BindingFlags.Public)
                    ?.Invoke(null, [options]) is IColorSelector instance)
            {
                builder.RegisterInstance(instance).As<IColorSelector>().SingleInstance();
            }
        });
        builder.RegisterType<TagCloudGenerator>().AsSelf().SingleInstance();

        builder.Register<ICloudShaper>(provider =>
            SpiralCloudShaper.Create(new Point(0, 0), coefficient: options.Density)).SingleInstance();
        builder.RegisterType<CloudLayouter>().AsSelf().SingleInstance();

        builder.Register(_ => new Font(new FontFamily(options.Font), options.FontSize)).As<Font>()
            .InstancePerLifetimeScope();
        builder.RegisterType<TextMeasurer>().AsSelf().SingleInstance();

        builder.Register(provider => TagCloudDrawer.Create(
            options.DestinationPath,
            options.Name,
            provider.Resolve<Font>(),
            provider.Resolve<IColorSelector>()
        )).As<ICloudDrawer>().SingleInstance();


        builder.RegisterType<MyStemWordFilter>().As<IWordFilter>().SingleInstance();

        builder.Register(provider =>
            new FileTextHandler(
                stream: File.Open(options.SourcePath, FileMode.Open),
                filter: provider.Resolve<IWordFilter>()
            )
        ).As<ITextHandler>().SingleInstance();

        return builder.Build();
    }

    public static Result<T> ConfigureServiceAndGet<T>(Options option)
    {
        var result = Result.Of(() => ConfigureService(option));
        if (!result.IsSuccess) return Result.Fail<T>(result.Error);
        using var container = result.Value;
        return container.Resolve<T>();
    }
}