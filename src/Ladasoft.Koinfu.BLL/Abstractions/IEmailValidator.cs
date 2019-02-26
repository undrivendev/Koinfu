using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL
{
    public interface IEmailValidator
    {
        bool IsEmailValid(string email);
    }
}
