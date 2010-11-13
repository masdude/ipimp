using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using uWiMP.TVServer.MPWebServices;
using uWiMP.TVServer.MPWebServices.Classes;
using System.IO;


public partial class Streamer : System.Web.UI.Page
{
    int usedCard = -1;
    int usedChannel = -1;
    string tvServerUsername = "";

    int bufferSize = 524288;

    protected void Page_Load(object sender, EventArgs e)
    {
        string filename = "";

        List<EncoderConfig> cfgs = Utils.LoadConfig();
        EncoderConfig cfg = cfgs[Int32.Parse(Request.QueryString["idProfile"])];

        ServiceInterface server = new ServiceInterface();
        if (Request.QueryString["idChannel"] != null)
        {
            if (server.GetChannel(Int32.Parse(Request.QueryString["idChannel"])).isRadio)
                bufferSize = 2560;
            WebTvResult res = server.StartTimeShifting(Int32.Parse(Request.QueryString["idChannel"]));
            if (res.result != 0)
            {
                Utils.Log("Streamer.aspx: ERROR: StartTimeShifting failed with error: " + res.result.ToString());
                Response.Output.WriteLine("ERROR: StartTimeShifting failed with error: " + res.result.ToString());
                Response.End();
                return;
            }
            usedCard = res.user.idCard;
            usedChannel = res.user.idChannel;
            tvServerUsername = res.user.name;
            if (cfg.inputMethod == TransportMethod.Filename)
                filename = res.rtspURL;
            else
                filename = res.timeshiftFile;
        }
        else if (Request.QueryString["idRecording"] != null)
        {
            WebRecording rec = server.GetRecording(Int32.Parse(Request.QueryString["idRecording"]));
            filename = rec.fileName;
        }
        else if (Request.QueryString["idMovie"] != null)
        {
            WebMovie movie = server.GetMovie(Int32.Parse(Request.QueryString["idMovie"]));
            filename = movie.file;
        }
        else if (Request.QueryString["idMusicTrack"] != null)
        {
            WebMusicTrack track = server.GetMusicTrack(Int32.Parse(Request.QueryString["idMusicTrack"]));
            filename = track.file;
        }
        else if (Request.QueryString["idTvSeries"] != null)
        {
            WebSeries series = server.GetTvSeries(Request.QueryString["idTvSeries"]);
            filename = series.filename;
        }
        else if (Request.QueryString["idMovingPicture"] != null)
        {
            WebMovingPicture pic = server.GetMovingPicture(Int32.Parse(Request.QueryString["idMovingPicture"]));
            filename = pic.filename;
        }
        if (!File.Exists(filename) && !filename.StartsWith("rtsp://"))
        {
            Utils.Log("Streamer.aspx: Requested file " + filename + " does not exist.");
            return;
        }
        Response.Clear();
        Response.Buffer = false;
        Response.BufferOutput = false;
        Response.AppendHeader("Connection", "Keep-Alive");
        Response.ContentType = "video/x-msvideo";
        Response.StatusCode = 200;

        Stream mediaStream = null;
        EncoderWrapper encoder = null;
        Stream outStream = null;

        if (cfg.inputMethod != TransportMethod.Filename)
        {
            if (Request.QueryString["idChannel"] != null)
            {
                mediaStream = new TsBuffer(filename);
            }
            else
                mediaStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            encoder = new EncoderWrapper(mediaStream, cfg);
        }
        else
            encoder = new EncoderWrapper(filename, cfg);

        if (cfg.useTranscoding)
            outStream = encoder;
        else
            outStream = mediaStream;

        byte[] buffer = new byte[bufferSize];
        int read;
        try
        {
            while ((read = outStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                try
                {
                    Response.OutputStream.Write(buffer, 0, read);
                }
                catch (Exception)
                {
                    break; // stream is closed
                }
                if (Request.QueryString["idChannel"] != null)
                    server.SendHeartBeat(usedChannel, usedCard, tvServerUsername);
            }
        }
        catch (Exception ex)
        {
            Utils.Log("Streamer.aspx: Exception raised=" + ex.Message + Environment.NewLine + ex.StackTrace);
        }
        if (mediaStream != null)
            mediaStream.Close();
        if (Request.QueryString["idChannel"] != null)
            server.StopTimeShifting(usedChannel, usedCard, tvServerUsername);
        encoder.StopProcess();
        Response.End();
    }
}

