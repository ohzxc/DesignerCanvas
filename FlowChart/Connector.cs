using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace FlowChart
{
    public class Connector : Control, INotifyPropertyChanged
    {
        #region 属性
        // 在设计容器拖拽的起始位置
        private Point? dragStartPoint = null;

        public ConnectorOrientation Orientation { get; set; }

        //相对于DesignerCanvas的中心位置
        private Point position;
        public Point Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged("Position");
                }
            }
        }
        //设置designeritem模板
        private FlowItem parentFlowItem;
        public FlowItem ParentFlowItem
        {
            get
            {
                if (parentFlowItem == null)
                    parentFlowItem = this.DataContext as FlowItem;

                return parentFlowItem;
            }
        }

        //连接线条集合
        private List<Connection> connections;
        public List<Connection> Connections
        {
            get
            {
                if (connections == null)
                    connections = new List<Connection>();
                return connections;
            }
        }
        #endregion

        public Connector()
        {
            base.LayoutUpdated += new EventHandler(Connector_LayoutUpdated);
        }

        // 当布局更改时更行相对位置
        void Connector_LayoutUpdated(object sender, EventArgs e)
        {
            FlowCanvas designer = GetDesignerCanvas(this);
            if (designer != null)
            {
                this.Position = this.TransformToAncestor(designer).Transform(new Point(this.Width / 2, this.Height / 2));
            }


        }
        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            FlowCanvas canvas = GetDesignerCanvas(this);
            if (canvas != null)
            {
                // 相对于DesignerCanvas的位置
                this.dragStartPoint = new Point?(e.GetPosition(canvas));
                e.Handled = true;
            }
        }
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;
            if (this.dragStartPoint.HasValue)
            {
                //创建连接装饰器 
                var canvas = GetDesignerCanvas(this);
                if (canvas != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                    if (adornerLayer != null)
                    {
                        var adorner = new ConnectorAdorner(canvas, this);
                        if (adorner != null)
                        {
                            adornerLayer.Add(adorner);
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        internal ConnectorInfo GetInfo()
        {
            ConnectorInfo info = new ConnectorInfo();
            info.DesignerItemLeft = FlowCanvas.GetLeft(this.ParentFlowItem);
            info.DesignerItemTop = FlowCanvas.GetTop(this.ParentFlowItem);
            info.DesignerItemSize = new Size(this.ParentFlowItem.ActualWidth, this.ParentFlowItem.ActualHeight);
            info.Orientation = this.Orientation;
            info.Position = this.Position;
            return info;
        }

        /// <summary>
        /// 迭代通过视觉树查找父designercanvas容器中元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private FlowCanvas GetDesignerCanvas(DependencyObject element)
        {
            while (element != null && !(element is FlowCanvas))
                element = VisualTreeHelper.GetParent(element);

            return element as FlowCanvas;
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    /// <summary>
    /// 定义一个连接器结构
    /// </summary>
    internal struct ConnectorInfo
    {
        public double DesignerItemLeft { get; set; }
        public double DesignerItemTop { get; set; }
        public Size DesignerItemSize { get; set; }
        public Point Position { get; set; }
        public ConnectorOrientation Orientation { get; set; }
    }
    /// <summary>
    /// 连接线枚举
    /// </summary>
    public enum ConnectorOrientation
    {
        None,
        Left,
        Top,
        Right,
        Bottom
    }
}
