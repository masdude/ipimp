Here are the steps required to get iPiMP running in an existing Apache installation, you can skip this page if you used a **Simple Installation** option.  It assumes you have selected the **Advanced Installation** option and have selected at least options 2 (Apache mod\_aspdotnet) and 3 (iPiMP web files).

http://www.vanderboon.co.uk/ipimp/ExistingApache-002.PNG

  1. Download this [iPiMPinclude.conf](http://ipimp.googlecode.com/files/iPiMPinclude.conf) file and save it as C:\Program Files\iPiMP\Apache\Conf\iPiMPinclude.conf<br><br>
<ol><li>Edit this file as follows:<br>
<ul><li>Change ##PORT## to a port of your choosing (Lines 1, 2 & 3)<br>
</li><li>If required, change the paths on lines 4, 8, 14, 15, 16, 17, 19, 26, 32, 38 & 39<br><br>
</li></ul></li><li>Open your C:\Program Files\iPiMP\Aspx\web.config and add a valid value in the timeout attributes on lines 30 & 45.  These values are explained in the <a href='TipsAndTricks.md'>TipsAndTricks</a> section.<br><br>
</li><li>Add the following line to the end of your existing Apache httpd.conf file:<br>
<pre><code>Include "C:/Program Files/iPiMP/apache/Conf/iPiMPinclude.conf"<br>
</code></pre>
</li><li>Restart your Apache daemon.<br><br></li></ol>


These instructions are manual at the moment, this functionality will be added to the Advanced Install option in a future release.<br>
<br>
<a href='Usage.md'>Next</a>