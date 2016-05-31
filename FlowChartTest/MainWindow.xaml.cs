using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlowChart;

namespace FlowChartTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flowChart.LoadFlowChat(new List<Tx_Node>
            {
                new Tx_Node
                {
                    Code="1",
                    Conditions="2",
                   Sub_Code="2",
                   X=100,
                   Y=100,
                   Stat="4",
                   Name="ECIF"
                },
                new Tx_Node
                {
                    Code="2",
                    Conditions="3",
                    Sub_Code="3",
                    X=0,
                    Y=0,
                    Stat="4",
                    Name="核心"
                },
                new Tx_Node
                {
                    Code="3",
                    Conditions="3",
                    Sub_Code="",
                    X=0,
                    Y=0,
                    Stat="4",
                    Name="前端"
                },
            });
        }
    }
}
