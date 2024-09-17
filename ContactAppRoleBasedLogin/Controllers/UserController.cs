using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactAppRoleBasedLogin.Data;
using System.Web.Security;
using ContactAppRoleBasedLogin.Models;
using NHibernate.Linq;

namespace ContactAppRoleBasedLogin.Controllers
{
    /*
     ALTER TABLE Users
ADD CONSTRAINT DF_Users_Id DEFAULT NEWID() FOR Id
ALTER TABLE Contacts
ADD CONSTRAINT DF_Contacts_Id DEFAULT NEWID() FOR Id
ALTER TABLE ContactDetails
ADD CONSTRAINT DF_ContactDetails_Id DEFAULT NEWID() FOR Id
ALTER TABLE Roles
ADD CONSTRAINT DF_Roles_Id DEFAULT NEWID() FOR Id*/
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User user)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var findUser = session.Query<User>().SingleOrDefault(u => u.UserName == user.UserName);
                    if (findUser != null)
                    {
                        if (findUser.IsActive)
                        {
                            if (user.IsAdmin)
                            {
                                //if (BCrypt.Net.BCrypt.Verify(user.Password, findUser.Password))
                                if (user.Password == findUser.Password)
                                {
                                    FormsAuthentication.SetAuthCookie(user.UserName, true);
                                    Session["UserId"] = findUser.Id;
                                    return RedirectToAction("ViewAllStaff");
                                }
                            }
                        }
                    }
                    ModelState.AddModelError("", "UserName/Password doesn't exists");
                    return View();
                }
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public ActionResult ViewAllStaff()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var staffs = session.Query<User>().FetchMany(u => u.Contacts).Where(u => u.IsAdmin == false).ToList();
                return View(staffs);
            }
        }
        public ActionResult ViewAllAdmins()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var admins = session.Query<User>().Where(u => u.IsAdmin == true).ToList();
                return View(admins);
            }
        }
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(User user)
        {
            //user.Password= BCrypt.Net.BCrypt.HashPassword(user.Password);
            using (var session = NHibernateHelper.CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    user.Role.User = user;
                    user.IsActive = true;
                    if (user.IsAdmin)
                    {
                        user.Role.RoleName = "Admin";
                    }
                    else
                    {
                        user.Role.RoleName = "Staff";
                    }
                    session.Save(user);
                    transaction.Commit();
                    return RedirectToAction("ViewAllStaff");
                }
            }
        }
        public ActionResult EditUser(Guid userId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var findUser = session.Get<User>(userId);
                return View(findUser);
            }
        }
        [HttpPost]
        public ActionResult EditUser(User user)
        {
            using(var session = NHibernateHelper.CreateSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    user.Role.User = user;
                    if (user.IsAdmin)
                    {
                        user.Role.RoleName = "Admin";
                    }
                    else
                    {
                        user.Role.RoleName = "Staff";
                    }
                    session.Update(user);
                    transaction.Commit();
                    return RedirectToAction("ViewAllStaff");
                }
            }
        }
    }
}