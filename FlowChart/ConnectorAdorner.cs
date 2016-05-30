using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System;


namespace FlowChart
{
    public class ConnectorAdorner : Adorner
    {
        private PathGeometry pathGeometry;
        private FlowCanvas flowCanvas;
        private Connector sourceConnector;
        private Pen drawingPen;
        private FlowItem hitFlowItem;
        private FlowItem HitFlowItem
        {
            get { return hitFlowItem; }
            set
            {
                if (hitFlowItem != value)
                {
                    if (hitFlowItem != null)
                        hitFlowItem.IsDragConnectionOver = false;

                    hitFlowItem = value;

                    if (hitFlowItem != null)
                        hitFlowItem.IsDragConnectionOver = true;
                }
            }
        }

        private Connector hitConnector;
        private Connector HitConnector
        {
            get { return hitConnector; }
            set
            {
                if (hitConnector != value)
                {
                    hitConnector = value;
                }
            }
        }

        public ConnectorAdorner(FlowCanvas designer, Connector sourceConnector)
            : base(designer)
        {
            this.flowCanvas = designer;
            this.sourceConnector = sourceConnector;
            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            drawingPen.LineJoin = PenLineJoin.Round;
            this.Cursor = Cursors.Cross;
        }
        /// <summary>
        /// 鼠标谈起事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            bool IsExitConnections = true;
            if (HitConnector != null)
            {
                Connector sourceConnector = this.sourceConnector; //原组件
                Connector sinkConnector = this.HitConnector;//目标组件

                foreach (var item in this.flowCanvas.Children)
                {
                    if (item is Connection)
                    {
                        var sinkItem = item as Connection;
                        if (sinkItem.Sink.ParentFlowItem == sinkConnector.ParentFlowItem)
                        {
                            if (sinkItem.Sink.Name == sinkConnector.Name)
                            {
                                IsExitConnections = false;
                                MessageBox.Show("这两点之间已经存在线条，请换其他节点！", "提示框");
                                break;
                            }
                        }

                    }
                }
                bool _existParentElement = TraversalParentElement(sourceConnector.ParentFlowItem, sinkConnector.ParentFlowItem);
                if (_existParentElement)
                {
                    IsExitConnections = false;
                    MessageBox.Show("该操作违反流程配置原则", "提示框");
                }
                if (IsExitConnections)
                {
                    //var tmp = sourceConnector.FindVisualTreeAncestor(x => x is FlowItem) as FlowItem;
                    Connection newConnection = new Connection(sourceConnector, sinkConnector);//在原组件和目标组件之间画一条线
                    this.flowCanvas.Children.Add(newConnection);
                    Canvas.SetZIndex(newConnection, 2);
                    if (sinkConnector.ParentFlowItem != null)
                    {
                        if (sinkConnector.ParentFlowItem.IsDragConnectionOver == true)
                        {
                            sinkConnector.ParentFlowItem.ParentSerialNumber = sourceConnector.ParentFlowItem.CurrentSerialNumber;
                        }
                    }
                }
            }
            if (HitFlowItem != null)
            {
                this.HitFlowItem.IsDragConnectionOver = false;
            }

            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.flowCanvas);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
            }
        }
        FlowItem TempFlowItem = null;
        /// <summary>
        /// 递归查找流程线条，线条是否已经连接。       
        /// </summary>
        /// <param name="SourceFlowItem">原元素</param>
        /// <param name="HitFlowItem">目标元素</param>
        /// <returns>True-存在，False-不存在</returns>
        public bool TraversalParentElement(FlowItem SourceFlowItem, FlowItem HitFlowItem)
        {
            bool result = false;
            foreach (var item in this.flowCanvas.Children)
            {
                if (item is Connection)
                {
                    var sinkItem = item as Connection;
                    if (sinkItem.Source.ParentFlowItem == hitFlowItem && sinkItem.Sink.ParentFlowItem == SourceFlowItem)//判断相邻两个元素是否存在线条
                    {
                        if (sinkItem.Source.ParentFlowItem == hitFlowItem && sinkItem.Sink.ParentFlowItem == SourceFlowItem)
                        {
                            result = true;
                            break;
                        }
                    }
                    if (TempFlowItem != null)
                    {
                        if (TempFlowItem == hitFlowItem)
                        {
                            result = true;
                            break;
                        }
                    }

                    if (sinkItem.Sink.ParentFlowItem == SourceFlowItem)//判断不相邻两个元素是否存在线条
                    {
                        if (sinkItem.Source.ParentFlowItem != hitFlowItem)
                        {
                            TempFlowItem = sinkItem.Source.ParentFlowItem;
                            result = this.TraversalParentElement(sinkItem.Source.ParentFlowItem, hitFlowItem);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured) this.CaptureMouse();
                HitTesting(e.GetPosition(this));
                this.pathGeometry = GetPathGeometry(e.GetPosition(this));
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, drawingPen, this.pathGeometry);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        private PathGeometry GetPathGeometry(Point position)
        {
            PathGeometry geometry = new PathGeometry();
            ConnectorOrientation targetOrientation;
            if (HitConnector != null)
                targetOrientation = HitConnector.Orientation;
            else
                targetOrientation = ConnectorOrientation.None;

            List<Point> pathPoints = PathFinder.GetConnectionLine(sourceConnector.GetInfo(), position, targetOrientation);

            if (pathPoints.Count > 0)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = pathPoints[0];
                pathPoints.Remove(pathPoints[0]);
                figure.Segments.Add(new LineSegment(position, true));
                geometry.Figures.Add(figure);
            }
            return geometry;
        }
        /// <summary>
        /// 测试是否已经命中元素
        /// </summary>
        /// <param name="hitPoint">鼠标点击的坐标</param>
        private void HitTesting(Point hitPoint)
        {
            bool hitConnectorFlag = false;
            DependencyObject hitObject = flowCanvas.InputHitTest(hitPoint) as DependencyObject;
            while (hitObject != null &&
                   hitObject != sourceConnector.ParentFlowItem &&
                   hitObject.GetType() != typeof(FlowCanvas))
            {
                if (hitObject is Connector)
                {
                    HitConnector = hitObject as Connector;
                    hitConnectorFlag = true;
                }

                if (hitObject is FlowItem)
                {
                    HitFlowItem = hitObject as FlowItem;
                    //var parentitem = hitFlowItem.FindVisualTreeAncestor(x => x is Connector);
                    Control connectorDecorator = HitFlowItem.Template.FindName("PART_ConnectorDecorator", HitFlowItem) as Control;
                    List<DependencyObject> item = connectorDecorator.FindVisualTreeChildren(x => x is Connector);

                    foreach (var connector in item)
                    {
                        var data = connector as Connector;
                        if (Math.Abs(data.Position.X - hitPoint.X) < 18 && Math.Abs(data.Position.Y - hitPoint.Y) < 18)
                        {
                            hitConnector = data;
                            hitConnectorFlag = true;
                            break;
                        }
                    }
                    if (!hitConnectorFlag)
                    {
                        HitConnector = null;
                    }
                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
            HitFlowItem = null;
        }
    }
}