namespace AbbeyMortageAssessment.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using AbbeyMortageAssessment.Services.Friendship;
    using AbbeyMortageAssessment.Services.Comment;
    using AbbeyMortageAssessment.Services.TaggedUser;
    using AbbeyMortageAssessment.Services.User;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using AbbeyMortageAssessment.Services.JSON;
    using AbbeyMortageAssessment.Web.Models;

    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ICommentService _commentService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly IUserService _userService;
        private readonly IJsonService<UserServiceModel> _jsonService;

        public CommentsController(
            IFriendshipService friendshipService,
            ICommentService commentService,
            ITaggedUserService taggedUserService,
            IUserService userService,
            IJsonService<UserServiceModel> jsonService)
        {
            _friendshipService = friendshipService;
            _commentService = commentService;
            _taggedUserService = taggedUserService;
            _userService = userService;
            _jsonService = jsonService;
        }

        [HttpGet]
        public IActionResult Create(int postId, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var viewModel = new CommentViewModel
            {
                PostId = postId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userService
                    .GetUserByNameAsync(User.Identity.Name);

                await _commentService
                    .AddComment(new CommentServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        PostId = viewModel.PostId,
                        TaggedFriends = viewModel.TaggedFriends != null ?
                            _jsonService
                                .GetObjects(viewModel.TaggedFriends)
                                .ToList() :
                            new List<UserServiceModel>()
                    });

                if (TempData.ContainsKey("path"))
                {
                    return LocalRedirect(TempData["path"].ToString());
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var comment = await _commentService
                .GetComment(id);

            var currentUser = await _userService
                .GetUserByNameAsync(User.Identity.Name);

            if (comment == null ||
                currentUser.Id != comment.Author.Id)
            {
                return NotFound();
            }

            var viewModel = new CommentViewModel
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                DatePosted = comment.DatePosted,
                Author = comment.Author,
                PostId = comment.PostId
            };

            viewModel.TaggedFriends = _jsonService
                 .SerializeObjects(comment.TaggedFriends.ToList());

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = await _userService
                .GetUserIdByNameAsync(User.Identity.Name);

                var taggedFriends = _jsonService
                    .GetObjects(viewModel.TaggedFriends);

                await _taggedUserService.UpdateTaggedFriendsInCommentAsync(
                    taggedFriends.ToList(),
                    viewModel.CommentId,
                    currentUserId);

                await _commentService
                    .EditComment(new CommentServiceModel
                    {
                        CommentId = viewModel.CommentId,
                        Content = viewModel.Content
                    });

                if (TempData.ContainsKey("path"))
                {
                    return Redirect(TempData["path"].ToString());
                }
                else
                {
                    return NotFound();
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var comment = await _commentService
                .GetComment(id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _taggedUserService.DeleteTaggedFriendsCommentId(id);
            await _commentService.DeleteComment(id);

            if (TempData.ContainsKey("path"))
            {
                return Redirect(TempData["path"].ToString());
            }
            return NotFound();
        }
    }
}
