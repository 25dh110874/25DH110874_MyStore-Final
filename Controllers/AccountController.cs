using _25DH110874_MyStore.Models;
using _25DH110874_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _25DH110874_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }
        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", $"Mật khẩu không khớp. (Password: '{model.Password}', Confirm: '{model.ConfirmPassword}')");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                var existingUser = db.Users.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                // Create a new User object
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword, // Add this line!
                    UserRole = "C"
                };
                // Create a new Customer object
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    CustomerAddress = model.Address,
                    Username = model.Username // Link the customer to the user by username
                };
                try
                {
                    db.Users.Add(user);
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError("", $"Property: {validationError.PropertyName} - Error: {validationError.ErrorMessage}");
                        }
                    }
                    return View(model);
                }
                //// Add the new user and customer to the database
                //db.Users.Add(user);
                //db.Customers.Add(customer);
                //db.SaveChanges();
                //// Redirect to a success page or login page after successful registration
                //return RedirectToAction("Index", "Home");
            }
            // If we got this far, something failed; redisplay form
            return View(model);
        }
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }
        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username and password match a user in the database
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password == model.Password && u.UserRole == "C");
                if (user != null)
                {
                    // Store user information in session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;
                    FormsAuthentication.SetAuthCookie(user.Username, false); // Set authentication cookie
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            // If we got this far, something failed; redisplay form
            return View(model);
        }
        public ActionResult ProfileInfo()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        public ActionResult UpdateProfile()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
    }
}