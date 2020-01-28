using EZBudget.PopupForms.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EZBudget.PopupForms
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {

        private AddCategoryViewModel _mViewModel;
        public AddCategoryViewModel MViewModel { get { return _mViewModel; } }

        public AddCategoryWindow()
        {
            InitializeComponent();
            this._mViewModel = new AddCategoryViewModel();
            this.DataContext = _mViewModel;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // Validate Name
            if (string.IsNullOrEmpty(tbName.Text))
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidCategoryName_Visibility = Visibility.Visible;
            }
            else
            {
                // Hide X icon beside control 
                _mViewModel.InvalidCategoryName_Visibility = Visibility.Collapsed;
            }

            // Validate Description
            if (string.IsNullOrEmpty(tbDescription.Text))
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidCategoryDescription_Visibility = Visibility.Visible;
            }
            else
            {
                // Hide X icon beside control 
                _mViewModel.InvalidCategoryDescription_Visibility = Visibility.Collapsed;
            }

            // Validate Amount
            if (string.IsNullOrEmpty(tbAmount.Text))
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidCategoryAmount_Visibility = Visibility.Visible;
            }
            else
            {
                var value = new decimal(0);
                if (!Decimal.TryParse(tbAmount.Text, out value))
                {
                    isValid = false;
                    // Set X icon beside control
                    _mViewModel.InvalidCategoryAmount_Visibility = Visibility.Visible;
                }
                else
                {
                    // Hide X icon beside control 
                    _mViewModel.InvalidCategoryAmount_Visibility = Visibility.Collapsed;
                }
            }

            // Validate color tag
            if (string.IsNullOrEmpty(colorPicker.HexadecimalString))
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidCategoryColorTag_Visibility = Visibility.Visible;
            }
            else
            {
                // Hide X icon beside control 
                _mViewModel.InvalidCategoryColorTag_Visibility = Visibility.Collapsed;
            }

            if (isValid)
            {
                _mViewModel.CategoryColorTag = colorPicker.HexadecimalString;
                _mViewModel.IsCategoryValid = true;
                this.Close();
            }
        }
    }
}
