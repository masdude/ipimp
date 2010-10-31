/*
 * CopyStreamToStream taken from: http://msdn.microsoft.com/en-us/magazine/cc337900.aspx
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using TvLibrary.Log;

namespace uWiMP.TVServer.MPWebServices
{
    public abstract class TransportStream : Stream
    {
        private byte[] buffer;

        public abstract String Url { get; }
        public abstract Boolean IsReady { get; }
        public abstract void Start(Boolean isClient);
        public abstract Stream UnderlyingStreamObject { get; set; }

        public void CopyStream(Stream media)
        {
            // Make sure the stream is ready first.
            int tries = 10000; // 10 Seconds
            do
            {
                if (this.IsReady)
                    break;

                System.Threading.Thread.Sleep(1);
            } while (--tries != 0);
            
            if (IsReady)
            {
                buffer = new byte[0x1000];
                //Log.Info("iPiMPWeb  - CopyStream isReady");
                media.BeginRead(buffer, 0, buffer.Length, MediaReadAsyncCallback, media);
            }
            else
            {
                throw new Exception("Pipe Stream isn't ready.");
            }
        }

        private void MediaReadAsyncCallback(IAsyncResult ar)
        {
            //Log.Info("iPiMPWeb  - MediaReadAsyncCallback");
            try
            {
                var media = ar.AsyncState as Stream;
                //Log.Info("iPiMPWeb  - CopyStream length {0}", media.Length.ToString());
            
                var read = media.EndRead(ar);
                //Log.Info("iPiMPWeb  - MediaReadAsyncCallback {0}", read.ToString());
                
                if (read > 0)
                {
                    //Log.Info("iPiMPWeb  - MediaReadAsyncCallback Beginwrite");
                    BeginWrite(buffer, 0, read, writeResult =>
                    {
                        try
                        {
                            EndWrite(writeResult);
                            //Log.Info("iPiMPWeb  - MediaReadAsyncCallback Endwrite");
                            media.BeginRead(buffer, 0, buffer.Length, MediaReadAsyncCallback, media);
                        }
                        catch (Exception exc)
                        {
                            Log.Info("iPiMPWeb  - EndWrite exception {0}", exc.Message);
                            // var asyncResult = media.BeginRead(buffer, 0, buffer.Length, MediaReadAsyncCallback, media);
                        }
                    }, null);
                }
                else
                {
                    return;
                }
            }
            catch (Exception exc)
            {
                Log.Info("iPiMPWeb  - Read exception {0}", exc.Message);
            }
        }

    }
}
