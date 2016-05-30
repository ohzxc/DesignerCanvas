using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowChart.Interfaces
{
    /// <summary>
    /// 定义组件之间的关系接口
    /// </summary>
    public interface IGroupable
    {
        /// <summary>
        /// 当前元素的序号
        /// </summary>
        string CurrentSerialNumber { get; set; }
        /// <summary>
        /// 当前元素的父元素
        /// </summary>
        string ParentSerialNumber { get; set; }
        /// <summary>
        /// 是否分组
        /// </summary>
        bool IsGroup { get; set; }
    }
}
