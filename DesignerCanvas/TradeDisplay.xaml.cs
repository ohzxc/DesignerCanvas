using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DesignerCanvas.Controls;

namespace DesignerCanvas
{
    /// <summary>
    /// TradeDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class TradeDisplay : UserControl
    {
        #region 变量
        public List<TradeFlow> FlowList { get; set; }
        public List<SubRel> RelList { get; set; }
        /// <summary>
        /// 交易列表
        /// </summary>
        private List<T_tradeItem> tradeItemList;
        /// <summary>
        /// 子交易列表
        /// </summary>
        private List<T_subTradeItem> subTradeItemList;
        #endregion

        #region 构造函数，初始化
        public TradeDisplay()
        {
            InitializeComponent();
            GetTradeList();
            GetSubTradeList();
            //资源绑定
            lbTx.ItemsSource = tradeItemList;
            lbSubTx.ItemsSource = subTradeItemList;
            //获取有效返回值的List
            subTradeItemList.ForEach(x =>
            {
                mc.m_Connection.Add(x.subtxcode, x.conditionDic);
            });
        }
        /// <summary>
        /// 初始化交易列表
        /// </summary>
        /// <returns></returns>
        public void GetTradeList()
        {
            tradeItemList = new List<T_tradeItem>();
            tradeItemList.Add(new T_tradeItem
            {
                txcode = "2601",
                name = "你好吗不好了啊哈反对"
            });
            tradeItemList.Add(new T_tradeItem
            {
                txcode = "2602",
                name = "交易2"
            });
            tradeItemList.Add(new T_tradeItem
            {
                txcode = "2603",
                name = "交易3"
            });

        }
        /// <summary>
        /// 初始化子交易列表
        /// </summary>
        public void GetSubTradeList()
        {
            subTradeItemList = new List<T_subTradeItem>();
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D001",
                name = "组件1",
                conditionDic = new Dictionary<string, string>
                {
                    {"0001","转账"},
                    {"0000","成功"},
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D002",
                name = "组件2",
                conditionDic = new Dictionary<string, string>
                {
                    {"0001","转账"},
                    {"0002","现金"}
                    
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D003",
                name = "组件3",
                conditionDic = new Dictionary<string, string>
                {
                    {"0001","转账"},
                    {"0000","成功"},
                    {"0002","现金"}
                    
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D004",
                name = "组件4",
                conditionDic = new Dictionary<string, string>
                {
                    {"0000","转账"},
                    {"0002","成功"},
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D005",
                name = "组件5",
                conditionDic = new Dictionary<string, string>
                {
                    {"0000","转账"},
                    {"0002","成功"},
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D006",
                name = "组件6",
                conditionDic = new Dictionary<string, string>
                {
                    {"0001","转账"},
                    {"0002","现金"}
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D007",
                name = "组件7",
                conditionDic = new Dictionary<string, string>
                {
                    {"0001","转账"},
                    {"0002","现金"}
                }
            });
            subTradeItemList.Add(new T_subTradeItem
            {
                subtxcode = "D008",
                name = "组件8",
                conditionDic = new Dictionary<string, string>
                {
                    {"0001","转账"},
                    {"0002","现金"}
                }
            });
        }
        /// <summary>
        /// 获取某个交易的流程图
        /// </summary>
        /// <param name="txCode">交易号</param>
        /// <returns></returns>
        public bool initialTradeFlow(string txCode)
        {
            //通讯获取交易信息
            var tradeFlowListEntity = new List<T_plt_tradeFlowEntity>();
            var typeList = new List<Label>();
            var subRelListEntity = new List<T_plt_subrelEntity>();

            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D001", TradeFlow = "#=0000@6@#=0001@7@#$", FlowCode = "2" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D002", TradeFlow = "#$", FlowCode = "1" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D003", TradeFlow = "#=0000@2@#=0002@1@#=0001@3@#$", FlowCode = "0" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D004", TradeFlow = "#=0000@6@#=0002@5@#$", FlowCode = "3" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D005", TradeFlow = "#$", FlowCode = "4" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D006", TradeFlow = "#$", FlowCode = "5" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D007", TradeFlow = "#$", FlowCode = "6" });
            tradeFlowListEntity.Add(new T_plt_tradeFlowEntity() { TxCode = txCode, CompCode = "D008", TradeFlow = "#$", FlowCode = "7" });



            typeList.Add(new Label() { Tag = "D001", Visibility = Visibility.Visible, Content = "组件1" });
            typeList.Add(new Label() { Tag = "D002", Visibility = Visibility.Visible, Content = "组件2" });
            typeList.Add(new Label() { Tag = "D003", Visibility = Visibility.Visible, Content = "组件3" });
            typeList.Add(new Label() { Tag = "D004", Visibility = Visibility.Visible, Content = "组件4" });
            typeList.Add(new Label() { Tag = "D005", Visibility = Visibility.Visible, Content = "组件5" });
            typeList.Add(new Label() { Tag = "D006", Visibility = Visibility.Visible, Content = "组件6" });
            typeList.Add(new Label() { Tag = "D007", Visibility = Visibility.Visible, Content = "组件7" });
            typeList.Add(new Label() { Tag = "D008", Visibility = Visibility.Visible, Content = "组件8" });

            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D001", flow_no = "0", in_data = "0191", in_data_type = "0", out_data = "1+2A", memo = "控制标志" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D001", flow_no = "0", in_data = "0192", in_data_type = "0", out_data = "2B", memo = "启用0190" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D002", flow_no = "1", in_data = "0192", in_data_type = "0", out_data = "2B", memo = "启用0190" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D003", flow_no = "2", in_data = "0193", in_data_type = "0", out_data = "1024+102D", memo = "金额" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D004", flow_no = "3", in_data = "0194", in_data_type = "1", out_data = "1", memo = "存款" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D005", flow_no = "4", in_data = "0195", in_data_type = "1", out_data = "3", memo = "反对法" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D006", flow_no = "5", in_data = "0196", in_data_type = "0", out_data = "2A", memo = "反对法" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D006", flow_no = "5", in_data = "016", in_data_type = "0", out_data = "2A", memo = "反对法ew" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D007", flow_no = "6", in_data = "0196", in_data_type = "0", out_data = "2A", memo = "反对法" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D007", flow_no = "6", in_data = "0yt", in_data_type = "0", out_data = "25A", memo = "反对gf法" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D008", flow_no = "7", in_data = "yty", in_data_type = "0", out_data = "243", memo = "反对tr法" });
            subRelListEntity.Add(new T_plt_subrelEntity() { tx_code = txCode, comp_code = "D008", flow_no = "7", in_data = "ytyy", in_data_type = "0", out_data = "2A", memo = "反gf对法" });


            this.Open(tradeFlowListEntity, typeList, subRelListEntity);
            return true;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 打开流程图
        /// </summary>
        /// <param name="tradeFlowListEntity"></param>
        /// <param name="typeList"></param>
        /// <param name="subRelListEntity"></param>
        public void Open(List<T_plt_tradeFlowEntity> tradeFlowListEntity, List<Label> typeList, List<T_plt_subrelEntity> subRelListEntity)
        {
            mc.Open(tradeFlowListEntity, typeList, subRelListEntity);
        }
        /// <summary>
        /// 提交流程图
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            FlowList = new List<TradeFlow>();
            RelList = new List<SubRel>();
            if (mc.Children.Count == 0 || tbTitle.Text.Trim() == "" || tbTitle.Text == null) return false;
            FlowList = mc.Save();
            RelList = mc.SaveSubRel();
            //下面通讯进行保存待增加



            mc.reset();
            tbTitle.Text = "";
            tbTitle.ToolTip = "";
            return true;

        }
        /// <summary>
        /// 新建交易
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        private void ReceiveValues(string name, string code)
        {
            foreach (var x in tradeItemList)
            {
                if (x.txcode == code)
                {
                    MessageBox.Show("交易码已存在");
                    return;
                }
            }
            tbTitle.Text = name;
            tbTitle.ToolTip = mc.TradeCode = code;
            mc.reset();
            tradeItemList.Add(new T_tradeItem()
            {
                name = name,
                txcode = code
            });
            lbTx.Items.Refresh();
        }
        #endregion

        #region 事件

        private void txButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Button).Tag == null) return;
            mc.TradeCode = (sender as Button).Tag.ToString();
            tbTitle.Text = (sender as Button).Content.ToString();
            tbTitle.ToolTip = (sender as Button).Tag.ToString();
            initialTradeFlow((sender as Button).Tag.ToString());
        }

        private void subtxButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tbTitle.Text == "" || tbTitle.Text == null)
            {
                MessageBox.Show("请先新建交易或者双击某个交易进行编辑。");
                return;
            }
            if ((sender as Button).Tag == null) return;
            //var tmp=new Dictionary<string,string>();
            subTradeItemList.ForEach(x =>
            {
                if (x.subtxcode == (sender as Button).Tag.ToString())
                {
                    mc.Add(x);
                    return;
                }
            });
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            if (mc.Children.Count == 0) return;
            MessageBoxResult result = MessageBox.Show("确认清空面板吗?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                mc.reset();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (mc.Children.Count == 0 || mc.SelectedItems.Count < 1) return;
            MessageBoxResult result = MessageBox.Show("确认删除选中的组件吗?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (mc.SelectService.CurrentSelection.Count < 1)
                {
                    mc.SelectedItems.ForEach(x =>
                    {
                        mc.SelectService.AddToSelection(x);
                    });
                }
                mc.Delete();
                mc.SelectedItems.Clear();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "交易")
            {
                lbTx.Visibility = Visibility.Visible;
                lbSubTx.Visibility = Visibility.Hidden;
                btnSubTx.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
                btnTx.Background = new SolidColorBrush(Colors.White);

            }
            else
            {
                lbTx.Visibility = Visibility.Hidden;
                lbSubTx.Visibility = Visibility.Visible;
                btnSubTx.Background = new SolidColorBrush(Colors.White);
                btnTx.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
            }
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var list = new List<T_tradeItem>();
            var list1 = new List<T_subTradeItem>();
            if (string.IsNullOrWhiteSpace(textBoxSearch.TxtSearchContent))
            {
                lbTx.ItemsSource = tradeItemList;
                lbSubTx.ItemsSource = subTradeItemList;
                return;
            }
            var codeOrName = textBoxSearch.TxtSearchContent;
            list.Clear();
            list1.Clear();
            foreach (var item in tradeItemList)
            {
                if (item.txcode.Contains(codeOrName) || item.name.Contains(codeOrName))
                    list.Add(item);
            }
            lbTx.ItemsSource = list;
            foreach (var item in subTradeItemList)
            {
                if (item.subtxcode.Contains(codeOrName) || item.name.Contains(codeOrName))
                    list1.Add(item);
            }
            lbSubTx.ItemsSource = list1;
        }

        private void btnDeleteTrade_Click(object sender, RoutedEventArgs e)
        {
            if (tbTitle.Text == "" || tbTitle.Text == null) return;
            MessageBoxResult result = MessageBox.Show("确认删除交易吗?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;
            tradeItemList.ForEach(x =>
            {
                if (x.txcode == mc.TradeCode)
                    tradeItemList.Remove(x);
                //与核心通讯删除交易
            });
            lbTx.Items.Refresh();
            mc.reset();
            tbTitle.Text = "";
            tbTitle.ToolTip = "";
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var newtrade = new NewTrade();
            newtrade.PassValuesEvent += new NewTrade.PassValuesHandler(ReceiveValues);
            newtrade.ShowDialog();
        }
        #endregion
    }
}
