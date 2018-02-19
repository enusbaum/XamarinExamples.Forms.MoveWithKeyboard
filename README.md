# XamarinExamples.Forms.MoveWithKeyboard

This repository holds an example of the best way I've been able to come up with to handle moving Xamarin.Forms elements as to not be covered by the iOS soft keyboard when it appears.

I hope this is able to lend a hand to anyone else looking for a simple solution to this problem.

Cheers!

# How it works


### Xamarin.Forms Code

In the Xamarin.Forms project we setup a very basic Custom Renderer (in this case, but a Button element) that tracks a boolean value for "MoveWithKeyboard". 

```csharp
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

```

Then in our XAML we use the custom renderer for the button:

```xml
<custom:CustomButton MoveWithKeyboard="true" .... />
```

### Xamarin.iOS

For the Xamarin.iOS bit, we create a Custom Renderer for the Button element.

([**Link**](https://github.com/enusbaum/XamarinExamples.Forms.MoveWithKeyboard/blob/c07521ed7450b622405639c4de971248da31e21e/iOS/Renderers/CustomButton_IOS.cs#L84-L132) to where the magic happens in the IOS Custom Renderer)

```csharp
protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
{
        ... setup notifications ...
}
```

In the **OnElementChanged** method, we setup two observers and register them with NSNotificationCenter that we register the events on both the **UIKeyboard.WillShowNotification** and **UIKeyboard.WillHideNotification** which will invoke our delegates when the keyboard is displayed and hidden.

#### FYI

Worth noting, there's some hackery around ensuring the Touch event is fired. It appears in Xamarin.Forms "Touch" really means "TouchUp". The issue we run into while relocating a button is that "TouchDown" triggers the keyboard to be hidden and thus triggering the notification event to relocate the button back to its original position. 

Because the button is moved, the "TouchUp" event is not fired. The custom renderer in the iOS project has my workaround for this issue, which I have opened a Bugzilla case for (#[58263](https://bugzilla.xamarin.com/show_bug.cgi?id=58263)).

Cheers!

![Example of moving button](https://d2ffutrenqvap3.cloudfront.net/items/392u180T33063c2H0f1B/Screen%20Recording%202018-02-19%20at%2005.48%20PM.gif "Example of moving button")
