using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZBudget.DataModels
{
    public class PastMonth
    {
        public DateTime Date { get; set; }
        public string Month { get { return GetMonthName(Date.Month); } private set { this.Month = value; } }
        public string Year { get { return Date.Year.ToString(); } private set { this.Year = value; } }
        public decimal TotalExpenses { get; set; }

        private string GetMonthName(int month)
        {
            switch (month)
            {
                case 1: return "January";
                case 2: return "February";
                case 3: return "March";
                case 4: return "April";
                case 5: return "May";
                case 6: return "June";
                case 7: return "July";
                case 8: return "August";
                case 9: return "September";
                case 10: return "October";
                case 11: return "November";
                case 12: return "December";
                default: return "No month";
            }

        }
    }
}
