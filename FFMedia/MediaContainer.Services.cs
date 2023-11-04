using FFMedia.ServiceModel;
using System.Reflection;

namespace FFMedia;

public partial class MediaContainer
{
    private Lazy<IReadOnlyDictionary<Type, PropertyInfo>> ServiceProperties { get; }

    private IReadOnlyDictionary<Type, PropertyInfo> BuildServiceProperties()
    {
        const BindingFlags ServicePropertyBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var allProperties = GetType().GetProperties(ServicePropertyBindings);

        var serviceProperties = new Dictionary<Type, PropertyInfo>(allProperties.Length);
        foreach (var property in allProperties)
        {
            if (property is null || !property.CanWrite || !property.PropertyType.IsAssignableTo(typeof(IMediaContainerService)))
                continue;

            serviceProperties.Add(property.PropertyType, property);
        }

        return serviceProperties.AsReadOnly();
    }

    public void AddService<T>()
        where T : class, IMediaContainerService, new() => AddService<T>(new());

    public void AddService<T>(T service)
        where T : class, IMediaContainerService
    {
        if (service is null)
            throw new ArgumentNullException(nameof(service));

        var serviceInterfaces = typeof(T).GetInterfaces();
        var serviceProperty = ServiceProperties.Value.FirstOrDefault(p => serviceInterfaces.Contains(p.Key));

        if (serviceProperty.Key is null)
            throw new NotSupportedException(
                $"Service registration of type '{typeof(T).Name}' is not supported by this {nameof(MediaContainer)}.");

        if (serviceProperty.Value.GetValue(this) is not null)
            throw new InvalidOperationException($"Service Property {serviceProperty.Value.Name} has already been registered.");

        serviceProperty.Value.SetValue(this, service);
    }

    public ITimingService ClockService { get; private set; }


}
