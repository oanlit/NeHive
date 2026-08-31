namespace NeHive.Sample.Avalonia;

// Data Model Records
public record User(int? Id = null, string? Name = null);

public record Country(string? Name = null, string? Code = null);

// Demo Page Enumeration
public enum DemoView
{
    SimpleCounter,

    GridDemo,
    AbsoluteDemo,
    SplitViewDemo,
    SplitPanelDemo,
    UniformGridDemo,
    DockPanelDemo,
    WrapPanelDemo,
    GridSplitterDemo,
    ScrollDemo,

    TextBoxDemo,
    CommandDemo,
    AsyncCommandProgressDemo,
    CheckBoxDemo,
    RadioButtonDemo,
    ToggleSwitchDemo,
    FilePickerDemo,
    DragFileDemo,
    SliderDemo,
    PopupDemo,
    FlyoutDemo,
    WindowDemo,

    TreeViewDemo,
    ComboBoxDemo,

    ShowDemo,
    SwitchDemo,
    MatchDemo,
    ForEachDemo,
    LoadingDemo,

    LifecycleDemo,

    SpacingDemo,
    SizingDemo,
    PaddingDemo,
    LayoutDemo,
    TextStyleDemo,
    ColorDemo,
    BorderDemo,
    EffectsDemo,
    CursorDemo,
    TransitionDemo,
    TransformDemo,

    CustomSliderDemo,
    CustomScrollDemo,

    GroupDemo,
    ContextDemo,

    ClockDemo,
    MusicPlayerDemo,
    Unknown
}
