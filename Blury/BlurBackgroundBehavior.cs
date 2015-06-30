using System;
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
            "BackgroundContent", typeof (UIElement), typeof (BlurBackgroundBehavior), new PropertyMetadata(OnContainerChanged));

        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush", typeof (VisualBrush), typeof (BlurBackgroundBehavior), new PropertyMetadata(default(VisualBrush)));

        public UIElement BackgroundContent
        {
            get { return (UIElement)GetValue(BackgroundContentProperty); }
            set { SetValue(BackgroundContentProperty, value); }
        }

        public VisualBrush Brush
        {
            get { return (VisualBrush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        /// <summary>Called after the behavior is attached to an AssociatedObject.</summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            AssociatedObject.Effect = new BlurEffect
                                      {
                                          Radius = 80,
                                          KernelType = KernelType.Gaussian,
                                          RenderingBias = RenderingBias.Quality
                                      };

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
                var difference = AssociatedObject.TranslatePoint(new Point(), BackgroundContent);
                Brush.Viewbox = new Rect(difference, AssociatedObject.RenderSize);
            }
        }

        private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BlurBackgroundBehavior)d).OnContainerChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
        }

        private void OnContainerChanged(UIElement oldValue, UIElement newValue)
        {
            if (oldValue != null)
                oldValue.LayoutUpdated -= OnContainerLayoutUpdated;

            if (newValue != null)
            {
                Brush = new VisualBrush(newValue)
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
}
