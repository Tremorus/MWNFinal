using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MWN.Models;
using MWN.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace MWN.Controllers
{
    // https://metanit.com/sharp/aspnet5/16.3.php
    // В этом контроллере определим метод для регистрации пользователей:
    // 

    public class AccountController : Controller
    {
        /*
         *Поскольку в классе Startup были добавлены сервисы Identity, то здесь в контроллере через конструктор мы можем их получить. 
         *В данном случае мы получаем сервис по управлению пользователями - UserManager и сервис SignInManager, 
         *который позволяет аутентифицировать пользователя и устанавливать или удалять его куки.
         *
         **/
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email };

                /*С помощью метода _userManager.CreateAsync пользователь добавляется в базу данных. 
                 *В качестве параметра передается сам пользователь и его пароль.*/
                // adding user
                var result = await _userManager.CreateAsync(user, model.Password);
                /*Данный метод возвращает объект IdentityResult, с помощью которого можно узнать успешность выполненной операции. 
                 *Вполне возможно, что переданные значения не удовлетворяют требованиям, и тогда пользователь не будет добавлен в базу данных. */
                if (result.Succeeded)
                {
                    //setting cookie
                    await _signInManager.SignInAsync(user, false);
                    /*В случае удачного добавления с помощью метода _signInManager.SignInAsync() устанавливаем аутентификационные куки для добавленного пользователя. 
                     *В этот метод передается объект пользователя, который аутентифицируется, и логическое значение, указывающее, 
                     *надо ли сохранять куки в течение продолжительного времени. И далее выполняем переадресацию на главную страницу приложения.*/
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        /*Если добавление прошло неудачно, то добавляем к состоянию модели с помощью метода ModelState все возникшие при добавлении ошибки, 
                         *и отправленная модель возвращается в представление.*/
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }


        // https://metanit.com/sharp/aspnet5/16.4.php
        [HttpGet]
        public IActionResult Login(string returnUrl=null)
        {
            // В Get-версии метода Login мы получаем адрес для возврата в виде параметра returnUrl и передаем его в модель LoginViewModel.
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            /*
            В Post-версии метода Login получаем данные из представления в виде модели LoginViewModel. 
            Всю работу по аутентификации пользователя выполняет метод signInManager.PasswordSignInAsync(). 
            Этот метод принимает логин и пароль пользователя. Третий параметр метода указывает, 
            надо ли сохранять устанавливаемые куки на долгое время.*/
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    //checking URL is belong to app
                    if(!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Notes");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect password or Login");
                }
                
            }
            return View(model);
            /*
            Данный метод также возвращает IdentityResult, с помощью которого можно узнать, 
            завершилась ли аутентификация успешно. Если она завершилось успешно, то используем свойство ReturnUrl модели LoginViewModel 
            для возврата пользователя на предыдущее место. Для этого нужно еще удостовериться, что адрес возврата принадлежит приложению 
            с помощью метода Url.IsLocalUrl(). Это позволит избежать перенаправлений на нежелательные сайты. 
            Если же адрес возврата не установлен или не принадлежит приложению, выполняем переадресацию на главную страницу.*/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            /*
            Третий метод - метод LogOff выполняет выход пользователя из приложения. За выход отвечает метод _signInManager.SignOutAsync(), 
            который очищает аутентификационные куки.*/
            // delete auth cookies
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
 