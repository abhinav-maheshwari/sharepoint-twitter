# <font color='red'> Please visit <a href='Installation.md'>here</a> for latest installation guide </font> #


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

## Step 2. Register your Twitter Application ##
  1. Go to http://dev.twitter.com/start
  1. Now you need to register your application <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/register-application.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/register-application.png)
  1. Login with your existing credentials or Register first to get login credentials <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/login.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/login.png)
  1. Once you are done with the application registration process you will be redirected to application settings page. From this page we will get the Consumer Key and Consumer Secret <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/application-settings.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/application-settings.png)
  1. Next you need to fetch the Access Token and Access Token Secret from "My Access Token" link provide in the right navigation pane of application settings page <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/access-tokens.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/access-tokens.png)

## Step 3. Configuration of the Tweet webpart ##

  1. Now you need to add the web part on the page from Web Part gallery <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-pane.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-pane.png)
  1. Once added, Configure screen name , Consumer Key , Consumer Secret, Access Token, Access Token Secret from the saved information in step 2 and also configure the tweet count  and whether to show user image and description <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-properties.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-properties.png)
  1. Once configured you will be able to see the tweets of this user <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-demo.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-demo.png)

## Step 4. Configuration of the Show Tweets webpart ##
  1. Please note the code from previous step. Now you need to add the web part on the page from Web Part gallery <br /> http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-write-pane.PNG
  1. Once added, Configure following properties <br /> http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-write-properties.PNG
  1. Once configured you will be able to tweet for this user <br /> ![http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-write-demo.png](http://sharepoint-twitter.googlecode.com/svn/wiki/Images/webpart-write-demo.png)


