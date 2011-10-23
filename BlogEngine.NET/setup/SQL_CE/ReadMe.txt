Running BlogEngine.NET using SQL CE 4.0

If you wish to use SQL CE to store all your blog data, this folder has all the information you'll 
likely need.


ASP.NET Runtime Requirement for SQL CE 4.0
-----------------------------------------------------------
ASP.NET 4.0
- or -
ASP.NET 3.5 with Full Trust
- or -
ASP.NET 3.5 Medium Trust if web hoster has made configuration changes to allow SQL CE 4.0 to run.


SQL CE 4.0 .SDF File
-----------------------------------------------------------
The SQL CE .SDF file was created under the SQL CE version 4.0.8435.1 engine.  Although version 
4.0.8435.1 is a CTP version of SQL CE 4.0, this .SDF file should work with the final release of 
SQL CE 4.0.  If there are any compability issues, we will update the .SDF file to work with the 
final release.  The BlogEngine.sql file is for reference, but not needed to run SQL CE with 
BlogEngine.NET.  Only the .SDF file is needed.

The .SDF file is already configured with the necessary tables and initial data setup.


Sample Web.config files
-----------------------------------------------------------
There are 2 sample web.config files in this folder.
 (1) SQL_CE_for_ASP.NET_4.0_Web.Config
 (2) SQL_CE_for_ASP.NET_3.5_Web.Config

If your website runs in a ASP.NET 3.5 environment, please review the "ASP.NET Runtime 
Requirement" above.


Instructions for New Setup
-----------------------------------------------------------
1. When running BlogEngine.NET under SQL CE 4.0, you will either need SQL CE 4.0 installed on 
your computer, or if deploying to a webhost, you can simply copy the SQL CE 4.0 binary (DLL) 
files to your /BIN directory.  The DLL files will be located in the installation folder for SQL 
CE 4.0.  The installation folder is located at:

%ProgramFiles(x86)%\Microsoft SQL Server Compact Edition\v4.0\Private

If you do not have SQL CE 4.0 installed on your computer, or you need the DLL files, the latest 
CTP version of SQL CE 4.0 can be downloaded at:
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=0d2357ea-324f-46fd-88fc-7364c80e4fdb

However, please check for the latest version of SQL CE 4.0 before downloading the one at the 
above link.

2. Copy the following files/folders to your /BIN directory.  The files are located in the SQL CE 
4.0 installation folder:

%ProgramFiles(x86)%\Microsoft SQL Server Compact Edition\v4.0\Private

 (a) Copy the file "System.Data.SqlServerCe.dll" into your /BIN directory.
 (b) There are two sub-folders: AMD64 and X86.  Copy both folders to your /BIN directory.

After copying these files and folders, your /BIN directory will look like this:

/bin
   System.Data.SqlServerCe.dll
/bin/x86
   sqlceca40.dll 
   sqlcecompact40.dll 
   sqlceer40EN.dll 
   sqlceme40.dll 
   sqlceqp40.dll 
   sqlcese40.dll 
/bin/amd64 
   sqlceca40.dll 
   sqlcecompact40.dll 
   sqlceer40EN.dll 
   sqlceme40.dll 
   sqlceqp40.dll 
   sqlcese40.dll 

3. Rename the correct sample web.config file above to web.config and copy it to your blog's root 
folder.  (This will overwrite your existing web.config file.  If this is not a new installation, 
make sure you have a backup.)
4. Copy the BlogEngine.sdf file into the App_Data folder.
5. Surf out to your Blog and see the welcome post.
6. Login with the username admin and password admin.  Change the password.


Troubleshooting
-----------------------------------------------------------
If you use one of the sample web.config files, are running your site on your own machine or a 
server that SQL CE 4.0 is already installed on, and you receive the following error message 
when starting the site:

"Failed to find or load the registered .Net Framework Data Provider."

In this scenario, you may need to remove the <system.data> section out of the web.config file.


