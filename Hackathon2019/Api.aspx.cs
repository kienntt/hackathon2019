using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hackathon2019
{
    public partial class Api : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ApiType = string.IsNullOrEmpty(Request["t"]) ? "" : Request["t"].ToString();
            string Query = string.IsNullOrEmpty(Request["q"]) ? "" : Request["q"].ToString();
            string ReturnString = string.Empty;
            if (ApiType== "getphone" || ApiType=="lookalike")
            {
                var listphone = Request.Form["key"];
                ReturnString = Lookalike.getPhoneInfo(ApiType, listphone);
            }
            else
            {
                ReturnString = Lookalike.getPhoneInfo(ApiType, Query);
            }






            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "text/json";
            Response.Write(ReturnString);         
            Response.End();
        }
    }
}