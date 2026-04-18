using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using ZoDream.TexturePacker.ViewModels.Models;

namespace ZoDream.TexturePacker.Controls
{
    internal class MenuBarItemEx: MenuBarItem
    {

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(MenuBarItemEx),
            new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(MenuBarItemEx),
                new PropertyMetadata(null));

        public IEnumerable ItemsSource {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public ICommand Command {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuBarItemEx menuFlyoutEx && e.NewValue is IEnumerable itemsSource)
            {
                menuFlyoutEx.Items.Clear();

                foreach (object item in itemsSource)
                {
                    // 这里根据你的需要创建 MenuFlyoutItem
                    var flyoutItem = item is PluginMenuItem p ? new MenuFlyoutItem
                    {
                        Text = p.Name, // 或者使用 item 的某个属性
                        Command = p.Command ?? menuFlyoutEx.Command,
                        CommandParameter = item
                    } : new MenuFlyoutItem
                    {
                        Text = item.ToString(), // 或者使用 item 的某个属性
                        Command = menuFlyoutEx.Command,
                        CommandParameter = item
                    };
                    menuFlyoutEx.Items.Add(flyoutItem);
                }
            }
        }
    }
}
