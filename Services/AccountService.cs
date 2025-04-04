using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InternIntellegence_MovieApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace InternIntellegence_MovieApi.Services
{
	public class AccountService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailService _emailService;
		private readonly IConfiguration _configuration;

		public AccountService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IEmailService emailService,
			 IConfiguration configuration)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailService = emailService;
			_configuration = configuration;
		}

		public async Task<IdentityResult> RegisterAsync(string email, string password)
		{
			var user = new ApplicationUser { UserName = email, Email = email };
			var result = await _userManager.CreateAsync(user, password);
			
			if (result.Succeeded)
			{
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var callbackUrl = $"https://yourapp.com/confirm-email?userId={user.Id}&code={code}";
				
				await _emailService.SendEmailAsync(email, "Confirm your email",
					$"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
			}
			
			return result;
		}

		public async Task<(SignInResult Result, string? Token)> LoginAsync(string email, string password)
		{
			var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				var token = await GenerateJwtTokenAsync(email);
				return (result, token);
			}
			return (result, null);
		}

		public async Task ConfirmEmailAsync(string userId, string code)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new ApplicationException("User not found");
			}
			
			var result = await _userManager.ConfirmEmailAsync(user, code);
			if (!result.Succeeded)
			{
				throw new ApplicationException("Error confirming email");
			}
		}

		public async Task<string> GenerateJwtTokenAsync(string email)
		{
			var SecretKeyString = _configuration["Jwt:Key"];
			var issuer = _configuration["Jwt:Issuer"];
			var audience = _configuration["Jwt:Audience"];
			var SecretKeyByte = Encoding.ASCII.GetBytes(SecretKeyString!);
			SecurityKey securityKey = new SymmetricSecurityKey(SecretKeyByte);

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				throw new ApplicationException("User not found");
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Email, user.Email)
			};

			var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(1);

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	
	}
}
