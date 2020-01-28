using EZBudget.DataModels;
using EZBudget.PopupForms.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddExpenseWindow.xaml
    /// </summary>
    public partial class AddExpenseWindow : Window
    {
        public AddExpenseViewModel _mViewModel { get; set; }

        public AddExpenseWindow(int logedInUserId)
        {
            InitializeComponent();
            _mViewModel = new AddExpenseViewModel(logedInUserId);
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
                _mViewModel.InvalidExpenseName_Visibility = Visibility.Visible;
            }
            else
            {
                // Hide X icon beside control 
                _mViewModel.InvalidExpenseName_Visibility = Visibility.Collapsed;
            }

            // Validate Description
            if (string.IsNullOrEmpty(tbDescription.Text))
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidExpenseDescription_Visibility = Visibility.Visible;
            }
            else
            {
                // Hide X icon beside control 
                _mViewModel.InvalidExpenseDescription_Visibility = Visibility.Collapsed;
            }

            // Validate Amount
            if (string.IsNullOrEmpty(tbAmount.Text))
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidExpenseAmount_Visibility = Visibility.Visible;
            }
            else
            {
                var value = new decimal(0);
                if (!Decimal.TryParse(tbAmount.Text, out value))
                {
                    isValid = false;
                    // Set X icon beside control
                    _mViewModel.InvalidExpenseAmount_Visibility = Visibility.Visible;
                }
                else
                {
                    // Hide X icon beside control 
                    _mViewModel.InvalidExpenseAmount_Visibility = Visibility.Collapsed;
                }
            }

            // Validate category
            if (cmbCategories.SelectedItem == null)
            {
                isValid = false;
                // Set X icon beside control
                _mViewModel.InvalidExpenseCategory_Visibility = Visibility.Visible;
            }
            else
            {
                // Hide X icon beside control 
                _mViewModel.InvalidExpenseCategory_Visibility = Visibility.Collapsed;
            }

            if (isValid)
            {
                _mViewModel.selectedCategoryMonthlyId = ((Category)cmbCategories.SelectedItem).ID;
                _mViewModel.IsExpenseValid = true;
                this.Close();
            }
        }
    }
}
