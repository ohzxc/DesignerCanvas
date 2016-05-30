using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FlowChart.Interfaces;

namespace FlowChart
{
    public class FlowItem : ContentControl, ISelectable, IGroupable, INotifyPropertyChanged
    {
        public string CurrentSerialNumber
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

        /// <summary>
        ///  
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// 是否被选中
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

        public bool IsDragConnectionOver { get; internal set; }

        #region 实现INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
