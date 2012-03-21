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
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.ComponentModel;
using Twitterizer;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web;

namespace BrickRed.WebParts.Twitter
{
    [Guid("70edf42d-d03e-4179-a658-f7fc73cb207c")]
    public class Tweet : System.Web.UI.WebControls.WebParts.WebPart
    {
        Label LblMessage;
        TextBox textTweet;
        Label lblTweets;

        #region Webpart Properties
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("Screen Name"),
        WebDescription("Please enter the screen name")]
        public string ScreenName { get; set; }

        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.User),
        WebDisplayName("Consumer Key"),
        WebDescription("Please enter a ConsumerKey")]
        public string ConsumerKeyProperty
        {
            get { return ConsumerKey; }
            set { ConsumerKey = value; }
        }
        public string ConsumerKey;

        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.User),
        WebDisplayName("Consumer Secret"),
        WebDescription("Please enter ConsumerSecret")]

        public string ConsumerSecretProperty
        {
            get { return ConsumerSecret; }
            set { ConsumerSecret = value; }
        }
        public string ConsumerSecret;

        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.User),
        WebDisplayName("Access Token"),
        WebDescription("Please enter AccessToken")]

        public string AccessTokenProperty
        {
            get { return AccessToken; }
            set { AccessToken = value; }
        }
        public string AccessToken;

        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.User),
        WebDisplayName("Access Token Secret"),
        WebDescription("Please enter AccessTokenSecret")]

        public string AccessTokenSecretProperty
        {
            get { return AccessTokenSecret; }
            set { AccessTokenSecret = value; }
        }
        public string AccessTokenSecret;

        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Show User Name"),
        WebDescription("Would you like to show user name")]

        public bool EnableShowUserNameProperty
        {
            get { return EnableShowUserName; }
            set { EnableShowUserName = value; }
        }
        public bool EnableShowUserName;

        private bool _showHeader = false;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(false),
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


        public Tweet()
        {
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Table mainTable;
            TableRow tr;
            TableCell tc;
            ImageButton buttonTweet;

            mainTable = new Table();
            mainTable.Width = Unit.Percentage(100);
            mainTable.CellSpacing = 0;
            mainTable.CellPadding = 0;
            mainTable.CssClass = "ms-viewlsts";

            //Create the header
            if (this.ShowHeader)
            {
                tr = new TableRow();
                tc = new TableCell();
                tc.ColumnSpan = 2;
                tc.Controls.Add(CreateHeaderFooter("Header"));
                tr.Cells.Add(tc);
                mainTable.Rows.Add(tr);
            }

            //adding the text box
            tr = new TableRow();
            mainTable.Rows.Add(tr);
            tc = new TableCell();
            tc.ColumnSpan = 2;
            tr.Cells.Add(tc);

            //Create Div for placing the textbox
            HtmlGenericControl div = new HtmlGenericControl("DIV");
            div.ID = "Tweetdiv";
            tc.Controls.Add(div);
            //Search for the class and adding the css class to it
            HtmlControl control = tc.Controls[0] as HtmlControl;
            control.Attributes["class"] = "txtboxTweetWrapper";

            textTweet = new TextBox();
            textTweet.TextMode = TextBoxMode.MultiLine;
            textTweet.MaxLength = 140;
            textTweet.Width = Unit.Percentage(100);
            textTweet.Height = Unit.Pixel(100);
            div.Controls.Add(textTweet);


            //Adding the Tweet Buttton
            tr = new TableRow();
            mainTable.Rows.Add(tr);

            tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Right;
            tc.Width = Unit.Percentage(94);
            buttonTweet = new ImageButton();
            buttonTweet.ImageUrl = "/_layouts/Images/BrickRed/TweetButton.png";
            buttonTweet.Click += new ImageClickEventHandler(buttonTweet_Click);
            tc.Controls.Add(buttonTweet);
            tr.Cells.Add(tc);

            //Adding the tweet count image
            tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Center;
            tc.Width = Unit.Pixel(57);
            tc.Wrap = false;
            tc.Style.Add("background-image", "/_layouts/Images/BrickRed/TweetCount.png");
            tc.Style.Add("background-repeat", "no-repeat");
            tc.Style.Add("width", "56px !important");
            lblTweets = new Label();
            tc.Controls.Add(lblTweets);
            tr.Cells.Add(tc);

            //Create Footer
            if (this.ShowFooter)
            {
                tr = new TableRow();
                tc = new TableCell();
                tc.ColumnSpan = 2;
                tc.Controls.Add(CreateHeaderFooter("Footer"));
                tr.Cells.Add(tc);
                mainTable.Rows.Add(tr);
            }

            this.Controls.Add(mainTable);

        }

        /// <summary>
        /// Create the Header and footer of the Webpart
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        private Table CreateHeaderFooter(string Type)
        {
            Table tbHF;
            tbHF = new Table();
            tbHF.Width = Unit.Percentage(100);
            tbHF.CellPadding = 0;
            tbHF.CellSpacing = 0;
           

            if (!string.IsNullOrEmpty(this.ScreenName)
                && !string.IsNullOrEmpty(this.ConsumerKey)
                && !string.IsNullOrEmpty(this.ConsumerSecret)
                && !string.IsNullOrEmpty(this.AccessToken)
                && !string.IsNullOrEmpty(this.AccessTokenSecret))
            {
                TwitterResponse<TwitterStatusCollection> userInfo = GetTwitterStatus();

                #region Header
                if (Type.Equals("Header"))
                {
                    tbHF = Common.CreateHeaderFooter("Header", userInfo.ResponseObject, this.ShowHeaderImage, this.ShowFollowUs);
                }
                #endregion

                #region Footer
                if (Type.Equals("Footer"))
                {
                    tbHF = Common.CreateHeaderFooter("Footer", userInfo.ResponseObject, this.ShowHeaderImage, this.ShowFollowUs);
                }
                #endregion
            }
            return tbHF;
        }

        protected override void OnPreRender(EventArgs e)
        {

            try
            {
                TwitterResponse<TwitterStatusCollection> userInfo = GetTwitterStatus();
                textTweet.Text = "";
                if (this.EnableShowUserName)
                    textTweet.Text = SPContext.Current.Web.CurrentUser.Name + " : ";
               
                if (userInfo.ResponseObject.Count < 10000)
                {
                    lblTweets.Text = userInfo.ResponseObject.Count.ToString();
                }
                else
                {
                    lblTweets.Text = "10000+";
                }

            }
            catch (Exception Ex)
            {
                LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        void buttonTweet_Click(object sender, EventArgs e)
        {
            try
            {
                OAuthTokens tokens = new OAuthTokens();
                tokens.ConsumerKey = this.ConsumerKey;
                tokens.ConsumerSecret = this.ConsumerSecret;
                tokens.AccessToken = this.AccessToken;
                tokens.AccessTokenSecret = this.AccessTokenSecret;
                TwitterStatus.Update(tokens, textTweet.Text.Trim());
            }

            catch (Exception Ex)
            {
                Label LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        private TwitterResponse<TwitterStatusCollection> GetTwitterStatus()
        {
            TwitterResponse<TwitterStatusCollection> userInfo = new TwitterResponse<TwitterStatusCollection>();

            //use cache here
            if (Page.Cache[string.Format("TweetWrite-{0}", this.ScreenName)] == null)
            {
                //create a authorization token of the user
                OAuthTokens tokens = new OAuthTokens();
                tokens.ConsumerKey = this.ConsumerKey;
                tokens.ConsumerSecret = this.ConsumerSecret;
                tokens.AccessToken = this.AccessToken;
                tokens.AccessTokenSecret = this.AccessTokenSecret;

                //Set the query options

                UserTimelineOptions Useroptions = new UserTimelineOptions();
                Useroptions.ScreenName = this.ScreenName;

                //Get the account info
                userInfo = TwitterTimeline.UserTimeline(tokens, Useroptions);
                HttpContext.Current.Cache.Insert(string.Format("TweetWrite-{0}", this.ScreenName), userInfo, null, DateTime.Now.AddMinutes(Common.CACHEDURATION), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
            }
            else
            {
                userInfo = Page.Cache[string.Format("TweetWrite-{0}", this.ScreenName)] as TwitterResponse<TwitterStatusCollection>;
            }

            return userInfo;
        }
    }
}
