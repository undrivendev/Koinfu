using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL
{
    public interface IEmailValidator
    {
        bool IsEmailValid(string email);
    }
}
