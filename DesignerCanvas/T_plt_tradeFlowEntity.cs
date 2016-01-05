using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerCanvas
{
    public class T_plt_tradeFlowEntity 
    {
        /// <summary>
        /// 交易编号
        /// </summary>
        public string TxCode { get; set; }
        /// <summary>
        /// 组件编号
        /// </summary>
        public string CompCode { get; set; }
        /// <summary>
        /// 执行流程控制规则脚本
        /// </summary>
        public string TradeFlow { get; set; }
        /// <summary>
        /// 当前交易执行流程中组件代号
        /// </summary>
        public string FlowCode { get; set; }
        //public void ChangeFromTable(object[] parms)
        //{
        //    TxCode = parms[0].ToString();
        //    CompCode = parms[1].ToString();
        //    TradeFlow = parms[2].ToString();
        //    FlowCode = parms[3].ToString();
        //}

        //public string[] ChangeToTable()
        //{
        //    return new string[] { TxCode, CompCode, TradeFlow, FlowCode };
        //}
    }
}
