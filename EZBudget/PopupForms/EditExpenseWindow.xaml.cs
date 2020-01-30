using EZBudget.DataModels;
using EZBudget.Entity;
using EZBudget.PopupForms.ViewModel;
using Microsoft.Win32;
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
    /// Interaction logic for EditExpenseWindow.xaml
    /// </summary>
    public partial class EditExpenseWindow : Window
    {

        public AddExpenseViewModel _mViewModel { get; set; }

        public EditExpenseWindow(int logedInUserId, Expense expense)
        {
            InitializeComponent();
            _mViewModel = new AddExpenseViewModel(logedInUserId);
            this.DataContext = _mViewModel;

            // set view model datas
            _mViewModel.ExpenseName = expense.ExpenseName;
            _mViewModel.ExpenseDescription = expense.ExpenseDescription;
            _mViewModel.ExpenseAmount = expense.Amount.ToString();
            _mViewModel.ReceiptUrl = expense.ExpenseBillImageUrl;

            if (expense.ExpenseBillImageUrl != "")
                _mViewModel.ReceiptUrlPreview = expense.ExpenseBillImageUrl;
        }


        private void btnBrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _mViewModel.ReceiptUrl = openFileDialog.FileName;
                _mViewModel.ReceiptUrlPreview = openFileDialog.FileName;
            }
        }

        private void btnTrash_Click(object sender, RoutedEventArgs e)
        {
            _mViewModel.ReceiptUrl = "";
            _mViewModel.ResetImagePreview();
        }

        private void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (_mViewModel.ReceiptUrl != null && _mViewModel.ReceiptUrl != "")
                new ImageFullScreenViewerWindow(_mViewModel.ReceiptUrl).ShowDialog();
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
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
