using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlowChart
{
    public class DesignerCanvasAdorner : Adorner
    {
        public DesignerCanvasAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            if (adornedElement is FrameworkElement)
            {
                _adornedElement = adornedElement as FrameworkElement;
                CreateGrip();
            }
        }
        private void CreateGrip()
        {
            // Scaling grip
            Rectangle rect = new Rectangle();
            rect.Stroke = Brushes.Blue;
            rect.Fill = Brushes.Red;
            rect.Cursor = Cursors.SizeNWSE;

            rect.MouseDown += OnGripMouseDown;
            rect.MouseUp += OnGripMouseUp;
            rect.MouseMove += OnGripMouseMove;
            AddVisualChild(rect);
            _scalingGrip = rect;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _adornedElement != null ? 1 : 0;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (_adornedElement != null)
                return _scalingGrip;
            return base.GetVisualChild(index);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            if (_scalingGrip != null)
                _scalingGrip.Arrange(new Rect(finalSize.Width - 5, finalSize.Height - 5, 10, 10));
            return size;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1);
            renderPen.DashStyle = new DashStyle(new double[] { 2.5, 2.5 }, 0);

            Rect rect = new Rect(0, 0, _adornedElement.ActualWidth, _adornedElement.ActualHeight);
            drawingContext.DrawRectangle(Brushes.Transparent, renderPen, rect);

            base.OnRender(drawingContext);
        }

        private void OnGripMouseDown(object sender, MouseEventArgs args)
        {
            if (args.LeftButton != MouseButtonState.Pressed)
                return;

            Rectangle rect = sender as Rectangle;
            if (rect != null)
                Mouse.Capture(rect);
        }

        private void OnGripMouseUp(object sender, MouseEventArgs args)
        {
            Rectangle rect = sender as Rectangle;
            if (rect != null)
                Mouse.Capture(null);
        }

        private void OnGripMouseMove(object sender, MouseEventArgs args)
        {
            if (args.LeftButton != MouseButtonState.Pressed)
                return;

            Rectangle rect = sender as Rectangle;
            if (rect == null || _adornedElement == null)
                return;

            Point point = args.GetPosition(_adornedElement);
            _adornedElement.Width = point.X > 0 ? point.X : 0;
            _adornedElement.Height = point.Y > 0 ? point.Y : 0;
        }

        Rectangle _scalingGrip = null;
        FrameworkElement _adornedElement = null;
    }
}
