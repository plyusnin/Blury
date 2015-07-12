using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Blury
{
    public class ArrowBorder : Decorator
    {
        public static readonly DependencyProperty ArrowHeightProperty =
            DependencyProperty.Register("ArrowHeight", typeof(double), typeof(ArrowBorder),
                                        new FrameworkPropertyMetadata(12.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ArrowOffsetProperty =
            DependencyProperty.Register("ArrowOffset", typeof(double), typeof(ArrowBorder),
                                        new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundProperty =
            Panel.BackgroundProperty.AddOwner(typeof(ArrowBorder),
                                              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ArrowBorder),
                                        new FrameworkPropertyMetadata(default(Brush),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(ArrowBorder),
                                        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public double ArrowOffset
        {
            get { return (double)GetValue(ArrowOffsetProperty); }
            set { SetValue(ArrowOffsetProperty, value); }
        }

        public double ArrowHeight
        {
            get { return (double)GetValue(ArrowHeightProperty); }
            set { SetValue(ArrowHeightProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>Рисует содержимое объекта <see cref="T:System.Windows.Media.DrawingContext" /> в проходе визуализации элемента
        ///     <see cref="T:System.Windows.Controls.Panel" />.</summary>
        /// <param name="dc">Объект <see cref="T:System.Windows.Media.DrawingContext" /> для рисования.</param>
        protected override void OnRender(DrawingContext dc)
        {
            var g = new StreamGeometry();
            using (StreamGeometryContext gc = g.Open())
            {
                gc.BeginFigure(new Point(0, 0), true, true);
                gc.LineTo(new Point(ActualWidth, 0), false, false);
                gc.LineTo(new Point(ActualWidth, ActualHeight + ArrowOffset), false, false);
                gc.LineTo(new Point(0.5 * ActualWidth, ActualHeight + ArrowOffset + ArrowHeight), true, false);
                gc.LineTo(new Point(0, ActualHeight + ArrowOffset), true, false);
                gc.LineTo(new Point(0, 0), false, false);
            }
            dc.DrawGeometry(Background, new Pen(BorderBrush, BorderThickness), g);
        }
    }
}