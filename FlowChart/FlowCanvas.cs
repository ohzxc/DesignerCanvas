using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using FlowChart.Interfaces;

namespace FlowChart
{
    /// <summary>
    /// 设计画布
    /// </summary>
    public class FlowCanvas : Canvas
    {
        #region 字段
        /// <summary>
        /// 拖拽操作的起始点
        /// </summary>
        private Point? rubberbandSelectionStartPoint;
        /// <summary>
        /// 容器的样式
        /// </summary>
        //private static object DesignerStyle = null;
        /// <summary>
        /// 界面元素排序层数
        /// </summary>
        private int _layOutCount = 0;
        /// <summary>
        /// 递增序列号
        /// </summary>
        private int _index = 0;
        /// <summary>
        /// 开始节点X轴偏移位置
        /// </summary>
        private const float _startX = 30f;
        /// <summary>
        /// 开始节点Y轴偏移位置
        /// </summary>
        private const float _startY = 30f;
        /// <summary>
        /// 相邻元素之间的偏移量
        /// </summary>
        private const float _offsetX = 150f;
        /// <summary>
        /// 相邻元素之间的偏移量
        /// </summary>
        private const float _offsetY = 150f;
        /// <summary>
        /// 在界面最左边的节点图形的X坐标
        /// </summary>
        private float _leftestX = float.Epsilon;
        /// <summary>
        /// 组件的条件集合的集合,可能占用太多内存
        /// </summary>
        //public Dictionary<string, Dictionary<string, string>> m_Connection = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 确定位置后的交易节点集合
        /// </summary>
        //private List<TxNode> m_LstNodes;
        /// <summary>
        /// 流程集合列表
        /// </summary>
        //private List<TradeFlow> flowList;
        /// <summary>
        /// 递增序列号
        /// </summary>
        public List<int> SerialNumber;
        /// <summary>
        /// 交易流程中的结点
        /// </summary>
        //private List<Tx_Node> m_Nodes;
        /// <summary>
        /// 交易流程中的所有头结点
        /// </summary>
        //private List<Tx_Node> m_LstHeadNode;
        /// <summary>
        /// 交易流程中的线条集合
        /// </summary>
        //private List<Tx_Entry> m_LstEntrys;
        private List<ISelectable> selectedItems;
        #endregion

        #region 属性
        /// <summary>
        /// 初始化时所有组件的位置
        /// </summary>
        public List<Point> PointList { get; set; }
        /// <summary>
        /// 选中设计元素的集合
        /// </summary>
        public List<ISelectable> SelectedItems
        {
            get
            {
                if (selectedItems == null)
                    selectedItems = new List<ISelectable>();
                return selectedItems;
            }
            set
            {
                selectedItems = value;
            }
        }
        #endregion

        #region 构造函数
        public FlowCanvas()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true) return;
            //容器可进行拖拽操作
            AllowDrop = true;
            //ResourceDictionary anmationResource = new ResourceDictionary();
            //anmationResource.Source = new Uri("/TellerSystem.Controls.Ext;component/Controls/DiagramDesigner2/Resources/SimpleControlStyles.xaml", UriKind.RelativeOrAbsolute);
            //Application.Current.Resources.MergedDictionaries.Add(anmationResource);
            //DesignerStyle = Application.Current.Resources["SimpleDesignerStyle"];

        }
        #endregion

        #region 事件
        /// <summary>
        /// 鼠标按下的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                this.Focus();
                //获取鼠标在容器点击的位置坐标
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));
                //TODO 清空选中项
                //SelectService.ClearSelection();
                //如果直接点击画布,取消选中的元素
                foreach (ISelectable item in SelectedItems)
                    item.IsSelected = false;
                selectedItems.Clear();
            }
            e.Handled = true;
        }
        /// <summary>
        /// 鼠标划过触发的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //如果鼠标没有按下左键，代表没有拖拽操作
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            //但鼠标按钮按下，并且对象有值
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                //创建一个装饰器 
                this.Focus();
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// 鼠标拖拽触发的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrop(DragEventArgs e)
        {
            //base.OnDrop(e);
            //this.Focus();
            //DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            //if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
            //{
            //    DesignerItem newItem = null;
            //    Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

            //    if (content != null)
            //    {
            //        int maxValue = value++;
            //        newItem = new DesignerItem(TradeCode, maxValue.ToString()); newItem.Content = content;
            //        var lbl = content as Label;
            //        if (lbl != null)
            //        {
            //            newItem.ToolTip = lbl.ToolTip;
            //        }

            //        Point position = e.GetPosition(this);
            //        if (dragObject.DesiredSize.HasValue)
            //        {
            //            Size desiredSize = dragObject.DesiredSize.Value;
            //            newItem.Width = desiredSize.Width;
            //            newItem.Height = desiredSize.Height;

            //            MyCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
            //            MyCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
            //        }
            //        else
            //        {
            //            MyCanvas.SetLeft(newItem, Math.Max(0, position.X));
            //            MyCanvas.SetTop(newItem, Math.Max(0, position.Y));
            //        }

            //        this.Children.Add(newItem);

            //        //更新选中项
            //        foreach (ISelectable item in this.SelectedItems)
            //            item.IsSelected = false;
            //        SelectedItems.Clear();
            //        newItem.IsSelected = true;
            //        this.SelectedItems.Add(newItem);
            //    }

            //    e.Handled = true;
            }
        
        #endregion

    }
}
