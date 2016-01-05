using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace DesignerCanvas
{
    public class RouteValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            //if ((value as BindingGroup).BindingExpressions[0].Value == null || (value as BindingGroup).BindingExpressions[1].Value == null || (value as BindingGroup).BindingExpressions[3].Value == null) 
            //    return new ValidationResult(false, "不能为空。");
            var item = (value as BindingGroup).Items[0] as SubRel;
            if (item.Memo == null || item.InData == null || item.OutData == null || item.Memo == "" || item.InData == "" || item.OutData == "")
                return new ValidationResult(false, "填写不完整。");
            
            var regex = new Regex(@"^\d{4}$");
            var valiaInput = regex.IsMatch(item.InData);
            if (!valiaInput)
                return new ValidationResult(false, "域必须是四位数字!");
            
            return new ValidationResult(true, null);

        }
    }

    public class DataValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            //if ((value as BindingGroup).BindingExpressions[0].Value == null || (value as BindingGroup).BindingExpressions[1].Value == null || (value as BindingGroup).BindingExpressions[3].Value == null) 
            //    return new ValidationResult(false, "不能为空。");
            if (value == null)
                return new ValidationResult(false, "不能为空！");
            if (!System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), @"^\s*$|^[A-Za-z0-9]*$"))
                return new ValidationResult(false, "只能是数字和字母！");
            return new ValidationResult(true, null);

        }
    }

    public static class IsValid
    {
        public static bool IsHasValid(DependencyObject node)
        {
            // Check if dependency object was passed
            if (node != null)
            {
                // Check if dependency object is valid.
                // NOTE: Validation.GetHasError works for controls that have validation rules attached 
                bool isValid = !Validation.GetHasError(node);
                if (!isValid)
                {
                    // If the dependency object is invalid, and it can receive the focus,
                    // set the focus
                    if (node is IInputElement) Keyboard.Focus((IInputElement)node);
                    return false;
                }
            }

            // If this dependency object is valid, check all child dependency objects
            foreach (object subnode in LogicalTreeHelper.GetChildren(node))
            {
                if (subnode is DependencyObject)
                {
                    // If a child dependency object is invalid, return false immediately,
                    // otherwise keep checking
                    if (IsHasValid((DependencyObject)subnode) == false) return false;
                }
            }

            // All dependency objects are valid
            return true;
        }
    }
}
