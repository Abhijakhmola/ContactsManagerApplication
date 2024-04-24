using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
   
    [Route("[controller]")]
   // [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [Route("[action]")]
        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [Authorize("NotAuthenticated")]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            //Check for validation errors
            if (!ModelState.IsValid)
            {
                ViewBag.Errors=ModelState.Values.SelectMany(x => x.Errors).Select(temp=>temp.ErrorMessage).ToList();
                return View(registerDTO);
            }

            ApplicationUser user = new ApplicationUser() {Email=registerDTO.Email,PersonName=registerDTO.PersonName,PhoneNumber=registerDTO.Phone,UserName=registerDTO.Email };

            IdentityResult result =  await _userManager.CreateAsync(user,registerDTO.Password);

            if(result.Succeeded)
            {
                //Check status of the radio button
                if (registerDTO.UserType == UserTypeOptions.Admin)
                {
                    //Create admin role 
                    if(await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name=UserTypeOptions.Admin.ToString()};
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    //Add new user to the admin role
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                }
                else
                {
                    //Create user role 
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.User.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    //Add new user to the user role
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());

                }
                //Sign in 
                await _signInManager.SignInAsync(user,isPersistent:false);

                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View(registerDTO);
            }
            
        }
        [HttpGet]
        [Route("[action]")]
        [Authorize("NotAuthenticated")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login(LoginDTO loginDTO,string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(e => e.ErrorMessage).ToList();
                return View(loginDTO);
            }
            var result= await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false,lockoutOnFailure:false);

            if(result.Succeeded)
            {
                //Admin
                ApplicationUser? user= await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user != null)
                {
                    if(await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index","Home",new { area="Admin"});
                    }
                }

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction(nameof(PersonsController.Index),"Persons");
            }
            ModelState.AddModelError("Login", "Invalid email or password");
            return View(loginDTO);
        }
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index),"Persons");
        }

        [AllowAnonymous]
        [Route("[action]")]
        public async Task<IActionResult> isEmailAlreadyRegistered(string email)
        {
           ApplicationUser? user= await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);//valid
            }
            return Json(false);//invalid
        }
       
    }
}
