namespace NeHive.Reactive;

using System.Collections;
using System.Collections.ObjectModel;

/// <summary>
/// Marker interface for reactive collections.
///
/// Enumerating or accessing elements may establish
/// reactive dependencies depending on the operation.
/// Reactive collections should not be treated as
/// ordinary immutable snapshots.
/// </summary>
public interface IReactiveCollection<out T> : IEnumerable<T>
{
}

/// <summary>
/// A reactive list container that tracks individual element access, count changes,
/// and structural modifications. Provides fine-grained signal-based reactivity
/// for each index, batch updates, and version-based change detection.
/// 
/// Unlike Signal&lt;List&lt;T&gt;&gt;,
/// ListStore provides fine-grained reactivity.
/// Reading store[0] only depends on index 0,
/// while reading Count only depends on collection size.
/// </summary>
/// <typeparam name="T">The element type</typeparam>
/// <example>
/// <code>
/// var store = new ListStore&lt;string&gt;
/// {
///     "Alice",
///     "Bob"
/// };
///
/// new Effect(() =>
/// {
///     Console.WriteLine(store[0]);
/// });
///
/// store[0] = "Tom";
/// // Effect re-executes
/// </code>
/// </example>
public class ListStore<T> : IReactiveCollection<T>
{
    private const int DefaultCapacity = 4;

    private readonly List<T> _items;
    private readonly MutSignal<int> _countMutSignal;
    private readonly MutSignal<int> _versionMutSignal = new(0);
    private readonly SortedDictionary<int, MutSignal<T?>> _oldValueSignals = []; // index是固定的

    private bool _isBatch;
    private bool _isChange;

    /// <summary>
    /// Gets or sets the equality comparer used to detect value changes.
    /// Defaults to <see cref="EqualityComparer{T}.Default"/>.
    /// </summary>
    public Func<T?, T?, bool> Comparator { get; init; } = Constant.EqualFn;

    // 模仿 List 的用法
    /// <summary>
    /// Creates an empty ListStore with the specified capacity.
    /// </summary>
    /// <param name="capacity">Initial capacity (default 4)</param>
    public ListStore(int capacity = DefaultCapacity)
    {
        _items = new(capacity);
        _countMutSignal = new(_items.Count);
    }

    /// <summary>
    /// Creates a ListStore populated from an existing collection.
    /// </summary>
    /// <param name="collection">Initial elements</param>
    public ListStore(IEnumerable<T> collection)
    {
        _items = new(collection);
        _countMutSignal = new(_items.Count);
    }

    /// <summary>
    /// Gets or sets the underlying list capacity.
    /// </summary>
    public int Capacity
    {
        get => _items.Capacity;
        set => _items.Capacity = value;
    }

    /// <summary>
    /// Gets the number of elements (reactive - tracks dependency on count changes).
    /// </summary>
    public int Count => _countMutSignal.RxValue;

    private void _subscribeSignal(int index, T? initValue)
    {
        if (ReactiveContext.CurrentTracker is null) return;
        if (!_oldValueSignals.TryGetValue(index, out var signal))
        {
            signal = new MutSignal<T?>(initValue);
            _oldValueSignals[index] = signal;
        }

        _ = signal.RxValue; // Access the index signal to establish the dependency relationship
    }

    private void _updateSignalValue(int index, T value)
    {
        Rx.Batch(() =>
        {
            if (!_oldValueSignals.TryGetValue(index, out var indexSignal)) return;
            if (Comparator(indexSignal.Value, value)) return;
            _items[index] = value;
            _isChange = true;
            if (_isBatch) return;
            indexSignal.RxValue = value;
            _versionMutSignal.RxValue = _versionMutSignal.Value + 1;
        });
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    ///
    /// Reading an element establishes a dependency on
    /// that specific index only.
    /// </summary>
    /// <param name="index">
    /// Element index.
    /// </param>
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

    /// <summary>
    /// Attempts to read an element by index.
    ///
    /// Unlike the indexer, this method does not throw
    /// when the index is out of range.
    /// Reading still establishes a dependency on the
    /// specified index.
    /// </summary>
    public bool TryGetValue(int index, out T? value)
    {
        value = default;
        if (index >= 0 && index < _items.Count) value = _items[index];
        _subscribeSignal(index, value);
        return value is not null;
    }

    /// <summary>
    /// Safely sets a value by index without throwing on out-of-range.
    /// </summary>
    /// <param name="index">Element index</param>
    /// <param name="value">The new value</param>
    /// <returns>The new value or default if index was out of range</returns>
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
        Rx.Batch(() =>
        {
            foreach (var (key, signal) in _oldValueSignals)
            {
                if (key < fromIndex) continue;
                TryGetValue(key, out var newValue);
                if (Comparator(signal.Value, newValue)) continue;
                signal.RxValue = newValue;
            }

            if (_countMutSignal.Value != _items.Count)
                _countMutSignal.RxValue = _items.Count;

            _versionMutSignal.RxValue = _versionMutSignal.Value + 1;
            _isChange = false;
        });
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        _ = _versionMutSignal.RxValue;
        _items.CopyTo(index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _ = _versionMutSignal.RxValue;
        _items.CopyTo(array, arrayIndex);
    }

    public ReadOnlyCollection<T> AsReadOnly()
    {
        _ = _versionMutSignal.RxValue;
        return new ReadOnlyCollection<T>(_items);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
    {
        _ = _versionMutSignal.RxValue;
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
        Rx.Batch(() =>
        {
            List<int> removeKeys = [];
            foreach (var (key, signal) in _oldValueSignals)
            {
                if (!signal.HasObserver)
                {
                    removeKeys.Add(key); // 移除没有观察者的索引信号
                    continue;
                }

                if (Comparator(default, signal.Value)) continue;
                signal.RxValue = default;
            }

            foreach (var key in removeKeys)
            {
                _oldValueSignals.Remove(key);
            }

            _countMutSignal.RxValue = 0;
            _versionMutSignal.RxValue++;
        });
    }

    public bool Contains(T item)
    {
        _ = _versionMutSignal.RxValue; // Access the index signal to establish the dependency relationship
        return _items.Contains(item);
    }

    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        _ = _versionMutSignal.RxValue;
        return _items.ConvertAll(converter);
    }

    public int EnsureCapacity(int capacity) => _items.EnsureCapacity(capacity);

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        _ = _versionMutSignal.RxValue;
        return _items.FindIndex(startIndex, count, match);
    }

    public int FindIndex(Predicate<T> match)
        => FindIndex(0, _items.Count, match);

    public int FindIndex(int startIndex, Predicate<T> match)
        => FindIndex(startIndex, _items.Count - startIndex, match);

    public bool Exists(Predicate<T> match) => FindIndex(match) != -1;

    public T? Find(Predicate<T> match)
    {
        _ = _versionMutSignal.RxValue;
        return _items.Find(match);
    }

    public List<T> FindAll(Predicate<T> match)
    {
        _ = _versionMutSignal.RxValue;
        return _items.FindAll(match);
    }

    public T? FindLast(Predicate<T> match)
    {
        _ = _versionMutSignal.RxValue;
        return _items.FindLast(match);
    }

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        _ = _versionMutSignal.RxValue;
        return _items.FindLastIndex(startIndex, count, match);
    }

    public int FindLastIndex(Predicate<T> match)
        => FindLastIndex(_items.Count - 1, _items.Count, match);

    public int FindLastIndex(int startIndex, Predicate<T> match)
        => FindLastIndex(startIndex, startIndex + 1, match);

    /// <summary>
    /// Performs an action on each element. Establishes version-based reactive dependency.
    /// </summary>
    ///  <param name="action">
    /// Invoked for each element and its index.
    /// </param>
    public void ForEach(Action<T> action)
    {
        _ = _versionMutSignal.RxValue;
        _items.ForEach(action);
    }

    // 新增API
    /// <summary>
    /// Performs an action on each element with its index.
    /// Throws if the collection is modified during iteration.
    /// </summary>
    public void ForEach(Action<T, int> action)
    {
        var version = _versionMutSignal.RxValue;

        for (var i = 0; i < _items.Count; i++)
        {
            if (version != _versionMutSignal.Value)
                throw new InvalidOperationException();

            action(_items[i], i);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        _ = _versionMutSignal.RxValue; // Access the index signal to establish the dependency relationship
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public List<T> GetRange(int index, int count)
    {
        var result = _items.GetRange(index, count);
        _ = _versionMutSignal.RxValue;
        return result;
    }

    /// <summary>
    /// Returns a slice of the list from start with the given length.
    /// Alias for GetRange.
    /// </summary>
    public List<T> Slice(int start, int length) => GetRange(start, length);

    public int IndexOf(T item)
    {
        _ = _versionMutSignal.RxValue; // Access the index signal to establish the dependency relationship
        return _items.IndexOf(item);
    }

    public int IndexOf(T item, int index)
    {
        _ = _versionMutSignal.RxValue;
        return _items.IndexOf(item, index);
    }

    public int IndexOf(T item, int index, int count)
    {
        _ = _versionMutSignal.RxValue;
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
        _ = _versionMutSignal.RxValue;
        return _items.LastIndexOf(item);
    }

    public int LastIndexOf(T item, int index)
    {
        _ = _versionMutSignal.RxValue;
        return _items.LastIndexOf(item, index);
    }

    public int LastIndexOf(T item, int index, int count)
    {
        _ = _versionMutSignal.RxValue;
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
    /// Batch modification to enhance performance, especially for structural changes
    /// </summary>
    /// <param name="fn"></param>
    /// <example>
    /// <code>
    /// store.BatchModify(list =>
    /// {
    ///     list.Add("A");
    ///     list.Add("B");
    ///     list.RemoveAt(0);
    /// });
    ///
    /// // Observers are notified only once.
    /// </code>
    /// </example>
    public void BatchModify(Action<ListStore<T>> fn)
    {
        _isBatch = true;
        fn(this);
        _isBatch = false;
        UpdateSignals();
    }

    public T[] ToArray()
    {
        _ = _versionMutSignal.RxValue;
        return _items.ToArray();
    }

    /// <summary>
    /// Removes internal per-index signals that are no
    /// longer observed, reducing memory usage.
    /// </summary>
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

    /// <summary>
    /// Trims the underlying list capacity to match the current element count.
    /// </summary>
    public void TrimExcess()
    {
        _items.TrimExcess();
    }

    /// <summary>
    /// Trims both internal signal storage and list capacity.
    /// </summary>
    public void TrimAll()
    {
        TrimSignals();
        TrimExcess();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        _ = _versionMutSignal.RxValue;
        return _items.TrueForAll(match);
    }
}