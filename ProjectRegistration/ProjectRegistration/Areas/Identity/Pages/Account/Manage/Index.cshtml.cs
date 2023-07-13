// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectRegistration.Models;

namespace ProjectRegistration.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        [Display(Name = "MSSV/GV")]
        public string UserId { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [MinLength(5)]
            [Display(Name = "Tên đăng nhập")]
            public string Username { get; set; }

            [Phone]
            [Display(Name = "Số điện thoại")]
            public string PhoneNumber { get; set; }


            [Display(Name = "Họ và tên")]
            public string Fullname { get; set; }

            [Display(Name = "Ảnh đại diện")]
            public string ImagePath { get; set; }

            [Display(Name = "Ngày sinh")]
            public DateTime DateOfBirth { get; set; }


        }

        private async Task LoadAsync(User user)
        {
            bool checkManager = await _userManager.IsInRoleAsync(user, "Manager");
            if (!checkManager)
            {
                UserId = user.UserId;
                Input = new InputModel
                {
                    Username = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Fullname = user.Fullname,
                    ImagePath = user.ImagePath,
                    DateOfBirth = (DateTime)user.DateOfBirth,
                };
            }
            else
            {
                Input = new InputModel
                {
                    Username = user.UserName,
                    Fullname = user.Fullname,
                    ImagePath = user.ImagePath,
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Không thể đổi số điện thoại.";
                    return RedirectToPage();
                }
            }

            var userName = await _userManager.GetUserNameAsync(user);
            if (userName != Input.Username)
            {
                var existed = await _userManager.SetUserNameAsync(user, Input.Username);
                if (!existed.Succeeded)
                {
                    StatusMessage = "Tên tài khoản đã có người sử dụng.";
                    return RedirectToPage();
                }
            }

            if (Input.Fullname != user.Fullname)
            {
                if (!String.IsNullOrEmpty(Input.Fullname))
                {
                    user.Fullname = Input.Fullname;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thông tin tài khoản đã được cập nhật";
            return RedirectToPage();
        }
    }
}
