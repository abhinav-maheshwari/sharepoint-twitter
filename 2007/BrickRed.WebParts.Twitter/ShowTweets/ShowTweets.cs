﻿/*
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
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.ComponentModel;
using Twitterizer;
using System.Drawing;

namespace BrickRed.WebParts.Twitter
{
    [Guid("469e6aec-2377-44b9-a5ef-a1bbd6b47875")]
    public class ShowTweets : System.Web.UI.WebControls.WebParts.WebPart
    {
        public ShowTweets()
        {
        }

        #region Webpart Properties

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
                OAuthTokens tokens = new OAuthTokens();
                tokens.ConsumerKey = this.ConsumerKey;
                tokens.ConsumerSecret = this.ConsumerSecret;
                tokens.AccessToken = this.AccessToken;
                tokens.AccessTokenSecret = this.AccessTokenSecret;

                UserTimelineOptions options = new UserTimelineOptions();
                options.Count = this.TweetCount;

                TwitterStatusCollection userTimeline = TwitterTimeline.UserTimeline(tokens, options);
                Table mainTable;
                TableRow tr, tr2;
                TableCell tc, tc2, tc3;
                Label Caption, Caption2;
                HyperLink imgHyperLink;
                string strSource;
                mainTable = new Table();
                mainTable.Width = Unit.Percentage(100);
                mainTable.CellSpacing = 0;
                mainTable.CellPadding = 5;
                mainTable.BorderWidth = 1;
                mainTable.BorderColor = Color.LightGray;
                mainTable.CssClass = "ms-listviewtable";
                this.Controls.Add(mainTable);

                foreach (TwitterStatus tweet in userTimeline)
                {
                    if (i < this.TweetCount)
                    {
                        tr = new TableRow();
                        mainTable.Rows.Add(tr);
                        tc2 = new TableCell();

                        if (this.EnableShowImage)
                        {
                            imgHyperLink = new HyperLink();
                            imgHyperLink.ImageUrl = tweet.User.ProfileImageLocation;
                            imgHyperLink.NavigateUrl = "http://twitter.com/" + tweet.User.Name;
                            imgHyperLink.Attributes.Add("target", "_blank");
                            tc2.Width = Unit.Percentage(10);
                            tc2.RowSpan = 2;
                            tr.Cells.Add(tc2);
                            tc2.Controls.Add(imgHyperLink);
                        }

                        tc = new TableCell();
                        tc.Width = Unit.Percentage(90);
                        tr.Cells.Add(tc);

                        Caption = new Label();
                        Caption.Font.Bold = true;
                        Caption.Text = tweet.Text;
                        tc.Controls.Add(Caption);

                        tr2 = new TableRow();
                        tc3 = new TableCell();

                        if (this.EnableShowDesc)
                        {
                            tc3.VerticalAlign = VerticalAlign.Top;
                            mainTable.Rows.Add(tr2);
                            tr2.Cells.Add(tc3);
                            if (tweet.Source.StartsWith("<"))
                                strSource = tweet.Source.Substring(tweet.Source.IndexOf('>') + 1, tweet.Source.LastIndexOf('<') - tweet.Source.IndexOf('>') - 1);
                            else
                                strSource = tweet.Source;
                            Caption2 = new Label();
                            Caption2.Text = relativeTime(tweet.CreatedDate.ToString()) + " via " + strSource;
                            tc3.Controls.Add(Caption2);
                        }

                        if (i % 2 == 0)
                        {
                            tr.CssClass = "";
                            tr2.CssClass = "";
                            tc.CssClass = "ms-vb";
                            tc2.CssClass = "ms-vb";
                            tc3.CssClass = "ms-vb";
                        }
                        else
                        {
                            tr.CssClass = "ms-alternating";
                            tr2.CssClass = "ms-alternating";
                            tc.CssClass = "ms-vb";
                            tc2.CssClass = "ms-vb";
                            tc3.CssClass = "ms-vb";
                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
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