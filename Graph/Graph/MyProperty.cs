using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.Graph;

public class MyProperty
{
    public string DefaultValue { get; set; } = string.Empty;

    private Dictionary<string, string> _properties = [];

    public string this[string key]
    {
        get => GetProperty(key);
        set => SetProperty(key, value);
    }

    private void SetProperty(string key, string value)
    {
        if (value == DefaultValue)
        {
            _properties.Remove(key);
            return;
        }
        _properties[key] = value;
    }

    private string GetProperty(string key)
    {
        if (_properties.TryGetValue(key, out var value)) return value;
        return DefaultValue;
    }



}
