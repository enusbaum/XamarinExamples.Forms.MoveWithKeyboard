using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamarinExamples.Forms.MoveWithKeyboard.iOS.Renderers;
using XamarinExamples.Forms.MoveWithKeyboard.Renderers;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButton_IOS))]
namespace XamarinExamples.Forms.MoveWithKeyboard.iOS.Renderers
{
    /// <summary>
    ///     Custom Button renderer for IOS that shows how to 
    ///     move a forms element (this example is a button) when
    ///     the soft keyboard appears.
    /// 
    ///     This prevents the keyboard from hiding the element
    /// </summary>
    public class CustomButton_IOS : ButtonRenderer
    {
        //Y Coordinate Tracking
        nfloat _originalY = 0;
        nfloat _aboveKeyboardY = 0;

        //State Tracking
        bool _keyboardNotificationSetup = false;
        bool _keyboardVisible = false;

        //Observers
        NSObject _showObserver;
        NSObject _hideObserver;


        public CustomButton_IOS() { }

        /// <summary>
        ///     Implementation of IDisposable
        /// 
        ///     Cleans up any observers if they were setup
        /// </summary>
        /// <returns>The dispose.</returns>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_showObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_showObserver);

            if (_hideObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_hideObserver);
        }

        /// <summary>
        ///     Fired off when the button is firrst loaded and displayed to the user.
        /// 
        ///     We track if the keyboard notifications are already setup to prevent duplicate
        ///     notifications being sent to NSNotificationCenter
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if ((Control != null) && (e.NewElement != null))
            {
                var _customButton = (e.NewElement as CustomButton);

                if (_customButton != null && _customButton.MoveWithKeyboard && !_keyboardNotificationSetup)
                {
                    /*
                     *  Some things I need to document here
                     * 
                     *  1) These event notification delegates only allow the button touch events to still  work if you reference "this" 
                     *      not "control". Using "Control" will not allow any of the touch events to fire... for some reason...
                     * 
                     *  2) The hide event requires us to move the button async because having the move event here will result in 
                     *      the touch event not being fired (at least in the emulator). I should probably try it on the phone, but yeah,
                     *      this one took half a day to figure out. That being said, this seems to work.
                     * 
                     *      My guess is that this is a race condition where the move is happening before TouchUp, and when the btuton is move
                     *      the event sequence is interrupted. Will probably need to reach out to Xamarin about this one
                     */
                    _showObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, (notification) =>
                    {
                        try
                        {
                            //Calculate new point and save original one
                            if (_originalY == 0)
                                _originalY = this.Center.Y;

                            if (_aboveKeyboardY == 0)
                                _aboveKeyboardY = _originalY - ((NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey)).RectangleFValue.Height;

                            //Move the Button
                            this.Center = new CoreGraphics.CGPoint() { X = this.Center.X, Y = _aboveKeyboardY };

                            _keyboardVisible = true;
                            return;
                        }
                        catch
                        {
                            return;
                        }
                    });

                    _hideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, (notification) =>
                    {
                        try
                        {
                            //For hardware keyboard, the show observer is never registered, and a keybaord is never displayed
                            if (!_keyboardVisible)
                                return;

                            //-- DOES NOT WORK: Will not allow "Clicked" event to be fired from the Form
                            // Bugzilla Report: https://bugzilla.xamarin.com/show_bug.cgi?id=58263
                            //this.Center = new CoreGraphics.CGPoint() { X = this.Center.X, Y = _originalY };

                            //-- THIS WORKS: Delays the move just long enough to actually register the touch event before moving
                            Task.Factory.StartNew(() => {
                                RestoreOriginalPosition();
                            });

                            _keyboardVisible = false;
                            return;
                        }
                        catch
                        {
                            return;
                        }

                    });
                    _keyboardNotificationSetup = true;
                }
            }
        }

        private void RestoreOriginalPosition()
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    //Move the Button
                    if (!_keyboardVisible)
                        this.Center = new CoreGraphics.CGPoint() { X = this.Center.X, Y = _originalY };
                }
                catch
                {
                    //This shouldn't hit since we're disposing properly...
                    return;
                }
            });
        }
    }
}
