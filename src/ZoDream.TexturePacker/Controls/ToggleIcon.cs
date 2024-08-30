using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.TexturePacker.Controls
{
    public sealed class ToggleIcon : ButtonBase
    {
        public ToggleIcon()
        {
            this.DefaultStyleKey = typeof(ToggleIcon);
        }


        public string OnIcon {
            get { return (string)GetValue(OnIconProperty); }
            set { SetValue(OnIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnIconProperty =
            DependencyProperty.Register("OnIcon", typeof(string), 
                typeof(ToggleIcon), new PropertyMetadata(string.Empty, OnIconChanged));



        public string OffIcon {
            get { return (string)GetValue(OffIconProperty); }
            set { SetValue(OffIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffIconProperty =
            DependencyProperty.Register("OffIcon", 
                typeof(string), typeof(ToggleIcon), 
                new PropertyMetadata(string.Empty, OnIconChanged));

   

        public bool IsOn {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), 
                typeof(ToggleIcon), new PropertyMetadata(false, OnIconChanged));



        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(ToggleIcon), new PropertyMetadata(string.Empty));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ToggleIcon)?.UpdatedIcon();
        }

        private void UpdatedIcon()
        {
            Icon = IsOn ? OnIcon : OffIcon;
        }
    }
}
