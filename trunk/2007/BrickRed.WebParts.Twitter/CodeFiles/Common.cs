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
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Twitterizer;
using System.Web.UI;
using System.IO;

namespace BrickRed.WebParts.Twitter
{
    static class Common
    {
        public static int CACHEDURATION = 15;

        /// <summary>
        /// Creates the header and footer
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="userInfo"></param>
        /// <param name="ShowHeaderImage"></param>
        /// <param name="ShowFollowUs"></param>
        /// <returns></returns>
        public static Table CreateHeaderFooter(string Type, TwitterStatusCollection tweets, bool ShowHeaderImage, bool ShowFollowUs)
        {
            Table tbHF;
            TableRow trHF;
            TableCell tcHF;
            tbHF = new Table();

            if (!ShowHeaderImage)
            {
                tbHF.CellSpacing = 0;
                tbHF.CellPadding = 4;
            }
            else
            {
                tbHF.CellPadding = 0;
                tbHF.CellSpacing = 0;
            }

            tbHF.Width = Unit.Percentage(100);
            trHF = new TableRow();
            tcHF = new TableCell();

            #region Header
            if (Type.Equals("Header"))
            {
                Table tbinner = new Table();
                tbinner.Width = Unit.Percentage(100);
                TableRow trinner = new TableRow();
                TableCell tcinner = new TableCell();

                //Adding the Header Image
                if (ShowHeaderImage)
                {
                    HtmlImage image = new HtmlImage();
                    image.Src = tweets[0].User.ProfileImageLocation;
                    image.Height = 22;
                    image.Width = 35;
                    image.Border = 0;
                    HyperLink hplnkImage = new HyperLink();
                    hplnkImage.NavigateUrl = "http://twitter.com/" + tweets[0].User.ScreenName;
                    hplnkImage.Attributes.Add("target", "_blank");
                    hplnkImage.Controls.Add(image);
                    tcinner.Controls.Add(hplnkImage);
                    tcinner.CssClass = "twitHeaderImage";
                    tcinner.Width = Unit.Percentage(4);
                    trinner.Cells.Add(tcinner);
                }

                //Creating the name hyperlink in header
                tcinner = new TableCell();
                HyperLink hplnkName = new HyperLink();
                hplnkName.Text = tweets[0].User.Name;
                hplnkName.NavigateUrl = "http://twitter.com/" + tweets[0].User.ScreenName;
                hplnkName.Attributes.Add("target", "_blank");
                tcinner.Controls.Add(hplnkName);
                tcinner.VerticalAlign = VerticalAlign.Middle;
                tcinner.CssClass = "twitHeaderText";
                trinner.Cells.Add(tcinner);

                tcinner = new TableCell();
                HtmlImage imgHeaderTwitter = new HtmlImage();
                imgHeaderTwitter.Src = SPContext.Current.Web.Url + "/_layouts/Brickred.OpenSource.Twitter/twitterbird.png";
                imgHeaderTwitter.Height = 22;
                imgHeaderTwitter.Border = 0;
                tcinner.Controls.Add(imgHeaderTwitter);
                tcinner.HorizontalAlign = HorizontalAlign.Right;
                if (ShowHeaderImage)
                    tcinner.CssClass = "padding-align-right";
                trinner.Cells.Add(tcinner);

                //Adding controls to the main table
                tbinner.Rows.Add(trinner);
                tcHF.Controls.Add(tbinner);
                tcHF.CssClass = "twitHeaderBorder";
                trHF.Cells.Add(tcHF);
                tbHF.CssClass = "twitHeaderBorder";
            }
            #endregion

            #region Footer
            else if (Type.Equals("Footer"))
            {
                HyperLink hplnk = new HyperLink();
                hplnk.ImageUrl = SPContext.Current.Web.Url + "/_layouts/Brickred.OpenSource.Twitter/twitterlogo.png";
                hplnk.NavigateUrl = "https://twitter.com";
                hplnk.Attributes.Add("target", "_blank");
                tcHF.Controls.Add(hplnk);
                tcHF.CssClass = "twitFooterBorder";
                trHF.Cells.Add(tcHF);

                if (ShowFollowUs)
                {
                    tcHF = new TableCell();
                    HyperLink hplnkJoinus = new HyperLink();
                    hplnkJoinus.Text = "Follow Us";
                    hplnkJoinus.ForeColor = Color.White;
                    hplnkJoinus.NavigateUrl = "https://twitter.com/" + tweets[0].User.ScreenName;
                    hplnkJoinus.Attributes.Add("target", "_blank");
                    tcHF.Controls.Add(hplnkJoinus);
                    tcHF.CssClass = "padding-align-right";
                    trHF.Cells.Add(tcHF);
                    trHF.Cells.Add(tcHF);
                }

                tbHF.CssClass = "twitFooterBorder";
            }
            #endregion


            tbHF.Rows.Add(trHF);
            return tbHF;
        }

        /// <summary>
        /// Displays the Count of the followers/following
        /// </summary>
        /// <param name="Type"> enter the type as Followers/Following</param>
        /// <param name="twitterResponse">Twiiter object from which count can be retrieved</param>
        /// <param name="userInfo">Twiiter object from which user info can be retrieved</param>
        /// <returns></returns>
        public static Table ShowDisplayCount(string Type, TwitterResponse<TwitterUserCollection> twitterResponse, TwitterStatusCollection tweets)
        {
            Table tb = new Table();
            tb.Width = Unit.Percentage(100);
            TableRow tr = new TableRow();
            TableCell tc = new TableCell();

            #region Followers
            if (Type.Equals("Followers"))
            {
                int followersCount = Convert.ToInt32(twitterResponse.ResponseObject.Count);
                Label lblDisplayFollowerCount = new Label();
                lblDisplayFollowerCount.Text = followersCount + " people are following ";

                Label lblScreenName = new Label();
                lblScreenName.Text = "@" + tweets[0].User.Name;
                lblScreenName.Font.Bold = true;
                lblScreenName.Font.Size = FontUnit.XXSmall;
                lblScreenName.ForeColor = Color.Black;

                tc.Controls.Add(lblDisplayFollowerCount);
                tc.Controls.Add(lblScreenName);
            }
            #endregion

            #region Following
            else if (Type.Equals("Following"))
            {
                int followCount = Convert.ToInt32(twitterResponse.ResponseObject.Count);

                Label lblScreenName = new Label();
                lblScreenName.Text = "@" + tweets[0].User.Name;
                lblScreenName.Font.Bold = true;
                lblScreenName.Font.Size = FontUnit.XXSmall;
                lblScreenName.ForeColor = Color.Black;

                Label lblDisplayFollowerCount = new Label();
                lblDisplayFollowerCount.Text = " is following " + followCount + " people";
            }
            #endregion

            tc.CssClass = "twitDisplayCount";
            tr.Cells.Add(tc);
            tb.Rows.Add(tr);

            return tb;
        }

        /// <summary>
        /// To get the HTML Code
        /// </summary>
        /// <param name="cntrl"></param>
        /// <returns></returns>

        public static string GetHtmlCode(Table cntrl)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            //Method2
            //using(StringWriter sw = new StringWriter(CultureInfo.InvariantCulture)) {
            //using(HtmlTextWriter writer = new HtmlTextWriter(sw))
            //    control.RenderControl(writer);
            //sw.WriteLine();
            //return sw.ToString();


            //System.IO.StringWriter sw = new System.IO.StringWriter();
            //HtmlTextWriter hw = new HtmlTextWriter(sw);
            cntrl.RenderControl(hw);
            return sb.ToString();
        }
    }
}
