using FodraszatIdopont.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FodraszatIdopont.Controllers
{
    public class BaseController : Controller
    {
        protected void WriteToLog(string message, string rootPath)
        {
            LoggerHelper.WriteToLog(message, rootPath);
        }
    }
}