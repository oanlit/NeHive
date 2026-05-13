using NeHive.Core;
using Avalonia.Controls;
using static NeHive.UI.Avalonia.Components.BaseComponent;

namespace NeHive.UI.Avalonia.Components;

public struct LoadingProp<TData>(AsyncMemo<TData> dataSource)
{
    public readonly AsyncMemo<TData> DataSource = dataSource;

    public required Func<TData, IElement> Success { get; init; }

    public Func<IElement>? Loading { get; init; }

    public Func<Exception, IElement>? Error { get; init; }
}

public static partial class ControlFlow
{
    private static IElement DefaultLoading()
        => HTextBlock(new("RxLoading..."));

    private static IElement DefaultError(Exception ex)
        => HTextBlock(new($"RxError: {ex.Message}"));

    private static Element LoadingComp<T>(LoadingProp<T> prop, UiScope uiScope) where T : notnull
    {
        var container = new Panel();

        // 使用 Effect 监听 AsyncMemo 的状态变化
        uiScope.CreateEffect(epochScope =>
        {
            var memo = prop.DataSource;
            var state = epochScope.Track(() => memo.RxState); // 追踪状态

            IElement? newChild;

            switch (state)
            {
                case AsyncMemoState.Pending:
                case AsyncMemoState.Refreshing:
                    var loadingContent = prop.Loading?.Invoke() ?? DefaultLoading();
                    newChild = loadingContent;
                    break;

                case AsyncMemoState.Ready:
                    try
                    {
                        var data = memo.RxValue!; // 就绪时取值（可能是信号，直接读当前值）
                        newChild = prop.Success(data);
                    }
                    catch (Exception ex)
                    {
                        // RxValue 可能抛出异常（如果底层错误），转为错误状态处理
                        newChild = prop.Error?.Invoke(ex) ?? DefaultError(ex);
                    }

                    break;

                case AsyncMemoState.Errored:
                    // 若 AsyncMemo 支持异常存储，此处可获取；这里简化为从 RxValue 的捕获异常
                    var exception = memo.RxError ?? new Exception("Unknown error");
                    newChild = prop.Error?.Invoke(exception) ?? DefaultError(exception);
                    break;

                default:
                    newChild = DefaultLoading();
                    break;
            }

            container.Children.Add(newChild.Content);
            epochScope.OnDispose(newChild.Dispose);
        });

        return new Element(uiScope, container);
    }

    public static IElement Loading<T>(LoadingProp<T> prop) where T : notnull
        => Element.WithScope(LoadingComp, prop);
}