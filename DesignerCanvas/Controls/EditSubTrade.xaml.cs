using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
//using Public.Common;
//using Public.Communication;
//using Platform.Common;
using System.ComponentModel;

namespace DesignerCanvas.Controls
{
    public partial class EditSubTrade : Window, INotifyPropertyChanged
    {
        public string TradeCode { get; set; }
        //public delegate void PassValuesHandler(List<SubRel> e);
        public List<SubRel> SubReList { get; set; }
        //public event PassValuesHandler PassValuesEvent; 
        public EditSubTrade(string tradecode,string name, string item, string flowcode, string tradeFlow, List<SubRel> subReList, List<string> conditionList)
        {
            TradeCode = tradecode;
            InitializeComponent();
            tbName.Text = name;
            tbId.Text = item;
            tbFlowCode.Text = flowcode;
            lvConditionList.ItemsSource = conditionList;

            dgRel.DataContext = subReList;
            SubReList = new List<SubRel>();
            subReList.ForEach(x =>
            {
                SubReList.Add(new SubRel()
                {
                    //CompCode =(string) x.CompCode.Clone(),
                    InData = (string)x.InData.Clone(),
                    InType = x.InType,
                    Memo = (string)x.Memo.Clone(),
                    OutData = (string)x.OutData.Clone(),
                    //SerialNumber = (string)x.SerialNumber.Clone(),
                    //TradeCode = (string)x.TradeCode.Clone()
                });
            });
            tbTradeFLow.Text = tradeFlow;
        }
        
        #region 实现成员接口
        public event PropertyChangedEventHandler PropertyChanged;
        private void PropertyChange(string pro)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pro));
            }
        }
        #endregion

        private bool _isOK=false;
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //PassValuesEvent(dgRel.DataContext as List<SubRel>);

            //if (Validation.GetHasError(dgRel)) return;
            //dgRel.CommitEdit();
            //dgRel.CommitEdit(DataGridEditingUnit.Row, true);
            _isOK = true;
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            btnOK.Focus();
            var hasError = false;
            if(_isOK)
            {
                for (int i = 0; i < dgRel.Items.Count; i++)
                {
                    DependencyObject o = dgRel.ItemContainerGenerator.ContainerFromIndex(i);
                    hasError = Validation.GetHasError(o);
                    if (hasError)
                    {
                        MessageBox.Show("参数填写不正确，请将鼠标悬停到红色框内查看具体信息。", "提示");
                        e.Cancel = true;
                        return;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("是否保存？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < dgRel.Items.Count; i++)
                    {
                        DependencyObject o = dgRel.ItemContainerGenerator.ContainerFromIndex(i);
                        hasError = Validation.GetHasError(o);
                        if (hasError)
                        {
                            MessageBox.Show("参数填写不正确，请将鼠标悬停到红色框内查看具体信息。", "提示");
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                else
                {
                    //TODO 异常
                    (dgRel.DataContext as List<SubRel>).Clear();
                    SubReList.ForEach(x =>
                    {
                        (dgRel.DataContext as List<SubRel>).Add(new SubRel()
                        {
                            //CompCode =(string) x.CompCode.Clone(),
                            InData = (string)x.InData,
                            InType = x.InType,
                            Memo = (string)x.Memo,
                            OutData = (string)x.OutData,
                            //SerialNumber = (string)x.SerialNumber.Clone(),
                            //TradeCode = (string)x.TradeCode.Clone()
                        });
                    });
                    dgRel.Items.Refresh();
                }
            }
            (dgRel.DataContext as List<SubRel>).ForEach(x =>
            {
                x.CompCode = tbId.Text;
                x.SerialNumber = tbFlowCode.Text;
                x.TradeCode = TradeCode;
            });

            //Do whatever you want here..
        }
       
    }

}
