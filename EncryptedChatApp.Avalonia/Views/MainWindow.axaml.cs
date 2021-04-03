using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using EncryptedChatApp.Avalonia.Core;
using EncryptedChatApp.Avalonia.Pages;
using EncryptedChatApp.Avalonia.ViewModels;
using Org.BouncyCastle.Asn1.Crmf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;

namespace EncryptedChatApp.Avalonia.Views
{
    public class MainWindow : FluentWindow
    {
        private TabControl tabControlMenu;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            tabControlMenu = this.FindControl<TabControl>("tabControlMenu");

            var items = tabControlMenu.Items as global::Avalonia.Collections.AvaloniaList<object>;

            tabControlMenu.SelectedItem = items[1];
            tabControlMenu.SelectedItem = items[0];
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var tabItem = e.AddedItems[0] as TabItem;

            if (tabItem == null)
                return;

            var page = tabItem.Content as ILoadOnAppearingPage;

            if (page != null)
            {
                page.OnAppearing();
            }
        }
    }
}
