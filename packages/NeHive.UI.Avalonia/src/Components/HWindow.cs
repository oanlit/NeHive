using System.Runtime.InteropServices.ComTypes;
using Avalonia;
using Avalonia.Controls;
using NeHive.Model;
using NeHive.Reactive;

namespace NeHive.UI.Avalonia.Components;

public static partial class BaseComponent
{
    extension(UiScope scope)
    {
        public Window CreateWindow(Func<Window, Scope, IElement> content)
        {
            var scope2 = new Scope(scope);
            var window = new Window();
            var size = new MutSignal<Size>(window.ClientSize);
            window.Resized += (_, e) => size.RxValue = e.ClientSize;

            scope2
                .SetContext(NeHiveUiContext.Window, window)
                .SetContext(NeHiveUiContext.WindowSize, size);

            Control c;
            using (new ScopeFrame(scope2))
            {
                c = content(window, scope2).Content;
            }

            window.Content = c;
            // window.Width = c.Width;
            // window.Height = c.Height;

            var isClosed = false;
            window.Closing += (_, _) =>
            {
                isClosed = true;
                scope2.RemoveContext(NeHiveUiContext.WindowSize);
                scope2.RemoveContext(NeHiveUiContext.Window);
            };
            scope2.OnCleanup += () =>
            {
                if (isClosed) return;
                window.Close();
            };
            return window;
        }
    }
}