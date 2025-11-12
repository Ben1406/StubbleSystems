using System.ComponentModel;
using System.Collections.Concurrent;

namespace FortisFramework.Utilities;

public interface IDatastore
{
    bool Add(string name);
    bool Add(string name, object? value);
    bool AddOrSetValue(string name, object value);
    DatastoreItem? Get(string name);
    T? GetValue<T>(string name);
    bool SetValue(string name, object? value);

    void AttachChangedEvent(string name, EventHandler<DatastoreChangedEventArgs> datastoreChangedEvent);
    void DetachChangedEvent(string name, EventHandler<DatastoreChangedEventArgs> datastoreChangedEvent);
}

public class Datastore : IDatastore
{
    private readonly ConcurrentDictionary<string, DatastoreItem> _items = new();

    public bool Add(string name)
    {
        return CreateItem(name, null);
    }

    public bool Add(string name, object? value)
    {
        return CreateItem(name, value);
    }

    public bool AddOrSetValue(string name, object value)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        if (existingDatastoreElement is null)
        {
            return CreateItem(name, value);
        }

        return existingDatastoreElement.SetValue(value);
    }

    public DatastoreItem? Get(string name)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        return existingDatastoreElement;
    }

    public T? GetValue<T>(string name)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        if (existingDatastoreElement?.Value is null)
        {
            return default;
        }

        var value = existingDatastoreElement.Value;

        var typeConverter = TypeDescriptor.GetConverter(typeof(T));
        var requestType = typeof(T);
        if (existingDatastoreElement.OriginType == requestType)
        {
            return (T) value;
        }
        
        if (typeConverter.CanConvertFrom(value.GetType()))
        {
            return (T)typeConverter.ConvertFrom(value)!;
        }

        return default;
    }

    public bool SetValue(string name, object? value)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        if (existingDatastoreElement is null)
        {
            return false;
        }

        return existingDatastoreElement.SetValue(value);
    }

    private bool CreateItem(string name, object? value)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        if (existingDatastoreElement is not null)
        {
            return false;
        }

        var newItem = new DatastoreItem(name, value);
        return _items.TryAdd(name, newItem);
    }

    public void AttachChangedEvent(string name, EventHandler<DatastoreChangedEventArgs> datastoreChangedEvent)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        if (existingDatastoreElement is not null)
        {
            existingDatastoreElement.OnDatastoreChanged += datastoreChangedEvent;
        }
    }

    public void DetachChangedEvent(string name, EventHandler<DatastoreChangedEventArgs> datastoreChangedEvent)
    {
        _items.TryGetValue(name, out var existingDatastoreElement);
        if (existingDatastoreElement is not null)
        {
            existingDatastoreElement.OnDatastoreChanged -= datastoreChangedEvent;
        }
    }
}

public class DatastoreItem(string name, object? value)
{
    public event EventHandler<DatastoreChangedEventArgs> OnDatastoreChanged = delegate { };

    internal bool SetValue(object? newValue)
    {
        if (newValue is not null && OriginType is not null)
        {
            var typeConverter = TypeDescriptor.GetConverter(OriginType);
            var newType = newValue.GetType();
            if (OriginType != newType && !typeConverter.CanConvertFrom(newType))
            {
                return false;
            }
        }

        if (newValue is not null && OriginType is null)
        {
            OriginType = newValue.GetType();
        }

        Value = newValue;
        OnDatastoreChanged.Invoke(this, new DatastoreChangedEventArgs(this));
        return true;
    }

    public string Name { get; private set; } = name;
    public object? Value { get; private set; } = value;
    public Type? OriginType { get; internal set; } = value?.GetType();
}

public class DatastoreChangedEventArgs(DatastoreItem item)
{
    public DatastoreItem Item { get; } = item;
}
