//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EZBudget.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Setting
    {
        public int SettingID { get; set; }
        public Nullable<int> UserID { get; set; }
        public System.DateTime AccountCreationDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public string Language { get; set; }
    
        public virtual User User { get; set; }
    }
}
