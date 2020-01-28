using EZBudget.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ObservableCollection<Expense> Expenses { get; set; }
        

    }
}
