using System.Collections.Generic;
using System.Timers;
using System.Windows;
using FlowChart;

namespace FlowChartTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int i=0;

        public MainWindow()
        {
            InitializeComponent();
            //flowChart.LoadFlowChart(new List<Tx_Node>
            {
                new Tx_Node
                {
                    Code="1",
                    Conditions="2",
                   Sub_Code="2",
                   X=0,
                   Y=0,
                   Stat="Z",
                   Name="ECIF"
                },
                new Tx_Node
                {
                    Code="2",
                    Conditions="3",
                    Sub_Code="3",
                    X=0,
                    Y=0,
                    Stat="N",
                    Name="核心"
                },
                new Tx_Node
                {
                    Code="3",
                    Conditions="3",
                    Sub_Code="",
                    X=0,
                    Y=0,
                    Stat="N",
                    Name="前端"
                },
                new Tx_Node
                {
                    Code="4",
                    Conditions="3",
                    Sub_Code="3,5",
                    X=0,
                    Y=0,
                    Stat="S",
                    Name="财务"
                },
                new Tx_Node
                {
                    Code="5",
                    Conditions="3",
                    Sub_Code="",
                    X=0,
                    Y=0,
                    Stat="N",
                    Name="ods"
                },
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flowChart.SetNodeStat(i.ToString(), "Y");
            i++;
        }

     
    }
}
