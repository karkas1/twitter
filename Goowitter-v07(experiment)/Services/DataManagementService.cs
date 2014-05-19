using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManagement;

namespace Services
{
    public class DataManagementService
    {
        UsersContext MainDB = new UsersContext();

        public IEnumerable<Search> LoadSearch(string User)
        {
            var result = from r in MainDB.SearchesDB
                         where User == r.UserProfile_Id.UserName
                         select r;
            return result;
        }

        public UserProfile GetUserByName(string Name)
        {
            var result = from r in MainDB.UserProfiles
                         where r.UserName == Name
                         select r;
            return result.First();
        }

        public int SaveSearch(Search data)
        {
            MainDB.SearchesDB.Add(data);
            MainDB.SaveChanges();
            return data.Id; //returns search id
        }

        public int SaveTweet(Tweet data, int SearchID)
        {
            MainDB.TweetsDB.Add(data);
            MainDB.SaveChanges();
            return data.Id;
        }

        public void SaveChanges()
        {
            MainDB.SaveChanges();
        }

        public Search CheckIfSearchExists(string Query, DateTime DateFrom, DateTime DateTo)
        {
            var searches = from r in MainDB.SearchesDB
                         where r.Query == Query && r.From == DateFrom && r.To == DateTo
                         select r;
            if (searches.Count() != 0)
            {
                Search search = searches.First();
                var tweets = from r in MainDB.TweetsDB
                             where search.Id == r.Search_Id.Id
                             select r;
                search.Tweets = tweets.ToList();
                return search;
            }
            return null;
        }
    }
}
