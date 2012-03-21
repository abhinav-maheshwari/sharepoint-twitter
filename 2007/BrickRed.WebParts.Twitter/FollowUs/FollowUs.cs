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
using System.Web.UI.HtmlControls;

namespace BrickRed.WebParts.Twitter
{
    [Guid("185d7e70-8216-4b14-9b88-f5584f25c282")]
    public class FollowUs : System.Web.UI.WebControls.WebParts.WebPart
    {
        public FollowUs()
        {
        }

        [WebBrowsable(true),
        Category("Twitter Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Screen Name"),
        WebDescription("Please enter the screen name")]
        public string ScreenName { get; set; }

        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(this.ScreenName))
            {
                HtmlAnchor ancFollowUs = new HtmlAnchor();
                ancFollowUs.HRef = "https://twitter.com/" + this.ScreenName.Trim() ;
                ancFollowUs.Attributes.Add("class", "twitter-follow-button");
                ancFollowUs.Attributes.Add("data-show-count", "true");
                ancFollowUs.InnerText = "Follow@ " + this.ScreenName.Trim();

                this.Controls.Add(ancFollowUs);
            }
            else
            {
                Label lblNoSettings = new Label();
                lblNoSettings.Text = "Twitter webpart properties missing. Please update twitter settings from property pane.";
                this.Controls.Add(lblNoSettings);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            string scriptFollowUs = @"<script>!function(d,s,id){var js,fjs=d.getElementsByTagName(s)[0];if(!d.getElementById(id)){js=d.createElement(s);js.id=id;js.src='//platform.twitter.com/widgets.js';fjs.parentNode.insertBefore(js,fjs);}}(document,'script','twitter-wjs');</script>";

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "scriptFollowUs", scriptFollowUs);
        }
    }
}
