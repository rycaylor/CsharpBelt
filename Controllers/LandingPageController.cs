using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using blackbelt.Models;
using blackbelt.Factory;

namespace blackbelt.Controllers
{
    public class LandingPageController : Controller
    {
        private readonly UserFactory userFactory;
        private readonly AuctionFactory auctionFactory;
        public LandingPageController(UserFactory connection, AuctionFactory connection1)
        {
            userFactory = connection;
            auctionFactory = connection1;

        }

        // GET: /Home/
        [HttpGet]
        [Route("home")]
        public IActionResult Home()
        {
            if(HttpContext.Session.GetInt32("logged")==null){
                return Redirect("/");
            }
            var auctions = auctionFactory.GrabAllAuctions();
            foreach(var auction in auctions)
            {
                if(auction.TimeLeft.Days <= 0 && auction.TimeLeft.Hours <= 0)
                {
                    auctionFactory.CompleteAuction(auction.Id);
                }
            }
            ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
            ViewBag.auctions = auctionFactory.GrabAllAuctions();
            return View("Home");
        }

        [HttpGet]
        [Route("createAuction")]
        public IActionResult CreateAuction()
        {
            if(HttpContext.Session.GetInt32("logged")==null){
                return Redirect("/");
            }
            ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
            return View("CreateAuction");
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult DeleteAuction(int id)
        {
            if(HttpContext.Session.GetInt32("logged")==null){
                return Redirect("/");
            }
            auctionFactory.DeleteAuction(id);
            ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
            return RedirectToAction("Home");
        }

        [HttpPost]
        [Route("newAuction")]
        public IActionResult NewAuction(Auction newAuction)
        {
            if(HttpContext.Session.GetInt32("logged")==null){
                return Redirect("/");
            }
            if(ModelState.IsValid)
            {
                if(newAuction.EndingDate<DateTime.Now)
                {
                    ViewBag.DateStatus = "Please Enter An Ending Date After Today";
                    ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
                    return View("CreateAuction");
                }
                if(newAuction.Bid<= 0)
                {
                    ViewBag.BidStatus = "We Dont Give Stuff Away For Free";
                    ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
                    return View("CreateAuction");
                }
                else
                {
                    auctionFactory.CreateAuction(newAuction);
                    return RedirectToAction ("Home");
                }
            }
            ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
            return View("CreateAuction");
        }
        [HttpGet]
        [Route("/show/{id}")]
        public IActionResult ShowAuction(int id)
        {
            if(HttpContext.Session.GetInt32("logged")==null){
                return Redirect("/");
            }
            if(TempData["error"] != null)
            {
                ViewBag.error = TempData["error"];
            }
            ViewBag.auction = auctionFactory.GrabAuction(id);
            ViewBag.bidder = auctionFactory.GrabCurrentBidder(id);
            ViewBag.user = userFactory.GetUserById((int)HttpContext.Session.GetInt32("logged"));
            return View("SingAuction");
        }

        [HttpPost]
        [Route("/makebid/{id}")]
        public IActionResult MakeBid(int id, int user, float bidUpdate)
        {
            if(HttpContext.Session.GetInt32("logged")==null){
                return Redirect("/");
            }
            User bidd = userFactory.GetUserById(user);
            Auction auction = auctionFactory.GrabAuction(id);
            if(bidUpdate>bidd.Wallet)
            {
                TempData["error"] = "You dont have enough money!";
                return RedirectToAction("ShowAuction", id);
            }
            else if(bidUpdate == auction.Bid)
            {
                TempData["error"] = "Bid Needs to Be higher Than Current Bid";
                return RedirectToAction("ShowAuction", id);
            }
            Bidder updateBid = new Bidder {UserId = user, Bid = bidUpdate, AuctionId =id};
            auctionFactory.UpdateBid(updateBid);
                return RedirectToAction("Home");
        }

    }
}
