using Xamarin.Forms;
namespace XamarinExamples.Forms.MoveWithKeyboard.Renderers
{
    /// <summary>
    ///     Basic Custom Renderer with a bool option setup to track the "MoveWithKeyboard" option
    /// 
    ///     Nothing really special in here, just an example of what to add to an existing custom Button
    ///     renderer you might already have setup.
    /// </summary>
    public class CustomButton : Button
    {
        public const string MoveWithKeyboardName = "MoveWithKeyboard";

        public CustomButton() { }

        public static readonly BindableProperty MoveWithKeyboardProperty = BindableProperty.Create(
            propertyName: MoveWithKeyboardName,
            returnType: typeof(bool),
            declaringType: typeof(CustomButton),
            defaultValue: false);

        public bool MoveWithKeyboard
        {
            get { return (bool)GetValue(MoveWithKeyboardProperty); }
            set { SetValue(MoveWithKeyboardProperty, value); }
        }
    }
}