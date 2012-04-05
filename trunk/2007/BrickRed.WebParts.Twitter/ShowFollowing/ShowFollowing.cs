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
using System.Web.UI.HtmlControls;
using Twitterizer;
using System.ComponentModel;
using System.Drawing;
using System.Web;

namespace BrickRed.WebParts.Twitter
{
    [Guid("66b5cb21-d9ea-43c1-b486-9eb0971b4425")]
    public class ShowFollowing : System.Web.UI.WebControls.WebParts.WebPart
    {
        #region Declarations

        Table mainTable;

        #endregion

        #region WebPart Properties

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

        private int _usersColumnCount = 5;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Number of users in a row"),
        WebDescription("Please enter the number of columns in a row")]
        public int UsersColumnCount
        {
            get { return _usersColumnCount; }
            set { _usersColumnCount = value; }
        }

        private int _usersRowCount = 2;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Number of rows"),
        WebDescription("Please enter the number of rows")]
        public int UsersRowCount
        {
            get { return _usersRowCount; }
            set { _usersRowCount = value; }
        }

        private bool _showFollowingScreenName = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Show friend's screen name"),
        WebDescription("Would you like to show the friend's screen name")]
        public bool ShowFollowingScreenName
        {
            get { return _showFollowingScreenName; }
            set { _showFollowingScreenName = value; }
        }

        private bool _showImageAsLink = true;
        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Show friend's image as link"),
        WebDescription("Would you like to show the friend's screen name")]
        public bool ShowImageAsLink
        {
            get { return _showImageAsLink; }
            set { _showImageAsLink = value; }
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

        protected override void CreateChildControls()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.AccessTokenSecret) &&
                    !string.IsNullOrEmpty(this.AccessToken) &&
                    !string.IsNullOrEmpty(this.ConsumerSecret) &&
                    !string.IsNullOrEmpty(this.ConsumerKey) &&
                    !string.IsNullOrEmpty(Convert.ToString(this.UsersColumnCount)) &&
                    !string.IsNullOrEmpty(this.ScreenName))
                {
                    // Get the Twitter response for the Following and the User

                    TwitterResponse<TwitterUserCollection> twitterUsers = GetTwitterFriends();
                    TwitterResponse<TwitterStatusCollection> twitterStatus = GetTwitterTimeLine();

                    //creating WebPart structure
                    TableRow tr;
                    TableCell tc;

                    mainTable = new Table();
                    mainTable.CellPadding = 0;
                    mainTable.CellSpacing = 0;
                    mainTable.Width = Unit.Percentage(100);

                    //Create the header
                    if (this.ShowHeader)
                    {
                        tr = new TableRow();
                        tc = new TableCell();
                        tc.Controls.Add(Common.CreateHeaderFooter("Header", twitterStatus.ResponseObject, this.ShowHeaderImage, this.ShowFollowUs));
                        tr.Cells.Add(tc);
                        mainTable.Rows.Add(tr);
                    }

                    //Create the Count display section
                    if (this.ShowHeader && twitterUsers.ResponseObject.Count > 0)
                    {
                        tr = new TableRow();
                        tc = new TableCell();
                        tc.Controls.Add(Common.ShowDisplayCount("Following", twitterUsers, twitterStatus.ResponseObject));
                        tr.Cells.Add(tc);
                        mainTable.Rows.Add(tr);
                    }

                    //Contents
                    tr = new TableRow();
                    tc = new TableCell();
                    tc.Controls.Add(GetFollowing(twitterUsers, twitterStatus));
                    tr.Cells.Add(tc);
                    mainTable.Rows.Add(tr);
                    this.Controls.Add(mainTable);

                    //Create Footer
                    if (this.ShowFooter)
                    {
                        tr = new TableRow();
                        tc = new TableCell();
                        tc.Controls.Add(Common.CreateHeaderFooter("Footer", twitterStatus.ResponseObject, this.ShowHeaderImage, this.ShowFollowUs));
                        tr.Cells.Add(tc);
                        mainTable.Rows.Add(tr);
                    }
                }
                else
                {
                    Label lblNoSettings = new Label();
                    lblNoSettings.Text = "Twitter webpart properties missing. Please update twitter settings from property pane.";
                    this.Controls.Add(lblNoSettings);
                }
            }
            catch (Exception ex)
            {
                Label LblMessage = new Label();
                LblMessage.Text = ex.Message;
                this.Controls.Add(LblMessage);
            }

        }

        /// <summary>
        /// Get the following users
        /// </summary>
        /// <returns></returns>
        private Table GetFollowing(TwitterResponse<TwitterUserCollection> twitterUsers, TwitterResponse<TwitterStatusCollection> twitterStatus)
        {
            Table insideTable;
            TableRow tr = null;
            TableCell tc;

            insideTable = new Table();
            insideTable.CellPadding = 0;
            insideTable.CellSpacing = 0;
            insideTable.Width = Unit.Percentage(100);

            int r = 1;
            tr = new TableRow();

            if (twitterUsers.ResponseObject.Count > 0)
            {
                //Get the total number of followers
                int followersCount = Convert.ToInt32(twitterUsers.ResponseObject.Count);
                int c = 0;

                foreach (TwitterUser followingUsers in twitterUsers.ResponseObject)
                {
                    //Create a new row if Usercount limit exceeds
                    if (this.UsersColumnCount == c)
                    {
                        if (r < this.UsersRowCount)
                        {
                            tr = new TableRow();
                            r++;
                            c = 0;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Create a new cell
                    tc = new TableCell();
                    tc.Attributes.Add("valign", "top");

                    //create a new table in a cell
                    Table tb = new Table();
                    tb.Width = Unit.Percentage(100);

                    //Show Friend Image
                    HtmlImage imgFollower = new HtmlImage();
                    imgFollower.Src = followingUsers.ProfileImageLocation.ToString();
                    imgFollower.Border = 0;

                    TableRow tr1 = new TableRow();
                    TableCell tc1 = new TableCell();
                    tc1.CssClass = "alignCenter";


                    if (this.ShowImageAsLink)
                    {
                        HyperLink lnkFollower = new HyperLink();
                        lnkFollower.NavigateUrl = "http://twitter.com/" + followingUsers.ScreenName;
                        lnkFollower.Attributes.Add("target", "_blank");
                        lnkFollower.Controls.Add(imgFollower);
                        lnkFollower.ToolTip = followingUsers.Name;
                        tc1.Controls.Add(lnkFollower);
                        tc1.VerticalAlign = VerticalAlign.Top;
                        tc1.Width = Unit.Percentage(100 / this.UsersColumnCount);
                    }
                    else
                    {
                        tc1.Controls.Add(imgFollower);
                    }

                    tr1.Controls.Add(tc1);
                    tb.Rows.Add(tr1);

                    //Show Follower Name
                    if (this.ShowFollowingScreenName)
                    {
                        Label lblFollower = new Label();

                        if (followingUsers.Name.IndexOf(" ") != -1)
                            lblFollower.Text = followingUsers.Name.Substring(0, followingUsers.Name.IndexOf(" "));      //Get the first name only to display
                        else
                            lblFollower.Text = followingUsers.Name;

                        lblFollower.Font.Size = FontUnit.XXSmall;
                        TableRow tr2 = new TableRow();
                        TableCell tc2 = new TableCell();
                        tc2.CssClass = "alignCenter";
                        tc2.Width = Unit.Percentage(100 / this.UsersColumnCount);
                        tc2.Controls.Add(lblFollower);
                        tr2.Controls.Add(tc2);
                        tb.Rows.Add(tr2);
                    }

                    tc.Controls.Add(tb);
                    tr.Cells.Add(tc);
                    insideTable.Rows.Add(tr);
                    c++;
                }
            }
            else
            {
                // If there are no Friends

                insideTable = new Table();
                tr = new TableRow();
                tc = new TableCell();
                insideTable.Width = Unit.Percentage(100);
                insideTable.CellPadding = 5;

                //display grey tweet image
                HtmlImage imgGreyTweet = new HtmlImage();
                imgGreyTweet.Src = SPContext.Current.Web.Url + "/_layouts/Brickred.OpenSource.Twitter/Greytweet.png";
                imgGreyTweet.Border = 0;
                tc.Controls.Add(imgGreyTweet);
                tc.CssClass = "alignCenter";
                tc.VerticalAlign = VerticalAlign.Middle;
                tr.Cells.Add(tc);
                insideTable.Rows.Add(tr);

                //display message
                tr = new TableRow();
                tc = new TableCell();
                Label lblScreenName = new Label();
                lblScreenName.Text = "@" + twitterStatus.ResponseObject[0].User.Name;
                lblScreenName.Font.Size = FontUnit.Large;
                lblScreenName.ForeColor = Color.Gray;
                Label lblMessage = new Label();
                lblMessage.Text = " is not following anyone yet.";
                lblMessage.ForeColor = Color.Gray;
                lblScreenName.ForeColor = Color.Gray;
                tc.Controls.Add(lblScreenName);
                tc.Controls.Add(lblMessage);
                tc.CssClass = "alignCenter";
                tr.Cells.Add(tc);
                insideTable.Rows.Add(tr);
            }

            return insideTable;
        }

        /// <summary>
        /// Get the Twitter response object for the friends
        /// </summary>
        /// <returns></returns>
        private TwitterResponse<TwitterUserCollection> GetTwitterFriends()
        {
            TwitterResponse<TwitterUserCollection> twitterResponse = new TwitterResponse<TwitterUserCollection>();

            if (Page.Cache[string.Format("TwitterFriends-{0}", this.ScreenName)] == null)
        {
            //create a authorization token of the user
            OAuthTokens tokens = new OAuthTokens();
            tokens.ConsumerKey = this.ConsumerKey;
            tokens.ConsumerSecret = this.ConsumerSecret;
            tokens.AccessToken = this.AccessToken;
            tokens.AccessTokenSecret = this.AccessTokenSecret;

            //Set the query options
            FriendsOptions Friendoptions = new FriendsOptions();
            Friendoptions.ScreenName = this.ScreenName;
            Friendoptions.Cursor = -1;

            //get the Following Object from the Twitter
            twitterResponse = TwitterFriendship.Friends(tokens, Friendoptions);
                HttpContext.Current.Cache.Insert(string.Format("TwitterFriends-{0}", this.ScreenName), twitterResponse, null, DateTime.Now.AddMinutes(Common.CACHEDURATION), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
            }
            else
            {
                twitterResponse = Page.Cache[string.Format("TwitterFriends-{0}", this.ScreenName)] as TwitterResponse<TwitterUserCollection>;
            }

            return twitterResponse;
        }

        /// <summary>
        /// Get the Twitter response object for the tweets
        /// </summary>
        /// <returns></returns>
        private TwitterResponse<TwitterStatusCollection> GetTwitterTimeLine()
        {
            TwitterResponse<TwitterStatusCollection> userInfo = new TwitterResponse<TwitterStatusCollection>();
            if (Page.Cache[string.Format("TwitterTimeLine-{0}", this.ScreenName)] == null)
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
                Useroptions.Count = 1;
            Useroptions.Page = 1;

            //Get the account info
            userInfo = TwitterTimeline.UserTimeline(tokens, Useroptions);
                HttpContext.Current.Cache.Insert(string.Format("TwitterTimeLine-{0}", this.ScreenName), userInfo, null, DateTime.Now.AddMinutes(Common.CACHEDURATION), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
            }
            else
            {
                userInfo = Page.Cache[string.Format("TwitterTimeLine-{0}", this.ScreenName)] as TwitterResponse<TwitterStatusCollection>;
            }

            return userInfo;
        }

        /// <summary>
        /// For registering the css
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            //Get the Css Class
            this.Page.Header.Controls.Add(StyleSheet.CssStyle());

            base.OnPreRender(e);
        }
    }
}
