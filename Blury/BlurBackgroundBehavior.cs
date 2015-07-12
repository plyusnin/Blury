using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Blury
{
    public class BlurBackgroundBehavior : Behavior<Shape>
    {
        public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register(
            "BackgroundContent", typeof (FrameworkElement), typeof (BlurBackgroundBehavior), new PropertyMetadata(OnContainerChanged));

        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof (VisualBrush), typeof (BlurBackgroundBehavior), new PropertyMetadata(default(VisualBrush)));

        public static readonly DependencyProperty BrushXProperty = DependencyProperty.Register(
            "BrushX", typeof (VisualBrush), typeof (BlurBackgroundBehavior), new PropertyMetadata(default(VisualBrush)));

        private readonly Rectangle _x;
        internal double Radius = 80;

        public BlurBackgroundBehavior()
        {
            _x = new Rectangle();
            _x.Effect = new BlurEffect
                        {
                            Radius = this.Radius,
                            KernelType = KernelType.Gaussian,
                            RenderingBias = RenderingBias.Quality
                        };
            _x.SetBinding(Shape.FillProperty,
                          new Binding
                          {
                              Source = this,
                              Path = new PropertyPath(BrushXProperty)
                          });
        }

        public FrameworkElement BackgroundContent
        {
            get { return (FrameworkElement)GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }

        public VisualBrush Brush
        {
            get { return (VisualBrush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public VisualBrush BrushX
        {
            get { return (VisualBrush)GetValue(BrushXProperty); }
            set { SetValue(BrushXProperty, value); }
        }

        /// <summary>Called after the behavior is attached to an AssociatedObject.</summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            AssociatedObject.SetBinding(Shape.FillProperty,
                                        new Binding
                                        {
                                            Source = this,
                                            Path = new PropertyPath(BrushProperty)
                                        });

            AssociatedObject.LayoutUpdated += (sender, args) => UpdateBounds();
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            if (AssociatedObject != null && BackgroundContent != null && Brush != null)
            {
                BrushX.Viewbox = new Rect(new Point(), BackgroundContent.RenderSize);

                BrushX.ViewportUnits = BrushMappingMode.Absolute;
                BrushX.Viewport = new Rect(new Point(), BackgroundContent.RenderSize);

                var difference = AssociatedObject.TranslatePoint(new Point(), BackgroundContent);
                Brush.Viewbox = new Rect(difference, AssociatedObject.RenderSize);
            }
        }

        private Size half(Size s) { return new Size(s.Width / 2, s.Height / 2); }

        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlurBackgroundBehavior)d).OnContainerChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
        }

        private void OnContainerChanged(UIElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
                oldValue.LayoutUpdated -= OnContainerLayoutUpdated;

            if (newValue != null)
            {
                _x.SetBinding(FrameworkElement.WidthProperty,
                              new Binding
                              {
                                  Source = newValue,
                                  Path = new PropertyPath(FrameworkElement.ActualWidthProperty),
                                  Converter = new XSizeConverter(this)
                              });
                _x.SetBinding(FrameworkElement.HeightProperty,
                              new Binding
                              {
                                  Source = newValue,
                                  Path = new PropertyPath(FrameworkElement.ActualHeightProperty),
                                  Converter = new XSizeConverter(this)
                              });

                BrushX = new VisualBrush(newValue)
                         {
                             ViewboxUnits = BrushMappingMode.Absolute,
                             TileMode = TileMode.FlipXY
                         };

                Brush = new VisualBrush(_x)
                        {
                            ViewboxUnits = BrushMappingMode.Absolute
                        };

                newValue.LayoutUpdated += OnContainerLayoutUpdated;
                UpdateBounds();
            }
            else
                Brush = null;
        }

        private void OnContainerLayoutUpdated(object sender, EventArgs eventArgs) { UpdateBounds(); }
    }

    internal class XSizeConverter : IValueConverter
    {
        private readonly BlurBackgroundBehavior _blurBackgroundBehavior;
        public XSizeConverter(BlurBackgroundBehavior BlurBackgroundBehavior) { _blurBackgroundBehavior = BlurBackgroundBehavior; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + 2 * _blurBackgroundBehavior.Radius;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
    }
}
