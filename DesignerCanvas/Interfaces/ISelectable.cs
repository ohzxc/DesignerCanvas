using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerCanvas
{
    /// <summary>
    /// 定义选中元素的接口
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        bool IsSelected { get; set; }
    }
}
