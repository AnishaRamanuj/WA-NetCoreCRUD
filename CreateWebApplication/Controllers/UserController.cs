using CreateWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Security.Claims;

namespace CreateWebApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //GET: https://localhost:7123/User/Register
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Countries = new List<string> { "India", "USA", "UK" };
            UserModel model = new UserModel();
             if (ViewBag.Countries.Count > 0)
            {
                model.Country = ViewBag.Countries[0];
            }
            return View(model);
        }

        //POST: https://localhost:7123/User/Register
        [HttpPost]
        public IActionResult Register(UserModel model, string[] hobbies)
        {
            try
            {
                 model.Hobbies = string.Join(",",hobbies);
                // Create connection and command objects
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    using (SqlCommand command = new SqlCommand("sp_AddEditUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", model.ID);
                        command.Parameters.AddWithValue("@FullName", model.FullName);
                        command.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Password", model.Password);
                        command.Parameters.AddWithValue("@DateOfBirth", model.DOB);
                        command.Parameters.AddWithValue("@Gender", model.Gender);
                        command.Parameters.AddWithValue("@Hobbies", model.Hobbies);
                        command.Parameters.AddWithValue("@RoleId", 2);
                        command.Parameters.AddWithValue("@Country", model.Country);

                        // Open the connection
                        connection.Open();
                        // Execute the stored procedure
                        int userId = Convert.ToInt32(command.ExecuteScalar());
                        if (model.ID != 0)
                        {
                            return RedirectToAction("UserList");
                        }
                            // Registration successful
                            //return RedirectToAction("RegistrationSuccess", new { userId = userId });
                            TempData["RegisterSuccess"] = "Registration successful";
                        return RedirectToAction("Login", "User");

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                ModelState.AddModelError("", "An error occurred while registering the user: " + ex.Message);
                return View(model);
            }
        }
        public IActionResult RegistrationSuccess(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            UserModel model = new UserModel();
           
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            UserModel check = new UserModel();
           
            try
            {
                if (HttpContext.Session.GetString("UserName") == null)
                {
                    // Validate the model
                    if (!ModelState.IsValid)
                    {
                        HttpContext.Session.SetString("UserName", model.UserName.ToString());
                        return View(model);
                    }
                }
                // Create connection and command objects
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    using (SqlCommand command = new SqlCommand("sp_LoginUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@UserName", model.UserName);
                        command.Parameters.AddWithValue("@Password", model.Password);

                        // Open the connection
                        connection.Open();

                        // Execute the stored procedure
                        SqlDataReader reader = command.ExecuteReader();

                        // Check if the login was successful
                        if (reader.HasRows)
                        {
                            // Login successful
                            ViewBag.Message = "Login successful";
                            TempData["LoginSuccess"] = "Login successful";
                                
                            return RedirectToAction("UserList", "User"); // Redirect to home page or any other page
                        }
                        else
                        {
                            // Invalid username or password
                            ModelState.AddModelError("", "Invalid Mobile/ Email or password");
                                return RedirectToAction("Login");
                                //return View(model);
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                // Handle any errors
                ModelState.AddModelError("", "An error occurred while logging in: " + ex.Message);
                return View(model);
            }
        }

        public IActionResult UserList()
        {
            try
            {
                List<UserModel> userList = new List<UserModel>();
                

                // Create connection and command objects
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetAllUserDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Open the connection
                        connection.Open();

                        // Execute the stored procedure
                        SqlDataReader reader = command.ExecuteReader();

                        // Read the data and populate the userList
                        while (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                FullName = reader["FullName"].ToString(),
                                MobileNumber = reader["MobileNumber"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                DOB = Convert.ToDateTime(reader["DateOfBirth"]),
                                Gender = reader["Gender"].ToString(),
                                Hobbies = reader["Hobbies"].ToString(),
                                RoleId = Convert.ToInt32(reader["RoleId"]),
                                LastLoggedIn = Convert.ToBoolean(reader["LastLoggedIn"]),
                                Country = reader["Country"].ToString(),
                            };

                            userList.Add(user);
                        }
                    }
                }
                // Pass the logged-in user's ID to the view
                int lastLoggedInUserId = 0;
                int lastLoggedInUserRole = 0;
                var lastLoggedInUser = userList.FirstOrDefault(user =>user.LastLoggedIn == true);
                if (lastLoggedInUser != null)
                {
                    lastLoggedInUserId = lastLoggedInUser.ID;
                    lastLoggedInUserRole = lastLoggedInUser.RoleId;
                }
                ViewBag.LastLoggedInUserId = lastLoggedInUserId;
                ViewBag.LastLoggedInRoleId = lastLoggedInUserRole;

                return View(userList);
            }
            catch (Exception ex)
            {
                // Handle any errors
                ModelState.AddModelError("", "An error occurred while fetching user data: " + ex.Message);
                return View(new List<UserModel>());
            }
        }
        // GET: https://localhost:7123/User/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                // Create connection and command objects
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetUserByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                FullName = reader["FullName"].ToString(),
                                MobileNumber = reader["MobileNumber"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                DOB = Convert.ToDateTime(reader["DateOfBirth"]),
                                Gender = reader["Gender"].ToString(),
                                Hobbies = reader["Hobbies"].ToString(),
                                RoleId = Convert.ToInt32(reader["RoleId"]),
                                Country = reader["Country"].ToString(),
                            };
                            ViewBag.Countries = new List<string> { "India", "USA", "UK" };
                            
                            return View("Register",user);
                        }
                    }
                }

                // User not found
                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while retrieving user details: " + ex.Message);
                return RedirectToAction("UserList");
            }
        }
      
        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DBConnection")))
                {
                    using (SqlCommand command = new SqlCommand("sp_DeleteUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        TempData["DeleteSuccess"] = "User deleted successfully.";
                    }
                }
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Handle any errors
                TempData["DeleteError"] = "User not found or could not be deleted.";
                ModelState.AddModelError("", "An error occurred while deleting user data: " + ex.Message);
                return RedirectToAction("UserList");
            }
        }
        public ActionResult Logout()
        {

            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");

            return RedirectToAction("Login");
        }
    }
}


        

