namespace NeHive.Core;

using System.Collections;
using System.Collections.ObjectModel;

// 不要把它当成普通的集合，尤其是在Effect环境上，所以你最好知道它是响应式类型
public interface IReactiveCollection<out T> : IEnumerable<T>
{
    
}

public class ListStore<T> : IReactiveCollection<T>
{
    private const int DefaultCapacity = 4;

    private readonly List<T> _items;
    private readonly Signal<int> _countSignal;
    private readonly Signal<int> _versionSignal = new(0);
    private readonly SortedDictionary<int, Signal<T?>> _oldValueSignals = []; // index是固定的

    private bool _isBatch;
    private bool _isChange;

    public Func<T?, T?, bool> Comparator { get; init; } = Constant.EqualFn;

    // 模仿 List 的用法
    public ListStore(int capacity = DefaultCapacity)
    {
        _items = new(capacity);
        _countSignal = new(_items.Count);
    }

    public ListStore(IEnumerable<T> collection)
    {
        _items = new(collection);
        _countSignal = new(_items.Count);
    }

    public int Capacity
    {
        get => _items.Capacity;
        set => _items.Capacity = value;
    }

    public int Count => _countSignal.Value;

    private void _subscribeSignal(int index, T? initValue)
    {
        if (ReactiveContext.CurrentExecute is null) return;
        if (!_oldValueSignals.TryGetValue(index, out var signal))
        {
            signal = new Signal<T?>(initValue);
            _oldValueSignals[index] = signal;
        }

        _ = signal.Value; // 访问索引信号以建立依赖关系
    }

    private void _updateSignalValue(int index, T value)
    {
        Reactive.Batch(() =>
        {
            if (!_oldValueSignals.TryGetValue(index, out var indexSignal)) return;
            if (Comparator(indexSignal.UntrackValue, value)) return;
            _items[index] = value;
            _isChange = true;
            if (_isBatch) return;
            indexSignal.Value = value;
            _versionSignal.Value = _versionSignal.UntrackValue + 1;
        });
    }

    public T this[int index]
    {
        get
        {
            var result = _items[index];
            _subscribeSignal(index, result);
            return result;
        }
        set
        {
            if (index < 0 || index >= _items.Count) throw new ArgumentOutOfRangeException(nameof(index));
            _updateSignalValue(index, value);
        }
    }

    // 新增API，防止索引丢失报错
    public bool TryGetValue(int index, out T? value)
    {
        value = default;
        if (index >= 0 && index < _items.Count) value = _items[index];
        _subscribeSignal(index, value);
        return value is not null;
    }

    public T? TrySetValue(int index, T value)
    {
        if (index < 0 || index >= _items.Count) return default;
        _updateSignalValue(index, value);
        return value;
    }

    public void Add(T item)
    {
        var oldCount = _items.Count;
        _items.Add(item);
        _isChange = true;
        UpdateSignals(oldCount);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        var oldCount = _items.Count;
        _items.AddRange(collection);
        var newCount = _items.Count;
        if (oldCount == newCount) return;
        _isChange = true;
        UpdateSignals(oldCount);
    }

    private void UpdateSignals(int fromIndex = 0)
    {
        if (_isBatch) return;
        if (!_isChange) return;
        Reactive.Batch(() =>
        {
            foreach (var (key, signal) in _oldValueSignals)
            {
                if (key < fromIndex) continue;
                TryGetValue(key, out var newValue);
                if (Comparator(signal.UntrackValue, newValue)) continue;
                signal.Value = newValue;
            }

            if (_countSignal.UntrackValue != _items.Count)
                _countSignal.Value = _items.Count;

            _versionSignal.Value = _versionSignal.UntrackValue + 1;
            _isChange = false;
        });
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        _ = _versionSignal.Value;
        _items.CopyTo(index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _ = _versionSignal.Value;
        _items.CopyTo(array, arrayIndex);
    }

    public ReadOnlyCollection<T> AsReadOnly()
    {
        _ = _versionSignal.Value;
        return new ReadOnlyCollection<T>(_items);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
    {
        _ = _versionSignal.Value;
        return _items.BinarySearch(index, count, item, comparer);
    }

    public int BinarySearch(T item)
        => BinarySearch(0, Count, item, null);

    public int BinarySearch(T item, IComparer<T>? comparer)
        => BinarySearch(0, Count, item, comparer);


    public void Clear()
    {
        if (_items.Count == 0) return;
        _items.Clear();
        Reactive.Batch(() =>
        {
            List<int> removeKeys = [];
            foreach (var (key, signal) in _oldValueSignals)
            {
                if (!signal.HasObserver)
                {
                    removeKeys.Add(key); // 移除没有观察者的索引信号
                    continue;
                }

                if (Comparator(default, signal.UntrackValue)) continue;
                signal.Value = default;
            }

            foreach (var key in removeKeys)
            {
                _oldValueSignals.Remove(key);
            }

            _countSignal.Value = 0;
            _versionSignal.Value++;
        });
    }

    public bool Contains(T item)
    {
        _ = _versionSignal.Value; // 访问版本信号以建立依赖关系
        return _items.Contains(item);
    }

    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        _ = _versionSignal.Value;
        return _items.ConvertAll(converter);
    }

    public int EnsureCapacity(int capacity) => _items.EnsureCapacity(capacity);

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        _ = _versionSignal.Value;
        return _items.FindIndex(startIndex, count, match);
    }

    public int FindIndex(Predicate<T> match)
        => FindIndex(0, _items.Count, match);

    public int FindIndex(int startIndex, Predicate<T> match)
        => FindIndex(startIndex, _items.Count - startIndex, match);

    public bool Exists(Predicate<T> match) => FindIndex(match) != -1;

    public T? Find(Predicate<T> match)
    {
        _ = _versionSignal.Value;
        return _items.Find(match);
    }

    public List<T> FindAll(Predicate<T> match)
    {
        _ = _versionSignal.Value;
        return _items.FindAll(match);
    }

    public T? FindLast(Predicate<T> match)
    {
        _ = _versionSignal.Value;
        return _items.FindLast(match);
    }

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        _ = _versionSignal.Value;
        return _items.FindLastIndex(startIndex, count, match);
    }

    public int FindLastIndex(Predicate<T> match)
        => FindLastIndex(_items.Count - 1, _items.Count, match);

    public int FindLastIndex(int startIndex, Predicate<T> match)
        => FindLastIndex(startIndex, startIndex + 1, match);

    public void ForEach(Action<T> action)
    {
        _ = _versionSignal.Value;
        _items.ForEach(action);
    }

    // 新增API
    public void ForEach(Action<T, int> action)
    {
        var version = _versionSignal.Value;

        for (var i = 0; i < _items.Count; i++)
        {
            if (version != _versionSignal.UntrackValue)
                throw new InvalidOperationException();

            action(_items[i], i);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        _ = _versionSignal.Value; // 访问版本信号以建立依赖关系
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public List<T> GetRange(int index, int count)
    {
        var result = _items.GetRange(index, count);
        _ = _versionSignal.Value;
        return result;
    }

    public List<T> Slice(int start, int length) => GetRange(start, length);

    public int IndexOf(T item)
    {
        _ = _versionSignal.Value; // 访问版本信号以建立依赖关系
        return _items.IndexOf(item);
    }

    public int IndexOf(T item, int index)
    {
        _ = _versionSignal.Value;
        return _items.IndexOf(item, index);
    }

    public int IndexOf(T item, int index, int count)
    {
        _ = _versionSignal.Value;
        return _items.IndexOf(item, index, count);
    }

    public void Insert(int index, T item)
    {
        _items.Insert(index, item);
        _isChange = true;
        UpdateSignals(index);
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        var count = _items.Count;
        _items.InsertRange(index, collection);
        var newCount = _items.Count;
        if (count == newCount) return;
        _isChange = true;
        UpdateSignals(index);
    }

    public int LastIndexOf(T item)
    {
        _ = _versionSignal.Value;
        return _items.LastIndexOf(item);
    }

    public int LastIndexOf(T item, int index)
    {
        _ = _versionSignal.Value;
        return _items.LastIndexOf(item, index);
    }

    public int LastIndexOf(T item, int index, int count)
    {
        _ = _versionSignal.Value;
        return _items.LastIndexOf(item, index, count);
    }

    public bool Remove(T item)
    {
        var value = _items.Remove(item);
        if (value)
        {
            _isChange = true;
            UpdateSignals();
        }

        return value;
    }

    public int RemoveAll(Predicate<T> match)
    {
        var value = _items.RemoveAll(match);
        if (value > 0)
        {
            _isChange = true;
            UpdateSignals();
        }

        return value;
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
        _isChange = true;
        UpdateSignals(index);
    }

    public void RemoveRange(int index, int count)
    {
        if (count == 0) return;
        _items.RemoveRange(index, count);
        _isChange = true;
        UpdateSignals(index);
    }

    public void Reverse(int index, int count)
    {
        if (count is 0 or 1) return;
        _items.Reverse(index, count);
        _isChange = true;
        UpdateSignals(index);
    }

    public void Reverse()
        => Reverse(0, _items.Count);

    public void Sort(int index, int count, IComparer<T>? comparer = null)
    {
        if (count is 0 or 1) return;
        _items.Sort(index, count, comparer);
        _isChange = true;
        UpdateSignals(index);
    }

    public void Sort()
        => Sort(0, _items.Count);

    public void Sort(IComparer<T>? comparer)
        => Sort(0, _items.Count, comparer);

    public void Sort(Comparison<T> comparison)
    {
        _items.Sort(comparison);
        _isChange = true;
        UpdateSignals();
    }

    /// <summary>
    /// 批量修改以提高性能，尤其是修改结构
    /// </summary>
    /// <param name="fn"></param>
    public void BatchModify(Action<ListStore<T>> fn)
    {
        _isBatch = true;
        fn(this);
        _isBatch = false;
        UpdateSignals();
    }

    public T[] ToArray()
    {
        _ = _versionSignal.Value;
        return _items.ToArray();
    }

    public void TrimSignals()
    {
        foreach (var (key, signal) in _oldValueSignals)
        {
            if (!signal.HasObserver)
            {
                _oldValueSignals.Remove(key);
            }
        }
    }

    public void TrimExcess()
    {
        _items.TrimExcess();
    }

    public void TrimAll()
    {
        TrimSignals();
        TrimExcess();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        _ = _versionSignal.Value;
        return _items.TrueForAll(match);
    }
}