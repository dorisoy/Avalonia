using System;
using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Markup.Xaml.XamlIl;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using ControlCatalog.Pages;
using ControlCatalog.Models;

namespace ControlCatalog
{
    public class MainView : UserControl
    {
        public MainView()
        {
            AvaloniaXamlLoader.Load(this);

            var sideBar = this.FindControl<TabControl>("Sidebar");

            if (AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().IsDesktop)
            {
                IList tabItems = ((IList)sideBar.Items);
                tabItems.Add(new TabItem()
                {
                    Header = "Dialogs",
                    Content = new DialogsPage()
                });
                tabItems.Add(new TabItem()
                {
                    Header = "Screens",
                    Content = new ScreenPage()
                });

            }

            var themes = this.Find<ComboBox>("Themes");
            themes.SelectionChanged += (sender, e) =>
            {
                if (themes.SelectedItem is CatalogTheme theme)
                {
                    if (theme== CatalogTheme.FluentLight)
                    {
                        Application.Current.Styles[1] = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
                        {
                            Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/BaseLight.xaml"),
                        };
                        Application.Current.Styles[3] = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
                        {
                            Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentControlResourcesLight.xaml"),
                        };
                    }
                    else if (theme == CatalogTheme.FluentDark)
                    {
                        Application.Current.Styles[1] = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
                        {
                            Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/BaseDark.xaml"),
                        };
                        Application.Current.Styles[3] = new StyleInclude(new Uri("avares://ControlCatalog/Styles"))
                        {
                            Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentControlResourcesDark.xaml"),
                        };
                    }
                    else if (theme == CatalogTheme.DefaultLight)
                    {

                    }
                    else if (theme == CatalogTheme.DefaultDark)
                    {

                    }
                }
            };

            var decorations = this.Find<ComboBox>("Decorations");
            decorations.SelectionChanged += (sender, e) =>
            {
                if (VisualRoot is Window window
                    && decorations.SelectedItem is SystemDecorations systemDecorations)
                {
                    window.SystemDecorations = systemDecorations;
                }
            };

            var transparencyLevels = this.Find<ComboBox>("TransparencyLevels");
            IDisposable backgroundSetter = null, paneBackgroundSetter = null;
            transparencyLevels.SelectionChanged += (sender, e) =>
            {
                backgroundSetter?.Dispose();
                paneBackgroundSetter?.Dispose();
                if (transparencyLevels.SelectedItem is WindowTransparencyLevel selected
                    && selected != WindowTransparencyLevel.None)
                {
                    var semiTransparentBrush = new ImmutableSolidColorBrush(Colors.Gray, 0.5);
                    backgroundSetter = sideBar.SetValue(BackgroundProperty, semiTransparentBrush, Avalonia.Data.BindingPriority.Style);
                    paneBackgroundSetter = sideBar.SetValue(SplitView.PaneBackgroundProperty, semiTransparentBrush, Avalonia.Data.BindingPriority.Style);
                }
            };
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            var decorations = this.Find<ComboBox>("Decorations");
            if (VisualRoot is Window window)
                decorations.SelectedIndex = (int)window.SystemDecorations;
        }
    }
}
