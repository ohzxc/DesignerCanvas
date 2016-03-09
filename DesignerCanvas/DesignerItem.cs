using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System;
using DesignerCanvas.Controls;
using System.ComponentModel;

namespace DesignerCanvas
{
    //属性的命名，应用的模板类型
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable, INotifyPropertyChanged
    {
        #region 字段
        
        #endregion

        #region 属性

        /// <summary>
        /// 组件编号
        /// </summary>
        public string ComponentCode
        {
            get;
            set;
        }
        public new string Name
        {
            get
            {
                return (this.Content as Label).Content.ToString();
            }
            protected set { }
        }

        public string Tradecode { get; set; }
        public List<string> ConditionList
        {
            get;
            set;
        }

        public List<SubRel> SubRelList
        {
            get;set;
        }

        public string ToolTips { get; set; }
        #region 定义选中的依赖属性
        /// <summary>
        /// 已经选中
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));
        #endregion

        #region 定义拖拽数据模板的附加属性

        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);

        }
        #endregion

        #region 定义线条装饰器模板的附加属性
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region 定义是否拖拽结束的附加属性
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        #endregion

        #region 构造函数
        static DesignerItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }
        public DesignerItem(string tradecode,string key,List<SubRel> subRelList=null, Dictionary<string,string> m_Connection=null)
        {
            Tradecode = tradecode;
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
            this.CurrentSerialNumber = key;
            ConditionList = new List<string>();
            foreach (var item in m_Connection)
            {
                ConditionList.Add(item.Key + "," + item.Value);
            }
            SubRelList =new List<SubRel>();
            if (subRelList != null)  SubRelList=subRelList;                 
        }
        #endregion

        #region 事件
        /// <summary>
        /// Load加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                    {
                        UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                        Label txt = FindVisualChild<Label>(contentVisual as Label);
                        if (txt != null)
                            txt.Content = CurrentSerialNumber;
                        ComponentCode = (contentVisual as Label).Tag.ToString().Trim();
                        if (contentVisual != null)
                        {
                            DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                            Control connectorDecorator = this.Template.FindName("PART_ConnectorDecorator", this) as Control;
                            if (thumb != null)
                            {
                                ControlTemplate template =
                                    DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                                if (template != null)
                                    thumb.Template = template;
                            }
                            if (connectorDecorator != null)
                            {
                                ControlTemplate template =
                                    DesignerItem.GetConnectorDecoratorTemplate(contentVisual) as ControlTemplate;
                                if (template != null)
                                    connectorDecorator.Template = template;
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            MyCanvas designer = VisualTreeHelper.GetParent(this) as MyCanvas;
            //designer.Focus();
            var lbl = (this.Content) as Label;
            lbl.IsHitTestVisible = false;
            lbl.Focusable = false;
            if (designer != null)
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        this.IsSelected = false;
                        designer.SelectedItems.Remove(this);

                    }
                    else
                    {
                        this.IsSelected = true;
                        designer.SelectedItems.Add(this);
                    }
                else if (!this.IsSelected)
                {
                    foreach (ISelectable item in designer.SelectedItems)
                        item.IsSelected = false;

                    designer.SelectedItems.Clear();
                    this.IsSelected = true;
                    designer.SelectedItems.Add(this);
                }
            designer.Focus();

            e.Handled = false;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {

            base.OnMouseDoubleClick(e);
            MyCanvas designer = VisualTreeHelper.GetParent(this) as MyCanvas;
            var tradeFlow="";
            designer.Save().ForEach(x =>
            {
                if (x.FlowCode == this.CurrentSerialNumber)
                    tradeFlow = x.Trade_Flow;
            });
            
            if (designer != null)
            {
                if (SubRelList == null)
                    SubRelList = new List<SubRel>();
                Show(this.Name, ComponentCode, CurrentSerialNumber,tradeFlow,ConditionList,SubRelList);
            }
        }

        public void  Show(string compName, string comp_code, string serialnumber,string tradeFlow, List<string> conditionList, List<SubRel> subRelList = null)
        {
            //var list = new List<SubRel>();
            //if (subRelList == null) subRelList = new List<SubRel>();
            //subRelList.ForEach(x =>
            //    {
            //        if (x.SerialNumber == serialnumber)
            //        {
            //            list.Add(x);
            //        }
            //    });
            try
            {
                var editSubTrade = new EditSubTrade(Tradecode,compName, comp_code, serialnumber, tradeFlow, SubRelList, conditionList);
                editSubTrade.Owner = Application.Current.MainWindow;
                //editSubTrade.PassValuesEvent += new EditSubTrade.PassValuesHandler(ReceiveValues);
                editSubTrade.ShowDialog();
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            //return list;
        }
        #endregion
        
        #region 实现IGroupable接口的成员
        /// <summary>
        /// 关联组件序号
        /// </summary>
        public string ParentSerialNumber
        {
            get;
            set;
        }

        public bool IsGroup
        {
            get;
            set;
        }
        private string _currentSerialNumber;
        /// <summary>
        /// 当前组件序号
        /// </summary>
        public string CurrentSerialNumber
        {
            get
            {
                return _currentSerialNumber;
            }
            set
            {
                if (_currentSerialNumber != value)
                {
                    _currentSerialNumber = value;
                    OnPropertyChanged("CurrentSerialNumber");
                }
            }
        }

        #endregion
        #region 实现INotifyPropertyChanged 成员
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
        /// <summary>
        ///查找元素的子元素
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FindVisualChild<T>(DependencyObject obj)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
