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
    #region TextBox Input Demo

    private static IElement TextBoxDemo()
    {
        var textSignal = new MutSignal<string?>("Default input text value");
        var log = new MutSignal<string?>("");

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("TextBox Text Input Control Demo", strStyle: SectionTitleStyle),
            HTextBox(
                bindText: textSignal,
                placeholderText: "Enter custom text content here...",
                strStyle: InputBaseStyle + "text-base focus:ring-offset-2",
                onTextChanging: _ => log.RxValue = $"Input content updated: {textSignal.Value}"
            ), // HTextBox
            HTextBox(
                bindText: textSignal,
                placeholderText: "Type something...",
                strStyle: InputBaseStyle + "text-base selection:fg-matcha-800",
                onTextChanged: _ => log.RxValue = $"Input content updated: {textSignal.Value}"
            ), // HTextBox
            HText(new(() => $"Realtime Bound Value: {textSignal.RxValue}"),
                strStyle: "mt-2 text-base fw-medium fg-gray-800"),
            HText(log, strStyle: "text-sm fg-gray-500 mt-1 italic")
        }); // HStackPanel
    }

    #endregion

    #region Command Demo

    private static IElement CommandDemo()
    {
        var count1 = new MutSignal<int>(0);
        var count2 = new MutSignal<int>(0);
        var count3 = new MutSignal<int>(0);
        var total = () => count1.RxValue + count2.RxValue + count3.RxValue;

        var increaseCommand = new NeHiveCommand<MutSignal<int>>(parameter: count1,
            canExecute: p => p.RxValue < 10 && total() < 15,
            execute: p => p.RxValue++
        );
        var decreaseCommand = new NeHiveCommand<MutSignal<int>>(parameter: count1,
            canExecute: p => p.RxValue > -10 && total() > -15,
            execute: p => p.RxValue--
        );

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Command Demo", strStyle: SectionTitleStyle),
            HText(new(() => $"Total Count {total()}"), strStyle: SectionTitleStyle),
            HStackPanel(_ => new(strStyle: VerticalStackBase)
            {
                Counter2(count1),
                Counter2(count2),
                Counter2(count3),
                HButton("Dispose Command", strStyle: PrimaryBtnBase, onClick: _ => DisposeCommand())
            }) // HStackPanel
        }); // HStackPanel;

        void DisposeCommand()
        {
            increaseCommand.Dispose();
            decreaseCommand.Dispose();
        }

        IElement Counter2(MutSignal<int> count)
        {
            return HStackPanel(_ => new(strStyle: HorizontalRowBase)
            {
                HButton("-1",
                    strStyle: SecondaryBtnBase + "disabled:bg-coffee-50 disabled:fg-coffee-400",
                    // onClick: _ => count.RxValue--,
                    command: new(decreaseCommand.WithParameter(count))
                ), // HButton
                HText(new(() => $"{count.RxValue}"), strStyle: "text-xl fw-bold fg-matcha-600"),
                HButton("+1",
                    strStyle: PrimaryBtnBase + "disabled:bg-coffee-50 disabled:fg-coffee-400",
                    // onClick: _ => count.RxValue++,
                    command: new(increaseCommand.WithParameter(count))
                ) // HButton
            }); // HStackPanel
        }
    }

    #endregion

    #region Async Command Progress Demo

    private static IElement AsyncCommandProgressDemo()
    {
        // var progress = new MutSignal<double>(0);
        var progress = new MutSignal<double>(0,
            onSet: (_, newValue, setter) => Dispatcher.UIThread.Post(() => setter(newValue)));
        var isIndeterminate = new MutSignal<bool>(false);

        var loadingCommand = new AsyncNeHiveCommand(
            canExecute: () => !isIndeterminate.RxValue,
            executeAsync: async () =>
            {
                for (var i = 0; i <= 100; i++)
                {
                    progress.RxValue = i;
                    await Task.Delay(50);
                }
            }
        );

        var resetCommand = new NeHiveCommand(
            canExecute: () => !isIndeterminate.RxValue && progress.RxValue != 0 && !loadingCommand.IsRunning.RxValue,
            execute: () => progress.RxValue = 0);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("AsyncNeHiveCommand & Progress Task Indicator Demo", strStyle: SectionTitleStyle),
            HProgressBar(value: progress, minimum: 0, maximum: 100, isIndeterminate: isIndeterminate,
                strStyle:
                "w-full h-4 overflow-hidden horizontal bg-coffee-200 fg-matcha-500 border-w-2 border-coffee-700 rounded-full"),
            HStackPanel(_ => new(strStyle: HorizontalRowBase + "mt-2")
            {
                HButton("Start Load", strStyle: PrimaryBtnBase + "disabled:bg-coffee-50 disabled:fg-coffee-400",
                    hotKey: new KeyGesture(Key.L, KeyModifiers.Control),
                    command: loadingCommand),
                HButton("Reset Progress",
                    strStyle: PrimaryBtnBase + "bg-matcha-300 disabled:bg-coffee-50 disabled:fg-coffee-400",
                    command: resetCommand),
                HButton("Switch Model", strStyle: SecondaryBtnBase,
                    onClick: _ => isIndeterminate.RxValue = !isIndeterminate.Value)
            }), // HStackPanel
            HText(new(() => $"Current Completion Rate: {progress.RxValue:F0}%"),
                strStyle: "mt-1 fw-medium fg-gray-700")
        }); // HStackPanel
    }

    #endregion

    #region CheckBox Three-State Demo

    private static IElement CheckBoxDemo()
    {
        return Element.WithScope(uiScope =>
        {
            var isChecked = new MutSignal<bool?>(null);

            var root = HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
            {
                HText("CheckBox Tri-State Selection Demo", strStyle: SectionTitleStyle),
                HCheckBox(_ => new(
                    bindIsChecked: isChecked,
                    strStyle: "p-2 rounded-lg hover:bg-gray-50 transition-colors w-full"
                )
                {
                    HText("I agree to the service terms", strStyle: "text-base fg-gray-800 ml-2")
                }), // HCheckBox
                HButton("Toggle Checkbox State", strStyle: SecondaryBtnBase,
                    onClick: _ => isChecked.NotifySet(prev => prev is false))
            }); // HStackPanel

            uiScope.CreateEffect(() => Console.WriteLine($"Agreement Checkbox State: {isChecked.RxValue is true}"));

            return root;
        });
    }

    #endregion

    #region RadioButton Single Select Demo

    private static IElement RadioButtonDemo()
    {
        var threeState = new MutSignal<bool?>(null);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("RadioButton Exclusive Single Select Demo", strStyle: SectionTitleStyle),
            HRadioButton(_ => new(
                bindIsChecked: threeState,
                onClick: isChecked => Console.WriteLine($"Radio Selection State: {isChecked}"),
                strStyle: "p-2 rounded-lg hover:bg-gray-50 transition-colors w-full"
            )
            {
                HText("I accept the privacy policy", strStyle: "text-base fg-gray-800 ml-2")
            }), // HRadioButton
            HButton("Switch Radio Selected State", strStyle: SecondaryBtnBase,
                onClick: _ => threeState.NotifySet(prev => prev is false))
        }); // HStackPanel
    }

    #endregion

    #region ToggleSwitch On/Off Switch Demo

    private static IElement ToggleSwitchDemo()
    {
        var wifiEnabled = new MutSignal<bool?>(null);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("ToggleSwitch Boolean Switch Control Demo", strStyle: SectionTitleStyle),
            HToggleSwitch(_ => new(
                bindIsChecked: wifiEnabled,
                strStyle: "m-2 p-2 rounded-lg hover:bg-gray-50 w-full transition-colors",
                onIsCheckedChanged: _ => Console.WriteLine($"Wireless Network Toggle State: {wifiEnabled.Value}"
                ))
            {
                HText("Enable Wireless Network", strStyle: "text-base fg-gray-800 ml-2")
            }), // HToggleSwitch
            HText(
                new(() => $"Wireless Network Status: {(wifiEnabled.RxValue is true ? "ENABLED" : "DISABLED")}"),
                strStyle: "mt-1 text-base fw-medium fg-coffee-700"
            ) // HText
        }); // HStackPanel
    }

    #endregion

    #region File Picker Dialog Demo

    private static IElement FilePickerDemo()
    {
        FilePickerFilter[] filterFlies =
        [
            new("Image Files", "*.png", "*.jpg", "*.jpeg"),
            new("All File Types", "*.*")
        ];

        var selectedFile = new MutSignal<string?>(null);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("FilePicker Local File Selection Dialog Demo", strStyle: SectionTitleStyle),
            HFilePicker(
                bindSelectedPath: selectedFile,
                title: "Select An Image File",
                filters: filterFlies,
                strStyle: PrimaryBtnBase
            ), // HFilePicker
            HText(new(() => $"Selected File Path: {selectedFile.RxValue ?? "No file selected"}"),
                strStyle: "mt-2 text-sm fg-coffee-700"),
            HBorder(_ => new(strStyle: new(() => $"mt-2 w-64 h-64 overflow-hidden {MaskColor()} border rounded-xl"))
            {
                HImage(uri: selectedFile,
                    stretch: Stretch.UniformToFill,
                    strStyle:
                    "mask-gradient-b mask-from-50 mask-to-20 transition-transform ease-in-out duration-500 hover:scale-110"
                ), // HImage
            }) // HBorder
        }); // HStackPanel

        string MaskColor()
        {
            return selectedFile.RxValue is null
                ? "bg-gray-50 border-gray-200"
                : "gradient-b from-pink-300 to-blue-300 border-pink-200";
        }
    }

    #endregion

    #region Drag File Drop Demo

    private static IElement DragFileDemo()
    {
        var imgPath = new MutSignal<string?>(null);
        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Drag Image File Demo", strStyle: SectionTitleStyle),

            HDrop(_ => new(
                isAllowDrop: true,
                strStyle: new(() =>
                    $"mt-2 w-64 h-64 overflow-hidden relativeTarget {MaskColor()} border rounded-xl dragover:bg-sky-200"),
                onDragOver: DragOver,
                onDrop: Drop
            )
            {
                Show(new(new(() => imgPath.RxValue is null))
                {
                    IfTrue = () => HText("Drag image files here\nSupports PNG / JPG / JPEG",
                        strStyle: "w-full h-full text-center text-lg fg-gray-400 dragover:fg-sky-600"),
                    IfFalse = () => HImage(uri: imgPath,
                        stretch: Stretch.UniformToFill,
                        strStyle:
                        "mask-gradient-b mask-from-50 mask-to-20 transition-transform ease-in-out duration-500 hover:scale-110"
                    ) // HImage
                }) // Show
            }) // HBorder
        }); // HStackPanel

        string MaskColor()
        {
            return imgPath.RxValue is null
                ? "bg-gray-50 border-gray-200"
                : "gradient-b from-pink-300 to-blue-300 border-pink-200";
        }

        void DragOver(DragEventArgs e)
        {
            var accept = e.DataTransfer.Formats.Contains(DataFormat.File);
            e.DragEffects = accept ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        void Drop(DragEventArgs e)
        {
            var files = e.DataTransfer.TryGetFile();
            imgPath.RxValue = files?.Path.LocalPath;
            e.Handled = true;
        }
    }

    #endregion

    #region Slider Value Slider Control Demo

    private static IElement SliderDemo()
    {
        var volume = new MutSignal<double>(50);

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Slider Continuous Value Adjustment Demo", strStyle: SectionTitleStyle),
            HSlider(
                bindValue: volume,
                minimum: 0,
                maximum: 100,
                isSnapToTickEnabled: true,
                tickFrequency: 10,
                tickPlacement: TickPlacement.Outside,
                strStyle: "w-72 h-6 horizontal"
            ), // HSlider
            HText(new(() => $"Audio Volume Level: {volume.RxValue:F0}"),
                strStyle: "mt-2 text-xl fw-semibold fg-matcha-600")
        }); // HStackPanel
    }

    #endregion

    #region Popup Demo

    private static IElement PopupDemo()
    {
        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Popup Demo", strStyle: SectionTitleStyle),
            HStackPanel(props => new(strStyle: HorizontalRowBase + "p-2 bg-matcha-200")
            {
                Children =
                [
                    HButton("File", strStyle: PrimaryBtnBase),
                    HButton("Edit", strStyle: PrimaryBtnBase)
                ],
                Popups =
                [
                    HPopup(_ => new(
                        isOpen: props.IsPointerOver,
                        placement: PlacementMode.Top,
                        verticalOffset: -10
                    )
                    {
                        HText("Help", strStyle: "px-2 py-1 fg-white text-base bg-matcha-200 border rounded-lg")
                    }) // HPopup
                ] // HStackPanel.HPopup
            }) // HStackPanel
        }); // HStackPanel
    }

    #endregion

    #region Flyout Demo

    private static IElement FlyoutDemo()
    {
        var select = new MutSignal<string>("");
        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Flyout Demo", strStyle: SectionTitleStyle),
            HStackPanel(_ => new(strStyle: HorizontalRowBase)
            {
                HFlyout(flyout => new(
                    placement: PlacementMode.Right,
                    showMode: FlyoutShowMode.Transient
                )
                {
                    Host = host =>
                        HButton("File",
                            strStyle: PrimaryBtnBase,
                            onClick: _ => flyout.ShowAt(host, showAtPointer: true)),
                    Content = HStackPanel(_ => new(strStyle: VerticalStackBase +
                                                             "gap-y-1 p-2 bg-matcha-50 border border-matcha-300 rounded-lg shadow")
                    {
                        HButton("Open", strStyle: "text-sm fg-matcha-700 bg-matcha-200/0 hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Open")),
                        HButton("Save", strStyle: "text-sm fg-matcha-700 bg-transparent hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Save")),
                        HSeparator(strStyle: "w-full h-0.25 bg-matcha-700"),
                        HButton("Exit", strStyle: "text-sm fg-matcha-700 bg-transparent hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Exit")),
                    }) // HFlyout.Content
                }), // HFlyout

                HFlyout(flyout => new(
                    placement: PlacementMode.Right,
                    showMode: FlyoutShowMode.Transient
                )
                {
                    Host = host =>
                        HButton("Edit",
                            strStyle: PrimaryBtnBase,
                            onClick: _ => flyout.ShowAt(host, showAtPointer: true)),
                    Content = HStackPanel(_ => new(strStyle: VerticalStackBase +
                                                             "gap-y-1 p-2 bg-matcha-50 border border-matcha-300 rounded-lg shadow")
                    {
                        HButton("Copy", strStyle: "text-sm fg-matcha-700 bg-matcha-200/0 hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Copy")),
                        // HButton
                        HButton("Paste", strStyle: "text-sm fg-matcha-700 bg-transparent hover:bg-matcha-300",
                            onClick: _ => SetSelect(flyout, "Paste")),
                        // HButton
                    }) // HFlyout.Content
                }), // HFlyout
            }),
            HText(new(() => $"You Selected: {select.RxValue}."),
                strStyle: "fg-coffee-500")
        }); // HStackPanel

        void SetSelect(HFlyoutProps flyout, string value)
        {
            select.RxValue = value;
            flyout.Hide();
        }
    }

    #endregion

    #region Window Demo

    private static IElement WindowDemoComp(UiScope scope)
    {
        var text = new MutSignal<string?>("");

        return HStackPanel(_ => new(strStyle: DemoCardBase + VerticalStackBase)
        {
            HText("Window & Dialog Management", strStyle: SectionTitleStyle),
            HText("Open a modal dialog to input text; result reflects back here",
                strStyle: "text-sm fg-coffee-700 mb-2"),

            HText(new(() => string.IsNullOrEmpty(text.RxValue) ? "(No input yet)" : text.RxValue),
                strStyle:
                "text-lg fw-medium fg-matcha-800 p-3 bg-matcha-50 rounded-xl border border-matcha-200 w-full"),

            HStackPanel(_ => new(strStyle: HorizontalRowBase + "mt-2")
            {
                HButton("Open Dialog",
                    strStyle: PrimaryBtnBase,
                    onClick: _ => OpenDialog(scope, text))
            }) // HStackPanel
        });
    }

    private static void OpenDialog(UiScope scope, MutSignal<string?> text)
    {
        var parentWindow = scope.GetContext(NeHiveUiContext.Window);
        if (parentWindow is null)
        {
            Console.WriteLine("Parent Window is null");
            return;
        }

        var parentIsLock = scope.GetContext(ContextKey.LockWindowCount);

        var dialog = scope.CreateWindow((window, _) =>
        {
            return HStackPanel(_ => new(strStyle: VerticalStackBase + "mt-2 w-100 h-60 p-4")
            {
                HText("Enter your text", strStyle: "text-lg fw-semibold fg-matcha-800"),
                HTextBox(
                    bindText: text,
                    // placeholderText: "Type something...",
                    strStyle: InputBaseStyle + "text-base focus:border-matcha-500"
                ), // HTextBox
                HStackPanel(_ => new(strStyle: HorizontalRowBase + "justify-end gap-2 mt-2")
                {
                    HButton("Cancel",
                        strStyle: SecondaryBtnBase,
                        onClick: _ => CancelInput(window)
                    ), // HButton
                    HButton("OK",
                        strStyle: PrimaryBtnBase,
                        onClick: _ => window.Close())
                }) // HStackPanel
            }); // HStackPanel
        }); // dialog

        // dialog.Width = 400;
        // dialog.Height = 240;

        dialog.Closed += (_, _) =>
        {
            if (parentIsLock is null) return;
            parentIsLock.RxValue = parentIsLock.Value - 1;
        };
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog(parentWindow);
        if (parentIsLock is null) return;
        parentIsLock.RxValue = parentIsLock.Value + 1;
        return;

        void CancelInput(Window window)
        {
            text.RxValue = "";
            window.Close();
        }
    }

    private static IElement WindowDemo()
        => Element.WithScope(WindowDemoComp);

    #endregion

}
