using System.Collections;
using System.Collections.Specialized;

namespace Controls.Collection;

public class SyncMapCollection<TSource, TTarget>: IDisposable where TSource : notnull
{
    private Func<TSource, TTarget>? _factory;

    public Action<TTarget>? CleanUp;

    private readonly Dictionary<TSource, TTarget> _dictionary = [];

    private readonly INotifyCollectionChanged _sourceChanged;

    private readonly ICollection<TSource> _sources;

    private readonly ICollection<TTarget> _targets;

    private readonly bool _clear;

    public SyncMapCollection(
        INotifyCollectionChanged sourceChanged,
        ICollection<TSource> sources, ICollection<TTarget> target,
        Func<TSource, TTarget> factory,
        bool clear)
    {
        _sourceChanged = sourceChanged;
        _sources = sources;
        _targets = target;
        _factory = factory;
        _sourceChanged.CollectionChanged += SourceChanged_CollectionChanged;
        _clear = clear;
    }

    private void SourceChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Move:
                return;
            case NotifyCollectionChangedAction.Reset:
                Rebuild();
                return;
        }
        if (e.OldItems != null) Remove(e.OldItems);
        if (e.NewItems != null) Add(e.NewItems);
    }

    private void InitialBuild()
    {
        foreach (var source in _sources)
        {
            AddSource(source);
        }
    }

    private void Clear()
    {
        if (_clear)
        {
            _dictionary.Clear();
            if (CleanUp != null)
            {
                foreach (var item in _targets) CleanUp(item);
            }
            _targets.Clear();
            return;
        }
        foreach(var source in _sources)
        {
            RemoveSource(source);
        }
    }

    private void Add(IList list)
    {
        foreach (var item in list)
        {
            if (item is not TSource source) continue;
            AddSource(source);
        }
    }

    private void Remove(IList list)
    {
        foreach (var item in list)
        {
            if (item is not TSource source) continue;
            RemoveSource(source);
        }
    }

    private void AddSource(TSource source)
    {
        _factory ??= ExceptionHelper();
        var target = _factory(source);
        _dictionary[source] = target;
        _targets.Add(target);
    }

    private void RemoveSource(TSource source)
    {
        if (!_dictionary.TryGetValue(source, out var target)) return;
        _targets.Remove(target);
        CleanUp?.Invoke(target);
        _dictionary.Remove(source);
    }

    public void Rebuild()
        => Rebuild(_factory ?? ExceptionHelper());

    public void Rebuild(Func<TSource, TTarget> factory)
    {
        _factory = factory;
        Clear();
        InitialBuild();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Clear();
        _sourceChanged.CollectionChanged -= SourceChanged_CollectionChanged;
        _factory = null;
        CleanUp = null;
    }

    public IEnumerable<TTarget> GetTargets(IEnumerable<TSource> source)
    {
        return source.Select(a => _dictionary[a]);
    }

    private static Func<TSource, TTarget> ExceptionHelper()
    {
        throw new InvalidOperationException("Factory cannot be null");
    }
}
