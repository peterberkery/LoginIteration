using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginIteration.Models;
using System.Net.Mail;
using System.Net;
using System.Web.Security;

namespace LoginIteration.Controllers
{
    public class TableController : Controller
    {
            //Registration Action
            [HttpGet]
            public ActionResult Registration()
        {
            return View();
        }

            //Registratiion Post Action
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Registration([Bind(Exclude ="IsEmailVerified,ActivationCode")]Table table)
        {
            bool Status = false;
            string message = "";
            //Model Validation
            if (ModelState.IsValid)
            {
                #region//Email Already Exists
                var ifExist = IfEmailExist(table.EmailId);
                if (ifExist)
                {
                    ModelState.AddModelError("EmailExist", "An account with this email already exists");
                        return View(table);
                }

                #endregion

                #region//Activation code generation
                table.ActivationCode = Guid.NewGuid();
                #endregion

                #region//Password hashing
                table.Password = Crypto.Hash(table.Password);
                table.ConfirmPassword = Crypto.Hash(table.ConfirmPassword);
                #endregion
                table.IsEmailVerified = false;

                #region Save to DB
                using (FYPEntities dc = new FYPEntities())
                {
                    dc.Tables.Add(table);
                    dc.SaveChanges();

                    //Send Email to user
                    SendVerificationLinkEmail(table.EmailId, table.ActivationCode.ToString());
                    message = "Registration Sucessfully Done. Account activation link " +
                        " has been sent to your provided email:" + table.EmailId;
                    Status = true;

                }
                #endregion
            }
            else
            {
                message = "Invalid Requet";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(table);
        }

        //Verify Account through email

            [HttpGet]
            public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (FYPEntities dc = new FYPEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line avoids confirm password mismatch issue
                                                                // on save change method
                var v = dc.Tables.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "invalid request";
                }
            }
            ViewBag.Status = Status;
            return View();
  
        }

        //Login view
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login Post Action & password verification against DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl="")
        {
            string message = "";
            using (FYPEntities dc = new FYPEntities())
            {
                var v = dc.Tables.Where(a => a.EmailId == login.EmailId).FirstOrDefault();
                if (v != null)
                {
                    // check if email is verified
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    //inner method to compare hash password
                    if (string.Compare(Crypto.Hash(login.Password),v.Password) == 0)
                    {                  
                        // authentication cookie & storage if select 'remember me' tickbox
                        int timeout = login.RememberMe ? 525600 : 1; //525600 minutes = 1 year
                        var ticket = new FormsAuthenticationTicket(login.EmailId, login.RememberMe, timeout); // timeout if not selected
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        //RedirectToAction("Index", "Home");


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            //redirects user to home page
                            RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credentials provided";
                    }

                }
                else
                {
                    message = "Invalid credentials provided";
                }
            }

                ViewBag.Message = message;
            return View();
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        //Registration: emails with existing accounts:
        [NonAction]
        public bool IfEmailExist(string emailId)
        {
            using (FYPEntities dc = new FYPEntities())
            {
                var v = dc.Tables.Where(a => a.EmailId == emailId).FirstOrDefault();
                return v != null;
            }
        }

        //Sends verification code email through site gmail address to credentials of email provided
        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationCode)
        {
            var verifyUrl = "/Table/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("bitvestorsite@gmail.com", "Bitvestor Site");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "fypemail";
            string subject = "Your account is successfully created";
            string body = "<br/><br/>We're happy to tell you that your BitVestor account is" +
                "almost ready for use! Please click on the below link to verify your account" +
                " <br/<a href='" + link + "'>" + link + "<a/> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


        //CRUD operations on user data
       

    }
}