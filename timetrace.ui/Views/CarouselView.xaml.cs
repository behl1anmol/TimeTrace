using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace timetrace.ui.Views;

/// <summary>
/// Interaction logic for CarouselView.xaml
/// </summary>
public partial class CarouselView : UserControl
{
    private int _previousIndex = -1;
    private bool _isAnimating;

    private static readonly Duration SlideDuration = new(TimeSpan.FromMilliseconds(250));
    private static readonly CubicEase SlideEase = new() { EasingMode = EasingMode.EaseOut };
    private const double SlideDistance = 300;

    public CarouselView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldVm)
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        if (e.NewValue is INotifyPropertyChanged newVm)
            newVm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != "CurrentIndex") return;

        var currentIndex = GetCurrentIndex();
        if (currentIndex < 0 || currentIndex == _previousIndex)
        {
            _previousIndex = currentIndex;
            return;
        }

        var direction = currentIndex > _previousIndex ? 1 : -1; // 1 = forward (slide left), -1 = back (slide right)
        var oldIndex = _previousIndex;
        _previousIndex = currentIndex;

        // Skip animation on first load or reset
        if (oldIndex < 0) return;

        AnimateSlide(direction);
    }

    private int GetCurrentIndex()
    {
        var prop = DataContext?.GetType().GetProperty("CurrentIndex");
        return prop?.GetValue(DataContext) is int idx ? idx : -1;
    }

    private ImageSource? GetCurrentImageSource()
    {
        var currentImageProp = DataContext?.GetType().GetProperty("CurrentImage");
        var currentImage = currentImageProp?.GetValue(DataContext);
        if (currentImage == null) return null;

        var imgSourceProp = currentImage.GetType().GetProperty("ImageSource");
        return imgSourceProp?.GetValue(currentImage) as ImageSource;
    }

    private void AnimateSlide(int direction)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        // OldImage shows what was displayed; NewImage shows the new content (already bound)
        OldImage.Source = NewImage.Source;
        OldImage.Opacity = 1;

        // Force the NewImage binding to update
        NewImage.Source = GetCurrentImageSource();

        // Old image: slide out in the direction of navigation and fade out
        var oldSlideOut = new DoubleAnimation(-direction * SlideDistance, SlideDuration) { EasingFunction = SlideEase };
        var oldFadeOut = new DoubleAnimation(0, SlideDuration) { EasingFunction = SlideEase };

        // New image: slide in from the opposite side
        NewImageTranslate.X = direction * SlideDistance;
        NewImage.Opacity = 0;

        var newSlideIn = new DoubleAnimation(0, SlideDuration) { EasingFunction = SlideEase };
        var newFadeIn = new DoubleAnimation(1, SlideDuration) { EasingFunction = SlideEase };

        newFadeIn.Completed += (_, _) =>
        {
            _isAnimating = false;
            // Reset old image
            OldImage.Opacity = 0;
            OldImageTranslate.X = 0;
        };

        // Start all animations
        OldImageTranslate.BeginAnimation(TranslateTransform.XProperty, oldSlideOut);
        OldImage.BeginAnimation(OpacityProperty, oldFadeOut);
        NewImageTranslate.BeginAnimation(TranslateTransform.XProperty, newSlideIn);
        NewImage.BeginAnimation(OpacityProperty, newFadeIn);
    }
}
