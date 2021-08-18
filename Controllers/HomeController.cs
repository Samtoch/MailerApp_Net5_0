using MailerApp_Net5_0.Interfaces;
using MailerApp_Net5_0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MailerApp_Net5_0.Controllers
{
    public class HomeController : Controller
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private readonly IMailService _mailService;
        public HomeController(IMailService mailService)
        {
            _mailService = mailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Email email)
        {
            try
            {
                log.Info("Input to send mail by: " + email.Recipient);
                string response = await _mailService.SendMail(email);

                TempData["resMeg"] = "Response from Mail Kit is: " + response;
                TempData.Keep();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Error in Index file for: " + email.Recipient + "\r\n" + ex);
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
