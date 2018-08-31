using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MefWebApp.Models;
using MefWebApp.Pipelines;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MefWebApp.Controllers
{
    public class HomeController : Controller
    {

        // Injected reference to the MessagePipeline
        private readonly MessagePipeline _messagePipeline;


        public HomeController(MessagePipeline messagePipeline)
        {
            _messagePipeline = messagePipeline;

        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Determine the list of steps that apply for this page so that we can render the list.
            var actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var stepsForThisAction = _messagePipeline.GetSteps(actionDescriptor.ActionName);
            ViewData["PipelineSteps"] = stepsForThisAction.Select(s => s.StepName).ToArray();
            ViewData["ActionName"] = actionDescriptor.ActionName;
        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var message = "Your application description page.";

            // Run the pipeline filter steps on the message.
            message = _messagePipeline.Execute(message, this.ControllerContext.ActionDescriptor.ActionName);

            ViewData["Message"] = message;
            
            return View();
        }

        public IActionResult Contact()
        {
            var message = "Your contact page.";

            // Run the pipeline filter steps.
            message = _messagePipeline.Execute(message, this.ControllerContext.ActionDescriptor.ActionName);

            ViewData["Message"] = message;

            return View();
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
