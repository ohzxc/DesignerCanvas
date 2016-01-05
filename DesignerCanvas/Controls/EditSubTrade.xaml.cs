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
using System.Windows.Shapes;
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

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //PassValuesEvent(dgRel.DataContext as List<SubRel>);

            //if (Validation.GetHasError(dgRel)) return;
            //dgRel.CommitEdit();
            //dgRel.CommitEdit(DataGridEditingUnit.Row, true);

            var hasError = false;
            for (int i = 0; i < dgRel.Items.Count; i++)
            {
                DependencyObject o = dgRel.ItemContainerGenerator.ContainerFromIndex(i);
                hasError = Validation.GetHasError(o);
                if (hasError)
                {
                    return;
                }
            }
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            btnOK.Focus();
            var hasError = false;
            for (int i = 0; i < dgRel.Items.Count; i++)
            {
                DependencyObject o = dgRel.ItemContainerGenerator.ContainerFromIndex(i);
                hasError = Validation.GetHasError(o);
                if (hasError)
                {
                    e.Cancel = true;
                    return;
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
