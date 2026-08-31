using System.Text;
using NeHive.Model;
using NeHive.UI.Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using NeHive.Reactive;
using NeHive.UI.Avalonia.Components;
using static NeHive.UI.Avalonia.Components.BaseComponent;
using static NeHive.UI.Avalonia.Components.ControlFlow;
using static NeHive.UI.Avalonia.Components.AttachComponent;

namespace NeHive.Sample.Avalonia;

public static partial class DemoComponent
{
    #region TreeView Hierarchical Tree Demo

    private static IElement TreeViewDemo()
    {
        return HStackPanel(_ => new(strStyle: VerticalStackBase)
        {
            HText("TreeView Hierarchical Data Tree Demo", strStyle: SectionTitleStyle),
            HTreeView(new(strStyle: DemoCardBase + "w-80 h-96 overflow-hidden")
            {
                new HTreeViewItemArgs("Root Directory 1",
                    strStyle: "p-1 rounded hover:bg-gray-100 transition-colors")
                {
                    Children =
                    {
                        new HTreeViewItemArgs("Sub Folder 1.1", strStyle: "p-1 hover:bg-gray-50 rounded"),
                        new HTreeViewItemArgs("Sub Folder 1.2", strStyle: "p-1 hover:bg-gray-50 rounded")
                    } // HTreeViewItemArgs.Children
                }, // HTreeViewProp
                new HTreeViewItemArgs("Root Directory 2", isExpanded: true,
                    strStyle: "p-1 rounded hover:bg-gray-100 transition-colors")
                {
                    Children =
                    {
                        new HTreeViewItemArgs("Sub Folder 2.1", strStyle: "p-1 hover:bg-gray-50 rounded")
                    } // HTreeViewProp.Children
                } // HTreeViewProp
            }) // HTreeView
        }); // HStackPanel
    }

    #endregion

    #region ComboBox Dropdown Select Demo

    private static IElement ComboBoxDemo()
    {
        var countries = new MutSignal<IReadOnlyList<Country>>([
            new Country { Name = "China", Code = "CN" },
            new Country { Name = "United States", Code = "US" },
            new Country { Name = "Japan", Code = "JP" }
        ]);

        var selectedCountry = new MutSignal<Country?>(null);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("ComboBox Dropdown Selection List Demo", strStyle: SectionTitleStyle),
            HComboBox<Country>(_ => new(
                countries,
                bindSelectedItem: selectedCountry,
                strStyle: InputBaseStyle + "w-64"
            )
            {
                ItemsPanel = HStackPanel(
                    strStyle: "w-full h-full vertical bg-matcha-100 border border-matcha-300 rounded-lg"),
                ItemTemplate = c =>
                    HText($"{c.Name} (Region Code: {c.Code})",
                        strStyle: "w-full p-2 fw-medium fg-matcha-700 hover:bg-matcha-200"),
                SelectionBoxItemTemplate = data => Show(new(new(() => data.RxValue is not null))
                {
                    IfTrue = () => HText(new(() => $"You Select : {data.RxValue!.Name}"),
                        strStyle: "fg-coffee-700 text-base"),
                    IfFalse = () => HText("Please select a country", strStyle: "fg-coffee-200 text-base")
                }) // HComboBox<Country>.SelectionBoxItemTemplate
            }), // HComboBox<Country>
            HText(new(() => $"Selected Region: {selectedCountry.RxValue?.Code ?? "Nothing selected"}"),
                strStyle: "mt-2 fw-medium fg-matcha-600")
        }); // HStackPanel
    }

    #endregion

}
