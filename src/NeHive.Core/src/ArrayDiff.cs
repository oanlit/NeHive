namespace NeHive.Core;

using System.Collections;

public struct ArrayDiffResult
{
    public List<int> RemoveItemsIndex;
    public List<(int OldIndex, int NewIndex)> OldIndex2News;
    public List<int> NewItemsIndex;
}

public static class ArrayDiffUtil
{
    public static ArrayDiffResult ArrayDiff<T>(
        IReadOnlyList<T> newList,
        IReadOnlyList<T> oldList) where T : notnull
    {
        return ArrayDiffCore<T, T>(
            newList,
            oldList,
            item => item);
    }

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

            // var lastRepeatIndex = newIndices.TryGetValue(key, out var v)
            //     ? v
            //     : noIndex;
            var lastRepeatIndex = newIndices.GetValueOrDefault(key, noIndex);

            newIndicesNext[newIndex] = lastRepeatIndex;

            newIndices[key] = newIndex;
        }

        // scan old list
        for (var oldIndex = preIndex; oldIndex <= oldSufIndex; oldIndex++)
        {
            var key = keyFn(oldList[oldIndex]);

            var newIndex = newIndices.TryGetValue(key, out var v)
                ? v
                : noIndex;

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

public class ArrayMapResult<T, TU> where T : notnull
{
    public IReadOnlyList<T> SourceList
    {
        get => _sourceList;
        set => SetSourceList(value);
    }

    public IReadOnlyList<TU> MapList => _mapCache;
    private IReadOnlyList<T> _sourceList;
    private readonly List<TU> _mapCache;
    private readonly Func<T, TU> _mapFn;
    private readonly Action<TU>? _removeFn;
    private readonly Func<T, object>? _keyFn;

    public ArrayMapResult(
        T[] initSourceList,
        Func<T, TU> mapFn,
        Func<T, object>? keyFn = null,
        Action<TU>? removeFn = null)
    {
        _sourceList = initSourceList;
        _mapFn = mapFn;
        _removeFn = removeFn;
        _keyFn = keyFn;

        _mapCache = new List<TU>(initSourceList.Length);

        for (var i = 0; i < initSourceList.Length; i++)
        {
            _mapCache.Add(mapFn(initSourceList[i]));
        }
    }

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

        var tempMap = new TU[maxLen];
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

public static partial class Reactive
{
}

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

public class ArrayMapMemo<TItem, TMap, TKey> : IReadOnlySignal<IReadOnlyList<TMap>>
    where TItem : notnull where TKey : notnull
{
    public IReadOnlyList<TMap> Value => _mapCache.ReadSignal();
    public IReadOnlyList<TMap> UntrackValue => _mapCache.UntrackValue;

    public bool IsInvalid { get; private set; }
    private readonly ScopeNode _scope;
    private readonly MemoNode<DenseBuffer<TMap>> _mapCache;
    private readonly IReadOnlySignal<IReadOnlyList<TItem>> _sourceListSignal;
    private IReadOnlyList<TItem> _oldList = [];
    private readonly DenseBuffer<Action> _disposers = []; // _oldList的平行数组
    private readonly DenseBuffer<Signal<int>>? _indexSignals; // // _oldList的平行数组

    private readonly Func<TItem, TMap>? _mapFn;
    private readonly Func<TItem, IReadOnlySignal<int>, TMap>? _mapFnWithIndex;
    private readonly Func<TItem, TKey>? _keyFn;

    public ArrayMapMemo(IReadOnlySignal<IReadOnlyList<TItem>> sourceListSignal, Func<TItem, TMap>? mapFn,
        Func<TItem, TKey>? keyFn = null)
    {
        _sourceListSignal = sourceListSignal;
        _mapFn = mapFn;
        _keyFn = keyFn;
        _scope = _createSelfScope();
        _mapCache = _createMapCache();
    }

    public ArrayMapMemo(IReadOnlySignal<IReadOnlyList<TItem>> sourceListSignal,
        Func<TItem, IReadOnlySignal<int>, TMap>? mapFnWithIndex,
        Func<TItem, TKey>? keyFn = null)
    {
        _sourceListSignal = sourceListSignal;
        _mapFnWithIndex = mapFnWithIndex;
        _keyFn = keyFn;
        _indexSignals = [];
        _scope = _createSelfScope();
        _mapCache = _createMapCache();
    }

    private ScopeNode _createSelfScope()
    {
        var current = ReactiveContext.CurrentScope;

        var scope = new ScopeNode(
            parent: current,
            children: null,
            context: current.Context,
            cleanups: null
        );
        scope.Cleanups.Add(() =>
        {
            IsInvalid = true;
            _disposeAll();
        });

        return scope;
    }
    
    private MemoNode<DenseBuffer<TMap>> _createMapCache()
    {
        var mapCache = ComputationNode.RunInScope(_scope, () =>
            new MemoNode<DenseBuffer<TMap>>(
                _effectFn,
                comparator: Constant.EqualFn,
                isPure: true
            )
            {
                Phase = ComputationPhase.Resolved,
                Value = []
            });
        mapCache.UpdateComputation();

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
        var scope = new ScopeNode(
            parent: _scope,
            children: null,
            context: _scope.Context,
            cleanups: null
        );

        var result = ComputationNode.RunInScope(scope, () =>
        {
            if (_indexSignals is not null)
            {
                Signal<int> indexSignal = new(index);
                _indexSignals[index] = indexSignal;
                _disposers[index] = scope.Dispose;
                return _mapFnWithIndex!.Invoke(sourceList[index], indexSignal);
            }

            _disposers[index] = scope.Dispose;
            return _mapFn!.Invoke(sourceList[index]);
        });
        return result;
    }

    private DenseBuffer<TMap> _effectFn(DenseBuffer<TMap> oldMap)
    {
        var newList = _sourceListSignal.Value;
        return Reactive.Untrack(() =>
        {
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
            newMap = oldMap.Copy(maxLen);
            newMap.Length = maxLen;
            var tempMap = new TMap[maxLen];
            var tempDisposers = new Action?[maxLen];
            var tempIndex = _indexSignals is null ? null : new Signal<int>?[maxLen];

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
                _indexSignals[newIndex].Value = newIndex; // 触发更新索引信号
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
        });
    }

    public void Dispose()
    {
        if (IsInvalid) return;
        _scope.Dispose();
        IsInvalid = true;
    }
}