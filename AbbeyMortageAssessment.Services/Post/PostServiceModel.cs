namespace AbbeyMortageAssessment.Services.Post
{
    using AbbeyMortageAssessment.Services.Comment;
    using AbbeyMortageAssessment.Services.Group;
    using AbbeyMortageAssessment.Services.User;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PostServiceModel
    {
        public PostServiceModel()
        {
            TaggedFriends = new List<UserServiceModel>();
            _comments = new List<CommentServiceModel>();
        }

        public int PostId { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        public int? GroupId { get; set; }

        public GroupServiceModel Group { get; set; }

        public ICollection<UserServiceModel> TaggedFriends { get; set; }

        private List<CommentServiceModel> _comments;
        public ICollection<CommentServiceModel> Comments
        {
            get => _comments;
            set
            {
                if (value.Count > 0)
                {
                    _comments = value
                        .OrderByDescending(d => d.DatePosted)
                        .ToList();
                }
            }
        }
    }
}
