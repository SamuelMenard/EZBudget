using EZBudget.DataModels;
using EZBudget.Queries;
using EZBudget.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EZBudget.PopupForms.ViewModel
{
    public class AddExpenseViewModel : BaseViewModel
    {
        private int LogedInUserId { get; set; }
        private string NoImageURL { get; } = @"C:\Users\Samuel Ménard\repos\EZBudget\EZBudget\Images\Others\no_image.png";

        public string ReceiptUrlPreview { get; set; }
        public string ReceiptUrl { get; set; }


        // Expense name
        public string ExpenseName { get; set; }
        public Visibility InvalidExpenseName_Visibility { get; set; } = Visibility.Collapsed;

        // Expense description
        public string ExpenseDescription { get; set; }
        public Visibility InvalidExpenseDescription_Visibility { get; set; } = Visibility.Collapsed;

        // Expense amount
        public string ExpenseAmount { get; set; }
        public Visibility InvalidExpenseAmount_Visibility { get; set; } = Visibility.Collapsed;

        // Expense category
        public ObservableCollection<Category> Categories { get { return EZBudgetDB.GetCurrentMonthBudgetCategories(LogedInUserId); } }
        public int selectedCategoryMonthlyId { get; set; }
        public Visibility InvalidExpenseCategory_Visibility { get; set; } = Visibility.Collapsed;

        // Is Expense valid
        public bool IsExpenseValid { get; set; } = false;

        public AddExpenseViewModel(int logedInUserId)
        {
            this.LogedInUserId = logedInUserId;
            this.ReceiptUrlPreview = NoImageURL;
        }

        public void ResetImagePreview()
        {
            this.ReceiptUrlPreview = NoImageURL;
        }
    }
}
