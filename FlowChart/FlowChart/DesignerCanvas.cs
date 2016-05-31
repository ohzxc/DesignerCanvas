using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;
using System.Globalization;
using System.Linq;


namespace FlowChart
{
    public class DesignerCanvas : Canvas
    {
        //拖拽起始点
        private Point? rubberbandSelectionStartPoint = null;

        Adorner _preAdorder;
        AdornerLayer _focusAdornerLayer;
        //选中的节点集合
        private List<ISelectable> selectedItems;
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
        /// 界面元素排序层数
        /// </summary>
        private int m_LayOutCount = 0;
        /// <summary>
        /// 开始节点和结束节点X轴偏移位置
        /// </summary>
        private const float mc_START_X = 20f;
        /// <summary>
        /// 开始节点和结束节点Y轴偏移位置
        /// </summary>
        private const float mc_START_Y = 10f;
        /// <summary>
        /// 相邻元素之间的偏移量
        /// </summary>
        private const float mc_OFFSET_X = 220f;
        /// <summary>
        /// 相邻元素之间的偏移量
        /// </summary>
        private const float mc_OFFSET_Y = 120f;
        /// <summary>
        /// 在界面最左边的节点图形的X坐标
        /// </summary>
        private float m_Leftest_X = float.Epsilon;
        /// <summary>
        /// 开始节点编码
        /// </summary>
        private string startNode = "0";
        /// <summary>
        /// 结束节点编码
        /// </summary>
        private string endNode = "0";
        /// <summary>
        /// 确定位置后的交易节点集合
        /// </summary>
        private List<Tx_Node> m_LstNodes;
        /// <summary>
        /// 交易流程中的结点
        /// </summary>
        private List<Tx_Node> m_Nodes;
        /// <summary>
        /// 交易流程中的所有头结点
        /// </summary>
        private List<Tx_Node> m_LstHeadNode;
        /// <summary>
        /// 交易流程中的线条集合
        /// </summary>
        private List<Tx_Entry> m_LstEntrys;
        /// <summary>
        /// 节点
        /// </summary>
        private object nodeStyle = null;

        public DesignerCanvas()
        {
            this.AllowDrop = true;
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == true) return;
            ResourceDictionary anmationResource = new ResourceDictionary();
            anmationResource.Source = new Uri("/FlowChart;component/Resources/NodeControlStyles.xaml", UriKind.RelativeOrAbsolute);
            Application.Current.Resources.MergedDictionaries.Add(anmationResource);
            nodeStyle = Application.Current.Resources["NodeStyle"] as Style;
        }
        /// <summary>
        /// 初始化流程图
        /// </summary>
        /// <param name="flowList"></param>
        public void LoadFlowChat(List<Tx_Node> flowList)
        {
            m_LstNodes = new List<Tx_Node>();
            m_Nodes = new List<Tx_Node>();//交易流程节点集合
            m_LstEntrys = new List<Tx_Entry>();//确定线条集合
            if (flowList == null || flowList.Count() < 1) return;
            int m_MaxFlowCode = flowList.Count() + 1;
            startNode = flowList.Select(x => x.Sub_Code).Min();
            endNode = m_MaxFlowCode.ToString();
            m_Nodes.Add(new Tx_Node(startNode, "", "开始"));
            m_Nodes.Add(new Tx_Node(endNode, "", "结束"));
            LayOut(flowList);//确定布局,初始化数据
            if (m_LstNodes == null) return;
            #region 构造流程节点
            foreach (var node in m_LstNodes)
            {
                Label lbl = new Label { Width = 150, Height = 45, Content = node.Name, Tag = node.Code };
                lbl.Style = nodeStyle as Style;
                DesignerItem item = new DesignerItem(node.Code);
                item.Remark = node.Content;//备注
                item.ErrorInfo = node.ErrInfo;//错误信息  
                switch (node.Stat.ToUpper())
                { 
                    case "Z":
                        item.SourceTipSymbol = DesignerItemTip.Error; //失败
                        item.ToolTip = "结账失败";
                        break;
                    case "Y":
                        item.SourceTipSymbol = DesignerItemTip.Success;//成功
                        item.ToolTip = "结账成功";
                        break;
                    case "N":
                        item.SourceTipSymbol=DesignerItemTip.UnHandled;//未处理
                        item.ToolTip = "未处理";
                        break;
                    case "S":
                        item.SourceTipSymbol = DesignerItemTip.Execute;//正在执行
                         //if (_focusAdornerLayer == null)
                         //            {
                         //               _focusAdornerLayer = AdornerLayer.GetAdornerLayer(item);
                         //           }                            
                         //           _preAdorder = new FocusAdorner(item);
                         //           _preAdorder.IsClipEnabled = true;
                         //           _focusAdornerLayer.Add(_preAdorder);
                        item.ToolTip = "正在执行";
                        break;
                    default:
                        item.SourceTipSymbol = DesignerItemTip.None;
                        break;
                }
                Canvas.SetLeft(item, Double.Parse("10", CultureInfo.InvariantCulture) + node.X);
                Canvas.SetTop(item, Double.Parse("5", CultureInfo.InvariantCulture) + node.Y);
                Canvas.SetZIndex(item, Int32.Parse("0"));
                item.Content = lbl;
                item.ToolTip = node.Content;
                if (!string.IsNullOrEmpty(node.Sub_Code.Trim()))
                    item.IsDragConnectionOver = true;
                this.Children.Add(item);
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item) as ControlTemplate;
                DesignerItem.SetConnectorDecoratorTemplate(item, template);
            }
            #endregion

            #region  构造流程线条
            this.InvalidateVisual();//使元素的呈现无效，并强制执行完整的新布局处理过程。布局循环完成后              
            if (m_LstEntrys == null) return;
            foreach (Tx_Entry entry in m_LstEntrys)
            {
                string sourceID = entry.StartNode;
                string sinkID = entry.EndNode;
                string sourceConnectorName = "ConnectorBottomName";
                string sinkConnectorName = "ConnectorTopName";
                if (!string.IsNullOrEmpty(sinkID) && !string.IsNullOrEmpty(sourceID))
                {
                    String strContent = entry.Condition;
                    Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);
                    Connection connection = new Connection(sourceConnector, sinkConnector, strContent);
                    Canvas.SetZIndex(connection, 5);
                    this.Children.Add(connection);
                }
            }
            #endregion
        }
        /// <summary>
        /// 初始化流程图
        /// </summary>
        /// <param name="flowxml">xml文本</param>
        public void LoadFlowChat(string flowxml)
        {
            var nodeList = new List<Tx_Node>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(flowxml);
            var allNodes = doc.SelectSingleNode("Nodes");
            //获取到Nodes所有子节点    
            XmlNodeList childNodeList = allNodes.ChildNodes;
            //遍历所有子节点    
            foreach (XmlNode xmlNode in childNodeList)
            {
                var code = xmlNode.Attributes["batcode"].Value;
                var name = xmlNode.Attributes["batname"].Value;
                var parentcode = xmlNode.Attributes["parentcode"].Value;
                var batdesc = xmlNode.Attributes["batdesc"].Value;
                var batstat = xmlNode.Attributes["batstat"].Value;
                var errinfo = xmlNode.Attributes["errinfo"].Value;
                var batremark = xmlNode.Attributes["batremark"].Value;
                nodeList.Add(new Tx_Node(code,parentcode,name,"",batdesc,batremark,batstat,errinfo));
            }
            this.LoadFlowChat(nodeList);
        }
        /// <summary>
        /// 设置流程节点状态
        /// </summary>
        /// <param name="flowxml">xml文本</param>
        public void SetNodeStat(string flowxml)
        {
            var lstDesigner= this.Children.OfType<ISelectable>();            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(flowxml);
            var allNodes = doc.SelectSingleNode("Nodes");
            //获取到Nodes所有子节点    
            XmlNodeList childNodeList = allNodes.ChildNodes;
            //遍历所有子节点    
            foreach (XmlNode xmlNode in childNodeList)
            {
                var code = xmlNode.Attributes["batcode"].Value;  
                var batstat = xmlNode.Attributes["batstat"].Value; //Y-结账成功 Z-正在执行 E-结账失败

                var errinfo = xmlNode.Attributes["errinfo"].Value;
                var batremark = xmlNode.Attributes["batremark"].Value;
                foreach (DesignerItem item in lstDesigner)
                {
                    if (item.CurrentSerialNumber == code)
                    {
                        item.Remark = batremark;
                        item.ErrorInfo = errinfo;
                        switch (batstat.ToUpper())
                        {
                            case "Z":
                                item.SourceTipSymbol = DesignerItemTip.Error; //失败
                                item.ToolTip = "结账失败";
                                break;
                            case "Y":
                                item.SourceTipSymbol = DesignerItemTip.Success;//成功
                                item.ToolTip = "结账成功";
                                break;
                            case "N":
                                item.SourceTipSymbol = DesignerItemTip.UnHandled;//未处理
                                item.ToolTip = "未处理";
                                break;
                            case "S":
                                item.SourceTipSymbol = DesignerItemTip.Execute;//正在执行
                                //if (_focusAdornerLayer == null)
                                //{
                                //    _focusAdornerLayer = AdornerLayer.GetAdornerLayer(item);
                                //}
                                //_preAdorder = new FocusAdorner(item);
                                //_preAdorder.IsClipEnabled = true;
                                //_focusAdornerLayer.Add(_preAdorder);
                                item.ToolTip = "正在执行";
                                break;
                            default:
                                item.SourceTipSymbol = DesignerItemTip.None;
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 初始化数据 
        /// 确定交易流程节点集合
        /// 确定线条集合
        /// 确定节点位置
        /// </summary>
        internal void LayOut(List<Tx_Node> flowList)
        {
            this.m_LayOutCount = 0; //初始化界面层级数
            foreach (var item in flowList)
            {
                var _s = item.Sub_Code.Split(',');
                _s.ToList().ForEach(k =>
                {
                    Tx_Entry entry = new Tx_Entry(k, item.Code, item.Conditions);
                    m_LstEntrys.Add(entry);
                });
                m_Nodes.Add(item);
            }
            Tx_Node lstNode = FindLstNode(flowList);//查找最后一个节点和结束节点串联
            if (lstNode != null)
                m_LstEntrys.Add(new Tx_Entry(lstNode.Code, endNode, ""));//添加虚拟结束节点
            if (m_Nodes.Count < 1) return;
            var nodes = from n in m_Nodes                      
                        orderby int.Parse(n.Code) ascending
                        select n;
            Tx_Node firstNode = this.FindFirstNode(startNode.Trim());
            firstNode.X = mc_START_X;
            firstNode.Y = mc_START_Y;
            this.m_LayOutCount++;
            m_LstNodes.Add(firstNode);
            this.LayOut_NextNodes(firstNode);
            m_LstHeadNode = GetHeadNodes();   //查找非起始交易的头节点        
            foreach (Tx_Node nextLayNode in nodes)
            {
                if (m_LstHeadNode != null && m_LstHeadNode.Count() > 0)
                {
                    //非起始子交易的头结点树的分布控制
                    foreach (Tx_Node txNodeHaad in m_LstHeadNode)
                    {
                        if (txNodeHaad.Code == nextLayNode.Code)
                        {
                            txNodeHaad.X = mc_START_X + 100;
                            txNodeHaad.Y = mc_START_Y;
                            this.m_LayOutCount++;
                            m_LstNodes.Add(txNodeHaad);
                            this.LayOut_NextNodes(txNodeHaad);
                        }
                    }
                }                
                this.LayOut_NextNodes(nextLayNode);
            }
        }
        /// <summary>
        /// 找出交易流程的所有头结点
        /// </summary>
        /// <returns></returns>
        private List<Tx_Node> GetHeadNodes()
        {
            List<Tx_Node> headNodes = new List<Tx_Node>();
            foreach (Tx_Node node in m_Nodes)
            {
                bool found = false;
                if (node.Code == startNode || node.Code == endNode)
                {
                    continue;
                }
                foreach (Tx_Entry entry in this.m_LstEntrys)
                {
                    if (entry.EndNode == node.Code)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    headNodes.Add(node);
                }
            }
            return headNodes;
        }
        /// <summary>
        /// 计算一个节点后续节点的位置算法
        /// </summary>
        /// <param name="nodeIndex"></param>
        private void LayOut_NextNodes(Tx_Node node)
        {
            //获取后续节点串
            List<Tx_Node> lstNextNodes = this.FindNextNodes(node);
            //若后续节点串为空或者已布局节点数等于全部节点数，则退出布局
            if (lstNextNodes == null || lstNextNodes.Count == 0)
            {
                return;
            }
            float half = 0f;
            //偶数个节点
            if (lstNextNodes.Count % 2 == 0)
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

                    if (lstNextNodes[i].Code != endNode)
                    {
                        lstNextNodes[i].X = node.X + (i - half) * mc_OFFSET_X;
                        if (!string.IsNullOrEmpty(node.Sub_Code))
                        {
                            string[] sub_node = node.Sub_Code.Split(',');
                            for (int y = 0; y < sub_node.Count(); y++)
                            {
                                if (sub_node.Count() <= 1)
                                {
                                    break;
                                }
                                //else
                                //{
                                //    float offset = i - half == 0 ? 1 : i - half;
                                //    lstNextNodes[i].X = node.X + offset * mc_OFFSET_X;
                                //}
                            }
                        }
                    }
                    else
                    {
                        lstNextNodes[i].X = mc_START_X;
                        float MaxY = 0.0f;
                        foreach (var maxNodeY in m_LstNodes)
                        {
                            if (maxNodeY.Y >= MaxY)
                            {
                                MaxY = maxNodeY.Y;
                            }
                        }
                        lstNextNodes[i].Y = MaxY + mc_OFFSET_Y;
                    }
                    m_LstNodes.Add(lstNextNodes[i]);
                    //保存界面最左的图形节点的X坐标值
                    if (lstNextNodes[i].X < m_Leftest_X)
                    {
                        m_Leftest_X = lstNextNodes[i].X;
                    }
                    if (lstNextNodes.Count() == 1)
                    {
                        this.m_LayOutCount++;
                        //递归对后续节点进行布局
                        this.LayOut_NextNodes(lstNextNodes[i]);
                    }
                }
            }
            //递归对后续节点进行布局
            if (lstNextNodes.Count() > 1)
            {
                this.m_LayOutCount++;
            }
        }
        /// <summary>
        /// 找到第一个没有前导节点的节点
        /// </summary>
        /// <returns></returns>
        private Tx_Node FindFirstNode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;
            foreach (Tx_Node node in this.m_Nodes)
            {
                if (node.Code == code.Trim())
                    return node;
            }
            return null;
        }
        /// <summary>
        /// 找到节点序列中最后一个节点
        /// </summary>
        /// <param name="flowList"></param>
        /// <returns></returns>
        private Tx_Node FindLstNode(List<Tx_Node> flowList)
        {
            if (flowList == null)
                return null;
            foreach (Tx_Node node in flowList)
            {
                bool isExist = false;
                foreach (Tx_Node subnode in flowList)
                { 
                     var _s = subnode.Sub_Code.Split(',');
                     if (_s.Contains(node.Code))
                     {
                         isExist = true;
                         break;      
                     }           
                }
                if (!isExist)
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
        private List<Tx_Node> FindNextNodes(Tx_Node node)
        {
            List<Tx_Node> lstNextNodes = new List<Tx_Node>();
            //根据线条查找后续节点串
            foreach (Tx_Entry entry in this.m_LstEntrys)
            {
                if (entry.StartNode == node.Code)
                {
                    foreach (var nodes in m_Nodes)
                    {
                        if (nodes.Code == entry.EndNode)
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
        private List<Tx_Node> FindPreNodes(Tx_Node node)
        {
            string subtxCode = "";
            List<Tx_Node> lstPreNodes = new List<Tx_Node>();
            //根据线条查找后续节点串
            foreach (Tx_Node entry in this.m_Nodes)
            {
                if (entry.Code == node.Code)
                {
                    subtxCode = node.Code;
                    foreach (Tx_Node lstentry in this.m_LstNodes)
                    {
                        if (lstentry.Sub_Code == subtxCode)
                        {
                            lstPreNodes.Add(lstentry);
                        }
                    }
                }
            }
            return lstPreNodes;
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }
        private Connector GetConnector(string itemID, String connectorName)
        {
            DesignerItem designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.CurrentSerialNumber == itemID
                                         select item).FirstOrDefault();
            if (designerItem != null)
                designerItem.ApplyTemplate();
            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            if (connectorDecorator != null)
                connectorDecorator.ApplyTemplate();
            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }
        /// <summary>
        /// 更新设计面板中集合
        /// </summary>
        private void UpdateZIndex()
        {
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }
        /// <summary>
        /// 判断奇数和偶数
        /// </summary>
        /// <param name="n"></param>
        /// <returns>True-奇数，False-偶数</returns>
        static bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                // in case that this click is the start for a 
                // drag operation we cache the start point
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                foreach (ISelectable item in SelectedItems)
                    item.IsSelected = false;
                selectedItems.Clear();

                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
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

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null && !String.IsNullOrEmpty(dragObject.Xaml))
            {
                DesignerItem newItem = null;
                Object content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

                if (content != null)
                {
                    newItem = new DesignerItem("");
                    newItem.Content = content;

                    Point position = e.GetPosition(this);

                    if (dragObject.DesiredSize.HasValue)
                    {
                        Size desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                    }
                    else
                    {
                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    }

                    this.Children.Add(newItem);

                    //update selection
                    foreach (ISelectable item in this.SelectedItems)
                        item.IsSelected = false;
                    SelectedItems.Clear();
                    newItem.IsSelected = true;
                    this.SelectedItems.Add(newItem);
                }

                e.Handled = true;
            }
        }

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
    }
}
