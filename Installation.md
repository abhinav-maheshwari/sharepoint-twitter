## Prerequisites ##
  1. WSS 3.0 / MOSS 2007 Environment
> > OR
  1. SharePoint Foundation 2010 / SharePoint 2010 Environment
## Step 1. WSP Installation ##
  1. [Download](http://code.google.com/p/sharepoint-twitter/downloads/list) WSP for 2007 or WSP for 2010 and unzip the Twitter archive.

> This contains the WSP file, deploy solution batch file and retract solution batch file
  1. Change URL property from "<<Server URL>>" to your server URL in both the deploy and retract solution files
  1. Now run deploy solution file <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/install-solution.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/install-solution.png)
  1. This will add the solution in solutions gallery <br /> http://sharepoint-twitter.googlecode.com/svn/wiki/Images/solution-properties.PNG
  1. This will also deploy the feature in site collection features gallery on specified web portal url<br /> http://sharepoint-twitter.googlecode.com/svn/wiki/Images/write-features-gallery.PNG


# Next Step : Twitter Configuration #

You can [register](ApplicationRegistrationTwitter.md) application on Twitter with our step by step registration guide.