using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DesignerCanvas
{
    static class Common
    {
        /// <summary>
        /// 沿着视觉树查找符合条件的子元素集合
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<DependencyObject> FindVisualTreeChildren(this DependencyObject target, Predicate<DependencyObject> predicate = null)
        {
            var result = new List<DependencyObject>();
            if (target != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(target);
                for (int i = 0; i < count; i++)
                {
                    var element = VisualTreeHelper.GetChild(target, i);
                    if (element != null)
                    {
                        if (predicate == null || predicate(element)) result.Add(element);
                        result.AddRange(element.FindVisualTreeChildren(predicate));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 沿着视觉树查找符合条件的父元素
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static DependencyObject FindVisualTreeAncestor(this DependencyObject target, Predicate<DependencyObject> predicate = null)
        {
            DependencyObject result = null;
            if (target != null)
            {
                var parent = VisualTreeHelper.GetParent(target);
                if (predicate == null)
                    result = parent;
                else if (parent != null)
                    result = predicate(parent) ? parent : parent.FindVisualTreeAncestor(predicate);
            }
            return result;
        }
     
        /// <summary>
        /// 在逻辑树中，查找页面的父容器
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Window GetOwnerWindow(this DependencyObject source)
        {
            var parent = LogicalTreeHelper.GetParent(source);
            if (parent == null)
                return null;
            var win = parent as Window;
            return
                win != null ?
                parent as Window :
                GetOwnerWindow(parent);
        }
           
    }
}
