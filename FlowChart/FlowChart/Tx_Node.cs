using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowChart
{
    public class Tx_Node
    {
        #region 字段
        private string m_Code;
        private string m_Sub_Code;
        private string m_Name;
        private string m_Condition=string.Empty;
        private string m_Content=null;
        private string m_stat;
        private string m_errinfo;
        private string m_desc;
        private float m_X = 0.0f;
        private float m_Y = 0.0f;
        #endregion 字段

        #region 属性
        /// <summary>
        /// 流程节点编号  
        /// 0-流程开始节点
        /// 1-流程结束节点
        /// </summary>
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        /// <summary>
        /// 子节点编号,逗号分割
        /// </summary>
        public string Sub_Code
        {
            get { return m_Sub_Code; }
            set { m_Sub_Code = value; }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        /// <summary>
        /// 条件
        /// </summary>
        public string Conditions
        {
            get { return m_Condition; }
            set { m_Condition = value; }
        }
        /// <summary>
        /// 节点说明
        /// </summary>
        public string Desc
        {
            get { return m_desc; }
            set { m_desc = value; }
        }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public string Stat
        {
            get { return m_stat; }
            set { m_stat = value; }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo
        {
            get { return m_errinfo; }
            set { m_errinfo = value; }
        }
        /// <summary>
        /// X坐标
        /// </summary>
        public float X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public float Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        #endregion 属性

        public Tx_Node()
        { }
        public Tx_Node(string code, string subtxCode, string name,string condition="",string desc="",string content="",string stat="",string errinfo="")
        {
            this.m_Code = code;
            this.m_Sub_Code= subtxCode;
            this.m_Name = name;
            this.m_Condition = condition;
            this.m_Content = content;
            this.m_stat = stat;
            this.m_errinfo = errinfo;
            this.m_desc = desc;
        }
    }
}
