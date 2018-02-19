using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
namespace XamarinExamples.Forms.MoveWithKeyboard
{
    public partial class XamarinExamples_Forms_MoveWithKeyboardPage : ContentPage
    {
        public XamarinExamples_Forms_MoveWithKeyboardPage()
        {
            InitializeComponent();

            //Set safe areas for IOS
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            DisplayAlert("WHOA!", "Check out this event being fired!", "RAD!");
        }
    }
}
