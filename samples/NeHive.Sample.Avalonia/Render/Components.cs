using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NeHive.Core;

namespace NeHive.Sample.Avalonia.Render;

public static class Components
{
    public static readonly Component Empty = new(() => Element.Empty);

    public struct ShowProp(Accessor<bool> when)
    {
        public required Component Children;
        public readonly Accessor<bool> When = when;
    }

    private static readonly Component<ShowProp> CompShow = new((prop, uiScope) =>
    {
        var panel = new Panel();

        uiScope.AddEffect(epochScope =>
        {
            var when = epochScope.Track(prop.When);

            if (!when)
                return;

            var child = prop.Children.Create();
            panel.Children.Add(child.Content);
            epochScope.OnDispose(child.Dispose);
        });

        return new Element(uiScope, panel);
    });

    public static IElement Show(ShowProp prop)
        => CompShow.Create(prop);

    public static IElement ForEach<T>(Components<T>.ForEachProp<T> prop) where T : notnull
        => Components<T>.ForEach.Create(prop);

    public struct HStackPanelProp(IEnumerable<IElement>? children = null)
    {
        public IEnumerable<IElement> Children = children ?? [];
    }

    private static readonly Component<HStackPanelProp> CompStackPanel = new((prop, uiScope) =>
    {
        var stack = new StackPanel();
        foreach (var el in prop.Children)
        {
            stack.Children.Add(el.Content);
        }

        return new Element(uiScope, stack);
    });

    public static IElement HStackPanel(HStackPanelProp prop)
        => CompStackPanel.Create(prop);

    public class HTextBlockProp(Accessor<string>? text = null)
    {
        public readonly Accessor<string> Text = text ?? "";
    }

    private static readonly Component<HTextBlockProp> CompTextBlock = new((prop, uiScope) =>
    {
        var text = new TextBlock();
        uiScope.AddEffect(() => { text.Text = prop.Text.Value; });
        return new Element(uiScope, text);
    });

    public static IElement HTextBlock(HTextBlockProp prop)
        => CompTextBlock.Create(prop);

    public struct HButtonProp(Accessor<string>? text = null, Action<RoutedEventArgs>? click = null)
    {
        public readonly Accessor<string> Text = text ?? "";
        public readonly Action<RoutedEventArgs>? Click = click;
    }

    public class HButtonExpose
    {
        public Action<RoutedEventArgs> Click = _ => { };
    }

    private static readonly Component<HButtonProp, HButtonExpose> CompButton = new((prop, uiScope) =>
    {
        // 创建基础视觉元素
        var border = new Border
        {
            // 设置默认样式
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = Brushes.LightGray,
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 4)
        };

        var textBlock = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        border.Child = textBlock;

        // 响应式更新文本
        uiScope.AddEffect(() => textBlock.Text = prop.Text.Value);

        var expose = new HButtonExpose();

        // 鼠标交互状态（悬停、按下等）
        var isPressed = false;

        // 统一触发点击的方法
        void RaiseClick()
        {
            var args = new RoutedEventArgs(Button.ClickEvent);
            prop.Click?.Invoke(args);
            expose.Click.Invoke(args);
        }

        // 事件挂载
        uiScope.OnMount(() =>
        {
            border.PointerPressed += (_, e) =>
            {
                if (!e.GetCurrentPoint(border).Properties.IsLeftButtonPressed)
                    return;

                isPressed = true;
                border.Background = Brushes.DarkGray; // 按下效果
                e.Handled = true;
            };

            border.PointerReleased += (_, e) =>
            {
                if (isPressed && border.IsPointerOver)
                {
                    RaiseClick();
                }

                isPressed = false;
                border.Background = Brushes.LightGray; // 恢复
                e.Handled = true;
            };

            border.PointerExited += (_, _) =>
            {
                isPressed = false; // 移出区域时取消按下状态
                border.Background = Brushes.LightGray;
            };

            border.PointerEntered += (_, _) =>
            {
                if (!isPressed)
                    border.Background = Brushes.LightGray; // 恢复（可选悬停变色）
            };
        });

        uiScope.OnDispose(() =>
        {
            // 清理事件（可选，因为控件生命周期通常由框架管理）
        });

        return new Element<HButtonExpose>(uiScope, border, expose);
    });

    public static IElement<HButtonExpose> HButton(HButtonProp prop)
        => CompButton.Create(prop);

    public static IElement<HButtonExpose> HButton(HButtonProp prop, out IElement<HButtonExpose> expose)
        => CompButton.Create(prop, out expose);
}

public static class Components<T> where T : notnull
{
    public class ForEachProp<T1>(Accessor<IReadOnlyList<T1>> each)
    {
        public required Func<T1,IElement> Children;
        public readonly Accessor<IReadOnlyList<T1>> Each = each;
    }

    public static readonly Component<ForEachProp<T>> ForEach = new((prop, uiScope) =>
    {
        var panel = new StackPanel();

        // 用 ArrayMapMemo 做“数据层 diff + 生命周期管理”
        var memo = new ArrayMapMemo<T, IElement, T>(prop.Each, prop.Children);

        uiScope.AddEffect(epochScope =>
        {
            var list = epochScope.Track(memo);

            // —— UI 最小更新（核心）——
            for (var i = 0; i < list.Count; i++)
            {
                var childrenContent = list[i].Content;

                if (i >= panel.Children.Count)
                {
                    // 追加
                    panel.Children.Add(childrenContent);
                }
                else if (!ReferenceEquals(panel.Children[i], childrenContent))
                {
                    // 位置不一致 → 移动（或替换）
                    panel.Children.RemoveAt(i);
                    panel.Children.Insert(i, childrenContent);
                }
            }
        });

        uiScope.OnDispose(memo.Dispose);

        return new Element(uiScope, panel);
    });
}