﻿using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

public class ApplicationIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName) => new IdentityError { Code = nameof(DuplicateUserName), Description = $"این نام کاربری ({userName}) قبلا توسط شخص دیگری گرفته شده است." };
    public override IdentityError InvalidUserName(string? userName) => new IdentityError { Code = nameof(InvalidUserName), Description = $"نام کاربری ({userName}) معتبر نمیباشد؛نام کاربری باید شامل حروف (a-z) و اعداد (0-9) باشد." };
    public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "پسورد باید شامل یک کارکتر غیر عددی و حرفی (!,@,#,$) باشد." };
    public override IdentityError PasswordRequiresDigit() => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "پسورد باید شامل حداقل یکی از اعداد (0-9) باشد." };
    public override IdentityError PasswordRequiresLower() => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "پسورد باید شامل حداقل یک حرف کوچک (a-z) باشد." };
    public override IdentityError PasswordRequiresUpper() => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "پسورد باید شامل حداقل یک حرف بزرگ (A-Z) باشد." };
    public override IdentityError PasswordTooShort(int length) => new IdentityError { Code = nameof(PasswordTooShort), Description = $"پسورد باید حداقل شما {length} حروف،عدد و علامت باشد." };
    public override IdentityError DuplicateRoleName(string role) => new IdentityError { Code = nameof(DuplicateRoleName), Description = $"نقش '{role}' وجود دارد" };
    public override IdentityError DuplicateEmail(string email) => new IdentityError { Code = nameof(DuplicateEmail), Description = $"ایمیل {email} قبلا در سیستم ثبت نام کرده است." };
    public override IdentityError InvalidToken() => new IdentityError { Code = nameof(InvalidToken), Description = "توکن ساخته شده منقضی شده است، مجددا از اول اقدام کنید." };
}
