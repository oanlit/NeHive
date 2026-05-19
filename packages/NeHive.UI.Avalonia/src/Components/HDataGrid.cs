// using System.Collections;
// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Layout;
// using NeHive.Reactive;
// using static NeHive.UI.Avalonia.Components.BaseComponent;
//
// namespace NeHive.UI.Avalonia.Components;
//
// // 列定义基类（用户不需要直接使用）
// public abstract class HDataGridColumn
// {
//     public Accessor<string> Header { get; set; } = "";
//     public Accessor<bool> IsReadOnly { get; set; } = false;
//     public Accessor<double?> Width { get; set; } = null;
// }
//
// // 文本列
// public class HDataGridTextColumn : HDataGridColumn
// {
//     public Accessor<string> BindingPath { get; set; } = "";
// }
//
// // 模板列（允许自定义显示内容，接收行数据对象）
// public class HDataGridTemplateColumn : HDataGridColumn
// {
//     public Func<object, IElement> CellTemplate { get; set; } = _ => HTextBlock("");
// }
//
// /// <summary>
// /// DataGrid 配置类（泛型，T 为行数据类型）
// /// </summary>
// /// <typeparam name="T">行数据项类型</typeparam>
// public class HDataGridProp<T>(
//     Accessor<double>? width = null,
//     Accessor<double>? height = null,
//     Accessor<Thickness>? margin = null,
//     Accessor<HorizontalAlignment>? horizontalAlignment = null,
//     Accessor<VerticalAlignment>? verticalAlignment = null,
//     Accessor<bool>? canUserReorderColumns = null,
//     Accessor<bool>? canUserResizeColumns = null,
//     Accessor<bool>? canUserSortColumns = null,
//     Accessor<SelectionMode>? selectionMode = null
// ) : IEnumerable<HDataGridColumn>
//     where T : class
// {
//     private readonly List<HDataGridColumn> _columns = [];
//
//     public readonly Accessor<double>? Width = width;
//     public readonly Accessor<double>? Height = height;
//     public readonly Accessor<Thickness>? Margin = margin;
//     public readonly Accessor<HorizontalAlignment> HorizontalAlignment =
//         horizontalAlignment ?? global::Avalonia.Layout.HorizontalAlignment.Stretch;
//     public readonly Accessor<VerticalAlignment> VerticalAlignment =
//         verticalAlignment ?? global::Avalonia.Layout.VerticalAlignment.Stretch;
//     public readonly Accessor<bool>? CanUserReorderColumns = canUserReorderColumns;
//     public readonly Accessor<bool>? CanUserResizeColumns = canUserResizeColumns;
//     public readonly Accessor<bool>? CanUserSortColumns = canUserSortColumns;
//     public readonly Accessor<SelectionMode>? SelectionMode = selectionMode;
//
//     // 数据源
//     public Accessor<IEnumerable<T>>? ItemsSource { get; set; }
//
//     // 选中项（可选双向绑定）
//     public MutSignal<IEnumerable<T>>? SelectedItems { get; set; }
//     public MutSignal<T?>? BindSelectedItem { get; set; }
//
//     // 列索引器
//     public HDataGridColumn this[HDataGridColumn column]
//     {
//         set => _columns.Add(column);
//     }
//
//     // 集合初始化器支持
//     public void Add(HDataGridColumn column) => _columns.Add(column);
//
//     public IEnumerator<HDataGridColumn> GetEnumerator() => _columns.GetEnumerator();
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
// }
//
// public static partial class BaseComponent
// {
//     private static class DataGridInternal<T> where T : class
//     {
//         public static readonly Component<HDataGridProp<T>> Comp = new((prop, uiScope) =>
//         {
//             var grid = new DataGrid();
//
//             // 应用布局属性
//             uiScope.CreateEffect(() =>
//             {
//                 if (prop.Width is not null) grid.Width = prop.Width.RxValue;
//                 if (prop.Height is not null) grid.Height = prop.Height.RxValue;
//                 if (prop.Margin is not null) grid.Margin = prop.Margin.RxValue;
//                 grid.HorizontalAlignment = prop.HorizontalAlignment.RxValue;
//                 grid.VerticalAlignment = prop.VerticalAlignment.RxValue;
//             });
//
//             // 可选属性
//             if (prop.CanUserReorderColumns is not null)
//                 uiScope.CreateEffect(() => grid.CanUserReorderColumns = prop.CanUserReorderColumns.RxValue);
//             if (prop.CanUserResizeColumns is not null)
//                 uiScope.CreateEffect(() => grid.CanUserResizeColumns = prop.CanUserResizeColumns.RxValue);
//             if (prop.CanUserSortColumns is not null)
//                 uiScope.CreateEffect(() => grid.CanUserSortColumns = prop.CanUserSortColumns.RxValue);
//             if (prop.SelectionMode is not null)
//                 uiScope.CreateEffect(() => grid.SelectionMode = prop.SelectionMode.RxValue);
//
//             // 动态构建列
//             uiScope.CreateEffect(() =>
//             {
//                 grid.Columns.Clear();
//                 foreach (var colDef in prop)
//                 {
//                     DataGridColumn? column = null;
//                     switch (colDef)
//                     {
//                         case HDataGridTextColumn textCol:
//                             column = new DataGridTextColumn
//                             {
//                                 Binding = new Avalonia.Data.Binding(textCol.BindingPath.RxValue),
//                                 Header = textCol.Header.RxValue,
//                                 IsReadOnly = textCol.IsReadOnly.RxValue,
//                                 Width = textCol.Width?.RxValue ?? DataGridLength.Auto
//                             };
//                             break;
//                         case HDataGridTemplateColumn templateCol:
//                             // 注意：DataGridTemplateColumn 需要定义 CellTemplate，但 Avalonia 中需要 DataTemplate
//                             // 这里简化实现：使用一个自定义的列类型，动态生成内容
//                             // 为保持简洁，此处先抛出异常，建议用户使用 TextColumn 或后续扩展
//                             throw new NotImplementedException("TemplateColumn 需要额外封装，建议使用 TextColumn 或等待扩展");
//                     }
//
//                     if (column != null)
//                         grid.Columns.Add(column);
//                 }
//             });
//
//             // 绑定数据源
//             if (prop.ItemsSource is not null)
//                 uiScope.CreateEffect(() => grid.ItemsSource = prop.ItemsSource.RxValue);
//
//             // 双向绑定选中项
//             if (prop.BindSelectedItem is not null)
//             {
//                 // 信号 -> 控件
//                 uiScope.CreateEffect(() =>
//                 {
//                     var selected = prop.BindSelectedItem.RxValue;
//                     if (selected is not null && grid.BindSelectedItem != selected)
//                         grid.BindSelectedItem = selected;
//                 });
//                 // 控件 -> 信号
//                 grid.SelectionChanged += (_, _) =>
//                 {
//                     var newSelected = grid.BindSelectedItem as T;
//                     if (newSelected != prop.BindSelectedItem.RxValue)
//                         prop.BindSelectedItem.RxValue = newSelected;
//                 };
//             }
//
//             if (prop.SelectedItems is not null)
//             {
//                 uiScope.CreateEffect(() =>
//                 {
//                     var selectedSet = new HashSet<T>(prop.SelectedItems.RxValue ?? Enumerable.Empty<T>());
//                     foreach (var item in grid.SelectedItems.OfType<T>().ToList())
//                     {
//                         if (!selectedSet.Contains(item))
//                             grid.SelectedItems.Remove(item);
//                     }
//                     foreach (var item in selectedSet)
//                     {
//                         if (!grid.SelectedItems.Contains(item))
//                             grid.SelectedItems.Add(item);
//                     }
//                 });
//                 grid.SelectionChanged += (_, _) =>
//                 {
//                     var selected = grid.SelectedItems.OfType<T>().ToList();
//                     prop.SelectedItems.RxValue = selected;
//                 };
//             }
//
//             return new Element(uiScope, grid);
//         });
//     }
//
//     /// <summary>
//     /// 创建 DataGrid (表格) 组件
//     /// </summary>
//     public static IElement HDataGrid<T>(HDataGridProp<T> prop) where T : class
//         => DataGridInternal<T>.Comp.Create(prop);
// }