using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.TexturePacker.Controls
{
    public sealed partial class ImageEditor : UserControl
    {
        public ImageEditor()
        {
            this.InitializeComponent();
            Loaded += ImageEditor_Loaded;
        }

        private readonly double _dpiScale = App.ViewModel.GetDpiScaleFactorFromWindow();

        private void ImageEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Resize((int)(ActualWidth * _dpiScale), (int)(ActualHeight * _dpiScale));
        }

        public ICommand SelectedCommand {
            get { return (ICommand)GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCommandProperty =
            DependencyProperty.Register("SelectedCommand", typeof(ICommand), typeof(ImageEditor), new PropertyMetadata(null));



        private void CanvasTarget_PaintSurface(object sender, SkiaSharp.Views.Windows.SKPaintSurfaceEventArgs e)
        {
            Paint(e.Surface.Canvas, e.Info);
        }

        private void CanvasTarget_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(CanvasTarget);
            Tap((float)(point.X * _dpiScale), (float)(point.Y * _dpiScale));
        }

        private void ResizeWithControl(int width, int height) 
        {
            CanvasTarget.Width = width / _dpiScale;
            CanvasTarget.Height = height / _dpiScale;
            //Canvas.SetLeft(CanvasTarget, 0);
            //Canvas.SetTop(CanvasTarget, 0);
            //HScrollBar.Value = 0;
            //VScrollBar.Value = 0;
            //HScrollBar.Maximum = CanvasTarget.ActualWidth;
            //VScrollBar.Maximum = CanvasTarget.ActualHeight;
            //HScrollBar.Visibility = CanvasTarget.ActualWidth > ActualWidth ? Visibility.Visible : Visibility.Collapsed;
            //VScrollBar.Visibility = CanvasTarget.ActualHeight > ActualHeight ? Visibility.Visible : Visibility.Collapsed;
        }

        //private void VScrollBar_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        //{
        //    Canvas.SetTop(CanvasTarget, -e.NewValue);
        //}

        //private void HScrollBar_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        //{
        //    Canvas.SetLeft(CanvasTarget, -e.NewValue);
        //}
    }
}
