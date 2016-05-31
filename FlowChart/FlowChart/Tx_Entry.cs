using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowChart
{
    internal class Tx_Entry
    {
        #region 字段
        private string m_StartNode;
        private string m_EndNode;
        private string m_Condition;
        #endregion 字段

        #region 属性

        /// <summary>
        /// 开始节点
        /// </summary>
        internal string StartNode
        {
            get { return m_StartNode; }
            set { m_StartNode = value; }
        }

        /// <summary>
        /// 结束节点
        /// </summary>
        internal string EndNode
        {
            get { return m_EndNode; }
            set { m_EndNode = value; }
        }

        /// <summary>
        /// 条件
        /// </summary>
        public string Condition
        {
            get { return m_Condition; }
            set { m_Condition = value; }
        }

        #endregion 属性

        public Tx_Entry(string start, string end, string condition)
        {
            this.m_StartNode = start;
            this.m_EndNode = end;
            this.m_Condition = condition;
        }
    }
}
