Find the IP Address of your PC running the iPiMP web application and remember the TCP port you selected in the installer, then launch Safari on your iPhone and navigate to http://<i>IPaddress</i>:port, for example http://192.168.1.2:80, if you left the TCP port blank in the installer you can drop the :80 and just go with http://<i>IPaddress</i>

You should be presented with the iPiMP logon page:

http://www.vanderboon.co.uk/ipimp/IMG_0324.PNG

There is an option to remember your login, if you select this your login details will be remembered for the timeout period selected in the setup program.

By default two users are created ‘admin’ and ‘demo’, the passwords for these are the same as the usernames. The first thing you’ll want to do is change the admin password.  So, Logon with the admin user account and follow the prompts:

**Administration -> Change Password**

http://www.vanderboon.co.uk/ipimp/IMG_0325.PNG http://www.vanderboon.co.uk/ipimp/IMG_0326.PNG

To use iPiMP to remotely control your MediaPortal client it has to be registered with iPiMP, this should happen automatically when your MediaPortal client starts up with the client plugin loaded. This will work on IPv4 networks only. If your client isn't shown then you have to add it into the web application client database manually, so whilst logged on as admin you can add them in:

**Administration -> MediaPortal clients -> Add**

You are then prompted for three fields, a friendly name which is used in the iPiMP web application, a hostname or IP address of the MediaPortal client you want to remotely control, this is used to communicate between the iPiMP web server PC and the MediaPortal client, lastly there is an optional field for the TCP port if you need to change it from its default of 55667.

You can find instructions on where to find these details on the [Client Details](Client_Details.md) page.

http://www.vanderboon.co.uk/ipimp/IMG_0327.PNG http://www.vanderboon.co.uk/ipimp/IMG_0328.PNG

That is enough information to get you running with the iPiMP application and remote control.

[Next](Navigation_Basics.md)