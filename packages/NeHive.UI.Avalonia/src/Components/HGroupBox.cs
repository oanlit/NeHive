// using System.Collections;
//
// using Avalonia.Controls;
// using Avalonia.Controls.Primitives;
// using NeHive.Reactive;
//
// namespace NeHive.UI.Avalonia.Components;
//
// /// <summary>
// /// GroupBox 配置类
// /// </summary>
// public class HGroupBoxProp(
//     Accessor<string>? strStyle = null,
//     Accessor<HPanelStyle>? style = null,
//     Accessor<string>? header = null,           // 文本标题（响应式）
//     IElement? headerContent = null             // 或使用自定义控件作为标题
// ) : IEnumerable<IElement>
// {
//     private readonly List<IElement> _children = [];
//
//     // 合并后的样式
//     public readonly Accessor<HPanelStyle>? Style = (style, strStyle) switch
//     {
//         (not null, not null) => new Computed<HPanelStyle>(() =>
//             HPanelStyle.Parse(strStyle).RxValue.Merge(style.RxValue)),
//         (not null, _) => style,
//         (_, not null) => HPanelStyle.Parse(strStyle),
//         _ => null
//     };
//
//     public Accessor<string>? Header { get; } = header;
//     public IElement? HeaderContent { get; } = headerContent;
//
//     // 添加子内容（GroupBox 通常只有一个子元素作为内容，但支持多个时自动包裹）
//     public void Add(IElement element) => _children.Add(element);
//     public IEnumerator<IElement> GetEnumerator() => _children.GetEnumerator();
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
// }
//
// public static partial class BaseComponent
// {
//     /// <summary>
//     /// 创建 GroupBox 组件
//     /// </summary>
//     public static IElement<global::Avalonia.Controls.GroupBox> HGroupBox(HGroupBoxProp prop)
//     {
//         var uiScope = new UiScope();
//         var groupBox = new GroupBox();
//
//         // 应用样式
//         if (prop.Style != null)
//         {
//             uiScope.CreateEffect(scope =>
//             {
//                 var style = scope.Track(prop.Style);
//                 ApplyPanelStyle(groupBox, style);
//             });
//         }
//
//         // 设置标题
//         if (prop.Header != null)
//             uiScope.CreateEffect(() => groupBox.Header = prop.Header.RxValue);
//         else if (prop.HeaderContent != null)
//             groupBox.Header = prop.HeaderContent.Content;
//
//         // 设置内容：若只有一个子元素则直接作为 Content，否则用 StackPanel 包裹
//         var children = prop.ToList();
//         if (children.Count == 0)
//         {
//             groupBox.Content = null;
//         }
//         else if (children.Count == 1)
//         {
//             groupBox.Content = children[0].Content;
//         }
//         else
//         {
//             var stack = new StackPanel { Spacing = 4 };
//             foreach (var child in children)
//                 stack.Children.Add(child.Content);
//             groupBox.Content = stack;
//         }
//
//         return new Element<GroupBox>(uiScope, groupBox, groupBox);
//         
//         void ApplyPanelStyle(Control control, HPanelStyle style)
//         {
//             control.Margin = style.Margin;
//             control.HorizontalAlignment = style.HorizontalAlignment;
//             control.VerticalAlignment = style.VerticalAlignment;
//             // control.Background = style.Background;
//             // 可根据需要扩展更多属性（如 BorderBrush, BorderThickness 等）
//         }
//     }
// }