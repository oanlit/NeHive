using NeHive.Core;
using Avalonia.Controls;
using static NeHive.Sample.Avalonia.Render.Components.Base;

namespace NeHive.Sample.Avalonia.Render.Components;

public static partial class Base
{
    public struct LoadingProp<TData>(AsyncMemo<TData> dataSource)
    {
        public readonly AsyncMemo<TData> DataSource = dataSource;

        public required Func<TData, IElement> Success { get; init; }

        public Func<IElement>? Loading { get; init; }

        public Func<Exception, IElement>? Error { get; init; }
    }

    public static IElement Loading<T>(LoadingProp<T> prop) where T : notnull
        => Base<T>.CompLoading.Create(prop);
}

public static partial class Base<T> where T : notnull
{
    private static IElement DefaultLoading()
        => HTextBlock(new("Loading..."));

    private static IElement DefaultError(Exception ex)
        => HTextBlock(new($"Error: {ex.Message}"));

    internal static readonly Component<LoadingProp<T>> CompLoading = new((prop, uiScope) =>
    {
        var container = new Panel();

        // 使用 Effect 监听 AsyncMemo 的状态变化
        uiScope.AddEffect(epochScope =>
        {
            var memo = prop.DataSource;
            var state = epochScope.Track(() => memo.State); // 追踪状态

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
                        var data = memo.Value!; // 就绪时取值（可能是信号，直接读当前值）
                        newChild = prop.Success(data);
                    }
                    catch (Exception ex)
                    {
                        // Value 可能抛出异常（如果底层错误），转为错误状态处理
                        newChild = prop.Error?.Invoke(ex) ?? DefaultError(ex);
                    }

                    break;

                case AsyncMemoState.Errored:
                    // 若 AsyncMemo 支持异常存储，此处可获取；这里简化为从 Value 的捕获异常
                    var exception = memo.Error ?? new Exception("Unknown error");
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
    });
}