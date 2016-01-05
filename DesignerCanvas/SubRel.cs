using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DesignerCanvas
{
    public class SubRel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region 接口
        public event PropertyChangedEventHandler PropertyChanged;
        private void PropertyChange(string pro)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pro));
            }
        }
        #endregion

        #region 属性
        private string _tradeCode;
        /// <summary>
        /// 交易编码
        /// </summary>
        public string TradeCode
        {
            get { return _tradeCode; }
            set
            {
                _tradeCode = value;
                PropertyChange("TradeCode");
            }
        }
        private string _compCode;
        /// <summary>
        /// 组件编码
        /// </summary>
        public string CompCode
        {
            get { return _compCode; }
            set
            {
                _compCode = value;
                PropertyChange("CompCode");
            }
        }
        private string _serialNumber;
        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                PropertyChange("SerialNumber");
            }
        }
        private string _inData;
        /// <summary>
        /// 输入域
        /// </summary>
        public string InData
        {
            get { return _inData; }
            set
            {
                if (value == null) value = "";
                _inData = value;
                PropertyChange("InData");
            }
        }
        private string _outData;
        /// <summary>
        /// 输出域
        /// </summary>
        public string OutData
        {
            get { return _outData; }
            set
            {
                if (value == null) value = "";
                _outData = value;
                PropertyChange("OutData");
            }
        }
        //private string _intype;
        /// <summary>
        /// 输入域类型
        /// </summary>
        //public string InType
        //{
        //    get { return _intype; }
        //    set
        //    {
        //        _intype = value;
        //        PropertyChange("InType");
        //    }
        //}
        private TypeOpt _intype;

        public TypeOpt InType
        {
            get { return _intype; }
            set
            {
                if (value == null) value = TypeOpt.常量;
                _intype = value;
                PropertyChange("InType");
            }
        }
        
        private string _memo;
        /// <summary>
        /// 输入域描述
        /// </summary>
        public string Memo
        {
            get { return _memo; }
            set
            {
                if (value == null) value = "";
                _memo = value;
                PropertyChange("Memo");
            }
        }
         ///<summary>
         ///数据域集合
         ///</summary>
        //public List<DataDic> _dataDicList = null;
        //public List<DataDic> DicList
        //{
        //    get
        //    {
        //        return _dataDicList;
        //    }
        //    set
        //    {
        //        if (_dataDicList == null)
        //        {
        //            _dataDicList = new List<DataDic>();
        //        }
        //        _dataDicList = value;
        //        PropertyChange("DicList");
        //    }
        //}
        /// <summary>
        /// 输入数据集合
        /// </summary>
        //public List<InDataType> _dataTypeList = null;
        //public List<InDataType> TypeList
        //{
        //    get
        //    {
        //        return _dataTypeList;
        //    }
        //    set
        //    {
        //        if (_dataTypeList == null)
        //        {
        //            _dataTypeList = new List<InDataType>();
        //        }
        //        _dataTypeList = value;
        //        PropertyChange("TypeList");
        //    }
        //}
        #endregion

        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case "InData":
                        if (string.IsNullOrEmpty(InData))
                        {
                            result = "不允许为空";
                        }
                        break;
                    case "Memo":
                        if (string.IsNullOrEmpty(Memo))
                        {
                            result = "不允许为空";
                        }
                        break;
                }

                return result;
            }
        }
    }

    public enum TypeOpt { 常量, 表达式 };


}
