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
    #region Global Shared StrStyle Constants

    /// <summary>Base style for all card containers</summary>
    private const string DemoCardBase = """
                                        m-2 p-4 w-full border-w-2 bg-matcha-50 border-matcha-200 rounded-xl shadow-md
                                        hover:border-matcha-400 hover:shadow-lg 
                                        """;

    private const string DemoTitle = "mb-1 text-2xl fw-bold fg-matcha-800 selection:bg-matcha-300 tracking-tight ";
    private const string DemoDesc = "mb-2 text-sm fg-coffee-700 selection:bg-coffee-200 ";
    private const string DemoContent = "p-4 bg-white rounded-xl border border-matcha-100";

    /// <summary>Vertical stack layout base</summary>
    private const string VerticalStackBase = "gap-4 vertical items-start w-full ";

    /// <summary>Horizontal row layout base with wrap support</summary>
    private const string HorizontalRowBase = "gap-3 horizontal items-center ";

    /// <summary>Primary button base style</summary>
    private const string PrimaryBtnBase = @"px-3 py-1.5 fg-white bg-matcha-400 border border-matcha-500 rounded-lg 
hover:bg-matcha-500 click:bg-matcha-600 transition-transform duration-100 click:scale-95 ";

    /// <summary>Secondary button base style</summary>
    private const string SecondaryBtnBase = @"px-3 py-1.5 fg-white bg-coffee-400 border border-coffee-500 rounded-lg 
hover:bg-coffee-500 click:bg-coffee-700 transition-transform duration-100 click:scale-95 ";

    /// <summary>Text input universal base style</summary>
    private const string InputBaseStyle = """
                                          w-full p-2.5 bg-coffee-50 rounded-lg border border-matcha-200
                                          hover:cursor-text hover:border-matcha-300
                                          focus:border-matcha-500 focus:ring-w-2 focus:ring-matcha-200 selection:bg-matcha-300 
                                          transition-colors duration-200 
                                          """;

    /// <summary>Scrollable container base style</summary>
    private const string ScrollContainerBase = "p-3 bg-matcha-50 border border-matcha-200 rounded-xl overflow-hidden ";

    /// <summary>Section header text unified style</summary>
    private const string SectionTitleStyle = "text-xl fw-bold fg-matcha-700 mb-2 tracking-wide ";

    #endregion

    #region Counter Component

    private static IElement Counter(int id)
    {
        var count = new MutSignal<int>(0);
        var countText = () => $"Count: {count.RxValue}";

        return HStackPanel(_ => new(strStyle: DemoCardBase + " w-64 gap-3 vertical")
        {
            HText($"Counter Instance #{id}",
                strStyle: "text-lg fw-semibold fg-matcha-700"
            ), // HText
            HText(countText,
                strStyle: "text-4xl fw-bold fg-matcha-600 leading-10"
            ), // HText
            HStackPanel(_ => new(strStyle: HorizontalRowBase)
            {
                HButton("+1",
                    strStyle: PrimaryBtnBase,
                    onClick: _ => count.RxValue++
                ), // HButton
                HButton("-1",
                    strStyle: SecondaryBtnBase,
                    onClick: _ => count.RxValue--
                ) // HButton
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    private static IElement DemoSection(string title, IEnumerable<IElement> children)
    {
        var stackPanelArgs = new HStackPanelArgs(strStyle: "gap-2 vertical pl-2");
        foreach (var child in children) stackPanelArgs.Add(child);

        return HStackPanel(_ => new(strStyle: "gap-3 vertical")
        {
            HText(title, strStyle: SectionTitleStyle + "pl-4 py-1 fg-lime-700 border-l-4 border-lime-700"),
            HStackPanel(_ => stackPanelArgs)
        }); // HStackPanel
    }

}
