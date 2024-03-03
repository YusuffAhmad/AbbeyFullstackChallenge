namespace AbbeyMortageAssessment.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Users")]
    public class User : IdentityUser
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
            FriendshipAddressee = new HashSet<Friendship>();
            FriendshipRequester = new HashSet<Friendship>();
            Groups = new HashSet<UserInGroup>();
            Images = new HashSet<Image>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string City { get; set; }
        public string Country { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }
        public string Locale { get; set; } = "en-GB";
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Friendship> FriendshipAddressee { get; set; }
        public ICollection<Friendship> FriendshipRequester { get; set; }
        public ICollection<UserInGroup> Groups { get; set; }
        public ICollection<TagFriendInPost> TaggerInPosts { get; set; }
        public ICollection<TagFriendInPost> TaggedInPosts { get; set; }
        public ICollection<TagFriendInComment> TaggerInComments { get; set; }
        public ICollection<TagFriendInComment> TaggedInComments { get; set; }
        public ICollection<Image> Images { get; set; }
        public int AvatarId { get; set; }
        public Avatar Avatar { get; set; }
    }

    public enum Gender
    {
        male = 0,
        female = 1
    }
}
