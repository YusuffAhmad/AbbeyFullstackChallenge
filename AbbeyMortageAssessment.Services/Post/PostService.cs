namespace AbbeyMortageAssessment.Services.Post
{
    using AbbeyMortageAssessment.Services.Comment;
    using AbbeyMortageAssessment.Services.TaggedUser;
    using Microsoft.EntityFrameworkCore;
    using AbbeyMortageAssessment.Data;
    using AbbeyMortageAssessment.Data.Models;
    using AbbeyMortageAssessment.Services.Group;
    using AbbeyMortageAssessment.Services.User;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _data;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;

        public PostService(
            ApplicationDbContext data,
            ITaggedUserService taggedUserService,
            ICommentService commentService)
        {
            _data = data;
            _taggedUserService = taggedUserService;
            _commentService = commentService;
        }

        public async Task AddPost(PostServiceModel serviceModel)
        {
            var post = new Post
            {
                Content = serviceModel.Content,
                DatePosted = serviceModel.DatePosted,
                AuthorId = serviceModel.Author.Id,
                TaggedUsers = _taggedUserService.GetTagFriendsInPostsEntities(
                    serviceModel.Author.Id,
                    serviceModel.TaggedFriends
                        .Select(i => i.Id)
                        .ToList())
            };

            if (serviceModel.GroupId != null)
            {
                post.GroupId = serviceModel.GroupId;
            }

            await _data.Posts.AddAsync(post);
            await _data.SaveChangesAsync();
        }

        public async Task EditPost(PostServiceModel serviceModel)
        {
            var post = await _data.Posts
                .FirstOrDefaultAsync(i => i.PostId == serviceModel.PostId);

            post.Content = serviceModel.Content;

            _data.Update(post);
            await _data.SaveChangesAsync();
        }

        public async Task DeletePost(int id)
        {
            var post = await _data.Posts
                .FirstOrDefaultAsync(i => i.PostId == id);

            _data.Posts.Remove(post);
            await _data.SaveChangesAsync();
        }

        public async Task<PostServiceModel> GetPost(int id)
        => await _data.Posts
            .Select(p => new PostServiceModel
            {
                PostId = p.PostId,
                Content = p.Content,
                DatePosted = p.DatePosted,
                Author = new UserServiceModel
                {
                    Id = p.Author.Id,
                    UserName = p.Author.UserName,
                    FullName = p.Author.FullName,
                    Country = p.Author.Country,
                    DateOfBirth = p.Author.DOB
                },
                GroupId = p.GroupId,
                Group = new GroupServiceModel
                {
                    Title = p.Group.Title,
                    Description = p.Group.Description
                },
                TaggedFriends = p.TaggedUsers
                    .Select(t => new UserServiceModel
                    {
                        Id = t.Tagged.Id,
                        UserName = t.Tagged.UserName,
                        FullName = t.Tagged.FullName,
                        Country = t.Tagged.Country,
                        DateOfBirth = t.Tagged.DOB
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(i => i.PostId == id);

        public async Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId)
        {
            var posts = _data.Posts
                .Where(i => i.AuthorId == userId)
                .Select(p => new PostServiceModel
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    Author = new UserServiceModel
                    {
                        Id = p.Author.Id,
                        UserName = p.Author.UserName,
                        FullName = p.Author.FullName,
                        Country = p.Author.Country,
                        DateOfBirth = p.Author.DOB
                    },
                    GroupId = p.GroupId,
                    Group = new GroupServiceModel
                    {
                        Title = p.Group.Title,
                        Description = p.Group.Description
                    },
                    TaggedFriends = p.TaggedUsers
                        .Select(t => new UserServiceModel
                        {
                            Id = t.Tagged.Id,
                            UserName = t.Tagged.UserName,
                            FullName = t.Tagged.FullName,
                            Country = t.Tagged.Country,
                            DateOfBirth = t.Tagged.DOB
                        })
                        .ToList()
                })
                .ToList();

            foreach (var post in posts)
            {
                post.Comments = await _commentService
                    .GetCommentsByPostIdAsync(post.PostId);
            }

            return posts;
        }

        public async Task<ICollection<PostServiceModel>> GetPostsByGroupIdAsync(int groupId)
        {
            var posts = _data.Posts
               .Where(i => i.GroupId == groupId)
               .Select(p => new PostServiceModel
               {
                   PostId = p.PostId,
                   Content = p.Content,
                   DatePosted = p.DatePosted,
                   Author = new UserServiceModel
                   {
                       Id = p.Author.Id,
                       UserName = p.Author.UserName,
                       FullName = p.Author.FullName,
                       Country = p.Author.Country,
                       DateOfBirth = p.Author.DOB
                   },
                   TaggedFriends = p.TaggedUsers
                       .Select(t => new UserServiceModel
                       {
                           Id = t.Tagged.Id,
                           UserName = t.Tagged.UserName,
                           FullName = t.Tagged.FullName,
                           Country = t.Tagged.Country,
                           DateOfBirth = t.Tagged.DOB
                       })
                       .ToList()
               })
               .ToList();

            foreach (var post in posts)
            {
                post.Comments = await _commentService
                    .GetCommentsByPostIdAsync(post.PostId);
            }

            return posts;
        }

        public async Task<int?> GetGroupIdOfPost(int id)
        {
            var post = await GetPost(id);

            return post.GroupId;
        }
    }
}
