namespace AbbeyMortageAssessment.Web.Controllers
{
    using AbbeyMortageAssessment.Web.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using AbbeyMortageAssessment.Services.Comment;
    using AbbeyMortageAssessment.Services.Friendship;
    using AbbeyMortageAssessment.Services.JSON;
    using AbbeyMortageAssessment.Services.Post;
    using AbbeyMortageAssessment.Services.TaggedUser;
    using AbbeyMortageAssessment.Services.User;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    public class PostsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IPostService _postService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IJsonService<UserServiceModel> _jsonService;

        public PostsController(
            IFriendshipService friendshipService,
            IPostService postService,
            ITaggedUserService taggedUserService,
            ICommentService commentService,
            IUserService userService,
            IJsonService<UserServiceModel> jsonService)
        {
            _friendshipService = friendshipService;
            _postService = postService;
            _taggedUserService = taggedUserService;
            _commentService = commentService;
            _userService = userService;
            _jsonService = jsonService;
        }

        [HttpGet]
        public IActionResult Create(int? groupId, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var viewModel = new PostViewModel
            {
                GroupId = groupId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userService
                    .GetUserByNameAsync(User.Identity.Name);

                await _postService
                    .AddPost(new PostServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        GroupId = viewModel.GroupId,
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

            var post = await _postService.GetPost(id);

            var currentUser = await _userService
                .GetUserByNameAsync(User.Identity.Name);

            if (post == null ||
                currentUser.Id != post.Author.Id)
            {
                return NotFound();
            }

            var viewModel = new PostViewModel
            {
                PostId = post.PostId,
                Content = post.Content,
                GroupId = post.GroupId,
                Author = post.Author,
            };

            viewModel.TaggedFriends = _jsonService
                .SerializeObjects(post.TaggedFriends.ToList());

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = await _userService
                .GetUserIdByNameAsync(User.Identity.Name);

                var taggedFriends = _jsonService
                    .GetObjects(viewModel.TaggedFriends);

                await _taggedUserService.UpdateTaggedFriendsInPostAsync(
                    taggedFriends.ToList(),
                    viewModel.PostId,
                    currentUserId);

                await _postService
                    .EditPost(new PostServiceModel
                    {
                        PostId = viewModel.PostId,
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

            var post = await _postService
                .GetPost(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //If groupId is not null it will be redirected to Group/Details/{groupId}
            var groupId = await _postService.GetGroupIdOfPost(id);

            var comments = await _commentService
                .GetCommentsByPostIdAsync(id);

            //Send collection of all comments ids and delete the tagged friends 
            await _taggedUserService.DeleteTaggedFriendsInComments(comments
                .Select(i => i.CommentId)
                .ToList());

            await _taggedUserService.DeleteTaggedFriendsPostId(id);
            await _postService.DeletePost(id);


            if (TempData.ContainsKey("path"))
            {
                return Redirect(TempData["path"].ToString());
            }
            else
            {
                return NotFound();
            }
        }
    }
}
