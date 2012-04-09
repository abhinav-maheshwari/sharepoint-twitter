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
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System.ComponentModel;
using Twitterizer;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.Caching;

namespace BrickRed.WebParts.Twitter
{
    [Guid("469e6aec-2377-44b9-a5ef-a1bbd6b47875")]
    public class ShowTweets : System.Web.UI.WebControls.WebParts.WebPart
    {
        #region Declaration

        ImageButton imgMoreTweet = new ImageButton();
        HtmlImage imgNoTweet = new HtmlImage();
        TableCell tcContent = new TableCell();
        TableCell tcpaging = new TableCell();
        string ImagePath = SPContext.Current.Web.Url + "/_layouts/Brickred.OpenSource.Twitter/";
        HiddenField objPageCount;
        string PageCountValue = string.Empty;

        #endregion

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

        private int _tweetCount = 10;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Tweet Count"),
        WebDescription("Please enter no of tweets you want to display")]
        public int TweetCount
        {
            get { return _tweetCount; }
            set { _tweetCount = value; }
        }

        private bool _enableShowImage = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Show User Image"),
        WebDescription("Would you like to show image")]
        public bool EnableShowImage
        {
            get { return _enableShowImage; }
            set { _enableShowImage = value; }
        }

        private bool _enableShowDesc = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Show Description"),
        WebDescription("Would you like to show description")]
        public bool EnableShowDesc
        {
            get { return _enableShowDesc; }
            set { _enableShowDesc = value; }
        }

        private bool _showHeader = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Show header"),
        WebDescription("Would you like to show header")]
        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        private bool _showHeaderImage = false;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(false),
        WebDisplayName("Show Image in header"),
        WebDescription("Would you like to show image in header")]
        public bool ShowHeaderImage
        {
            get { return _showHeaderImage; }
            set { _showHeaderImage = value; }
        }

        private bool _showFooter = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Show footer"),
        WebDescription("Would you like to show footer")]
        public bool ShowFooter
        {
            get { return _showFooter; }
            set { _showFooter = value; }
        }

        private bool _showFollowUs = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Show 'Follow Us' link at footer"),
        WebDescription("Would you like to show 'Follow Us' link at footer")]
        public bool ShowFollowUs
        {
            get { return _showFollowUs; }
            set { _showFollowUs = value; }
        }

        #endregion

        /// <summary>
        /// Page load event
        /// </summary>
        /// <returns></returns>
        protected override void OnLoad(EventArgs e)
        {
            //Creates the hidden field for keeping the page info
            //if(!Page.IsPostBack)
            CreateHiddenField();

            //Get the Css Class
            this.Page.Header.Controls.Add(StyleSheet.CssStyle());
            base.OnLoad(e);
        }

        /// <summary>
        /// Create child controls for this webpart
        /// </summary>
        /// <returns></returns>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            try
            {
                if (!string.IsNullOrEmpty(this.ConsumerKey) &&
                     !string.IsNullOrEmpty(this.ScreenName) &&
                     !string.IsNullOrEmpty(this.ConsumerSecret) &&
                     !string.IsNullOrEmpty(this.AccessToken) &&
                     !string.IsNullOrEmpty(this.AccessTokenSecret))
                {
                    //get the page count value from the hidden field
                    PageCountValue = GetPageNumber();
                    ShowPagedTweets(Convert.ToInt32(PageCountValue));
                }
                else
                {
                    Label LblMessage = new Label();
                    LblMessage.Text = "Twitter webpart properties missing. Please update twitter settings from property pane.";
                    this.Controls.Add(LblMessage);
                }
            }
            catch (Exception Ex)
            {
                Label LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        /// <summary>
        /// Gets the PageNumber value from the hidden field
        /// </summary>
        /// <returns></returns>
        private string GetPageNumber()
        {
            string pagenumber = string.Empty;

            if (ViewState["objPageCountId"] != null)
            {
                int count = 0;
                foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                {
                    string keyid = key.Replace("$", "_");
                    if (keyid.Equals(ViewState["objPageCountId"].ToString()))
                    {
                        break;
                    }
                    count++;
                }
                try
                {
                    pagenumber = HttpContext.Current.Request.Form[count];
                }
                catch
                {
                    pagenumber = "1";
                }
            }
            else
            {
                pagenumber = "1";
            }

            return pagenumber;
        }

        /// <summary>
        /// Creating the Paging for showing the tweets
        /// </summary>
        private void ShowPagedTweets(int PageNumber)
        {
            TwitterStatusCollection tweets;

            //First fetch the action tweets here
            tweets = FetchTweets(PageNumber);


            Table pagingTable;
            TableRow trpaging = new TableRow();
            TableCell tcpaging = new TableCell();
            pagingTable = new Table();
            pagingTable.Width = Unit.Percentage(100);
            pagingTable.CellSpacing = 0;
            pagingTable.CellPadding = 0;
            pagingTable.CssClass = "ms-viewlsts";

            imgMoreTweet.ImageUrl = ImagePath + "BlueTweet.png";
            imgMoreTweet.ID = "imgNextTweet";

            imgNoTweet.Src = ImagePath + "Greytweet.png";
            imgNoTweet.ID = "NoTweet";
            imgNoTweet.Visible = false;

            Table Maintable = new Table();
            Maintable.Width = Unit.Percentage(100);
            Maintable.CellPadding = 0;
            Maintable.CellSpacing = 0;
            TableRow trContent = new TableRow();
            tcContent = new TableCell();

            //add tweet table here
            tcContent.Controls.Add(CreateTweetTable(PageNumber, tweets));


            trContent.Controls.Add(tcContent);
            Maintable.Controls.Add(trContent);

            tcpaging.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            tcpaging.ID = "tcPaging";
            tcpaging.Controls.Add(imgMoreTweet);        //Add the blue tweet bird
            tcpaging.Controls.Add(imgNoTweet);          //Add the grey tweet bird
            trpaging.Cells.Add(tcpaging);
            pagingTable.Rows.Add(trpaging);
            if (this.ShowHeader)
                this.Controls.Add(Common.CreateHeaderFooter("Header", tweets, this.ShowHeaderImage, this.ShowFollowUs));
            this.Controls.Add(Maintable);
            this.Controls.Add(pagingTable);
            if (this.ShowFooter)
                this.Controls.Add(Common.CreateHeaderFooter("Footer", tweets, this.ShowHeaderImage, this.ShowFollowUs));
        }

        /// <summary>
        /// Get the tweets from the Twitter object
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        private TwitterStatusCollection FetchTweets(int PageNumber)
        {
            TwitterStatusCollection tweets = new TwitterStatusCollection();

            //cache the tweets here
            if (Page.Cache[string.Format("Tweet-{0}", PageNumber)] == null)
            {
                //set the tokens here
                OAuthTokens tokens = new OAuthTokens();
                tokens.ConsumerKey = this.ConsumerKey;
                tokens.ConsumerSecret = this.ConsumerSecret;
                tokens.AccessToken = this.AccessToken;
                tokens.AccessTokenSecret = this.AccessTokenSecret;


                UserTimelineOptions options = new UserTimelineOptions();
                options.Count = this.TweetCount * PageNumber;
                options.Page = 1;
                options.ScreenName = this.ScreenName;


                //now hit the twitter and get the response
                tweets = TwitterTimeline.UserTimeline(tokens, options).ResponseObject;

                if (PageNumber == 1)
                {
                    HttpContext.Current.Cache.Add(string.Format("Tweet-{0}", PageNumber), tweets, null, DateTime.Now.AddMinutes(Common.CACHEDURATION), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, CacheRemovedCallBack);
                }
                else
                {
                    HttpContext.Current.Cache.Insert(string.Format("Tweet-{0}", PageNumber), tweets, null, DateTime.Now.AddMinutes(Common.CACHEDURATION), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
                }
            }
            else
            {
                tweets = HttpContext.Current.Cache[string.Format("Tweet-{0}", PageNumber)] as TwitterStatusCollection;
            }

            return tweets;
        }

        /// <summary>
        /// Generates the tweet table
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        private Table CreateTweetTable(int PageNumber, TwitterStatusCollection tweets)
        {
            int i = 0;
            bool isTweetOnlyText = true;
            Table mainTable, innerTable;
            TableRow tr;
            TableCell tc, tcImage, tcText;
            HyperLink imgHyperLink;
            string strSource;
            Label lblContent;

            mainTable = new Table();
            mainTable.Width = Unit.Percentage(100);
            mainTable.CellSpacing = 0;
            mainTable.CellPadding = 0;
            this.Controls.Add(mainTable);


            if (tweets.Count > 0)
            {
                foreach (TwitterStatus tweet in tweets)
                {
                    isTweetOnlyText = true;
                    innerTable = new Table();
                    innerTable.CssClass = "ms-viewlsts";
                    innerTable.Width = Unit.Percentage(100);

                    if (i <= this.TweetCount * PageNumber)
                    {
                        tr = new TableRow();
                        mainTable.Rows.Add(tr);

                        tc = new TableCell();
                        tc.Width = Unit.Percentage(10);

                        tr.CssClass = " ms-WPBorderBorderOnly , twitBorderBottom";

                        #region UserImage
                        //Showing the user image
                        if (this.EnableShowImage)
                        {
                            imgHyperLink = new HyperLink();
                            imgHyperLink.ImageUrl = tweet.User.ProfileImageLocation;
                            imgHyperLink.NavigateUrl = "http://twitter.com/" + tweet.User.ScreenName;
                            imgHyperLink.Attributes.Add("target", "_blank");
                            tc.Controls.Add(imgHyperLink);
                            tc.CssClass = "twitHeaderImage";
                            tr.Cells.Add(tc);
                        }
                        #endregion

                        tc = new TableCell();
                        tc.Controls.Add(innerTable);
                        tr.Controls.Add(tc);

                        tr = new TableRow();
                        innerTable.Rows.Add(tr);

                        #region TwitPic
                        //Code for showing the image on the webpart
                        if (tweet.Entities.Count > 0)
                        {
                            int tweetCount = Convert.ToInt32(tweet.Entities.Count);

                            for (int tweetEntityCount = 0; tweetEntityCount < tweetCount; tweetEntityCount++)
                            {
                                //Check if the tweet is having the Picture
                                if (tweet.Entities[tweetEntityCount].ToString().Equals("Twitterizer.Entities.TwitterMediaEntity"))
                                {
                                    if (!string.IsNullOrEmpty(((Twitterizer.Entities.TwitterMediaEntity)(tweet.Entities[tweetEntityCount])).MediaUrl.ToString()))
                                    {
                                        //Create a new table to add the image and corresponding text
                                        tc = new TableCell();
                                        tr.Cells.Add(tc);
                                        Table tb = new Table();
                                        tb.Width = Unit.Percentage(100);
                                        tc.Controls.Add(tb);
                                        TableRow trinner = new TableRow();

                                        //get the image URL
                                        string ImageURL = ((Twitterizer.Entities.TwitterMediaEntity)(tweet.Entities[tweetEntityCount])).MediaUrl.ToString();

                                        tcImage = new TableCell();

                                        HyperLink imgTweet = new HyperLink();
                                        imgTweet.NavigateUrl = ImageURL;
                                        imgTweet.Attributes.Add("target", "_blank");

                                        //Added the HTMLImage Control to resize the image
                                        HtmlImage htmlImage = new HtmlImage();
                                        htmlImage.Src = ImageURL;
                                        htmlImage.Height = 100;
                                        htmlImage.Width = 137;
                                        htmlImage.Border = 0;
                                        imgTweet.Controls.Add(htmlImage);
                                        tcImage.Width = 137;
                                        tcImage.Controls.Add(imgTweet);
                                        //tcImage.Attributes.Add("style", "padding-top:0.5%");
                                        trinner.Cells.Add(tcImage);

                                        //Add the linkfied text
                                        lblContent = new Label();
                                        lblContent.Text = tweet.LinkifiedText();
                                        lblContent.ForeColor = Color.Black;

                                        //Show the text next to the Image
                                        tcText = new TableCell();
                                        tcText.Controls.Add(lblContent);
                                        trinner.Cells.Add(tcText);

                                        isTweetOnlyText = false;

                                        tb.Rows.Add(trinner);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Show Tweet
                        //If only the text is there in the image
                        if (isTweetOnlyText)
                        {
                            tc = new TableCell();
                            tr.Cells.Add(tc);

                            lblContent = new Label();
                            lblContent.Text = tweet.LinkifiedText();
                            lblContent.ForeColor = Color.Black;

                            tc.Controls.Add(lblContent);
                            tc.CssClass = "ms-vb2";
                        }
                        #endregion

                        #region Show Description
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

                            tc.Text = relativeTime(tweet.CreatedDate.ToString()) + " via " + strSource;
                            tc.CssClass = "ms-vb2";
                            tc.ForeColor = Color.Gray;
                        }
                        #endregion
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }

                imgMoreTweet.Visible = true;
                imgNoTweet.Visible = false;
            }
            else
            {
                imgMoreTweet.Visible = false;
                imgNoTweet.Visible = true;

                tr = new TableRow();
                mainTable.Rows.Add(tr);

                tc = new TableCell();
                tc.Width = Unit.Percentage(100);
                tc.CssClass = "ms-vb2";
                tc.HorizontalAlign = HorizontalAlign.Center;
                tc.ForeColor = Color.Gray;
                tr.Cells.Add(tc);

                tc.Text = string.Format("{0} hasn't tweeted yet.", this.ScreenName);

            }
            // if the number of tweet response is less than the number of tweets demanded than there are no more tweets : show grey tweet
            if (tweets.Count < this.TweetCount * PageNumber)
            {
                imgMoreTweet.Visible = false;
                imgNoTweet.Visible = true;
            }
            return mainTable;
        }

        /// <summary>
        /// Remove the dependent cache objects if primary cache is removed 
        /// </summary>
        /// <returns></returns>
        private void CacheRemovedCallBack(string key, object value, CacheItemRemovedReason reason)
        {
            int counter = 1;
            // If my first page cache is removed then remove all the other caches also
            if (key.Equals(string.Format("Tweet-{0}", counter)))
            {
                while (true)
                {
                    counter++;
                    if (HttpContext.Current.Cache.Get(string.Format("Tweet-{0}", counter)) != null)
                    {
                        HttpContext.Current.Cache.Remove(string.Format("Tweet-{0}", counter));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Getting the relative time display format
        /// </summary>
        /// <param name="pastTime"></param>
        /// <returns></returns>
        private string relativeTime(string pastTime)
        {
            DateTime origStamp = DateTime.Parse(pastTime.ToString());
            DateTime curDate = DateTime.Now;

            TimeSpan ts = curDate.Subtract(origStamp);
            string strReturn = string.Empty;

            if (ts.Days > 365)               //years
            {
                if (ts.Days == 365)
                    strReturn = "about " + 1 + " year ago";
                else
                    strReturn = "about " + ts.Days / 365 + " years ago";
            }
            else if (ts.Days >= 30)         //months
            {
                if (ts.Days == 30)
                    strReturn = "about " + 1 + " month ago";
                else
                    strReturn = "about " + ts.Days / 30 + " months ago";
            }
            else if (ts.Days >= 7)           //weeks
            {
                if (ts.Days == 7)
                    strReturn = "about " + 1 + " week ago";
                else
                    strReturn = "about " + ts.Days / 7 + " weeks ago";
            }
            else if (ts.Days > 0)          //days
            {
                strReturn = "about " + ts.Days + " days ago";
            }
            else if (ts.Hours >= 1)          //hours
            {
                strReturn = "about " + ts.Hours + " hours ago";
            }
            else
            {
                if (ts.Minutes >= 1)
                {
                    strReturn = "about " + ts.Minutes + " minutes ago";
                }
                else
                    strReturn = "about " + ts.Seconds + " seconds ago";
            }
            return strReturn;
        }

        /// <summary>
        /// Creates the hidden field for keeping the page info
        /// </summary>
        private void CreateHiddenField()
        {
            //Create the hidden control and update the value accordingly
            objPageCount = new HiddenField();
            if (string.IsNullOrEmpty(PageCountValue))
                objPageCount.Value = "1";
            else
                objPageCount.Value = PageCountValue;

            this.Controls.Add(objPageCount);


        }

        /// <summary>
        /// Registering the javascript for the next tweet buttom click event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            string scriptHideImageonLoad = string.Empty;

            if (!string.IsNullOrEmpty(this.ConsumerKey) &&
                     !string.IsNullOrEmpty(this.ScreenName) &&
                     !string.IsNullOrEmpty(this.ConsumerSecret) &&
                     !string.IsNullOrEmpty(this.AccessToken) &&
                     !string.IsNullOrEmpty(this.AccessTokenSecret))
            {
                if (objPageCount == null)
                {
                    CreateHiddenField();
                }


                //Update the hidden control Id to the viewstate
                ViewState["objPageCountId"] = objPageCount.ClientID;

                scriptHideImageonLoad = @"<script language='javascript' type='text/javascript'>
                                                    function HideImage(id)
                                                     {
                                                        document.getElementById('" + objPageCount.ClientID + @"').value = id;
                                                     }
                                                    </script>";

                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "scriptHideImageonLoad", scriptHideImageonLoad);
                imgMoreTweet.OnClientClick = "javascript:HideImage('" + Convert.ToString(Convert.ToInt32(objPageCount.Value) + 1) + "');";
            }
            base.OnPreRender(e);
        }

    }
}
