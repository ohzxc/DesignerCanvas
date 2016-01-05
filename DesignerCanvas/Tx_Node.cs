using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerCanvas
{
    public class Tx_Node
    {
        #region 字段
        private string m_Flow_code;
        private string m_Sub_tx_code;
        private string m_component_code;
        private float m_X;
        private float m_Y;
        private string m_state;
        private string m_processID;



        #endregion 字段

        #region 属性

        /// <summary>
        /// 交易编号 0表示开始交易 //1表示结束交易
        /// </summary>
        public string Flow_code
        {
            get { return m_Flow_code; }
            set { m_Flow_code = value; }
        }

        /// <summary>
        /// 子交易码
        /// </summary>
        public string Sub_tx_code
        {
            get { return m_Sub_tx_code; }
            set { m_Sub_tx_code = value; }
        }
        /// <summary>
        /// 组件编码
        /// </summary>
        public string Component_code
        {
            get
            {
                return m_component_code;
            }
            set
            {
                m_component_code = value;
            }
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

        /// <summary>
        /// 状态
        /// </summary>
        public string State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        /// <summary>
        /// 流程ID
        /// </summary>
        public string ProcessID
        {
            get { return m_processID; }
            set { m_processID = value; }
        }

        #endregion 属性

        public Tx_Node()
        { }

        public Tx_Node(string flowCode, string subtxCode, string componentCode, string state, string processId)
        {
            this.m_Flow_code = flowCode;
            this.m_Sub_tx_code = subtxCode;
            this.m_component_code = componentCode;
            this.m_state = state;
            this.ProcessID = processId;
        }
        public Tx_Node(string flowCode, string subtxCode, string componentCode)
        {
            this.m_Flow_code = flowCode;
            this.m_Sub_tx_code = subtxCode;
            this.m_component_code = componentCode;
        }
    }
}
