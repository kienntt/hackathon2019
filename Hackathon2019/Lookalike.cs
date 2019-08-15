using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Hackathon2019
{
    public class Lookalike
    {
        static Elasticsearch<AuthorData> elasticAuthorData = new Elasticsearch<AuthorData>("");
        static Elasticsearch<Data> elasticData = new Elasticsearch<Data>("");
        public static string getPhoneInfo(string ApiType, string Query)
        {

            string ReturnString = "";
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Accept-Language", " en-US");
            client.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
            client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
            switch (ApiType)
            {
                case "profile":

                    ReturnString = "";
                    if (Query != "")
                    {
                        if (((Query.Substring(0, 2) == "84") && (Query.Length == 11)) || ((Query.Substring(0, 1) == "0") && (Query.Length == 10)))
                        {
                            string wildcard_phone = "*";
                            wildcard_phone += Query.Substring((Query.Length - 9));
                            ReturnString = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=fb_data.mobile_phone:" + wildcard_phone);
                            JObject user = JObject.Parse(ReturnString);
                            if (Int32.Parse(user["hits"]["total"].ToString()) == 0)
                                ReturnString = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=phone_mobile:" + wildcard_phone);
                        }
                        else
                            ReturnString = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=" + Query);
                    }

                    break;


                case "post":
                    //ReturnString = "post(" + client.DownloadString("http://192.168.36.254:9200/post_2018_07/_search?q=" + Query) + ")";

                    ReturnString = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/post_2019_06,post_2019_07,post_2019_08/_search?q=" + Query);
                    break;
            
                case "friends":
                    string[] IdArray = Query.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);


                    string FirstList = client.DownloadString(ConfigurationManager.AppSettings["FriendsApi"].ToString() + "/default.aspx?id=" + IdArray[0]);
                    //string[] hi = new string[] { "100003849427694", "100010200010823" };
                    string statisticUser;
                    //JObject data = JObject.Parse(FirstList);
                    //var friend = data["hits"]["hits"][0]["_source"]["fr_list"].ToString().Split(' ');
                    var friend = FirstList.Split(' ');
                    List<Object> author = new List<Object>();
                    author = elasticAuthorData.GetInfoFriend("fb_author_data", friend, out statisticUser);
                    string jsonObjectStr = "";

                    jsonObjectStr += "{ ";



                    jsonObjectStr += "\"friend\": " + JsonConvert.SerializeObject(author) + ",";
                    jsonObjectStr += statisticUser;
                    jsonObjectStr += " }";
                    ReturnString = jsonObjectStr;
                    break;
                case "topfriend":
                    string[] Id = Query.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    string FrList = client.DownloadString(ConfigurationManager.AppSettings["FriendsApi"].ToString() + "/default.aspx?id=" + Id[0]);
                    //string[] hio = new string[] { "100009753792896", "100010200010823", "100011697999725", "100006638527025", "100003849427694" };
                    var friends = FrList.Split(' ');

                    List<Object> topList = new List<Object>();
                    
                    topList = elasticAuthorData.GetTopSimilarFriend("", friends, Query);
                    string Str = "";

                    Str += "{ ";
                    Str += "\"topfriend\": " + JsonConvert.SerializeObject(topList);

                    Str += " }";
                    ReturnString = Str;
                    break;
                case "getphone":

                    var list_phone = Query.Split(' ');
                    List<string> listId = new List<string>();
                    
                    foreach (var phone in list_phone)
                    {
                        var data = client.DownloadString("" + phone);
                        data = data.Remove(0, 8);
                        data = data.Remove(data.Length - 1, 1);
                        JObject ob = JObject.Parse(data);
                        if (ob["hits"]["total"].ToString()!="0")
                        {
                            var phoneMb = ob["hits"]["hits"][0]["_source"]["phone_mobile"].ToString();
                            var mobilePh = "0";
                            if (ob["hits"]["hits"][0]["_source"]["phone_mobile"] != null)
                            {
                                mobilePh = ob["hits"]["hits"][0]["_source"]["phone_mobile"].ToString();
                            }

                            listId.Add(ob["hits"]["hits"][0]["_id"].ToString());
                        }
                        
                        
                    }
                    Dictionary<string, int> score = new Dictionary<string, int>();
                    foreach (var i in listId)
                    {
                        var listFrData = client.DownloadString("" + i);
                        var listFr = listFrData.Split(' ');
                        foreach (var id in listFr)
                        {
                            if (score.ContainsKey(id))
                            {
                                score[id] += 1;
                            }
                            else
                            {
                                score[id] = 1;
                            }
                        }

                    }
                    foreach (var i in list_phone)
                    {
                        score.Remove(i);
                    }
                    var sort = from pair in score
                               orderby pair.Value descending
                               select pair;
                    var top50 = sort.Take(50);
                    List<string> listSt = new List<string>();
                    foreach (var top in top50)
                    {
                        var data = client.DownloadString("" + top.Key);
                        data = data.Remove(0, 8);
                        data = data.Remove(data.Length - 1, 1);
                        JObject ob = JObject.Parse(data);
                        if (ob["hits"]["total"].ToString() != "0")
                        {
                            var phoneMb = ob["hits"]["hits"][0]["_source"]["phone_mobile"].ToString();
                            var mobilePh = "0";
                            if (ob["hits"]["hits"][0]["_source"]["phone_mobile"] != null)
                            {
                                mobilePh = ob["hits"]["hits"][0]["_source"]["phone_mobile"].ToString();
                            }
                            var phone = phoneMb != "0" ? phoneMb : mobilePh;

                            listSt.Add(ob["hits"]["hits"][0]["_id"].ToString() + ":" + ob["hits"]["hits"][0]["_source"]["author_name"].ToString() + ":" + phone);
                        }
                            
                       
                        
                    }
                   
                    ReturnString = String.Join(",", listSt.ToArray()); ;
                    break;
                case "lookalike":

                    var listPhone = Query.Split(' ');
                    List<string> listIds = new List<string>();

                    foreach (var phone in listPhone)
                    {
                        var data = "";
                        if (((phone.Substring(0, 2) == "84") && (phone.Length == 11)) || ((phone.Substring(0, 1) == "0") && (phone.Length == 10)))
                        {
                            string wildcard_phone = "*";
                            wildcard_phone += phone.Substring((phone.Length - 9));
                            data = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=fb_data.mobile_phone:" + wildcard_phone);
                            JObject user = JObject.Parse(data);
                            if (Int32.Parse(user["hits"]["total"].ToString()) == 0)
                                data = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=phone_mobile:" + wildcard_phone);
                        }
                        else
                            data = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=" + Query);
                        //var data = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=" + phone);
                        
                        JObject ob = JObject.Parse(data);
                        if (ob["hits"]["total"].ToString() != "0")
                        {
                            var phoneMb = ob["hits"]["hits"][0]["_source"]["phone_mobile"].ToString();
                            var mobilePh = "0";
                            if (ob["hits"]["hits"][0]["_source"]["fb_data"]["mobile_phone"] != null)
                            {
                                mobilePh = ob["hits"]["hits"][0]["_source"]["fb_data"]["mobile_phone"].ToString();
                            }

                            listIds.Add(ob["hits"]["hits"][0]["_id"].ToString());
                        }
                            

                    }
                    Dictionary<string, int> scoreFr = new Dictionary<string, int>();
                    foreach (var i in listIds)
                    {
                        var listFrData = client.DownloadString("" + i);
                        var listFr = listFrData.Split(' ');
                        foreach (var id in listFr)
                        {
                            if (scoreFr.ContainsKey(id))
                            {
                                scoreFr[id] += 1;
                            }
                            else
                            {
                                scoreFr[id] = 1;
                            }
                        }

                    }
                    foreach (var i in listPhone)
                    {
                        scoreFr.Remove(i);
                    }
                    var sortFr = from pair in scoreFr
                                 orderby pair.Value descending
                               select pair;
                    var top50Fr = sortFr.Take(50);
                    List<string> listStr = new List<string>();
                    foreach (var top in top50Fr)
                    {
                        var data = client.DownloadString(ConfigurationManager.AppSettings["ElasticLong"].ToString() + "/fb_author_data/authordata/_search?q=" + top.Key);
                        JObject ob = JObject.Parse(data);
                        if (ob["hits"]["total"].ToString() != "0")
                        {
                            var phoneMb = ob["hits"]["hits"][0]["_source"]["phone_mobile"].ToString();
                            var mobilePh = "0";
                            if (ob["hits"]["hits"][0]["_source"]["fb_data"]["mobile_phone"] != null)
                            {
                                mobilePh = ob["hits"]["hits"][0]["_source"]["fb_data"]["mobile_phone"].ToString();
                            }
                            var phone = phoneMb != "0" ? phoneMb : mobilePh;

                            listStr.Add(ob["hits"]["hits"][0]["_id"].ToString() + ":" + ob["hits"]["hits"][0]["_source"]["author_name"].ToString() + ":" + phone);
                        }
                            


                    }

                    ReturnString = String.Join(",", listStr.ToArray()); ;
                    break;
            }
            return ReturnString;
        }

    }
}