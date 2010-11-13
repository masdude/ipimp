using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using uWiMP.TVServer.MPWebServices;

namespace uWiMP.TVServer.MPWebServices
{
  public class EncoderConfig
  {
    public string displayName;
    public bool useTranscoding;
    public string fileName;
    public string args;
    public TransportMethod inputMethod;
    public TransportMethod outputMethod;

    public EncoderConfig(string displayName,bool useTranscoding, string fileName, string args, TransportMethod inputMethod, TransportMethod outputMethod)
    {
      this.displayName = displayName;
      this.useTranscoding = useTranscoding;
      this.fileName = fileName;
      this.args = args;
      this.inputMethod = inputMethod;
      this.outputMethod = outputMethod;
    }
  }
}
