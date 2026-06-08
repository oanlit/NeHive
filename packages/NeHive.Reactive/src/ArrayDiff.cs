// Reactive array diffing utilities inspired by SolidJS and S-array.
//
// This implementation adapts SolidJS's reactive array reconciliation
// strategy (mapArray / indexArray) to a .NET-based reactive runtime.
//
// Key differences from SolidJS:
// - Custom Scope / EpochScope execution model
// - Signal graph tracked via ExecuteNode instead of JS Proxy tracking
// - Deterministic lifecycle management via Scope disposal tree
//
// References:
// https://github.com/solidjs/solid
// https://github.com/adamhaile/S-array
//
// This file is fully reimplemented for the NeHive reactive system
// and is not a direct port.

using System.Collections;
using NeHive.Model;

namespace NeHive.Reactive;

/// <summary>
/// Represents the difference between two arrays,
/// including removed items, moved items,
/// and newly added items.
/// </summary>
public struct ArrayDiffResult
{
    /// <summary>
    /// Indices removed from the old array.
    /// </summary>
    public List<int> RemoveItemsIndex;

    /// <summary>
    /// Mapping from old indices to new indices
    /// for preserved items.
    /// </summary>
    public List<(int OldIndex, int NewIndex)> OldIndex2News;

    /// <summary>
    /// Indices that correspond to newly added items.
    /// </summary>
    public List<int> NewItemsIndex;
}

/// <summary>
/// Provides array diffing algorithms used by
/// incremental collection reconciliation.
/// </summary>
public static class ArrayDiffUtil
{
    /// <summary>
    /// Computes the diff between two lists using default equality comparison.
    /// </summary>
    /// <typeparam name="T">The element type (must be notnull for dictionary-based diffing)</typeparam>
    /// <param name="newList">The new/replacement list</param>
    /// <param name="oldList">The current/previous list</param>
    /// <returns>An ArrayDiffResult describing all changes</returns>
    public static ArrayDiffResult ArrayDiff<T>(
        IReadOnlyList<T> newList,
        IReadOnlyList<T> oldList) where T : notnull
    {
        return ArrayDiffCore<T, T>(
            newList,
            oldList,
            item => item);
    }

    /// <summary>
    /// Computes the diff between two lists using a custom key selector.
    /// Items are identified by their key, not by index or reference equality.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <param name="newList">The new/replacement list</param>
    /// <param name="oldList">The current/previous list</param>
    /// <param name="keyFn">A function extracting a stable key from each element</param>
    /// <returns>An ArrayDiffResult describing all changes</returns>
    public static ArrayDiffResult ArrayDiff<T, TKey>(
        IReadOnlyList<T> newList,
        IReadOnlyList<T> oldList,
        Func<T, TKey> keyFn) where TKey : notnull
    {
        return ArrayDiffCore(
            newList,
            oldList,
            keyFn);
    }

    private static ArrayDiffResult ArrayDiffCore<T, TKey>(
        IReadOnlyList<T> newList,
        IReadOnlyList<T> oldList,
        Func<T, TKey> keyFn) where TKey : notnull
    {
        const int noIndex = -1;
        const int maxStackItem = 256;
        var newLen = newList.Count;
        var oldLen = oldList.Count;

        int preIndex;
        int oldSufIndex;
        int newSufIndex;

        var endIndex = Math.Min(oldLen, newLen) - 1;

        Func<T, T, bool> equalsFn = (item1, item2) => EqualityComparer<TKey>.Default.Equals(keyFn(item1), keyFn(item2));

        // skip prefix
        for (preIndex = 0;
             preIndex <= endIndex &&
             equalsFn(oldList[preIndex], newList[preIndex]);
             preIndex++) ;

        // suffix
        for (
            oldSufIndex = oldLen - 1, newSufIndex = newLen - 1;
            oldSufIndex >= preIndex &&
            newSufIndex >= preIndex &&
            equalsFn(oldList[oldSufIndex], newList[newSufIndex]);
            oldSufIndex--, newSufIndex--) ;

        var range = Math.Max(oldSufIndex, newSufIndex) - preIndex + 1;
        var removeItemsIndex = new List<int>(range);
        var oldIndex2News = new List<(int, int)>(range);
        var newItemsIndex = new List<int>(range);

        var processedNewItemsIndex = newLen < maxStackItem
            ? stackalloc bool[newLen]
            : new bool[newLen];
        // 静态链表
        var newIndicesNext = newSufIndex + 1 < maxStackItem
            ? stackalloc int[newSufIndex + 1]
            : new int[newSufIndex + 1];

        var newIndices = new Dictionary<TKey, int>(range);

        // prepare map
        for (var newIndex = newSufIndex; newIndex >= preIndex; newIndex--)
        {
            var key = keyFn(newList[newIndex]);
            var lastRepeatIndex = newIndices.GetValueOrDefault(key, noIndex);

            newIndicesNext[newIndex] = lastRepeatIndex;
            newIndices[key] = newIndex;
        }

        // scan old list
        for (var oldIndex = preIndex; oldIndex <= oldSufIndex; oldIndex++)
        {
            var key = keyFn(oldList[oldIndex]);
            var newIndex = newIndices.GetValueOrDefault(key, noIndex);

            if (newIndex == noIndex)
            {
                removeItemsIndex.Add(oldIndex);
                continue;
            }

            processedNewItemsIndex[newIndex] = true;

            if (oldIndex != newIndex)
            {
                oldIndex2News.Add((oldIndex, newIndex));
            }

            var nextRepeatIndex = newIndicesNext[newIndex];
            newIndices[key] = nextRepeatIndex;
        }

        // suffix processing
        var lenDiff = newLen - oldLen;

        for (var i = newSufIndex + 1; i < newLen; i++)
        {
            processedNewItemsIndex[i] = true;

            if (lenDiff != 0)
            {
                oldIndex2News.Add((i - lenDiff, i));
            }
        }

        // find new items
        for (var newIndex = preIndex; newIndex < newLen; newIndex++)
        {
            if (!processedNewItemsIndex[newIndex])
                newItemsIndex.Add(newIndex);
        }

        return new ArrayDiffResult
        {
            RemoveItemsIndex = removeItemsIndex,
            OldIndex2News = oldIndex2News,
            NewItemsIndex = newItemsIndex
        };
    }
}

/// <summary>
/// Maintains a mapped cache of a source list
/// and updates it incrementally using array diffing.
///
/// Existing mapped values are reused whenever
/// possible to avoid unnecessary allocations.
/// </summary>
public class ArrayMapResult<T, Tu> where T : notnull
{
    private IReadOnlyList<T> _sourceList;
    private readonly List<Tu> _mapCache;
    private readonly Func<T, Tu> _mapFn;
    private readonly Action<Tu>? _removeFn;
    private readonly Func<T, object>? _keyFn;

    /// <summary>
    /// Updates the source list and applies
    /// incremental reconciliation.
    /// </summary>
    public IReadOnlyList<T> SourceList
    {
        get => _sourceList;
        set => SetSourceList(value);
    }

    /// <summary>
    /// The current mapped list.
    /// </summary>
    public IReadOnlyList<Tu> MapList => _mapCache;

    
    /// <summary>
    /// Creates an incrementally maintained mapped list.
    /// </summary>
    /// <param name="initSourceList">
    /// Initial source items.
    /// </param>
    /// <param name="mapFn">
    /// Maps source items to result items.
    /// </param>
    /// <param name="keyFn">
    /// Optional key selector used for identity tracking.
    /// </param>
    /// <param name="removeFn">
    /// Invoked when a mapped item is removed.
    /// </param>
    public ArrayMapResult(
        T[] initSourceList,
        Func<T, Tu> mapFn,
        Func<T, object>? keyFn = null,
        Action<Tu>? removeFn = null)
    {
        _sourceList = initSourceList;
        _mapFn = mapFn;
        _removeFn = removeFn;
        _keyFn = keyFn;

        _mapCache = new List<Tu>(initSourceList.Length);

        for (var i = 0; i < initSourceList.Length; i++)
        {
            _mapCache.Add(mapFn(initSourceList[i]));
        }
    }

    /// <summary>
    /// Maintains a mapped derived list that is incrementally updated when the source list changes.
    /// Uses array diffing to minimize re-mapping: moved items preserve their mapped value.
    /// </summary>
    /// <typeparam name="T">The source element type</typeparam>
    /// <typeparam name="Tu">The mapped result type</typeparam>
    private void SetSourceList(IReadOnlyList<T> newList)
    {
        var diff = _keyFn is null
            ? ArrayDiffUtil.ArrayDiff(
                newList,
                _sourceList
            )
            : ArrayDiffUtil.ArrayDiff(
                newList,
                _sourceList,
                _keyFn
            );

        var oldLen = _sourceList.Count;
        var newLen = newList.Count;
        var maxLen = Math.Max(oldLen, newLen);

        if (_removeFn is not null)
        {
            foreach (var index in diff.RemoveItemsIndex) _removeFn(_mapCache[index]);
        }

        while (_mapCache.Count < maxLen)
            _mapCache.Add(default!);

        var tempMap = new Tu[maxLen];
        Span<bool> definedTempMap = stackalloc bool[maxLen];

        foreach (var (oldIndex, newIndex) in diff.OldIndex2News)
        {
            tempMap[newIndex] = _mapCache[newIndex];
            definedTempMap[newIndex] = true;
            _mapCache[newIndex] = definedTempMap[oldIndex] ? tempMap[oldIndex] : _mapCache[oldIndex];
        }

        foreach (var newItemIndex in diff.NewItemsIndex)
        {
            _mapCache[newItemIndex] =
                _mapFn(newList[newItemIndex]);
        }

        _sourceList = newList;

        if (_mapCache.Count > newLen)
            _mapCache.RemoveRange(newLen, _mapCache.Count - newLen);
    }
}

/// <summary>
/// A resizable contiguous buffer with
/// separately managed logical length.
///
/// Similar to List&lt;T&gt; but exposes direct
/// control over Length and Capacity.
/// </summary>
internal class DenseBuffer<T> : IReadOnlyList<T>
{
    private T[] _items;

    // 逻辑长度
    private int _length;

    public int Length
    {
        get => _length;
        set
        {
            if (value >= _length)
            {
                EnsureCapacity(value);
                _length = value;
                return;
            }

            CutLength(value);
        }
    }

    public int Capacity => _items.Length;

    public DenseBuffer(int capacity = 4)
    {
        _items = new T[capacity];
    }

    public T this[int index]
    {
        get
        {
            if (index >= Length) throw new IndexOutOfRangeException();
            return _items[index];
        }
        internal set
        {
            if (index >= Length) throw new IndexOutOfRangeException();
            _items[index] = value;
        }
    }

    private void CutLength(int index)
    {
        if (index < 0 || index >= _length) throw new IndexOutOfRangeException();
        for (var i = index; i < _length; i++) _items[i] = default!; // 断掉断掉引用，用于垃圾回收，报空指针异常比错误赋值好
        _length = index;
    }

    public void Clear()
    {
        CutLength(0);
    }

    public void EnsureCapacity(int capacity)
    {
        if (capacity <= _items.Length) return;
        var newCap = Math.Max(capacity, _items.Length * 2);
        Array.Resize(ref _items, newCap);
    }

    public int Count => Length;

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Length; i++)
            yield return _items[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public DenseBuffer<T> Copy(int newLen)
    {
        var newBuf = new DenseBuffer<T>(Math.Max(newLen, _items.Length));
        newBuf._length = _length;
        Array.Copy(_items, newBuf._items, _length);
        return newBuf;
    }
}

/// <summary>
/// A reactive signal that maintains a mapped array derived from a source signal.
/// Uses incremental array diffing to minimize re-creation of mapped items.
/// Supports key-based identity tracking and per-item disposal.
/// </summary>
/// <typeparam name="TItem">The source element type</typeparam>
/// <typeparam name="TMap">The mapped result type</typeparam>
/// <typeparam name="TKey">The key type used for identity tracking</typeparam>
public class ArrayMapMemo<TItem, TMap, TKey> :
    Signal<IReadOnlyList<TMap>>
    where TItem : notnull where TKey : notnull
{
    /// <summary>
    /// Indicates whether this memo
    /// has been disposed.
    /// </summary>
    public bool IsInvalid { get; private set; }

    private readonly Scope _scope;

    private readonly Accessor<IReadOnlyList<TItem>> _sourceListSignal;
    private IReadOnlyList<TItem> _oldList = [];
    private readonly DenseBuffer<Action> _disposers = []; // _oldList的平行数组
    private readonly DenseBuffer<MutSignal<int>>? _indexSignals; // // _oldList的平行数组

    private readonly Func<TItem, TMap>? _mapFn;
    private readonly Func<TItem, ISignal<int>, TMap>? _mapFnWithIndex;
    private readonly Func<TItem, TKey>? _keyFn;

    /// <summary>
    /// Creates a reactive mapped array that incrementally
    /// reconciles items when the source list changes.
    ///
    /// Existing mapped items are preserved whenever possible,
    /// allowing per-item state and scopes to survive reordering.
    /// </summary>
    /// <param name="sourceListSignal">
    /// The source list accessor.
    /// Changes to this list trigger incremental reconciliation.
    /// </param>
    /// <param name="mapFn">
    /// Maps a source item to a result item.
    /// Each mapped item is created inside its own scope.
    /// </param>
    /// <param name="keyFn">
    /// Optional key selector used to preserve item identity
    /// across insertions, removals, and reordering.
    /// If omitted, items are matched using default equality.
    /// </param>
    /// <example>
    /// <code>
    /// var users = new MutSignal&lt;IReadOnlyList&lt;User&gt;&gt;(
    /// [
    ///     new User(1, "Alice"),
    ///     new User(2, "Bob")
    /// ]);
    ///
    /// var rows = new ArrayMapMemo&lt;User, UserRow, int&gt;(
    ///     users,
    ///     user => new UserRow(user),
    ///     user => user.Id
    /// );
    ///
    /// // Reorder items
    /// users.RxValue =
    /// [
    ///     new User(2, "Bob"),
    ///     new User(1, "Alice")
    /// ];
    ///
    /// // Existing UserRow instances are preserved.
    /// // Only their positions are updated.
    /// </code>
    /// </example>
    public ArrayMapMemo(Accessor<IReadOnlyList<TItem>> sourceListSignal, Func<TItem, TMap>? mapFn,
        Func<TItem, TKey>? keyFn = null)
    {
        _sourceListSignal = sourceListSignal;
        _mapFn = mapFn;
        _keyFn = keyFn;
        _scope = _createSelfScope();
        InternalSignal = _createMapCache();
    }

    /// <summary>
    /// Creates a reactive mapped array that incrementally
    /// reconciles items when the source list changes and
    /// exposes a reactive index signal for each item.
    ///
    /// The index signal automatically updates when items
    /// are inserted, removed, or moved within the list.
    /// </summary>
    /// <param name="sourceListSignal">
    /// The source list accessor.
    /// Changes to this list trigger incremental reconciliation.
    /// </param>
    /// <param name="mapFnWithIndex">
    /// Maps a source item to a result item and provides
    /// a reactive signal representing the item's current index.
    /// </param>
    /// <param name="keyFn">
    /// Optional key selector used to preserve item identity
    /// across insertions, removals, and reordering.
    /// If omitted, items are matched using default equality.
    /// </param>
    /// <example>
    /// <code>
    /// var rows = new ArrayMapMemo&lt;User, UserRow, int&gt;(
    ///     users,
    ///     (user, index) =>
    ///         new UserRow(user, index),
    ///     user => user.Id
    /// );
    ///
    /// // Inside UserRow:
    /// new Effect(() =>
    /// {
    ///     Console.WriteLine(
    ///         $"{user.Name}: {index.RxValue}");
    /// });
    ///
    /// // When the list is reordered,
    /// // the index signal updates automatically.
    /// </code>
    /// </example>
    public ArrayMapMemo(Accessor<IReadOnlyList<TItem>> sourceListSignal,
        Func<TItem, ISignal<int>, TMap>? mapFnWithIndex,
        Func<TItem, TKey>? keyFn = null)
    {
        _sourceListSignal = sourceListSignal;
        _mapFnWithIndex = mapFnWithIndex;
        _keyFn = keyFn;
        _indexSignals = [];
        _scope = _createSelfScope();
        InternalSignal = _createMapCache();
    }

    private Scope _createSelfScope()
    {
        var current = NeHiveContext.CurrentScope;

        var scope = new Scope(current);
        scope.OnCleanup += () =>
        {
            IsInvalid = true;
            _disposeAll();
        };

        return scope;
    }

    private ComputedNode<IReadOnlyList<TMap>> _createMapCache()
    {
        var mapCache = _scope.RunInScope(() =>
            new ComputedNode<IReadOnlyList<TMap>>(
                _effectFn,
                comparator: Constant.EqualFn
            )
            {
                Phase = ExecutePhase.Resolved,
                Value = new DenseBuffer<TMap>()
            });
        mapCache.UpdateComputation();
        mapCache.Holder = new(this);

        return mapCache;
    }

    private void _disposeAll()
    {
        foreach (var disposer in _disposers) disposer();
    }

    private void _removeAll()
    {
        _disposeAll();
        _oldList = [];
        _disposers.Clear();
        _indexSignals?.Clear();
    }

    private TMap _mapper(IReadOnlyList<TItem> sourceList, int index)
    {
        var scope = new Scope(_scope);

        TMap result;

        using (new ReactiveContextHelper(scope, null))
        {
            if (_indexSignals is not null)
            {
                MutSignal<int> indexMutSignal = new(index);
                _indexSignals[index] = indexMutSignal;
                _disposers[index] = scope.Dispose;
                result = _mapFnWithIndex!.Invoke(sourceList[index], indexMutSignal);
            }
            else
            {
                _disposers[index] = scope.Dispose;
                result = _mapFn!.Invoke(sourceList[index]);
            }
        }

        return result;
    }

    private DenseBuffer<TMap> _effectFn(ITrack track, IReadOnlyList<TMap> oldMap)
    {
        var newList = _sourceListSignal.InternalSignal is null
            ? track.Track(_sourceListSignal.RxValueGetter)
            : track.Pull(_sourceListSignal.InternalSignal);

        var newLen = newList.Count;
        var oldLen = _oldList.Count;
        var maxLen = Math.Max(oldLen, newLen);
        DenseBuffer<TMap> newMap;
        if (newLen == 0 && oldLen != 0)
        {
            _removeAll();
            return new DenseBuffer<TMap>();
        }

        if (oldLen == 0)
        {
            newMap = new DenseBuffer<TMap>(newLen);
            _oldList = newList;
            newMap.Length = newLen;
            _disposers.Length = newLen;
            _indexSignals?.Length = newLen;

            for (var i = 0; i < newLen; i++)
            {
                newMap[i] = _mapper(newList, i);
            }

            return newMap;
        }

        var diff = _keyFn is null
            ? ArrayDiffUtil.ArrayDiff(newList, _oldList)
            : ArrayDiffUtil.ArrayDiff(newList, _oldList, _keyFn);
        foreach (var removeItemIndex in diff.RemoveItemsIndex)
        {
            _disposers[removeItemIndex]();
        }

        _disposers.Length = maxLen;
        _indexSignals?.Length = maxLen;
        newMap = ((DenseBuffer<TMap>)oldMap).Copy(maxLen);
        newMap.Length = maxLen;
        var tempMap = new TMap[maxLen];
        var tempDisposers = new Action?[maxLen];
        var tempIndex = _indexSignals is null ? null : new MutSignal<int>?[maxLen];

        foreach (var (oldIndex, newIndex) in diff.OldIndex2News)
        {
            var isOverlaid = tempDisposers[oldIndex] is not null; // 作为平行数组是通用的
            tempMap[newIndex] = newMap[newIndex];
            newMap[newIndex] = isOverlaid ? tempMap[oldIndex] : newMap[oldIndex];
            tempDisposers[newIndex] = _disposers[newIndex];
            _disposers[newIndex] = isOverlaid ? tempDisposers[oldIndex]! : _disposers[oldIndex];

            if (_indexSignals is null) continue;
            tempIndex![newIndex] = _indexSignals[newIndex];
            _indexSignals[newIndex] = isOverlaid ? tempIndex[oldIndex]! : _indexSignals[oldIndex];
            _indexSignals[newIndex].RxValue = newIndex; // 触发更新索引信号
        }

        foreach (var newItemIndex in diff.NewItemsIndex)
        {
            newMap[newItemIndex] = _mapper(newList, newItemIndex);
        }

        _oldList = [..newList];
        _disposers.Length = newLen;
        _indexSignals?.Length = newLen;
        newMap.Length = newLen;

        return newMap;
    }

    /// <summary>
    /// Disposes the memo and all mapped item scopes.
    /// </summary>
    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
        IsInvalid = true;
    }
}