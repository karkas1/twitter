using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManagement
{
    [Table("Search")]
    public class Search
    {
        public Search()
        {
            Tweets = new List<Tweet>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Query { get; set; }
        public DateTime SDate { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IList<Tweet> Tweets { get; set; }
        public UserProfile UserProfile_Id { get; set; }
    }
}
