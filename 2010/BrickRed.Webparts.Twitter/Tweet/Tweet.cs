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
    public class Tweet : Microsoft.SharePoint.WebPartPages.WebPart
    {
        Label LblMessage;
        TextBox textTweet;
        Label lblTweets;

        #region Webpart Properties
        [WebBrowsable(true),
       Category("Twitter Settings"),
       Personalizable(PersonalizationScope.User),
       DefaultValue("TZtFUsssMnP5i55w588ig"),
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
       DefaultValue("MxqUfcj9gKClzmAneybEBCBw397X6Nz93iVnxWLqc"),
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
       DefaultValue("174638173-RPDdivpkNU5YuEvUJ6mjD9uOClk5ahydXHP3HvQ4"),
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
       DefaultValue("RxiFxqfx0Niix7qPV7b6w7V2p3LNeEJveSk31U18"),
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
            mainTable.CellPadding = 5;

            mainTable.CssClass = "ms-viewlsts";
            this.Controls.Add(mainTable);

            tr = new TableRow();
            mainTable.Rows.Add(tr);
            tc = new TableCell();
            tc.ColumnSpan = 2;
            tr.Cells.Add(tc);
            textTweet = new TextBox();
            textTweet.TextMode = TextBoxMode.MultiLine;
            textTweet.MaxLength = 140;
            textTweet.Width = Unit.Percentage(100);
            textTweet.Height = Unit.Pixel(100);
            tc.Controls.Add(textTweet);

            tr = new TableRow();
            mainTable.Rows.Add(tr);

            tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Right;
            buttonTweet = new ImageButton();
            buttonTweet.ImageUrl = "/_layouts/Images/BrickRed/TweetButton.png";
            buttonTweet.Click += new ImageClickEventHandler(buttonTweet_Click);
            tc.Controls.Add(buttonTweet);
            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.HorizontalAlign = HorizontalAlign.Center;
            tc.VerticalAlign = VerticalAlign.Middle;
            tc.Wrap = false;
            tc.Style.Add("background-image", "/_layouts/Images/BrickRed/TweetCount.png");
            tc.Style.Add("background-repeat", "no-repeat");
            tc.Style.Add("width", "30px! important");
            lblTweets = new Label();
            lblTweets.Text = "0000";
            tc.Controls.Add(lblTweets);
            tr.Cells.Add(tc);
        }

        protected override void OnPreRender(EventArgs e)
        {

            try
            {
                textTweet.Text = "";
                if (this.EnableShowUserName)
                    textTweet.Text = SPContext.Current.Web.CurrentUser.Name + " : ";


                OAuthTokens tokens = new OAuthTokens();
                tokens.ConsumerKey = this.ConsumerKey;
                tokens.ConsumerSecret = this.ConsumerSecret;
                tokens.AccessToken = this.AccessToken;
                tokens.AccessTokenSecret = this.AccessTokenSecret;
                TwitterResponse<TwitterStatusCollection>  userTimeline = TwitterTimeline.UserTimeline(tokens);
                lblTweets.Text = userTimeline.ResponseObject.Count.ToString();

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
    }
}
