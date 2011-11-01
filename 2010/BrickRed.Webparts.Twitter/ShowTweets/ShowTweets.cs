/*
 ===========================================================================
 Copyright (c) 2010 BrickRed Technologies Limited

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 ===========================================================================
 */
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Twitterizer;

namespace BrickRed.Webparts.Twitter
{
    [ToolboxItemAttribute(false)]
    public class ShowTweets : Microsoft.SharePoint.WebPartPages.WebPart
    {
        public ShowTweets()
        {
        }

        #region Webpart Properties


        [WebBrowsable(true),
     Category("Twitter Settings"),
     Personalizable(PersonalizationScope.Shared),
      WebPartStorage(Storage.Shared),
     WebDisplayName("Screen Name"),
     WebDescription("Please enter the screen name")]

        public string ScreenName { get; set; }


        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       WebDisplayName("Consumer Key"),
       WebDescription("Please enter a Consumer key")]

        public string ConsumerKey { get; set; }

        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       WebDisplayName("Consumer Secret"),
       WebDescription("Please enter Consumer secret")]

        public string ConsumerSecret { get; set; }

        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       WebDisplayName("Access Token"),
       WebDescription("Please enter Access token")]

        public string AccessToken { get; set; }

        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
       WebPartStorage(Storage.Shared),
       WebDisplayName("Access Token Secret"),
       WebDescription("Please enter Access token secret")]

        public string AccessTokenSecret { get; set; }

        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue("10"),
       WebDisplayName("Tweet Count"),
       WebDescription("Please enter no of tweets you want to display")]


        public int TweetCount { get; set; }

        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue("true"),
       WebDisplayName("Show User Image"),
       WebDescription("Would you like to show image")]

        public bool EnableShowImage { get; set; }

        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue("true"),
       WebDisplayName("Show Description"),
       WebDescription("Would you like to show description")]

        public bool EnableShowDesc { get; set; }

        #endregion

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            int i = 0;
            try
            {

                if (!string.IsNullOrEmpty(this.ConsumerKey) &&
                     !string.IsNullOrEmpty(this.ScreenName) &&
                     !string.IsNullOrEmpty(this.ConsumerSecret) &&
                     !string.IsNullOrEmpty(this.AccessToken) &&
                     !string.IsNullOrEmpty(this.AccessTokenSecret))
                {

                    OAuthTokens tokens = new OAuthTokens();
                    tokens.ConsumerKey = this.ConsumerKey;
                    tokens.ConsumerSecret = this.ConsumerSecret;
                    tokens.AccessToken = this.AccessToken;
                    tokens.AccessTokenSecret = this.AccessTokenSecret;

                    UserTimelineOptions options = new UserTimelineOptions();
                    options.Count = this.TweetCount;
                    options.ScreenName = this.ScreenName;

                    TwitterResponse<TwitterStatusCollection> userTimeline = TwitterTimeline.UserTimeline(tokens, options);
                    Table mainTable, innerTable;
                    TableRow tr;
                    TableCell tc;
                    HyperLink imgHyperLink;
                    string strSource;
                    mainTable = new Table();

                    mainTable.Width = Unit.Percentage(100);
                    mainTable.CellSpacing = 0;
                    mainTable.CellPadding = 0;
                    mainTable.BorderWidth = 0;
                    mainTable.CssClass = "ms-viewlsts";

                    this.Controls.Add(mainTable);

                    foreach (TwitterStatus tweet in userTimeline.ResponseObject)
                    {
                        innerTable = new Table();
                        innerTable.CssClass = "ms-viewlsts";
                        innerTable.Width = Unit.Percentage(100);

                        if (i < this.TweetCount)
                        {
                            tr = new TableRow();
                            mainTable.Rows.Add(tr);

                            tc = new TableCell();
                            tc.Width = Unit.Percentage(10);
                            if (i % 2 != 0)
                                tr.CssClass = "ms-alternatingstrong";
                            else
                                tr.CssClass = " ";
                            tc.CssClass = "ms-vb2";
                            if (this.EnableShowImage)
                            {
                                imgHyperLink = new HyperLink();
                                imgHyperLink.ImageUrl = tweet.User.ProfileImageLocation;
                                imgHyperLink.NavigateUrl = "http://twitter.com/" + tweet.User.Name;
                                imgHyperLink.Attributes.Add("target", "_blank");

                                tc.Controls.Add(imgHyperLink);
                                tr.Cells.Add(tc);
                            }

                            tc = new TableCell();
                            tc.Controls.Add(innerTable);
                            tr.Controls.Add(tc);

                            tr = new TableRow();
                            innerTable.Rows.Add(tr);
                            tc = new TableCell();
                            tr.Cells.Add(tc);

                            tc.Text = tweet.Text;
                            tc.CssClass = "ms-vb2";

                            if (this.EnableShowDesc)
                            {
                                tr = new TableRow();
                                innerTable.Rows.Add(tr);
                                tc = new TableCell();
                                tr.Cells.Add(tc);

                                if (tweet.Source.StartsWith("<"))
                                    strSource = tweet.Source.Substring(tweet.Source.IndexOf('>') + 1, tweet.Source.LastIndexOf('<') - tweet.Source.IndexOf('>') - 1);
                                else
                                    strSource = tweet.Source;

                                tc.Style.Add("color", "Gray");
                                tc.Text = relativeTime(tweet.CreatedDate.ToString()) + " via " + strSource;
                                tc.CssClass = "ms-vb2";
                                if (i % 2 != 0)
                                    tr.CssClass = "ms-alternatingstrong";
                                else
                                    tr.CssClass = " ";
                            }
                        }
                        else
                        {
                            break;
                        }
                        i++;
                    }
                }
                else
                {
                    Label LblMessage = new Label();
                    LblMessage.Text = "Twitter webpart properties missing. Please update twitter settings from property pane.";
                }
            }
            catch (Exception Ex)
            {
                Label LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        private string relativeTime(string pastTime)
        {
            DateTime origStamp = DateTime.Parse(pastTime.ToString());
            DateTime curDate = DateTime.Now;

            TimeSpan ts = curDate.Subtract(origStamp);
            string strReturn = string.Empty;

            if (ts.Days >= 1)
            {
                strReturn = String.Format("{0:hh:mm tt MMM dd}" + "th", Convert.ToDateTime(pastTime).ToUniversalTime());
            }
            else
            {
                if (ts.Hours >= 1)
                    strReturn = "about " + ts.Hours + " hours ago";
                else
                {
                    if (ts.Minutes >= 1)
                    {
                        strReturn = "about " + ts.Minutes + " minutes ago";
                    }
                    else
                        strReturn = "about " + ts.Seconds + " seconds ago";
                }
            }
            return strReturn;
        }
    }
}
