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
using DesignerCanvas;

namespace DesignerCanvas.Controls
{

    /// <summary>
    /// NewTrade.xaml 的交互逻辑
    /// </summary>
    public partial class NewTrade : Window
    {
        public string TxtRoute
        {
            get;
            set;
        }
        public NewTrade()
        {
            InitializeComponent();
        }

        public delegate void PassValuesHandler(string name, string code);
        public event PassValuesHandler PassValuesEvent;

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "" || tbName.Text == null || tbCode.Text == null || tbCode.Text == "")
            {
                MessageBox.Show("填写不完整。");
                return;
            }
            if (Validation.GetHasError(tbCode)) return;
            PassValuesEvent(tbName.Text, tbCode.Text);
            this.Close();
        }

    }
}
