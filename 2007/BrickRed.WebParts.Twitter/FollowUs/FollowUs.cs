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
            if (!string.IsNullOrEmpty(this.ScreenName.Trim()))
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
