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
        => HText("RxLoading...");

    private static IElement DefaultError(Exception ex)
        => HText($"RxError: {ex.Message}");

    public static IElement Loading<T>(LoadingProp<T> prop)
    {
        return Element.WithScope(uiScope =>
        {
            var container = new Panel();

            uiScope.CreateEffect(epochScope =>
            {
                var memo = prop.DataSource;
                var state = epochScope.Track(() => memo.RxState);

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
                            data = memo.RxValue!;
                            using (new ScopeFrame(uiScope))
                            {
                                newChild = prop.Success(data);
                                _ = newChild.Content;
                            }
                        }
                        catch (Exception ex)
                        {
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