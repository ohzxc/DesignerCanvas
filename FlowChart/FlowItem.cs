using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FlowChart.Interfaces;

namespace FlowChart
{
    //属性的命名，应用的模板类型
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class FlowItem : ContentControl, ISelectable, IGroupable, INotifyPropertyChanged
    {
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


        public string ToolTips { get; set; }
        #region 定义选中的依赖属性
        /// <summary>
        ///是否被选中
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(FlowItem),
                                       new FrameworkPropertyMetadata(false));
        #endregion

        #region 定义拖拽数据模板的附加属性

        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(FlowItem));

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
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(FlowItem));

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
                                         typeof(FlowItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        #endregion

        public string CurrentSerialNumber
        {
            get;
            set;
        }

        /// <summary>
        ///  
        /// </summary>
        public bool IsGroup { get; set; }

        public string ParentSerialNumber
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

       

        #region 实现INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region 构造函数
        static FlowItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlowItem), new FrameworkPropertyMetadata(typeof(FlowItem)));
        }
        public FlowItem()
        {
            this.Loaded += FlowItem_Loaded;
        }
        #endregion
        private void FlowItem_Loaded(object sender, RoutedEventArgs e)
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
                        //Label txt = Common.FindVisualChild<Label>(contentVisual as Label);
                        //if (txt != null)
                        //    txt.Content = CurrentSerialNumber;
                        //ComponentCode = (contentVisual as Label).Tag.ToString().Trim();
                        if (contentVisual != null)
                        {
                            DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                            Control connectorDecorator = this.Template.FindName("PART_ConnectorDecorator", this) as Control;
                            if (thumb != null)
                            {
                                ControlTemplate template =
                                    FlowItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                                if (template != null)
                                    thumb.Template = template;
                            }
                            if (connectorDecorator != null)
                            {
                                ControlTemplate template =
                                    FlowItem.GetConnectorDecoratorTemplate(contentVisual) as ControlTemplate;
                                if (template != null)
                                    connectorDecorator.Template = template;
                            }
                        }
                    }

                }
            }
        }
        
    }
}
