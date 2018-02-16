Many a times, there will be a need to identify invalid / disabled users from a SharePoint site, which 
essentially means, users may be present in the user groups of Share Point, but they may have moved out 
of the organization and hence may have been disabled / deactivated from the Active Directory (for non-
AD users also, I have a solution.So, don't worry.:). This is especially the case, when there is a migration 
from older versions of a SharePoint to newer versions. There are 3rd party tools out there in the market, 
which can actually do this for you, but most of them are not free. Also, there can be server based 
solutions using Object Model, which can also help to achieve the same results, but the drawback is that, 
they need to be run from within the server. This may not be possible many times. This client based utility
exactly addresses these issues. It is a console application based on .Net APIs and native Share Point Web 
Services. So, all you need to run this utility is .Net framework 3.5 or higher, which essentially will be a part 
of most of the Windows OS. But, please do make sure, you run it with an account which has at least read 
access to the site, from where you want to get this information.

For scenarios, where the users are not in Active Directory, but maintained in a different store like SQL 
Server, I have given a different flavor, which again uses, native SP web services, but without .Net APIs 
for Active Directory.

The utility is flexible enough to be used with both SharePoint 2007 and SharePoint 2010 versions, since it 
relies on native SP web services. The utility exports the invalid users, as a CSV (comma separated) file and it also prints the name(s) and count of the invalid users on the console. It also prints the total user count on the console. The below screen shot demonstrates a sample.

Note: Make sure you are connected to the network to be able to access the site first and LDAP server is 
available, in case you are using AD. You can infact, use the Web service version alone for both AD and 
non-AD scenarios. The reason for using .Net AD APIs for the AD based version, is that, it is slightly faster 
than the flavor using only the web services. However, you can always use the web service flavor of the 
utility in both the scenarios.
