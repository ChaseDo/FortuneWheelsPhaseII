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
            string sErrorMessage = string.Empty;
            string sPhoneNumber = string.Empty;

            if (this.Request.QueryString["mo"] != null)
            {
                sPhoneNumber = this.Request.QueryString["mo"];
            }
            else if (this.Request.QueryString["Phone"] != null)
            {
                sPhoneNumber = this.Request.QueryString["Phone"];
            }
            else
            {
                sPhoneNumber = string.Empty;
            }    

            if (string.IsNullOrEmpty(sPhoneNumber))
            {
                sErrorMessage = "请先登录再开始下载任务";
            }
            else
            {
                LotteryLogic.StoreLotteryUser(sPhoneNumber);

                //登录页面，若用户还未下载完所有任务
                if (!LotteryLogic.CheckAllDownload(sPhoneNumber))
                {
                    sErrorMessage = "请在活动页面上下载指定游戏，参与活动吧！";
                }
                //用户完成所有下载任务，拿到并用完了所有抽奖机会
                if (!LotteryLogic.CheckLotteryTime(sPhoneNumber))
                {
                    sErrorMessage = "谢谢参与，敬请期待下期活动";
                }
            }
            //sErrorMessage = "活动已结束，无法抽奖";
            ViewBag.ErrorMessage = sErrorMessage;
            return View();
        }

        public ActionResult Begin(string sPhoneNumber)
        {
            int sAngle = 180;
            string sErrorMessage = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(sPhoneNumber))
                {
                    sErrorMessage = "请先登录再开始下载任务";
                }
                else
                {
                    if (LotteryLogic.CheckLotteryTime(sPhoneNumber))
                    {
                        sAngle = LotteryLogic.StartLottery(sPhoneNumber).Angle;
                    }
                    else
                    {
                        sErrorMessage = "您已使用完抽奖次数，谢谢参与";
                    }
                }
                //sErrorMessage = "活动已结束，无法抽奖";
            }
            //catch (HttpException e)
            //{
            //    sErrorMessage = "发送短信失败";
            //}
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

        public ActionResult Download(string sPhoneNumber)
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

        //
        // Change Rate Page
        //

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
