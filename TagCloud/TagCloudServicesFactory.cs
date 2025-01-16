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
        
        builder.RegisterType<TagCloudGenerator>().AsSelf().SingleInstance();
        
        builder.Register<ICloudShaper>(provider => SpiralCloudShaper.Create(new Point(0, 0), coefficient: options.Density)).SingleInstance();
        builder.RegisterType<CloudLayouter>().AsSelf().SingleInstance();
        
        builder.Register(_ => new Font(new FontFamily(options.Font), options.FontSize)).As<Font>().InstancePerLifetimeScope();
        builder.RegisterType<TextMeasurer>().AsSelf().SingleInstance();
        
        builder.Register(provider => TagCloudDrawer.Create(
            options.DestinationPath, 
            options.Name, 
            provider.Resolve<Font>(),
            provider.Resolve<IColorSelector>()
        )).As<ICloudDrawer>().SingleInstance();
        
        // builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()) //TODO: падает Circular component dependency detected: TagCloud.TagCloudGenerator -> ?:TagCloud.TagCloudDrawer -> ?:TagCloud.ColorSelectors.IColorSelector -> ?:TagCloud.ColorSelectors.IColorSelector[] -> ?:TagCloud.ColorSelectors.IColorSelector.
        //     .Where(t => t.IsAssignableTo<IColorSelector>())
        //     .As<IColorSelector>()
        //     .SingleInstance();
        //
        // builder.Register<IColorSelector>(context =>
        // {
        //     var colorSelectors = context.Resolve<IEnumerable<IColorSelector>>();
        //     var selector = colorSelectors.FirstOrDefault(s => s.IsMatch(options));
        //     
        //     return selector ?? new ConstantColorSelector(Color.Black);
        // }).SingleInstance(); //при ресолве будет использоваться последний зарегистрированный класс
        if (options.ColorScheme == "random")
            builder.RegisterType<RandomColorSelector>().As<IColorSelector>().SingleInstance();
        if (options.ColorScheme == "gray")
            builder.RegisterType<GrayScaleColorSelector>().As<IColorSelector>().SingleInstance();
        else if (ColorConverter.TryConvert(options.ColorScheme, out var color))
            builder.Register(provider => new ConstantColorSelector(color)).As<IColorSelector>().SingleInstance();
        else
            builder.Register(provider => new ConstantColorSelector(Color.Black)).As<IColorSelector>().SingleInstance();
        
        builder.RegisterType<MyStemWordFilter>().As<IWordFilter>().SingleInstance();
        
        builder.Register(provider => 
            new FileTextHandler(
                stream: File.Open(options.SourcePath, FileMode.Open), 
                filter: provider.Resolve<IWordFilter>()
            )
        ).As<ITextHandler>().SingleInstance();
        
        return builder.Build();    }

    public static Result<T> ConfigureServiceAndGet<T>(Options option)
    {
        var result = Result.Of(() => ConfigureService(option));
        if (!result.IsSuccess) return Result.Fail<T>(result.Error);
        using var container = result.Value;
        return container.Resolve<T>();
    }
}