using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Sanssoussi.Areas.Identity.Data;
using Sanssoussi.Data;
using Sanssoussi.Models;

namespace Sanssoussi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SanssoussiContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<SanssoussiUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<SanssoussiUser> userManager, SanssoussiContext context)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            this.ViewData["Message"] = "Parce que marcher devrait se faire SansSoussi";
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Comments()
        {
            var comments = new List<string>();

            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.View(comments);
            }

            comments = await this._context.Comments
                .Where(c => c.UserId == user.Id)
                .Select(c => c.Text)
                .ToListAsync();

            return this.View(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comments([FromForm] CommentInputModel input)
        {
            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null)
            {
                throw new InvalidOperationException("Vous devez vous connecter");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Commentaire invalide.");
            }

            var newComment = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Text = input.Comment
            };

            this._context.Comments.Add(newComment);
            await this._context.SaveChangesAsync();

            return this.Ok("Commentaire ajouté");
        }

        public async Task<IActionResult> Search(string searchData)
        {
            var searchResults = new List<string>();

            var user = await this._userManager.GetUserAsync(this.User);
            if (user == null || string.IsNullOrEmpty(searchData))
            {
                return this.View(searchResults);
            }

            searchResults = await this._context.Comments
                .Where(c => c.UserId == user.Id && c.Text.Contains(searchData))
                .Select(c => c.Text)
                .ToListAsync();

            return this.View(searchResults);
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return this.View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Emails()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Emails(object form)
        {
            var searchResults = await this._userManager.Users.Select(u => u.Email).ToListAsync();
            return this.Json(searchResults);
        }
    }

    public class CommentInputModel
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le commentaire est requis.")]
        [System.ComponentModel.DataAnnotations.StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères.")]
        public string Comment { get; set; }
    }
}