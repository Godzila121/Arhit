#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Додано для [Display]
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using FoodDelivery.Models; // Переконайся, що твій User тут

namespace FoodDelivery.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager; // <-- Це поле вже є у твоєму коді

    // Конструктор вже приймає RoleManager, тут все гаразд
        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager; // Присвоєння вже є
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Електронна пошта є обов'язковою.")] // Додав повідомлення
            [EmailAddress(ErrorMessage = "Некоректний формат електронної пошти.")] // Додав повідомлення
            [Display(Name = "Електронна пошта")] // Переклав
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль є обов'язковим.")] // Додав повідомлення
            [StringLength(100, ErrorMessage = "{0} має бути довжиною щонайменше {2} та максимум {1} символів.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")] // Переклав
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Підтвердження пароля")] // Переклав
            [Compare("Password", ErrorMessage = "Пароль та підтвердження пароля не співпадають.")] // Переклав
            public string ConfirmPassword { get; set; }

            // --- ДОДАЙ ЦЮ ВЛАСТИВІСТЬ ---
            [Display(Name = "Зареєструватися як власник ресторану?")]
            public bool IsRestaurantOwner { get; set; }
            // ---------------------------
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser(); // Використовуємо твій метод

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Користувач створив новий акаунт з паролем."); // Переклав

                    // --- ДОДАЙ ЦЮ ЛОГІКУ ПРИЗНАЧЕННЯ РОЛІ ---
                    string roleToAssign;
                    if (Input.IsRestaurantOwner)
                    {
                        roleToAssign = "RestaurantOwner";
                    }
                    else
                    {
                         roleToAssign = "User"; // Або інша назва ролі звичайного користувача
                    }

                    // Перевіряємо, чи існує роль, і створюємо, якщо ні
                    if (!await _roleManager.RoleExistsAsync(roleToAssign))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
                        _logger.LogWarning($"Роль '{roleToAssign}' не існувала і була створена під час реєстрації."); // Переклав
                    }

                    // Додаємо користувача до обраної ролі
                    await _userManager.AddToRoleAsync(user, roleToAssign);
                    _logger.LogInformation($"Користувача додано до ролі '{roleToAssign}'."); // Переклав
                    // ------------------------------------------


                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Підтвердіть вашу електронну пошту", // Переклав
                        $"Будь ласка, підтвердіть ваш акаунт, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натиснувши тут</a>."); // Переклав

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
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
            }

            // Якщо дійшли сюди, щось пішло не так, показуємо форму знову
            return Page();
        }

        // Твій метод CreateUser залишається без змін
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Неможливо створити екземпляр '{nameof(User)}'. " + // Переклав
                    $"Переконайтеся, що '{nameof(User)}' не є абстрактним класом і має конструктор без параметрів, або " +
                    $"перевизначте сторінку реєстрації в /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        // Твій метод GetEmailStore залишається без змін
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Стандартний UI потребує сховища користувачів з підтримкою електронної пошти."); // Переклав
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}