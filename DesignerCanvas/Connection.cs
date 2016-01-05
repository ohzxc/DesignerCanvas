using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using DesignerCanvas.Controls;

namespace DesignerCanvas
{
    
    public class Connection : Control, ISelectable, INotifyPropertyChanged
    {
        #region 字段
        /// <summary>
        /// 定义连接线装饰器
        /// </summary>
        private Adorner connectionAdorner;
        #endregion

        #region 属性
        /// <summary>
        /// 定义source源
        /// </summary>
        private Connector source;
        public Connector Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    if (source != null)
                    {
                        source.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
                        source.Connections.Remove(this);
                    }

                    source = value;

                    if (source != null)
                    {
                        source.Connections.Add(this);
                        source.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
                    }

                    UpdatePathGeometry();
                }
            }
        }

        /// <summary>
        /// 定义sink
        /// </summary>
        private Connector sink;
        public Connector Sink
        {
            get { return sink; }
            set
            {
                if (sink != value)
                {
                    if (sink != null)
                    {
                        sink.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
                        sink.Connections.Remove(this);
                    }

                    sink = value;

                    if (sink != null)
                    {
                        sink.Connections.Add(this);
                        sink.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
                    }
                    UpdatePathGeometry();
                }
            }
        }

        //定义一个画弧形的对象
        private PathGeometry pathGeometry;
        public PathGeometry PathGeometry
        {
            get { return pathGeometry; }
            set
            {
                if (pathGeometry != value)
                {
                    pathGeometry = value;
                    UpdateAnchorPosition();
                    OnPropertyChanged("PathGeometry");
                }
            }
        }

        /// <summary>
        /// 定义开始点和结束点之间的位置
        /// </summary>
        private Point anchorPositionSource;
        public Point AnchorPositionSource
        {
            get { return anchorPositionSource; }
            set
            {
                if (anchorPositionSource != value)
                {
                    anchorPositionSource = value;
                    OnPropertyChanged("AnchorPositionSource");
                }
            }
        }

        /// <summary>
        /// 在固定位置的路径
        /// 箭头旋转角度
        /// </summary>
        private double anchorAngleSource = 0;
        public double AnchorAngleSource
        {
            get { return anchorAngleSource; }
            set
            {
                if (anchorAngleSource != value)
                {
                    anchorAngleSource = value;
                    OnPropertyChanged("AnchorAngleSource");
                }
            }
        }

        /// <summary>
        /// 线条的影
        /// </summary>
        private Point anchorPositionSink;
        public Point AnchorPositionSink
        {
            get { return anchorPositionSink; }
            set
            {
                if (anchorPositionSink != value)
                {
                    anchorPositionSink = value;
                    OnPropertyChanged("AnchorPositionSink");
                }
            }
        }
        /// <summary>
        /// 线条的投影源
        /// </summary>
        private double anchorAngleSink = 0;
        public double AnchorAngleSink
        {
            get { return anchorAngleSink; }
            set
            {
                if (anchorAngleSink != value)
                {
                    anchorAngleSink = value;
                    OnPropertyChanged("AnchorAngleSink");
                }
            }
        }
        /// <summary>
        /// 投影箭头的枚举
        /// </summary>
        private ArrowSymbol sourceArrowSymbol = ArrowSymbol.None;
        public ArrowSymbol SourceArrowSymbol
        {
            get { return sourceArrowSymbol; }
            set
            {
                if (sourceArrowSymbol != value)
                {
                    sourceArrowSymbol = value;
                    OnPropertyChanged("SourceArrowSymbol");
                }
            }
        }
        /// <summary>
        /// 箭头的枚举
        /// </summary>
        public ArrowSymbol sinkArrowSymbol = ArrowSymbol.Arrow;
        public ArrowSymbol SinkArrowSymbol
        {
            get { return sinkArrowSymbol; }
            set
            {
                if (sinkArrowSymbol != value)
                {
                    sinkArrowSymbol = value;
                    OnPropertyChanged("SinkArrowSymbol");
                }
            }
        }

        // 在线条上定义一个文本框
        private Point labelPosition;
        public Point LabelPosition
        {
            get { return labelPosition; }
            set
            {
                if (labelPosition != value)
                {
                    labelPosition = value;
                    OnPropertyChanged("LabelPosition");
                }
            }
        }
        private List<string> _brach = new List<string>();
        /// <summary>
        /// 分支的数据
        /// </summary>
        public List<string> Brach
        {
            get
            {
                return _brach;
            }
            set
            {
                if (_brach != value)
                {
                    _brach = value;
                    OnPropertyChanged("Brach");
                }
            }
        }
        private string _selectedBrach = "";
        /// <summary>
        /// 分支的数据
        /// </summary>
        public string SelectedBrach
        {
            get
            {
                return _selectedBrach;
            }
            set
            {
                if (_selectedBrach != value)
                {
                    _selectedBrach = value;
                    OnPropertyChanged("SelectedBrach");
                }
            }
        }
        /// <summary>
        /// 线条之间的距离，是用来勾勒的连接路径模式
        /// </summary>
        private DoubleCollection strokeDashArray;
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return strokeDashArray;
            }
            set
            {
                if (strokeDashArray != value)
                {
                    strokeDashArray = value;
                    OnPropertyChanged("StrokeDashArray");
                }
            }
        }
        //如果连接上,ConnectionAdorner变得可见
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (isSelected)
                        ShowAdorner();
                    else
                        HideAdorner();
                }
            }
        }
        //public List<SubRel> _subRelList = null;
        ///// <summary>
        ///// 映射关系集合
        ///// </summary>
        //public List<SubRel> SubRelList
        //{
        //    get
        //    {
        //        if (_subRelList == null)
        //        {
        //            _subRelList = new List<SubRel>();
        //        }
        //        return _subRelList;
        //    }
        //    set
        //    {
        //        _subRelList = value;
        //    }
        //}
        #endregion

        #region 构造函数
        public Connection(Connector source, Connector sink, string strContent , List<string> m_Connection)
        {
            Source = source;
            Sink = sink;
            Brach = m_Connection;
            SelectedBrach = strContent;
            base.Loaded += new RoutedEventHandler(Connection_Loaded);
            base.Unloaded += new RoutedEventHandler(Connection_Unloaded);
        }
        public Connection(Connector source, Connector sink, List<string> m_Connection)
        {
            Source = source;
            Sink = sink;
            Brach = m_Connection;
            base.Loaded += new RoutedEventHandler(Connection_Loaded);
            base.Unloaded += new RoutedEventHandler(Connection_Unloaded);
        }
        #endregion

        /// <summary>
        /// 鼠标单击的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            MyCanvas designer = VisualTreeHelper.GetParent(this) as MyCanvas;
           // designer.Focus();
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                {
                    if (this.IsSelected)
                    {
                        this.IsSelected = false;
                        designer.SelectedItems.Remove(this);
                    }
                    else
                    {
                        this.IsSelected = true;
                        designer.SelectedItems.Add(this);
                    }
                }
                else if (!this.IsSelected)
                {
                    foreach (ISelectable item in designer.SelectedItems)
                        item.IsSelected = false;

                    designer.SelectedItems.Clear();
                    this.IsSelected = true;
                    designer.SelectedItems.Add(this);
                }
            }
            e.Handled = false;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            //this.ToolTip = "双击线条开始配置映射";
            e.Handled = false;

        }
        
        void OnConnectorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            //当线条位置改变后，必须更新相应的位置
            if (e.PropertyName.Equals("Position"))
            {
                UpdatePathGeometry();
            }
        }

        private void UpdatePathGeometry()
        { 
            if (Source != null && Sink != null)
            {
                PathGeometry geometry = new PathGeometry();
                List<Point> linePoints = PathFinder.GetConnectionLine(Source.GetInfo(), Sink.GetInfo(), true);
                if (linePoints.Count > 0)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = linePoints[0];
                    linePoints.Remove(linePoints[0]);
                    figure.Segments.Add(new LineSegment(sink.Position, true));
                    geometry.Figures.Add(figure);

                    this.PathGeometry = geometry;
                }
            }
        }

        private void UpdateAnchorPosition()
        {

            Point pathStartPoint, pathTangentAtStartPoint;
            Point pathEndPoint, pathTangentAtEndPoint;
            Point pathMidPoint, pathTangentAtMidPoint;

            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector 
            // on PathGeometry at the specified fraction of its length
            this.PathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
            this.PathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);
            this.PathGeometry.GetPointAtFractionLength(0.7, out pathMidPoint, out pathTangentAtMidPoint);

            // get angle from tangent vector
            this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            // add some margin on source and sink side for visual reasons only
            pathStartPoint.Offset(-pathTangentAtStartPoint.X * 2, -pathTangentAtStartPoint.Y * 2);
            pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
            this.LabelPosition = new Point(pathMidPoint.X-25,pathMidPoint.Y);
        }

        private void ShowAdorner()
        {
            //创建一个ConnectionAdorner装饰器
            if (this.connectionAdorner == null)
            {
                MyCanvas designer = VisualTreeHelper.GetParent(this) as MyCanvas;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designer);
                if (adornerLayer != null)
                {
                    this.connectionAdorner = new ConnectionAdorner(designer, this);
                    adornerLayer.Add(this.connectionAdorner);
                }
            }
            this.connectionAdorner.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// 隐藏装饰器
        /// </summary>
        internal void HideAdorner()
        {
            if (this.connectionAdorner != null)
                this.connectionAdorner.Visibility = Visibility.Collapsed;
        }
        void Connection_Loaded(object sender, RoutedEventArgs e)
        {
            source.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
            sink.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
            // remove adorner
            if (this.connectionAdorner != null)
            {
                MyCanvas designer = VisualTreeHelper.GetParent(this) as MyCanvas;
                if (designer == null)
                    return;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designer);
                if (adornerLayer != null)
                {
                    adornerLayer.Add(this.connectionAdorner);
                    this.connectionAdorner = null;
                }
            }
        }
        /// <summary>
        /// 销毁线条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Connection_Unloaded(object sender, RoutedEventArgs e)
        {

            source.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
            sink.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
            // remove adorner
            if (this.connectionAdorner != null)
            {
                MyCanvas designer = VisualTreeHelper.GetParent(this) as MyCanvas;
                if (designer == null)
                    return;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designer);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.connectionAdorner);
                    this.connectionAdorner = null;
                }
            }
        }

        /// <summary>
        /// 弹出映射关系界面
        /// </summary>
        /// <param name="tx_code">交易编号</param>
        /// <param name="comp_code">组件编号</param>
        /// <param name="serialnumber">序号</param>
        /// <param name="serialnumber">数据域字典列表</param>
        /// <param name="serialnumber">输入数据类型列表</param>
        /// <param name="subRelList">映射关系列表</param>
        /// <returns>SubRel列表</returns>
        

        #region 实现INotifyPropertyChanged 成员
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

    public enum ArrowSymbol
            {
                None,
                Arrow,
                Diamond
            }

}