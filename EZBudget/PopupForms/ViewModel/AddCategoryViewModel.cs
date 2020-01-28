using EZBudget.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EZBudget.PopupForms.ViewModel
{
    public class AddCategoryViewModel : BaseViewModel
    {
        // Category name
        public string CategoryName { get; set; }
        public Visibility InvalidCategoryName_Visibility { get; set; } = Visibility.Collapsed;

        // Category description
        public string CategoryDescription { get; set; }
        public Visibility InvalidCategoryDescription_Visibility { get; set; } = Visibility.Collapsed;

        // Category amount
        public string CategoryAmount { get; set; }
        public Visibility InvalidCategoryAmount_Visibility { get; set; } = Visibility.Collapsed;

        // Category color tag
        public string CategoryColorTag { get; set; }
        public Visibility InvalidCategoryColorTag_Visibility { get; set; } = Visibility.Collapsed;

        // Is Category valid
        public bool IsCategoryValid { get; set; } = false;
    }
}
