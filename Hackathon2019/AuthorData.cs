using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon2019
{
    public class AuthorData
    {
        public string id { get; set; }
        public DateTime create_date { get; set; }
        public DateTime update_date { get; set; }
        public string author_name { get; set; }

        //[Nest.Text(Analyzer = "ngram_index", SearchAnalyzer = "ngram_search")]
        public string atc_author_name { get; set; }
        public string author_avatar_url { get; set; }
        public int follower_count { get; set; }
        public int friend_count { get; set; }
        public int alexa_rank { get; set; }
        public int author_is_a_fanpage { get; set; }

        //[Nest.Text(Analyzer = "whitespace")]
        public string host { get; set; }
        public int type { get; set; }
        public int type_inside { get; set; }
        public string author_url { get; set; }
        public string service { get; set; }
        public int influence_score { get; set; }


        //rank from page
        public int rank { get; set; }
        public int likes { get; set; }
        public string username { get; set; }
        public string atcUsername { get; set; }
        public string about { get; set; }


        //biograph info
        public int ageRange { get; set; }
        public int gender { get; set; }
        public int location { get; set; }
        public int educationLevel { get; set; }
        public int jobLevel { get; set; }
        public int incomeLevel { get; set; }
        public int marrigeStatus { get; set; }
        public int birthYear { get; set; }
        public int birthYearPredict { get; set; }
        public int locationPredict { get; set; }

        public string education_name { get; set; }
        public string job_level_name { get; set; }
        public string industry_name { get; set; }
        public string current_city { get; set; }
        public string hometown { get; set; }
        public string phone_mobile { get; set; }
        public string email { get; set; }
        public int is_user { get; set; }
        public DateTime user_post_updated_at { get; set; }
        public DateTime user_post_last_data { get; set; }
        public int user_post_status { get; set; }
        public int user_post_failed_total { get; set; }
        public int total_feed { get; set; }


        //new fields
        //[Nest.Text(Analyzer = "whitespace")]
        public string actorFbId { get; set; }

        //[Nest.Text(Analyzer = "whitespace")]
        public string actorProfileId { get; set; }
        public string actorName { get; set; }
        public string actorSystemId { get; set; }


        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object fb_data { get; set; }
    }
}