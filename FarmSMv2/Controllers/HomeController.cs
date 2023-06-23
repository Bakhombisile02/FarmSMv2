using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using FarmSMv2.Models;

namespace FarmSMv2.Controllers
{
    public class HomeController : Controller
    {
        public int Id = 0;//variable to store id 
        FarmCentralSMEntities1 db = new FarmCentralSMEntities1(); //database connection using Entity Frameworks

        public ActionResult AddFarmer() //create farmer page 
        {
            return View();
        }

        //--------------------------------------------------------------------------------------//
        //method to add farmers
        [HttpPost]
        public ActionResult AddFarmer(Farmer farmer) //This saves the data to the database 
        {
            try
            {
                db.Farmers.Add(farmer);
                db.SaveChanges();
                return RedirectToAction("Farmers", "Home"); //returns to farmers list
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "Error: " + ex.Message;
                return View();
            }
        }

        //--------------------------------------------------------------------------------------//
        //method to list farmers 
        public ActionResult Farmers()
        {
            try
            {
                return View(db.Farmers.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "Error: " + ex.Message;
                return View();
            }
        }

        //--------------------------------------------------------------------------------------//
        //welcome page call
        public ActionResult Welcome()
        {
            return View();
        }

        //--------------------------------------------------------------------------------------//
        //method to add products 
        public ActionResult AddProducts(int id)
        {
            Product product = new Product();
            product.farmer_id = id;

            return View(product);
        }


        [HttpPost]
        public ActionResult AddProducts(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Products", "Home", new { id = product.farmer_id });
            }

            // If the model state is not valid, return the view with validation errors
            return View(product);
        }

        //--------------------------------------------------------------------------------------//
        //methid to view products 
        public ActionResult Products(int id)
        {
            try
            {
                ViewBag.Notification = id;

                if (Request.HttpMethod == "POST")
                {
                    var farmerProducts = db.Products.Where(x => x.farmer_id == id).OrderBy(x => x.product_type_id).ToList();
                    return View(farmerProducts);
                }
                else
                {
                    var farmerProducts = db.Products.Where(x => x.farmer_id == id).ToList();
                    return View(farmerProducts);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "Error: " + ex.Message;
                return View();
            }
        }

        //--------------------------------------------------------------------------------------//
        //method to logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Welcome", "Home");
        }

        //--------------------------------------------------------------------------------------//
        //method to login 
        [HttpGet]
        public ActionResult FarmerLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FarmerLogin(Farmer farmer)
        {
            try
            {
                var logincheck = db.Farmers.Where(x => x.name.Equals(farmer.name) && x.password.Equals(farmer.password)).FirstOrDefault();
                if (logincheck != null)
                {
                    Session["Seshid"] = logincheck.farmer_id.ToString();
                    Session["Seshuser"] = logincheck.name.ToString();
                    int User = logincheck.farmer_id;
                    return RedirectToAction("Products", "Home", new { id = User });
                }
                else
                {
                    ViewBag.Notification = "Incorrect Username or password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "Error: " + ex.Message;
                return View();
            }
        }

        //--------------------------------------------------------------------------------------//
        //method to login employee

        [HttpGet]
        public ActionResult EmployeeLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeLogin(employee Employee)
        {
            try
            {
                var logincheck = db.employees.Where(x => x.username.Equals(Employee.username) && x.password.Equals(Employee.password)).FirstOrDefault();
                if (logincheck != null)
                {
                    Session["Seshid"] = Employee.id.ToString();
                    Session["Seshuser"] = Employee.username.ToString();
                    return RedirectToAction("Farmers", "Home");
                }
                else
                {
                    ViewBag.Notification = "Incorrect Username or password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "Error: " + ex.Message;
                return View();
            }
        }

        //--------------------------------------------------------------------------------------//
        //method to register

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(employee Employee)
        {
            try
            {
                if (db.employees.Any(x => x.username == Employee.username))
                {
                    ViewBag.Notification = "This account already exists";
                    return View();
                }
                else
                {
                    try
                    {
                        db.employees.Add(Employee);
                        db.SaveChanges();

                        Session["Seshid"] = Employee.id.ToString();
                        Session["Seshuser"] = Employee.username.ToString();
                        return RedirectToAction("Farmers", "Home");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Notification = "Error: " + ex.Message;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Notification = "Error: " + ex.Message;
                return View();
            }
        }
    }
}
//-----------------------------------------------End of File--------------------------------------------------------//
