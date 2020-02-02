using EZBudget.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EZBudget.DataModels
{
    public class Category
    {
        public int ID { get; set; }
        public int GlobalID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ColorTag { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalExpenses { get { return Expenses.Sum(x => x.Amount); } }
        public decimal BudgetLeft { get { return Amount - TotalExpenses; } }

        public string PercentageSpent
        {
            get
            {
                var percentage = TotalExpenses / Amount;
                return Math.Floor(percentage * 100).ToString() + "%";
            }
        }
        public SolidColorBrush BudgetLeftColor { 
            get { 
                return (BudgetLeft >= 0) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00e600")) 
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e63900")); 
            } 
        }

        public double ProgressBarWidth
        {
            get
            {
                var percentage = TotalExpenses / Amount;
                percentage = (percentage > 1) ? new decimal(1) : percentage;

                // 200 is the progress bar max width
                return Decimal.ToDouble(200 * percentage);
            }
        }

        public List<Expense> Expenses { get; set; }
        

    }
}
