using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using BusinessLogic;
using System.Threading;
using Entity.Model;

namespace FortuneWheel.Controllers
{
    public class LotteryController : Controller
    {
        //
        // GET: /Lottery/

        public ActionResult Lottery()
        {
            string sPhoneNumber = this.Request.QueryString["Phone"];
            string sErrorMessage = string.Empty;
            //if (LotteryLogic.QueryIsDownLoad(sPhoneNumber))
            //{
            //    LotteryLogic.StoreLotteryUser(sPhoneNumber);
            //    if (!LotteryLogic.CheckLotteryTime(sPhoneNumber))
            //    {
            //        sErrorMessage = "您已使用完抽奖次数，谢谢参与";
            //    }
            //}
            //else
            //{
            //    sErrorMessage = "请点击“火速前往”，下载安装《街头枪战2》游戏参与活动!";
            //}
            sErrorMessage = "活动已结束，无法抽奖";
            ViewBag.ErrorMessage = sErrorMessage;
            return View();
        }

        public ActionResult Begin(string sPhoneNumber)
        {
            int sAngle = 180;
            string sErrorMessage = string.Empty;
            try
            {
                //if (LotteryLogic.QueryIsDownLoad(sPhoneNumber))
                //{
                //    if (LotteryLogic.CheckLotteryTime(sPhoneNumber))
                //    {
                //        sAngle = LotteryLogic.StartLottery(sPhoneNumber).Angle;
                //    }
                //    else
                //    {
                //        sErrorMessage = "您已使用完抽奖次数，谢谢参与";
                //    }
                //}
                //else
                //{
                //    sErrorMessage = "快去下载安装《街头枪战2》游戏参与抽奖吧~";
                //}
                sErrorMessage = "活动已结束，无法抽奖";
            }
            catch (HttpException e)
            {
                sErrorMessage = "发送短信失败";
            }
            catch (Exception e)
            {
                sErrorMessage = " 发生错误请试重";
            }
            return Json(new
            {
                result = sAngle,
                error = sErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Refresh(string sPhoneNumber)
        {
            string sErrorMessage = string.Empty;
            int num = 0;
            try
            {
                num = LotteryLogic.GetLotteryTime(sPhoneNumber);
            }
            catch (Exception e)
            {
                sErrorMessage = " 发生错误请重试";
            }
            return Json(new
            {
                num = num,
                error = sErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sleep()
        {
            Thread.Sleep(3000);
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string sUserName, string sPassword)
        {
            if (sUserName == "admin" && sPassword == "gamecj@654321")
            {
                Session["user"] = "Authorized";
                return RedirectToAction("Detail");
            }
            return View();
        }

        public ActionResult Detail()
        {
            if (Session["user"] == null || Session["user"].ToString() != "Authorized")
            {
                return RedirectToAction("Login");
            }

            LotteryLogic.ResultList = null;
            List<LotteryAwards> userList = LotteryLogic.ResultList;
            return View(userList);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["user"] == null || Session["user"].ToString() != "Authorized")
            {
                return RedirectToAction("Login");
            }
            LotteryAwards model = LotteryLogic.ResultList.Where(t => t.AwardId == id).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(LotteryAwards model)
        {
            if (Session["user"] == null || Session["user"].ToString() != "Authorized")
            {
                return RedirectToAction("Login");
            }
            if (LotteryLogic.UpdateAward(model))
            {
                return RedirectToAction("Detail");
            }
            return View();
        }
    }
}
