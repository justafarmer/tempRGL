using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using rungreenlake.web.Areas.Identity.Data;
using rungreenlake.Models.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using rungreenlake.Controllers;
using rungreenlake.web.Data;
using rungreenlake.Models;
using Microsoft.EntityFrameworkCore;


namespace rungreenlake.web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<RungreenlakeUser> _signInManager;
        private readonly UserManager<RungreenlakeUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<RungreenlakeUser> userManager,
            SignInManager<RungreenlakeUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public List<SelectListItem> RaceTypeList { get; set; }
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters.")]
            [Display(Name = "First Name")]
            public string Firstname { get; set; }

            [Required]
            [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters.")]
            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [Range(1, 5, ErrorMessage = "Race type is required.")]
            public int RaceType { get; set; }

            [Range(0, 23, ErrorMessage = "Value for Hours must be between {1} and {2}.")]
            public int RaceTimeHours { get; set; }
            [Range(0, 59, ErrorMessage = "Value for Minutes must be between {1} and {2}.")]
            public int RaceTimeMinutes { get; set; }
            [Range(0, 59, ErrorMessage = "Value for Seconds must be between {1} and {2}.")]
            public int RaceTimeSeconds { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            RaceTypeList = new List<SelectListItem>()
            {
                new SelectListItem() { Text="One Mile", Value="1"},
                new SelectListItem() { Text="5 Kilometers", Value="2"},
                new SelectListItem() { Text="10 Kilometers", Value="3"},
                new SelectListItem() { Text="Half-Marathon", Value="4"},
                new SelectListItem() { Text="Full-Marathon", Value="5"}
            };

        //Input.FirstTimeEntry = new RaceRecordViewModel();
        //var inflow = new InputModel();
        //inflow.FirstTimeEntry = new RaceRecordViewModel();
        ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            RaceTypeList = new List<SelectListItem>()
            {
                new SelectListItem() { Text="One Mile", Value="1"},
                new SelectListItem() { Text="5 Kilometers", Value="2"},
                new SelectListItem() { Text="10 Kilometers", Value="3"},
                new SelectListItem() { Text="Half-Marathon", Value="4"},
                new SelectListItem() { Text="Full-Marathon", Value="5"}
            };

            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            if (ModelState.IsValid)
            {
                //Calculate total and mile time.
                int totalTime = Input.RaceTimeHours * 3600 + Input.RaceTimeMinutes * 60 + Input.RaceTimeSeconds;
                var mileTime = Functions.GetMileTime(totalTime, Input.RaceType);

                //Check if 1 mile time is absurd.
                if (mileTime > 223)
                {
                    var user = new RungreenlakeUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.Firstname,
                        LastName = Input.Lastname,
                        EmailConfirmed = true
                    };

                    //bundle registration transaction
                    using (var registrationTransaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            //Create user profile, which will generate a "Link ID"
                            var result = await _userManager.CreateAsync(user, Input.Password);

                            if (result.Succeeded)
                            {
                                //Create profile entry.
                                var profile = new rungreenlake.Models.Profile
                                {
                                    ProfileID = user.LinkID,
                                    CreationDate = DateTime.Now,
                                    LinkID = user.LinkID
                                };

                                //Create initial time entry.
                                var race = new RaceRecord
                                {
                                    RaceType = Input.RaceType,
                                    RaceTime = totalTime,
                                    ProfileID = user.LinkID,
                                    MileTime = mileTime
                                };
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Profiles ON");
                                _context.Add(profile);
                                _context.SaveChanges();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Profiles OFF");
                                _context.Add(race);
                                _context.SaveChanges();
                                registrationTransaction.Commit();
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                                //return RedirectToAction("Success", "Home");
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            registrationTransaction.Rollback();
                            ModelState.AddModelError("", "Unable to register, please contact your administrator for more details.");                      
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Wow, you must be fast!  Unfortnately your mile time must be greater than 3:43!");
                }

                /*
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                */

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
