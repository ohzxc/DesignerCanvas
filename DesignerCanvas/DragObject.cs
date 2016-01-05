using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;
using System.IO;

namespace DesignerCanvas
{
    /// <summary>
    /// 定义一个拖拽对象
    /// </summary>
    public class DragObject
    {
        public String Xaml { get; set; }
        public Size? DesiredSize { get; set; }
    }
}
