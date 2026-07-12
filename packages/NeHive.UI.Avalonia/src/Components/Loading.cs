using NeHive.Reactive;
using Avalonia.Controls;
using NeHive.Model;
using static NeHive.UI.Avalonia.Components.BaseComponent;

namespace NeHive.UI.Avalonia.Components;

public struct LoadingProp<TData>(AsyncMemo<TData> dataSource)
{
    public readonly AsyncMemo<TData> DataSource = dataSource;

    public required Func<TData, IElement> Success { get; init; }

    public Func<TData?, IElement>? Loading { get; init; }

    public Func<Exception, IElement>? Error { get; init; }
}

public static partial class ControlFlow
{
    private static IElement DefaultLoading()
        => HTextBlock(new("RxLoading..."));

    private static IElement DefaultError(Exception ex)
        => HTextBlock(new($"RxError: {ex.Message}"));

    public static IElement Loading<T>(LoadingProp<T> prop)
    {
        return Element.WithScope(uiScope =>
        {
            var container = new Panel();

            // 使用 Effect 监听 AsyncMemo 的状态变化
            uiScope.CreateEffect(epochScope =>
            {
                var memo = prop.DataSource;
                var state = epochScope.Track(() => memo.RxState); // 追踪状态

                IElement? newChild;
                T? data = default;

                switch (state)
                {
                    case AsyncMemoState.Pending:
                    case AsyncMemoState.Refreshing:
                        using (new ScopeFrame(uiScope))
                        {
                            var loadingContent = prop.Loading?.Invoke(data) ?? DefaultLoading();
                            newChild = loadingContent;
                            _ = newChild.Content;
                        }

                        break;

                    case AsyncMemoState.Ready:
                        try
                        {
                            data = memo.RxValue!; // 就绪时取值（可能是信号，直接读当前值）
                            using (new ScopeFrame(uiScope))
                            {
                                newChild = prop.Success(data);
                                _ = newChild.Content;
                            }
                        }
                        catch (Exception ex)
                        {
                            // RxValue 可能抛出异常（如果底层错误），转为错误状态处理
                            using (new ScopeFrame(uiScope))
                            {
                                newChild = prop.Error?.Invoke(ex) ?? DefaultError(ex);
                                _ = newChild.Content;
                            }
                        }

                        break;

                    case AsyncMemoState.Errored:
                        var exception = memo.RxError ?? new Exception("Unknown error");
                        newChild = prop.Error?.Invoke(exception) ?? DefaultError(exception);
                        break;

                    default:
                        newChild = DefaultLoading();
                        _ = newChild.Content;
                        break;
                }

                container.Children.Add(newChild.Content);
                epochScope.OnCleanup += newChild.Dispose;
            });

            return container;
        });
    }
}