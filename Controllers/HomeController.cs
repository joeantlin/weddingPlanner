using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Register", "Login");
            }
            return RedirectToAction("Dashboard");
        }
        [Route("dashboard")]
        [HttpGet]
        public IActionResult Dashboard()
        {
            // List<RSVP> allRSVP = dbContext.RSVPs
            //     .Include(j => j.User)
            //     .Include(j => j.Wedding)
            //     .ToList();
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Register", "Login");
            }
            int? IntVariable = HttpContext.Session.GetInt32("UserID");
            int sessionId = IntVariable ?? default(int);
            ViewBag.MyId = sessionId;

            List<Wedding> allWeddings = dbContext.Weddings
                .Include(j => j.Users)
                .ThenInclude(k => k.User)
                .ToList();
            return View(allWeddings);
        }

        [Route("newwedding")]
        [HttpGet]
        public IActionResult NewWedding()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Register", "Login");
            }
            return View();
        }
        
        [Route("createwedding")]
        [HttpPost]
        public IActionResult CreateWedding(Wedding wedding)
        {
            if(ModelState.IsValid)
            {
                dbContext.Weddings.Add(wedding);
                dbContext.SaveChanges();
                int wId = wedding.Id;

                int? IntVariable = HttpContext.Session.GetInt32("UserID");
                int sessionId = IntVariable ?? default(int);

                RSVP newRsvp = new RSVP();
                newRsvp.UserId = sessionId;
                newRsvp.WeddingId = wId;
                newRsvp.Creator = true;
                dbContext.RSVPs.Add(newRsvp);
                dbContext.SaveChanges();

                return RedirectToAction("ViewWedding", new {id = wId});
            }
            return View("NewWedding");
        }

        [Route("viewwedding/{id}")]
        [HttpGet]
        public IActionResult ViewWedding(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Register", "Login");
            }
            Wedding wedding = dbContext.Weddings
                .Include(j => j.Users)
                .ThenInclude(k => k.User)
                .FirstOrDefault(i => i.Id == id);
            return View(wedding);
        }

        [Route("newrsvp/{id}")]
        [HttpGet]
        public IActionResult NewRSVP(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("UserID");
            int sessionId = IntVariable ?? default(int);

            RSVP newRsvp = new RSVP();
            newRsvp.UserId = sessionId;
            newRsvp.WeddingId = id;
            dbContext.RSVPs.Add(newRsvp);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [Route("deletersvp/{id}")]
        [HttpGet]
        public IActionResult DeleteRSVP(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("UserID");
            int sessionId = IntVariable ?? default(int);

            RSVP rsvp = dbContext.RSVPs.SingleOrDefault(i => i.UserId == sessionId && i.WeddingId == id);
            dbContext.RSVPs.Remove(rsvp);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [Route("deletewedding/{id}")]
        [HttpGet]
        public IActionResult DeleteWedding(int id)
        {
            int? IntVariable = HttpContext.Session.GetInt32("UserID");
            int sessionId = IntVariable ?? default(int);

            Wedding wedding = dbContext.Weddings
                .Include(j => j.Users)
                .ThenInclude(k => k.User)
                .FirstOrDefault(i => i.Id == id);

            dbContext.Weddings.Remove(wedding);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
