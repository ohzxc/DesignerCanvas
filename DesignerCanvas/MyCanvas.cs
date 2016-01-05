using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Windows.Media;
using Microsoft.Win32;
using System.Linq;
using System.ComponentModel;
using System.Text;

namespace DesignerCanvas
{
    /// <summary>
    /// 设计组件容器
    /// </summary>
    public class MyCanvas : Canvas
    {

        #region 字段
        /// <summary>
        /// 拖拽操作的起始点
        /// </summary>
        private Point? rubberbandSelectionStartPoint = null;
        /// <summary>
        /// 容器的样式
        /// </summary>
        private static object DesignerStyle = null;
        /// <summary>
        /// 界面元素排序层数
        /// </summary>
        private int m_LayOutCount = 0;
        /// <summary>
        /// 递增序列号
        /// </summary>
        private int value = 0;
        /// <summary>
        /// 开始节点X轴偏移位置
        /// </summary>
        private const float mc_START_X = 30f;
        /// <summary>
        /// 开始节点Y轴偏移位置
        /// </summary>
        private const float mc_START_Y = 30f;
        /// <summary>
        /// 相邻元素之间的偏移量
        /// </summary>
        private const float mc_OFFSET_X = 150f;
        /// <summary>
        /// 相邻元素之间的偏移量
        /// </summary>
        private const float mc_OFFSET_Y = 150f;
        /// <summary>
        /// 在界面最左边的节点图形的X坐标
        /// </summary>
        private float m_Leftest_X = float.Epsilon;
        /// <summary>
        /// 组件的条件集合的集合,可能占用太多内存
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> m_Connection = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 确定位置后的交易节点集合
        /// </summary>
        private List<Tx_Node> m_LstNodes;
        /// <summary>
        /// 流程集合列表
        /// </summary>
        private List<TradeFlow> flowList;
        /// <summary>
        /// 递增序列号
        /// </summary>
        public List<int> SerialNumber = new List<int>();
        /// <summary>
        /// 交易流程中的结点
        /// </summary>
        private List<Tx_Node> m_Nodes;
        /// <summary>
        /// 交易流程中的所有头结点
        /// </summary>
        //private List<Tx_Noqde> m_LstHeadNode;
        /// <summary>
        /// 交易流程中的线条集合
        /// </summary>
        private List<Tx_Entry> m_LstEntrys;
        /// <summary>
        /// 临时记录节点分支，比如：0-1,2 1-3,4 2-5,6,此时记录1,2,3,4,5,6,，一层一层的记录，知道循环找到所有的位置坐标
        /// </summary>
        //private List<Tx_Node> m_LayOutNextNodes;
        private SelectionService _selectService;
        private List<ISelectable> selectedItems;
        //public List<DataDic> _dataFields = new List<DataDic>();
        //public List<InDataType> _inDataTypeList = new List<InDataType>();
        #endregion

        #region 属性
        /// <summary>
        /// 初始化时所有组件的位置
        /// </summary>
        public List<Point> pointList { get; set; }
        ///<summary>
        ///定义操作DesignerCanvas的方法
        ///</summary>
        public SelectionService SelectService
        {
            get
            {
                if (_selectService == null)
                    _selectService = new SelectionService(this);

                return _selectService;
            }
        }
        // 选中设计元素的集合       
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
        /// <summary>
        /// 交易编码
        /// </summary>
        public string TradeCode
        {
            get;
            set;
        }
        ///// <summary>
        ///// 数据域集合
        ///// </summary>
        //public List<DataDic> DataFields
        //{
        //    get
        //    {
        //        return _dataFields;
        //    }
        //}
        ///// <summary>
        ///// 输入数据域类型
        ///// </summary>
        //public List<InDataType> InDataTypeList
        //{
        //    get
        //    {
        //        return _inDataTypeList;
        //    }
        //}
        #endregion

        #region 构造函数
        public MyCanvas()
        {
            //容器可进行拖拽操作
            this.AllowDrop = true;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true)
            {
                return;
            }
            ResourceDictionary anmationResource = new ResourceDictionary();
            anmationResource.Source = new Uri("/DesignerCanvas;component/Resources/SimpleControlStyles.xaml", UriKind.RelativeOrAbsolute);
            Application.Current.Resources.MergedDictionaries.Add(anmationResource);
            DesignerStyle = Application.Current.Resources["SimpleDesignerStyle"];
            ////初始化数据域
            //if (DataFields.Count() < 1)
            //{
            //    InitDataDic();
            //}
            ////初始化输出域类型
            //if (InDataTypeList.Count() < 1)
            //{
            //    InitInDataType();
            //}
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
                SelectService.ClearSelection();
                //如果直接点击画布,取消选中的元素
                foreach (ISelectable item in SelectedItems)
                    item.IsSelected = false;
                selectedItems.Clear();
            }
            e.Handled = true;
        }
        /// <summary>
        /// 键盘按键触发的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Delete)
            {
                MessageBoxResult result = MessageBox.Show("确认删除选中的组件吗?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (SelectService.CurrentSelection.Count < 1)
                    {
                        this.SelectedItems.ForEach(x =>
                        {
                            SelectService.AddToSelection(x);
                        });
                    }
                    this.Delete();
                }
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {
                this.SelectAll();
            }
            e.Handled = false;
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
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
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
            base.OnDrop(e);
            this.Focus();
            DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
            {
                DesignerItem newItem = null;
                Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

                if (content != null)
                {
                    int maxValue = value++;
                    newItem = new DesignerItem(TradeCode,maxValue.ToString());
                    newItem.Content = content;
                    var lbl = content as Label;
                    if (lbl != null)
                    {
                        newItem.ToolTip = lbl.ToolTip;
                    }

                    Point position = e.GetPosition(this);
                    if (dragObject.DesiredSize.HasValue)
                    {
                        Size desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        MyCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                        MyCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                    }
                    else
                    {
                        MyCanvas.SetLeft(newItem, Math.Max(0, position.X));
                        MyCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    }

                    this.Children.Add(newItem);

                    //更新选中项
                    foreach (ISelectable item in this.SelectedItems)
                        item.IsSelected = false;
                    SelectedItems.Clear();
                    newItem.IsSelected = true;
                    this.SelectedItems.Add(newItem);
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// 重新绘制元素的尺寸
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            foreach (UIElement element in base.Children)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 打开流程
        /// </summary>
        /// <param name="tradeFlowListEntity">交易流程实体层</param>
        /// <param name="typeList">组件类型列表</param>
        /// <param name="subRelListEntity">映射关系实体层</param>
        
        public void Open(List<T_plt_tradeFlowEntity> tradeFlowListEntity, List<Label> typeList, List<T_plt_subrelEntity> subRelListEntity)
        {
            #region 初始化
            
            flowList = new List<TradeFlow>(); //流程实例列表
            var sublist = new List<SubRel>();//参数关系列表
            //List<string> connectionOverList = new List<string>();//零时序号列表

            if (SerialNumber != null)
            {
                SerialNumber.Clear(); //清空序号
            }
            Children.Clear();//清空画布
            SelectService.ClearSelection();//清空画布引用关系
            #endregion
            #region 实体转换
            if (tradeFlowListEntity != null)
            {
                if (tradeFlowListEntity.Count() > 0)
                {
                    tradeFlowListEntity.ForEach(x5 =>
                    {
                        var flow = new TradeFlow();
                        flow.TradeCode = x5.TxCode.Trim();
                        flow.CompCode = x5.CompCode.Trim();
                        flow.Trade_Flow = x5.TradeFlow.Trim();
                        flow.FlowCode = x5.FlowCode.Trim();
                        flowList.Add(flow);
                    });
                }
            }
            if (subRelListEntity != null)
            {
                if (subRelListEntity.Count() > 0)
                {
                    subRelListEntity.ForEach(x =>
                    {
                        var rel = new SubRel();
                        rel.TradeCode = x.tx_code.Trim();
                        rel.CompCode = x.comp_code.Trim();
                        rel.OutData = x.out_data.Trim();
                        rel.InType = x.in_data_type.Trim()=="1"?TypeOpt.常量:TypeOpt.表达式;
                        rel.InData = x.in_data.Trim();
                        rel.Memo = x.memo.Trim();
                        rel.SerialNumber = x.flow_no.Trim();
                        sublist.Add(rel);
                    });
                }
            }
            #endregion

            if (flowList.Count() < 1) return;   //如果不存在交易流程，返回

            #region item和线的处理
            LayOut();//对容器的元素进行布局
            var i = 0;
            foreach (Tx_Node node in m_LstNodes)
            {
                SerialNumber.Add(Convert.ToInt32(node.Flow_code)); //组件
                var lbl = typeList.Where(z => z.Tag.ToString().Trim() == node.Component_code.Trim()).FirstOrDefault();
                if (lbl != null)
                {
                    var tmp=new List<SubRel>();
                    sublist.ForEach(x =>
                    {
                        if (x.SerialNumber == node.Flow_code) tmp.Add(x);
                    });
                    var item = DeserializeDesignerItem(TradeCode,node.Flow_code, node.X, node.Y, lbl, m_Connection[node.Component_code], tmp);
                    if (!string.IsNullOrWhiteSpace(node.Sub_tx_code.Trim()))
                        item.IsDragConnectionOver = true;
                    Canvas.SetZIndex(item, 1);
                    this.Children.Add(item);
                    ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item) as ControlTemplate;
                    DesignerItem.SetConnectorDecoratorTemplate(item, template);
                    
                }
                i++;
            }

            this.InvalidateVisual();//使元素的呈现无效，并强制执行完整的新布局处理过程。布局循环完成后
            if (m_LstEntrys == null)
            {
                return;
            }

            foreach (Tx_Entry entry in m_LstEntrys)
            {
                string sourceID = entry.StartNode;
                string sinkID = entry.EndNode;
                if (!string.IsNullOrEmpty(sinkID) && !string.IsNullOrEmpty(sourceID))
                {
                    var sourceConnector = GetConnector(sourceID, 6);
                    var sinkConnector = GetConnector(sinkID, 2);
                    var connection = new Connection(sourceConnector, sinkConnector, entry.Condition, entry.ConditionList);
                    Canvas.SetZIndex(connection, 2);
                    this.Children.Add(connection);
                }
            }
            #endregion
            //value = SerialNumber.OrderByDescending(x2 => x2).FirstOrDefault() + 1;//出
            UpdateZIndex();
        }
        /// <summary>
        /// 布局算法 确定节点位置
        /// </summary>
        public void LayOut()
        {
            m_LayOutCount = 0;
            m_LstNodes = new List<Tx_Node>();
            m_Nodes = new List<Tx_Node>();
            m_LstEntrys = new List<Tx_Entry>();
            foreach (TradeFlow item in flowList)
            {
                var tradeFlowJoin = new StringBuilder();//串连交易流程
                string[] str = { "#", "=" };
                var _tradeFlow = item.Trade_Flow.Trim();
                var x3 = _tradeFlow.Remove(_tradeFlow.Length - 2, 2);
                var _s = x3.Split(str, StringSplitOptions.RemoveEmptyEntries);
                if (_s.Count() > 0)
                {
                    var i=0;
                    _s.ToList().ForEach(k =>
                    {
                        i++;
                        var m = k.Remove(k.Length - 1, 1);
                        var condition = m.Split('@')[0].ToString();
                        var subtxCode = m.Split('@')[1].ToString();
                        
                        tradeFlowJoin.Append(subtxCode);
                        if (_s.Count() > 1 && i < _s.Count())
                        {
                            tradeFlowJoin.Append('@');
                        }
                        
                        var entry = new Tx_Entry(item.FlowCode, subtxCode,condition, m_Connection[item.CompCode]);
                        m_LstEntrys.Add(entry);
                    });
                }
                if (tradeFlowJoin == null)
                    tradeFlowJoin.Append("");
                var node = new Tx_Node(item.FlowCode.Trim(), tradeFlowJoin.ToString(), item.CompCode.Trim())
                {
                    X=0.0f,
                    Y=0.0f
                };
                m_Nodes.Add(node);
            }
            if (m_Nodes.Count < 1)
            {
                return;
            }
            var firstNode = FindFirstNode();
            firstNode.X = mc_START_X;
            firstNode.Y = mc_START_Y;
            m_LstNodes.Add(firstNode);
            pointList = new List<Point>();
            pointList.Add(new Point(firstNode.X, firstNode.Y));
            m_LayOutCount++;
            LayOut_NextNodes(firstNode);
            //m_LstHeadNode = GetHeadNodes();
            //foreach (Tx_Node nextLayNode in m_Nodes)
            //{
            //    if (m_LstHeadNode != null)
            //    {
            //        //非起始子交易的头结点树的分布控制
            //        foreach (Tx_Node txNodeHaad in m_LstHeadNode)
            //        {
            //            if (txNodeHaad.Flow_code == nextLayNode.Flow_code)
            //            {
            //                txNodeHaad.X = mc_START_X + 250;
            //                txNodeHaad.Y = mc_START_Y;
            //                this.m_LayOutCount++;
            //                m_LstNodes.Add(txNodeHaad);
            //                this.LayOut_NextNodes(txNodeHaad);
            //            }
            //        }
            //    }
            //    this.LayOut_NextNodes(nextLayNode);
            //}
        }
        /// <summary>
        /// 找出交易流程的所有头结点
        /// </summary>
        /// <returns></returns>
        //public List<Tx_Node> GetHeadNodes()
        //{
        //    List<Tx_Node> headNodes = new List<Tx_Node>();

        //    foreach (Tx_Node node in m_Nodes)
        //    {
        //        bool found = false;
        //        foreach (Tx_Entry entry in this.m_LstEntrys)
        //        {
        //            if (entry.EndNode == node.Flow_code)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }

        //        if (!found)
        //        {
        //            headNodes.Add(node);
        //        }
        //    }
        //    return headNodes;
        //}
        /// <summary>
        /// 计算一个节点后续节点的位置算法
        /// </summary>
        /// <param name="nodeIndex"></param>
        public void LayOut_NextNodes(Tx_Node node)
        {
            //获取后续节点串
            var lstNextNodes = this.FindNextNodes(node);

            //若后续节点串为空或者已布局节点数等于全部节点数，则退出布局

            if (lstNextNodes == null || lstNextNodes.Count == 0)
            {
                return;
            }
            var half = 0f;

            //偶数个节点
            if (lstNextNodes.Count % 2 != 0)
            {
                half = lstNextNodes.Count / 2 - 1f;
            }
            else
            {
                half = lstNextNodes.Count / 2;
            }

            for (int i = 0; i < lstNextNodes.Count; i++)
            {
                //只对还没有定位的，进行定位;并对其后续节点定位
                if (lstNextNodes[i].X == 0 && lstNextNodes[i].Y == 0)
                {
                    lstNextNodes[i].Y = node.Y + mc_OFFSET_Y;
                    
                    lstNextNodes[i].X = node.X + (i - half) * mc_OFFSET_X;
                    if (lstNextNodes[i].X <30)
                    {
                        lstNextNodes[i].X = 30;
                    }
                    
                    if (!string.IsNullOrWhiteSpace(node.Sub_tx_code))
                    {

                        string[] sub_node = node.Sub_tx_code.Split('@');
                        //for (int y = 0; y < sub_node.Count(); y++)
                        //{
                        //    if (sub_node.Count() <= 1)
                        //    {
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        float offset = i - half == 0 ? 1 : i - half;
                        //        //lstNextNodes[i].X =node.X + offset * mc_OFFSET_X;
                        //    }
                        //    //if (lstNextNodes[i].Sub_tx_code == sub_node[y])
                        //    //{

                        //    //}
                        //}
                    }
                    pointList.ForEach(x =>
                    {
                        if (x.Y == lstNextNodes[i].Y)
                        {
                            if (x.X >= lstNextNodes[i].X)
                            {
                                lstNextNodes[i].X = (float)x.X + 150;
                            }
                        }
                    });
                    

                    m_LstNodes.Add(lstNextNodes[i]);
                    pointList.Add(new Point(lstNextNodes[i].X, lstNextNodes[i].Y));
                    //保存界面最左的图形节点的X坐标值
                    if (lstNextNodes[i].X < m_Leftest_X)
                    {
                        m_Leftest_X = lstNextNodes[i].X;
                    }
                    
                    if (lstNextNodes.Count() >= 1)
                    {
                        this.m_LayOutCount++;
                        //递归对后续节点进行布局
                        LayOut_NextNodes(lstNextNodes[i]);
                    }
                }
                
            }

            ////递归对后续节点进行布局
            //if (lstNextNodes.Count() > 1)
            //{
            //    this.m_LayOutCount++;
            //}
        }
        /// <summary>
        /// 找到第一个没有前导节点的节点
        /// </summary>
        /// <returns></returns>
        public Tx_Node FindFirstNode()
        {
            foreach (Tx_Node node in this.m_Nodes)
            {
                if (node.Flow_code == "0")
                {
                    return node;
                }
            }
            return null;
        }
        /// <summary>
        /// 查找指定节点的后续节点
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        public List<Tx_Node> FindNextNodes(Tx_Node node)
        {
            var lstNextNodes = new List<Tx_Node>();

            //根据线条查找后续节点串
            foreach (Tx_Entry entry in this.m_LstEntrys)
            {
                if (entry.StartNode == node.Flow_code)
                {
                    foreach (var nodes in m_Nodes)
                    {
                        if (nodes.Flow_code == entry.EndNode)
                        {
                            lstNextNodes.Add(nodes);
                        }
                    }
                }
            }
            return lstNextNodes;
        }
        /// <summary>
        /// 查找指定节点的前导节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<Tx_Node> FindPreNodes(Tx_Node node)
        {
            var subtxCode = "";
            List<Tx_Node> lstPreNodes = new List<Tx_Node>();
            //根据线条查找后续节点串
            foreach (Tx_Node entry in this.m_Nodes)
            {
                if (entry.Flow_code == node.Flow_code)
                {
                    subtxCode = node.Flow_code;
                    foreach (Tx_Node lstentry in this.m_LstNodes)
                    {
                        if (lstentry.Sub_tx_code == subtxCode)
                        {
                            lstPreNodes.Add(lstentry);
                        }
                    }
                }
            }
            return lstPreNodes;
        }
        /// <summary>
        /// 保存流程图
        /// </summary>
        public List<TradeFlow> Save()
        {
            List<TradeFlow> designerItemsXML = null;
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();
            var connectionsXML = SerializeConnections(connections);
            designerItemsXML = SerializeDesignerItems(designerItems, connectionsXML);
            return designerItemsXML;
        }
        /// <summary>
        /// 保存组件映射关系
        /// </summary>
        /// <returns></returns>
        public List<SubRel> SaveSubRel()
        {
            var list = new List<SubRel>();
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            foreach (var Items in designerItems)
            {
                if (Items.SubRelList.Count<1) return list;
                Items.SubRelList.ForEach(x =>
                {
                    list.Add(x);
                });
            }
            return list;
        }
        /// <summary>
        /// 删除当前选中集合
        /// </summary>
        public void Delete()
        {
            

            foreach (DesignerItem item in SelectService.CurrentSelection.OfType<DesignerItem>())
            {
                if (item.CurrentSerialNumber=="0")
                {
                    MessageBox.Show("不能删除头结点，请使用清空");
                    item.IsSelected = false;
                    SelectedItems.Clear();
                    SelectService.ClearSelection();
                   
                    return;
                }
                var result = true;
                var cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                var connectors = new List<Connector>();
                GetConnectors(cd, connectors);
                
                foreach (Connector connector in connectors)
                {
                    if (result)
                    {
                        foreach (Connection con in connector.Connections)
                        {
                            this.Children.Remove(con);
                        }
                    }

                }
                if (result)
                {
                    this.Children.Remove(item);
                }

            }
            foreach (Connection connection in SelectService.CurrentSelection.OfType<Connection>())
            {
                this.Children.Remove(connection);
            }
            SelectService.ClearSelection();
            UpdateZIndex();
        }
        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            this.SelectService.SelectAll();
        }

        public void Add(T_subTradeItem subTrade)
        {
            var tmp=new Label() { Tag = subTrade.subtxcode, Visibility = Visibility.Visible, Content = subTrade.name };
            var item = DeserializeDesignerItem(TradeCode,value.ToString(), 180, 30, tmp, subTrade.conditionDic);
            Canvas.SetZIndex(item, 1);
            this.Children.Add(item);
            //value++;
            UpdateZIndex();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 重置面板
        /// </summary>
        internal void reset()
        {
            Children.Clear();
            flowList.Clear();
            //m_Connection.Clear();
            m_Leftest_X = float.Epsilon;
            m_LstEntrys.Clear();
            m_LstNodes.Clear();
            m_Nodes.Clear();
            pointList.Clear();
            SerialNumber.Clear(); 
            SelectedItems.Clear();
            SelectService.ClearSelection();
            m_LayOutCount = 0;
            value = 0;

        }
        
        /// <summary>
        /// 解析组件
        /// </summary>
        /// <param name="designerItems"></param>
        /// <returns></returns>
        public List<TradeFlow> SerializeDesignerItems(IEnumerable<DesignerItem> designerItems, List<string> connections)
        {
            var serializedItems = new List<TradeFlow>();
            foreach (var item in designerItems)
            {
                TradeFlow flow = new TradeFlow();
                flow.TradeCode = this.TradeCode;
                flow.CompCode = item.ComponentCode;
                flow.FlowCode = item.CurrentSerialNumber;
                string trade_FlowCode = "";
                connections.ForEach(x =>
                {
                    string[] str = x.Split('%');
                    if (str[0].ToString().Trim() == item.CurrentSerialNumber.ToString().Trim())
                    {
                        trade_FlowCode += str[1].ToString();
                    }

                });
                flow.Trade_Flow = trade_FlowCode == "" ? "#$" : trade_FlowCode += "#$";
                serializedItems.Add(flow);
            }
            return serializedItems;
        }
        /// <summary>
        /// 解析线条
        /// </summary>
        /// <param name="connections"></param>
        /// <returns></returns>
        public List<string> SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new List<string>();
            foreach (var connection in connections)
            {
                var b="";
                var a = connection.Source.ParentDesignerItem.CurrentSerialNumber + "%";
                var c = "@" + connection.Sink.ParentDesignerItem.CurrentSerialNumber + "@";
                if (connection.SelectedBrach != null)
                {
                    b = "#=" + connection.SelectedBrach.Split(',')[0];
                    serializedConnections.Add(a + b + c);
                }
                else serializedConnections.Add("#$");
            }
            return serializedConnections;
        }

        /// <summary>
        /// 反解析交易流程
        /// </summary>
        /// <param name="itemXML">TradeFlow对象</param>
        /// <param name="key">当前流程ID</param>
        /// <param name="OffsetX">X轴偏移量</param>
        /// <param name="OffsetY">Y轴偏移量</param>
        /// <param name="lbl">DesignerItem对象</param>
        /// <returns>DesignerItem</returns>
        private static DesignerItem DeserializeDesignerItem(string tradecode,string key, float OffsetX, float OffsetY, Label lbl, Dictionary<string, string> m_Connection, List<SubRel> subRelList=null)
        {
            var lb = new Label();
            lb.IsHitTestVisible = false;
            lb.Tag = lbl.Tag;
            lb.Foreground = Brushes.Red;
            lb.Width = 145;
            lb.Content = lbl.Content;
            var tmptooltip="";
            if (m_Connection != null && m_Connection.Count > 0)
            {
                foreach (var o in m_Connection)
                {
                    tmptooltip += o.Key + "," + o.Value + "\n";
                }
                tmptooltip += "---------------------\n";
            }
            if (subRelList != null && subRelList.Count > 0)
            {
                subRelList.ForEach(x =>
                {
                    tmptooltip += x.Memo + "," + x.InData + "," + x.InType.ToString() + "," + x.OutData + "\n";
                });               
            }
            lb.ToolTip = tmptooltip;
            lb.Style = DesignerStyle as Style;
            var item = new DesignerItem(tradecode, key,subRelList, m_Connection);
            item.Width = 145;
            item.Height = 23;
            item.IsGroup = true;
            Canvas.SetLeft(item, Double.Parse("10", CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse("5", CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, 1);
            item.Content = lb;
            item.ToolTips = lb.ToolTip.ToString();
            

            return item;
        }
        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }

        private Connector GetConnector(string itemID, int connectorNameNum)
        {
            string[] connectorNameList ={ "ConnectorLeftName", "ConnectorTopLeftName", "ConnectorTopName", "ConnectorTopRightName", "ConnectorRightName", "ConnectorBottomLeftName", "ConnectorBottomName", "ConnectorBottomRightName" };

            var connectorName = connectorNameList[connectorNameNum];

            var designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.CurrentSerialNumber == itemID
                                         select item).FirstOrDefault();
            if (designerItem != null)
                designerItem.ApplyTemplate();
            var connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            if (connectorDecorator != null)
                connectorDecorator.ApplyTemplate();
            
            var result= connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
            
            foreach (var item in this.Children)
            {
                if (item is Connection)
                {
                    var sinkItem = item as Connection;
                    if (sinkItem.Sink.ParentDesignerItem == result.ParentDesignerItem)
                    {
                        if (sinkItem.Sink.Name == connectorName)
                        {
                            connectorName = connectorNameList[connectorNameNum == 7 ? 0 : (connectorNameNum + 1)];
                            result = connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
                            break;
                        }
                    }

                }
            }
            return result;
        }

        /// <summary>
        /// 更新设计面板中集合
        /// </summary>
        private void UpdateZIndex()
        {
            //更新所有元素
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();
            
            for (int i = 0; i < ordered.Count; i++)
            {
                //Canvas.SetZIndex(ordered[i], i);
            }


            //更新item序号
            List<DesignerItem> ordereditem = (from DesignerItem item in this.Children.OfType<DesignerItem>()
                                              
                                              orderby Convert.ToInt32((item as DesignerItem).CurrentSerialNumber)
                                              select item as DesignerItem).ToList();
            value = ordereditem.Count;
            for (int i = 0; i < ordereditem.Count; i++)
            {
                ordereditem[i].CurrentSerialNumber = i.ToString();
                ContentPresenter contentPresenter =
                    ordereditem[i].Template.FindName("PART_ContentPresenter", ordereditem[i]) as ContentPresenter;
                if (contentPresenter != null)
                {
                    if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                    {
                        UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                        Label txt = DesignerItem.FindVisualChild<Label>(contentVisual as Label);
                        if (txt != null)
                            txt.Content = ordereditem[i].CurrentSerialNumber;
                    }
                }
            }

            //更新所有连线
            List<Connection> orderedconn = (from Connection item in this.Children.OfType<Connection>()
                                            select item as Connection).ToList();
            orderedconn.ForEach(x =>
            {
                if (x.Sink.ParentDesignerItem != null)
                {
                    x.Sink.ParentDesignerItem.ParentSerialNumber = x.Source.ParentDesignerItem.CurrentSerialNumber;
                }
            });
            
        }
        
        #endregion


        
    }
}
