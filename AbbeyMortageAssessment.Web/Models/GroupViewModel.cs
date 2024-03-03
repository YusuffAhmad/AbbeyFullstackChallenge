namespace AbbeyMortageAssessment.Web.Models
{
    using AbbeyMortageAssessment.Services.Post;
    using AbbeyMortageAssessment.Services.User;
    using System.Collections.Generic;

    public class GroupViewModel
    {
        public GroupViewModel()
        {
            Members = new List<UserServiceModel>();
            Posts = new List<PostServiceModel>();
        }

        public int GroupId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public UserServiceModel Admin { get; set; }

        public string CurrentUserId { get; set; }

        public bool IsCurrentUserMember { get; set; }

        public ICollection<UserServiceModel> Members { get; set; }

        public ICollection<PostServiceModel> Posts { get; set; }
    }
}
