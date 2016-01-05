using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerCanvas
{
    public class T_plt_subrelEntity 
    {
        /// <summary>
        /// 交易编码
        /// </summary>
        public string tx_code { get; set; }
        /// <summary>
        /// 组件编码
        /// </summary>
        public string comp_code { get; set; }
        /// <summary>
        /// 输入数据类型
        /// </summary>
        public string in_data_type { get; set; }
        /// <summary>
        /// 输出域
        /// </summary>
        public string out_data { get; set; }
        /// <summary>
        /// 输入数据
        /// </summary>
        public string in_data { get; set; }
        /// <summary>
        /// 输入数据描述
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string flow_no { get; set; }
        //public void ChangeFromTable(object[] parms)
        //{
        //    tx_code = parms[0].ToString();
        //    comp_code = parms[1].ToString();
        //    in_data_type = parms[2].ToString();
        //    out_data = parms[3].ToString();
        //    in_data = parms[4].ToString();
        //    memo = parms[5].ToString();
        //    flow_no = parms[6].ToString();
        //}

        //public string[] ChangeToTable()
        //{
        //    return new string[] { tx_code, comp_code, in_data_type, out_data, in_data, memo, flow_no };
        //}
    }
}
