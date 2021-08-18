using MailerApp_Net5_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailerApp_Net5_0.Interfaces
{
    public interface IMailService
    {
        Task<string> SendMail(Email email);
    }
}
