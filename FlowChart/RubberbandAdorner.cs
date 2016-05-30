using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using FlowChart.Interfaces;

namespace FlowChart
{
    public class RubberbandAdorner : Adorner
    {
        /// <summary>
        /// 开始坐标点
        /// </summary>
        private Point? startPoint;
        /// <summary>
        /// 结束坐标点
        /// </summary>
        private Point? endPoint;
        /// <summary>
        /// 定义一个Pen对象，用于定义鼠标在设计区域绘制框
        /// </summary>
        private Pen rubberbandPen;

        private FlowCanvas flowCanvas;

        public RubberbandAdorner(FlowCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this.flowCanvas = designerCanvas;
            this.startPoint = dragStartPoint;
            rubberbandPen = new Pen(Brushes.LightSlateGray, 1);
            rubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //获取一个值，元素是否捕获了鼠标
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();//尝试将鼠标强制捕获

                endPoint = e.GetPosition(this);
                UpdateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.flowCanvas);
            if (adornerLayer != null)
                adornerLayer.Remove(this);
            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
            if (this.startPoint.HasValue && this.endPoint.HasValue)
                dc.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(this.startPoint.Value, this.endPoint.Value));
        }
        /// <summary>
        /// 更新选择区域
        /// </summary>
        private void UpdateSelection()
        {
            foreach (ISelectable item in flowCanvas.SelectedItems)
                item.IsSelected = false;
            flowCanvas.SelectedItems.Clear();

            Rect rubberBand = new Rect(startPoint.Value, endPoint.Value);
            foreach (Control item in flowCanvas.Children)
            {
                Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                Rect itemBounds = item.TransformToAncestor(flowCanvas).TransformBounds(itemRect);

                if (rubberBand.Contains(itemBounds) && item is ISelectable)
                {
                    ISelectable selectableItem = item as ISelectable;
                    selectableItem.IsSelected = true;
                    flowCanvas.SelectedItems.Add(selectableItem);
                }
            }

        }
    }
}
