using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace DesignerCanvas
{
    public class TradeFlow : INotifyPropertyChanged
    {
        #region 属性
        private string _tradeCode;
        /// <summary>
        /// 交易编码
        /// </summary>
        public string TradeCode
        {
            get { return _tradeCode; }
            set
            {
                _tradeCode = value;
                PropertyChange("TradeCode");
            }
        }
        private string _compCode;
        /// <summary>
        /// 组件编码
        /// </summary>
        public string CompCode
        {
            get { return _compCode; }
            set
            {
                _compCode = value;
                PropertyChange("CompCode");
            }
        }
        private string _trade_flow;
        /// <summary>
        /// 执行流程控制规则脚本
        /// </summary>
        public string Trade_Flow
        {
            get { return _trade_flow; }
            set
            {
                _trade_flow = value;
                PropertyChange("Trade_Flow");
            }
        }
        private string _flowcode;
        /// <summary>
        /// 当前交易执行流程中组件代号
        /// </summary>
        public string FlowCode
        {
            get { return _flowcode; }
            set
            {
                _flowcode = value;
                PropertyChange("FlowCode");
            }
        }
        #endregion
        #region 实现接口成员
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
