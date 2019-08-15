using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon2019
{
    public class Data
    {
        public string contentSample { get; set; }

        public string id { get; set; }
        public string fbId { get; set; }
        public string message { get; set; }
        public string messageToken { get; set; }
        public DateTime createDate { get; set; }
        public DateTime updateDate { get; set; }
        public DateTime insertDate { get; set; }
        public int type { get; set; }
        public int type_inside { get; set; }
        public int spamType { get; set; }
        public string postType { get; set; }
        public string insideUrl { get; set; }
        public string outsiteUrl { get; set; }
        public string url { get; set; }
        public string tag { get; set; }
        public string sentiTag { get; set; }
        public int likeCount { get; set; }
        public int shareCount { get; set; }
        //public int dislikeCount { get; set; }
        public int commentCount { get; set; }
        public string parentFbId { get; set; }
        public string parentFbName { get; set; }
        public string parentPostId { get; set; }
        public string successorFbId { get; set; }
        public int sentiType { get; set; }
        public int sentiLabel { get; set; }
        public string sentiVector { get; set; }
        public string sentiContent { get; set; }
        public string domain { get; set; }
        public string category_news { get; set; }
        public string category_fb { get; set; }
        public string title { get; set; }   //"title": "HÓT HÓT------- CHƯA BAO GIỜ FLC HÓT ĐẾN VẬY???????? ????<br />\n₫1 - Hà Nội<br />\nEm cần Bán gấp căn hộ chung cư FLC Garden City cách sân vận động Mỹ Đình 3km<br />\nDiện tích 65,8m2: ...",
        public string caption { get; set; } //"Photos from Hoa Tường Vi's post",
        public string place_id { get; set; }
        public string place_name { get; set; }
        public string with_tag { get; set; }
        public string application { get; set; }
        public int reach { get; set; }
        public string is_visited { get; set; } //projectId_Value 123_1 124_1
        public string is_spam { get; set; }   //projectId_Value 123_1 124_0
        public string is_resenti { get; set; } //projectId_Value 123_0 124_1
        public string is_group { get; set; }  //groupIds 55 123_55 | 69 124_69        
        public string is_hidden { get; set; }
        public string is_edit { get; set; }
        public string views { get; set; }
        public int is_fb { get; set; }
        public int total_interaction { get; set; }

        public AuthorData author { get; set; }

        [Nest.Text(Name = "author.id")]
        public string author_id { get; set; }
    }
}