using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LinqToTwitter;
using System.Configuration;

namespace Services
{
    public class SearchService
    {
        DataManagementService DMService = new DataManagementService();
        public DataManagement.Search CurrentSearch { get; set; }

        public async Task<DataManagement.Search> Search(string query, DateTime DFrom, DateTime DTo, string UserName)
        {
            var result = DMService.CheckIfSearchExists(query, DFrom, DTo);
            if (result != null)
            {
                return result;
            }
            CurrentSearch = new DataManagement.Search();
            CurrentSearch.Query = query;
            CurrentSearch.From = DFrom;
            CurrentSearch.To = DTo;
            CurrentSearch.SDate = DateTime.Now;

            if (UserName == "")
            {
                DMService.SaveSearch(CurrentSearch);
            }
            else
            {
                var User = DMService.GetUserByName(UserName);
                User.Searches.Add(CurrentSearch);
                DMService.SaveChanges();
            }
            

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    //AccessToken = ConfigurationManager.AppSettings["accessToken"],
                    //AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
                }
            };
            ulong idfrom = 0, idto = long.MaxValue;

            using (var twitterCtx = new TwitterContext(auth))
            {
                //Getting oldest tweet id from DateTime(tfrom)

                var searchResponse = await
                    (from search in twitterCtx.Search
                     where search.Type == SearchType.Search &&
                           search.Query == "\"" + query + "\"" &&
                           search.Count == 1 &&
                           search.Until == DFrom
                     select search).SingleOrDefaultAsync();
                if (searchResponse != null && searchResponse.Statuses != null
                    && searchResponse.Statuses.Count != 0)
                    idfrom = searchResponse.Statuses.First().StatusID;

                //Getting tweets that match search string
                do
                {
                    searchResponse = await
                        (from search in twitterCtx.Search
                         where search.Type == SearchType.Search &&
                               search.Query == "\"" + query + "\"" &&
                               search.Count == 100 &&
                               search.SinceID == idfrom &&
                               search.Until == DTo &&
                               search.MaxID == idto
                         select search).SingleOrDefaultAsync();
                    if (searchResponse != null && searchResponse.Statuses != null)
                    {
                        foreach (var tweet in searchResponse.Statuses)
                        {
                            if(tweet.Coordinates != null && tweet.Coordinates.Latitude != 0 && tweet.Coordinates.Longitude != 0)
                            {
                                CurrentSearch.Tweets.Add(new DataManagement.Tweet
                                {
                                    User = tweet.User.ScreenNameResponse,
                                    Text = tweet.Text,
                                    Latitude = tweet.Coordinates.Latitude,
                                    Longitude = tweet.Coordinates.Longitude,
                                    Location = tweet.User.Location
                                });
                            }
                        }
                        if (searchResponse.Statuses.Count != 0
                            && idto != searchResponse.Statuses.Last().StatusID)
                        {
                            idto = searchResponse.Statuses.Last().StatusID;
                        }
                        else { break; }
                    }
                    DMService.SaveChanges();
                    System.Threading.Thread.Sleep(2000);        // 450/15min = 1/2s 
                } while (searchResponse.Statuses.Count != 0);
                return CurrentSearch;
            }
        }
    }
}
