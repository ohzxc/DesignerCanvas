using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignerCanvas
{
    /// <summary>
    /// 操作设计容器的方法
    /// </summary>
    public class SelectionService
    {
        /// <summary>
        /// 定义设计容器对象
        /// </summary>
        private MyCanvas designerCanvas;
        /// <summary>
        /// 定义当前选中的设计组件集合
        /// </summary>
        private List<ISelectable> currentSelection;
        public List<ISelectable> CurrentSelection
        {
            get
            {
                if (currentSelection == null)
                    currentSelection = new List<ISelectable>();
                return currentSelection;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="canvas">canvas对象</param>
        public SelectionService(MyCanvas canvas)
        {
            this.designerCanvas = canvas;
        }
        #region 公共操作方法
        /// <summary>
        ///选中的组件集合
        /// </summary>
        /// <param name="item"></param>
        public void SelectItem(ISelectable item)
        {
            this.ClearSelection();
            this.AddToSelection(item);
        }
        /// <summary>
        /// 添加设计组件
        /// </summary>
        /// <param name="item"></param>
        public void AddToSelection(ISelectable item)
        {
            if (item is IGroupable)
            {
                IEnumerable<IGroupable> list = designerCanvas.Children.OfType<IGroupable>();
                List<IGroupable> groupItems = new List<IGroupable>();

                foreach (IGroupable groupitem in list)
                {
                    if (groupitem.CurrentSerialNumber == (item as IGroupable).CurrentSerialNumber)
                    {
                        groupItems.Add(groupitem);
                        break;
                    }
                }
                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = true;
                    CurrentSelection.Add(groupItem);
                }
            }
            else
            {
                item.IsSelected = true;
                CurrentSelection.Add(item);
            }
        }

        /// <summary>
        /// 清空选中的集合
        /// </summary>
        public void ClearSelection()
        {
            CurrentSelection.ForEach(item => item.IsSelected = false);
            CurrentSelection.Clear();
        }
        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            ClearSelection();
            CurrentSelection.AddRange(designerCanvas.Children.OfType<ISelectable>());
            CurrentSelection.ForEach(item => item.IsSelected = true);
        }
        #endregion

        #region 备份
        ///// <summary>
        ///// 移除组件的关系
        ///// </summary>
        ///// <param name="item"></param>
        //public void RemoveFromSelection(ISelectable item)
        //{
        //    if (item is IGroupable)
        //    {
        //        List<IGroupable> groupItems = GetGroupMembers(item as IGroupable);

        //        foreach (ISelectable groupItem in groupItems)
        //        {
        //            groupItem.IsSelected = false;
        //            CurrentSelection.Remove(groupItem);
        //        }
        //    }
        //    else
        //    {
        //        item.IsSelected = false;
        //        CurrentSelection.Remove(item);
        //    }
        //}

        //public List<IGroupable> GetGroupMembers(IGroupable item)
        //{
        //    IEnumerable<IGroupable> list = designerCanvas.Children.OfType<IGroupable>();
        //    IGroupable rootItem = GetRoot(list, item);
        //    return GetGroupMembers(list, rootItem);
        //}

        //public IGroupable GetGroupRoot(IGroupable item)
        //{
        //    IEnumerable<IGroupable> list = designerCanvas.Children.OfType<IGroupable>();
        //    return GetRoot(list, item);
        //}

        //private IGroupable GetRoot(IEnumerable<IGroupable> list, IGroupable node)
        //{
        //    if (node == null || string.IsNullOrEmpty(node.ParentSerialNumber))
        //    {
        //        return node;
        //    }
        //    else
        //    {
        //        foreach (IGroupable item in list)
        //        {
        //            if (item.CurrentSerialNumber == node.CurrentSerialNumber)
        //            {
        //                return GetRoot(list, item);
        //                break;
        //            }
        //        }
        //        return null;
        //    }
        //}

        //private List<IGroupable> GetGroupMembers(IEnumerable<IGroupable> list, IGroupable parent)
        //{
        //    List<IGroupable> groupMembers = new List<IGroupable>();
        //    groupMembers.Add(parent);

        //    var children = list.Where(node => node.ParentSerialNumber == parent.CurrentSerialNumber);

        //    foreach (IGroupable child in children)
        //    {
        //        groupMembers.AddRange(GetGroupMembers(list, child));
        //    }

        //    return groupMembers;
        //}

        #endregion
    }
}
