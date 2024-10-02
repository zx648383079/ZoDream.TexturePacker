using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System.Windows.Input;

namespace ZoDream.TexturePacker.Behaviors
{
    public class TextChangedBehavior : Behavior<TextBox>
    {
        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TextChangedBehavior), new PropertyMetadata(null));


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanging += AssociatedObject_TextChanging; ;
        }

        private void AssociatedObject_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            Command?.Execute(!string.IsNullOrWhiteSpace(sender.Text));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanging -= AssociatedObject_TextChanging;
        }
    }
}
