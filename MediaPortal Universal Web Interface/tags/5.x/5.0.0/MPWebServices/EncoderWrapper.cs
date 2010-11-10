using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.IO.Pipes;

using uWiMP.TVServer.MPWebServices;
using TvLibrary.Log;

namespace uWiMP.TVServer.MPWebServices
{
  public class EncoderWrapper : Stream
  {
    // Transport Pipes
    private TransportStream encoderInput;
    private TransportStream encoderOutput;

    private Process applicationThread;
    private ProcessStartInfo applicationDetails;
    private EncoderConfig encCfg;

    // Media
    private readonly string _filename;
    private readonly Stream _mediaStream;

    public EncoderWrapper(string filename, EncoderConfig encCfg)
    {
      _filename = filename;
      _mediaStream = null;
      this.encCfg = encCfg;
      if (!encCfg.useTranscoding)
        return;
      SetupPipes();
      Log.Info("iPiMPWeb - Pipes setup for filename");
      Start();
      Log.Info("iPiMPWeb - Copy started for filename");
    }

    public EncoderWrapper(Stream mediaStream, EncoderConfig encCfg)
    {
      _filename = "";
      _mediaStream = mediaStream;
      this.encCfg = encCfg;
      if (!encCfg.useTranscoding)
        return;
      SetupPipes();
      Log.Info("iPiMPWeb - Pipes setup for mediaStream");
      Start();
      Log.Info("iPiMPWeb - Copy started for mediaStream");
    }

    private void SetupPipes()
    {
      Log.Info("iPiMPWeb - SetupPipes");
      switch (encCfg.inputMethod)
      {
        case TransportMethod.Filename:
          encoderInput = null;
          break;
        case TransportMethod.NamedPipe:
          encoderInput = new NamedPipe();
          break;
        case TransportMethod.StandardIn:
          encoderInput = new BasicStream();
          break;
        default:
          Log.Info("iPiMPWeb - Invalid inputMethod");
          break;
      }

      switch (encCfg.outputMethod)
      {
        case TransportMethod.NamedPipe:
          encoderOutput = new NamedPipe();
          break;
        case TransportMethod.StandardOut:
          encoderOutput = new BasicStream();
          break;
        case TransportMethod.None:
          break;
        default:
          Log.Info("iPiMPWeb - Invalid outputMethod");
          break;
      }
    }

    protected void Start()
    {
      Log.Info("iPiMPWeb - Start");
      StartPipe();
    }

    private void StartPipe()
    {
        Log.Info("iPiMPWeb - StartPipes");
        if (_mediaStream != null)
        {
            Log.Info("iPiMPWeb - mediaStream isNot Null"); 
            encoderInput.Start(false);
            Log.Info("iPiMPWeb - encoderInput started");
            if (encCfg.outputMethod == TransportMethod.None)
            {
                Log.Info("iPiMPWeb  - call StartProcess STREAM/NONE {0}", encoderInput.Url); 
                StartProcess(encoderInput.Url);
                }
            else
            {
                Log.Info("iPiMPWeb  - call StartProcess STREAM/BOTH {0} {1}", encoderInput.Url, encoderOutput.Url); 
                StartProcess(encoderInput.Url, encoderOutput.Url);
            }
            Log.Info("iPiMPWeb  - CopyStream"); 
            encoderInput.CopyStream(_mediaStream);
        }
        else
        {
            if (encCfg.outputMethod == TransportMethod.None)
            {
                Log.Info("iPiMPWeb  - call StartProcess FILE/NONE {0}", _filename);
                StartProcess(_filename);
            }
            else
            {
                Log.Info("iPiMPWeb  - call StartProcess FILE/BOTH {0} {1}", _filename, encoderOutput.Url);
                StartProcess(_filename, encoderOutput.Url);
            }
        }
        if (encCfg.outputMethod != TransportMethod.None)
        {
            Log.Info("iPiMPWeb - outputMethod is Not None");
            encoderOutput.Start(false);
            Log.Info("iPiMPWeb - encoderInput started");
            // Wait for the output encoder to connect.
            int tries = 10000;

            do
            {
                if (encoderOutput.IsReady)
                    break;

                System.Threading.Thread.Sleep(1);
            } while (--tries != 0);
        }
    }

    public override bool CanRead
    {
      get
      {
        return encoderOutput.CanRead;
      }
    }
    public override bool CanSeek
    {
      get
      {
        return encoderOutput.CanSeek;
      }
    }
    public override bool CanWrite
    {
      get { throw new NotSupportedException(); }
    }
    public override void Flush()
    {
      throw new NotSupportedException();
    }
    public override long Length
    {
      get
      {
        return encoderOutput.Length;
      }
    }
    public override long Position
    {
      get
      {
        return encoderOutput.Position;
      }
      set
      {
        encoderOutput.Position = value;
      }
    }
    public override int Read(byte[] buffer, int offset, int count)
    {
      return encoderOutput.Read(buffer, offset, count);
    }
    public override long Seek(long offset, SeekOrigin origin)
    {
      return encoderOutput.Seek(offset, origin);
    }
    public override void SetLength(long value)
    {
      encoderOutput.SetLength(value);
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public void StartProcess(String input, String output)
    {
      if (applicationThread != null)
        applicationThread.Kill();
      var args = encCfg.args.Replace("{0}",input);
      args=args.Replace("{1}",output);
      
      applicationDetails = new ProcessStartInfo(encCfg.fileName, args)
                               {
                                   UseShellExecute = false,
                                   RedirectStandardInput = (encCfg.inputMethod == TransportMethod.StandardIn),
                                   RedirectStandardOutput = (encCfg.outputMethod == TransportMethod.StandardOut)
                               };

        applicationThread = new Process {StartInfo = applicationDetails};
        if (!applicationThread.Start()) return;
        Log.Info("iPiMPWeb - process pid started {0}", applicationThread.Id.ToString());
        if (encCfg.inputMethod == TransportMethod.StandardIn)
            encoderInput.UnderlyingStreamObject = applicationThread.StandardInput.BaseStream;
        if (encCfg.outputMethod == TransportMethod.StandardOut)
            encoderOutput.UnderlyingStreamObject = applicationThread.StandardOutput.BaseStream;
    }

    public void StartProcess(String input)
    {
        Log.Info("iPiMPWeb - process input {0}", input);
        if (applicationThread != null)
            applicationThread.Kill();
        var args = encCfg.args.Replace("{0}", input);
        Log.Info("iPiMPWeb - process args {0}", args);
        applicationDetails = new ProcessStartInfo(encCfg.fileName, args)
                                 {
                                     UseShellExecute = false,
                                     RedirectStandardInput = (encCfg.inputMethod == TransportMethod.StandardIn)
                                 };

        applicationThread = new Process {StartInfo = applicationDetails};
        if (!applicationThread.Start()) return;
        Log.Info("iPiMPWeb - process pid started {0}", applicationThread.Id.ToString());
        if (encCfg.inputMethod == TransportMethod.StandardIn)
            encoderInput.UnderlyingStreamObject = applicationThread.StandardInput.BaseStream;
    }

    public void StopProcess()
    {
      if (applicationThread != null)
        applicationThread.Kill();
    }
  }
}
