namespace AbbeyMortageAssessment.Services.Comment
{
    using AbbeyMortageAssessment.Services.TaggedUser;
    using Microsoft.EntityFrameworkCore;
    using AbbeyMortageAssessment.Data;
    using AbbeyMortageAssessment.Data.Models;
    using AbbeyMortageAssessment.Services.User;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _data;
        private readonly ITaggedUserService _taggedUserService;

        public CommentService(
            ApplicationDbContext data,
            ITaggedUserService taggedUserService)
        {
            _data = data;
            _taggedUserService = taggedUserService;
        }

        public async Task AddComment(CommentServiceModel serviceModel)
        {
            await _data.Comments.AddAsync(
                new Comment
                {
                    Content = serviceModel.Content,
                    DatePosted = serviceModel.DatePosted,
                    AuthorId = serviceModel.Author.Id,
                    CommentedPostId = serviceModel.PostId,
                    TaggedUsers = _taggedUserService
                        .GetTagFriendsInCommentsEntities(
                            serviceModel.Author.Id,
                            serviceModel.TaggedFriends
                                .Select(i => i.Id)
                                .ToList())
                });

            await _data.SaveChangesAsync();
        }

        public async Task EditComment(CommentServiceModel serviceModel)
        {
            var comment = await _data.Comments
                .FirstOrDefaultAsync(i => i.Id == serviceModel.CommentId);

            comment.Content = serviceModel.Content;

            _data.Update(comment);
            await _data.SaveChangesAsync();
        }

        public async Task DeleteComment(int id)
        {
            var comment = await _data.Comments
                .FirstOrDefaultAsync(i => i.Id == id);

            _data.Remove(comment);
            await _data.SaveChangesAsync();
        }

        public async Task<CommentServiceModel> GetComment(int id)
        => await _data.Comments
            .Select(c => new CommentServiceModel
            {
                CommentId = c.Id,
                Content = c.Content,
                DatePosted = c.DatePosted,
                Author = new UserServiceModel
                {
                    Id = c.Author.Id,
                    UserName = c.Author.UserName,
                    FullName = c.Author.FullName,
                    Country = c.Author.Country,
                    DateOfBirth = c.Author.DOB
                },
                PostId = c.CommentedPostId,
                TaggedFriends = c.TaggedUsers
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
            .FirstOrDefaultAsync(i => i.CommentId == id);

        public async Task<ICollection<CommentServiceModel>> GetCommentsByPostIdAsync(int postId)
        => await _data.Comments
            .Where(i => i.CommentedPostId == postId)
            .Select(c => new CommentServiceModel
            {
                CommentId = c.Id,
                Content = c.Content,
                DatePosted = c.DatePosted,
                Author = new UserServiceModel
                {
                    Id = c.Author.Id,
                    UserName = c.Author.UserName,
                    FullName = c.Author.FullName,
                    Country = c.Author.Country,
                    DateOfBirth = c.Author.DOB
                },
                PostId = c.CommentedPostId,
                TaggedFriends = c.TaggedUsers
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
            .ToListAsync();
    }
}
