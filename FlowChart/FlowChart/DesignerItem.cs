using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media; 
using System.Windows.Documents;

using System.Windows.Controls.Primitives;
using System.ComponentModel;
namespace FlowChart
{
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, INotifyPropertyChanged
    {        
        #region IsSelected Property
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

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
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

        #region ConnectorDecoratorTemplate Property

        // can be used to replace the default template for the ConnectorDecorator
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

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
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

        private DesignerItemTip sourceTipSymbol = DesignerItemTip.None;
        public DesignerItemTip SourceTipSymbol
        {
            get { return sourceTipSymbol; }
            set
            {
                if (sourceTipSymbol != value)
                {
                    sourceTipSymbol = value;
                    OnPropertyChanged("SourceTipSymbol");
                }
            }
        }

        
        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }
        /// <summary>
        /// 当前序号
        /// </summary>
        public string CurrentSerialNumber{get;set;}
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo { get; set; }
        
        public DesignerItem(string key)
        {
            CurrentSerialNumber = key;
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            // update selection
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
            e.Handled = false;
        }
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
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            Popup pop = new Popup();
            TextBlock txbKeyword = new TextBlock();
            txbKeyword.Width = 360;          
            txbKeyword.FontSize = 15;
            txbKeyword.TextWrapping = TextWrapping.Wrap;
            txbKeyword.Text = this.Remark;
            var border = new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.DarkGreen),
                BorderThickness = new Thickness(5),
                Width = 400,    
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5)
            };           
             border.Child = txbKeyword;
            pop.Child = border;
            pop.PlacementTarget = this;
            pop.Placement = PlacementMode.Mouse ;           
            pop.IsOpen = true;
            pop.StaysOpen = false;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(txbKeyword);
            adornerLayer.Add(new DesignerCanvasAdorner(txbKeyword));            
            base.OnMouseDoubleClick(e);
        }
        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
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
    public enum DesignerItemTip
    {
        None,
        Error,
        UnHandled,
        Execute,
        Success
    }
}
