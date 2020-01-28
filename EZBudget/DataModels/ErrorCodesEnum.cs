using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZBudget.DataModels
{
    public enum ErrorCodesEnum
    {
        ConnectionOK = 200,
        UsernameWrongFormat = 300,
        PasswordWrongFormat = 301,
        UsernameAndPasswordWrongFormat = 302,
        UnknownError = 400,
        UsernameOrPasswordInvalid = 401

    }
}
