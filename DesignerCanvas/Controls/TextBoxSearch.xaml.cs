 
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
using System.ComponentModel;

namespace DesignerCanvas.Controls
{
    /// <summary>
    /// TextBoxSearch.xaml 的交互逻辑
    /// </summary>
    public partial class TextBoxSearch : UserControl, INotifyPropertyChanged
    {
        public TextBoxSearch()
        {
            InitializeComponent();
        }
        private void ImgDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearch.Text = "";
        }
        /// <summary>
        /// 文本框内容
        /// </summary>
        private string _txtSearchContent;
        public string TxtSearchContent
        {
            get { return _txtSearchContent; }
            set 
            { 
                _txtSearchContent = value;
                PropertyChange("TxtSearchContent");
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Style txt = (Style)this.Resources["TextBoxStyle"];
            var myTemplate = (ControlTemplate)(txt.Setters[9] as Setter).Value;
            Image img1 = (Image)myTemplate.FindName("ImgSerach", txtSearch);
            Image img2 = (Image)myTemplate.FindName("ImgDel", txtSearch);
            if (txtSearch.Text == "")
            {
               
                img1.Visibility = Visibility.Visible;
                img2.Visibility = Visibility.Collapsed;
            }
            else
            {
                img1.Visibility = Visibility.Collapsed;
                img2.Visibility = Visibility.Visible;
            }
        }
        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;
        private void PropertyChange(string pro)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pro));
            }
        }
        #endregion
       
    }
}
