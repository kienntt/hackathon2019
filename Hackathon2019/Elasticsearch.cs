using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Hackathon2019
{
    public class Elasticsearch<T> where T : class
    {
        ElasticClient client;
        bool is_empty_query_profile = true;
        bool is_non_network_filter = true;
        public static List<List<int>> total_subs = new List<List<int>>();
        //longterm Constructor
        public Elasticsearch(string indexName)
        {
            Uri node = new Uri(ConfigurationManager.AppSettings["ElasticLong"].ToString());
            ConnectionSettings settings = new ConnectionSettings(node);
            settings.DefaultIndex(indexName);
            settings.RequestTimeout(TimeSpan.FromMinutes(3));
            client = new ElasticClient(settings);
        }
        
        public List<Object> GetTopSimilarFriend(string index, string[] listIdFriend, string myid)
        {
            JObject userData = new JObject();
            var getDataUser = client.Search<AuthorData>(s => s.Index("fb_author_data")
                                .Query(q => q
                                    .Term(t => t.Field(f => f.id).Value(myid))));
            foreach (var result in getDataUser.Hits)
            {
                userData = JObject.FromObject(result.Source);
                //userData.Property("fb_data").Replace(new JProperty("fb_data", JObject.Parse(result.Source.fb_data.ToString())));
            }
            List<Object> facebookDataList = new List<Object>();
            Dictionary<JObject, long> similarFr = new Dictionary<JObject, long>();

            var results = client.Search<AuthorData>(s => s.Index("fb_author_data")
                    .From(0)
                    .Size(5000)
                    .Query(q => q
                        .Bool(b => b.
                            Must(m => m.
                            Terms(t => t.Field(fl => fl.id).Terms(listIdFriend))))
                        )
                    .Sort(sr => sr
                        .Field(f => f
                            .Field(ff => ff.influence_score)
                            .Descending())));
            // tao multi search lay tag score
            

            foreach (var friend in results.Hits)
            {


                //facebookDataList.Add(JObject.FromObject(result.Source));

                var friendData = JObject.FromObject(friend.Source);
                //friendData.Property("fb_data").Replace(new JProperty("fb_data", JObject.Parse(friend.Source.fb_data.ToString())));

                int similarScore = calculateScore(userData, friendData);

                long score =  similarScore;
                JObject dataReturn = new JObject();
                dataReturn.Add(new JProperty("id", friendData["id"].ToString()));
                dataReturn.Add(new JProperty("name", friendData["author_name"].ToString()));
                dataReturn.Add(new JProperty("similar_score", similarScore));

                similarFr.Add(dataReturn, score);
            }
            var sort = from pair in similarFr
                       orderby pair.Value descending
                       select pair;
            var top20 = sort.Take(20);
            foreach (var item in top20)
            {

                facebookDataList.Add(item.Key);
            }
            return facebookDataList;
        }
        public int calculateScore(JObject user1, JObject user2)
        {
            int score = 0;

            string[] dimens = { "ageRange", "birthYear", "birthYearPredict", "current_city", "educationLevel", "education_name", "gender", "hometown", "incomeLevel", "industry_name", "influence_score", "jobLevel", "job_level_name", "location", "marrigeStatus", "rank" };
            foreach (var i in dimens)
            {
                if (user1[i].ToString() == user2[i].ToString())
                    score++;
            }
            if ((user1["education"] != null) && (user2["education"] != null))
            {
                foreach (var i in user1["education"])
                {
                    var dataOb2 = user2["education"].ToString();
                    if (dataOb2.Contains(i["school"]["id"].ToString()))
                    {
                        score++;
                    }
                }
            }
            if ((user1["work"] != null) && (user2["work"] != null))
            {
                foreach (var i in user1["work"])
                {
                    var dataOb2 = user2["work"].ToString();
                    if (dataOb2.Contains(i["employer"]["id"].ToString()))
                    {
                        score++;
                    }
                }
            }
            return score;
        }
        
        public List<Object> GetInfoFriend(string index, string[] friend, out string statisticUser)
        {

            List<Object> facebookDataList = new List<Object>();
            Dictionary<string, int> workCount = new Dictionary<string, int>();
            Dictionary<string, string> workName = new Dictionary<string, string>();
            Dictionary<string, int> highSchoolCount = new Dictionary<string, int>();
            Dictionary<string, string> highSchoolName = new Dictionary<string, string>();
            Dictionary<string, int> collegeCount = new Dictionary<string, int>();
            Dictionary<string, string> collegeName = new Dictionary<string, string>();
            var results = client.Search<AuthorData>(s => s.Index("fb_author_data")
                    .From(0)
                    .Size(50)
                    .Query(q => q
                        .Bool(b => b.Must(m => m.
                        Terms(ts => ts.Field("id").Terms(friend))))
                        )
                    .Sort(sr => sr
                        .Field(f => f
                            .Field(ff => ff.influence_score)
                            .Descending())));
            foreach (var result in results.Hits)
            {

                string date = result.Source.create_date.ToString("yyyy-MM-dd HH:mm");
                result.Source.create_date = Convert.ToDateTime(date);
                var data = JObject.FromObject(result.Source);
                data["fb_data"] = result.Source.fb_data.ToString();
                facebookDataList.Add(data);
                var a = result.Source.fb_data;
                JObject Data = JObject.Parse(result.Source.fb_data.ToString());
                if (Data["work"] != null)
                {
                    foreach (var i in Data["work"])
                    {
                        var id = i["employer"]["id"].ToString();
                        var name = i["employer"]["name"].ToString();
                        if (!workName.ContainsKey(id))
                            workName.Add(id, name);
                        if (workCount.ContainsKey(id))
                            workCount[id] = workCount[id] + 1;
                        else
                            workCount.Add(id, 1);
                    }
                }
                if (Data["education"] != null)
                {
                    foreach (var i in Data["education"])
                    {
                        var id = i["school"]["id"].ToString();
                        var name = i["school"]["name"].ToString();
                        if (i["type"].ToString() == "College")
                        {
                            if (!collegeName.ContainsKey(id))
                                collegeName.Add(id, name);
                            if (collegeCount.ContainsKey(id))
                                collegeCount[id] = collegeCount[id] + 1;
                            else
                                collegeCount.Add(id, 1);
                        }
                        else
                        {
                            if (!highSchoolName.ContainsKey(id))
                                highSchoolName.Add(id, name);
                            if (highSchoolCount.ContainsKey(id))
                                highSchoolCount[id] = highSchoolCount[id] + 1;
                            else
                                highSchoolCount.Add(id, 1);
                        }
                    }
                }

            }





            string jsonWorkStr = "\"work\":{" + getTop10(workName, workCount) + "}";
            string jsonHighSchoolStr = "\"highschool\":{" + getTop10(highSchoolName, highSchoolCount) + "}";
            string jsonCollegeStr = "\"college\":{" + getTop10(collegeName, collegeCount) + "}";
            statisticUser = jsonWorkStr + "," + jsonHighSchoolStr + "," + jsonCollegeStr;
            return facebookDataList;
        }
        public string getTop10(Dictionary<string, string> name, Dictionary<string, int> count)
        {
            string json = "";
            int total = 0;
            foreach (KeyValuePair<string, int> i in count)
            {
                total += i.Value;
            }
            var sort = from pair in count
                       orderby pair.Value descending
                       select pair;
            var top10 = sort.Take(4);
            List<string> listData = new List<string>();
            foreach (KeyValuePair<string, int> item in top10)
            {
                listData.Add("\"" + name[item.Key] + "\":\"" + count[item.Key] + "\"");
                total -= item.Value;
            }
            listData.Add("\"Khác\":\"" + total + "\"");

            json = String.Join(",", listData.ToArray());
            return json;
        }
        
    }
}