using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignerCanvas
{
    public class T_subTradeItem
    {
        public string subtxcode { get; set; }
        public string name { get; set; }
        public Dictionary<string,string> conditionDic { get; set; }//例如 key=0000，value=交易成功
        //参数public
    }
}
