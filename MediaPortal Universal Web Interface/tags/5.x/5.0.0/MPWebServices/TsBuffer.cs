﻿/*
 * TODO: Go over the Class Template.
 * It isn't quite right.
 * 
 */

#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TvLibrary.Log;
#endregion

namespace uWiMP.TVServer.MPWebServices
{
  public class TsBuffer : Stream
  {
    #region Variables
    // Private Variables
    private State state = State.NoFileLoaded;   // State of the stream.

    private Stream tsBuffer;                    // tsBuffer information file.

    private long tsWriterPosition;              // tsWriters position.
    private long tsReaderPosition;              // Streams current position in the active file.

    private int tsBufferAdded;                  // Number of files added.
    private int tsBufferRemoved;                // Number of files removed.

    private List<TsFile> tsFiles;               // List of all the TS files used by the buffer.
    private TsFile tsReaderFile = null;         // Current TS file open.
    private Stream tsReader = null;             // Current TS file been read.
    #endregion
    #region Constructors/Destructors
    /// <summary>
    /// Load a tsBuffer file to read a multi-part TS Stream.
    /// </summary>
    /// <param name="path">Path to the .tsBuffer file.</param>
    public TsBuffer(String path)
    {
      Load(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    } // TsBuffer
    /// <summary>
    /// Load a tsBuffer file to read a multi-part TS Stream.
    /// </summary>
    /// <param name="data">Stream containing the .tsBuffer file.</param>
    public TsBuffer(Stream data)
    {
      Load(data);
    } // TsBuffer
    #endregion
    #region Public properties
    // Public properties
    /// <summary>
    /// Can read data from the stream.
    /// </summary>
    public override bool CanRead
    {
      get { return (state == State.Playing ? true : state == State.EndOfFile) ? true : false; }
    } // CanRead
    /// <summary>
    /// Can seek throughout the stream.
    /// </summary>
    /// <remarks>
    /// Sure I guess.
    /// </remarks>
    public override bool CanSeek
    {
      get { return (state == State.Playing ? true : state == State.EndOfFile) ? true : false; }
    } // CanSeek
    /// <summary>
    /// Can write to tsBuffer.
    /// </summary>
    /// <remarks>
    /// Purely a reading stream so always false.
    /// </remarks>
    public override bool CanWrite
    {
      get { return false; }
    } // CanWrite
    /// <summary>
    /// Current length of the file.
    /// </summary>
    /// <remarks>Due to the dynamic nature of the TS stream it is highly recommended you use this value for the total length.</remarks>
    public override long Length
    {
      get
      {
        // Refresh the tsWriters position.
        RefreshTsBuffer();

        // Determine the total length of all the files except the last one.
        long baseMaxPosition = tsFiles.Where(o => o != tsFiles.Last()).Sum(o => o.Length); // Select all except the last element and total the file lengths.

        return baseMaxPosition + tsWriterPosition;
      }
    } // Length
    /// <summary>
    /// Current position in the stream.
    /// </summary>
    /// <remarks>Due to the dynamic nature of the TS stream it is highly recommended you use this value for your current position.</remarks>
    public override long Position
    {
      get
      {
        return tsReaderPosition;
      }
      set
      {
        Seek(value, SeekOrigin.Begin);
      }
    } // Position
    #endregion
    #region Public methods
    /// <summary>
    /// Not Supported.
    /// </summary>
    public override void Flush()
    {
      throw new NotSupportedException("This stream does not support writing.");
    } // Flush
    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position
    /// within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
    /// byte array with the values between offset and (offset + count - 1) replaced
    /// by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
    /// from the current stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>The total number of bytes read into the buffer. This can be less than the
    ///     number of bytes requested if that many bytes are not currently available,
    ///     or zero (0) if the end of the stream has been reached.</returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
      if (state != State.Playing)
      {
        return 0;
      }

      try
      {
        int tries = 10000;
        do
        {
          try
          {
            // Refresh the .tsBuffer file to update the pointers.
            RefreshTsBuffer();

            // Check if the tsWriter has moved to another file.
            if (tsWriterPosition < tsReaderPosition)
            {
              // Check if there is enough data to fill the buffer.
              if (((tsReader.Length - tsReaderPosition) + tsWriterPosition) > count)
              {
                // Read the remainder of the file.
                int bytesToRead = Convert.ToInt32(tsReader.Length - tsReaderPosition);
                if (bytesToRead > count)
                {
                  bytesToRead = count;
                }

                int byteCount = tsReader.Read(buffer, offset, bytesToRead);

                // Check if the end of the file has been reached.
                if (tsReader.Length == tsReaderPosition)
                {
                  // Change to the next file.
                  ChangeTsFile(tsFiles.FindIndex(o => o == tsReaderFile) + 1);

                  // Check if it changed successfully.
                  if (state == State.Playing)
                  {
                    // Fill the rest of the buffer.
                    byteCount += tsReader.Read(buffer, offset + byteCount, count - byteCount);
                  }
                }
                // Increment the current position.
                tsReaderPosition += byteCount;

                // Return what we have.
                return byteCount;
              }
            }
            else
            {
              // Check if there is enough data to fill the buffer.
              if ((tsWriterPosition - tsReaderPosition - count) > count) // Make sure there is a extra packets buffer.
              {
                // Should be enough data.
                int byteCount = tsReader.Read(buffer, offset, count);

                // Increment the current position.
                tsReaderPosition += byteCount;
#if RELEASE
                                Log.Info(String.Format("Tries: {0}", tries));
#endif

                return byteCount;
              }
            }
          }
#if DEBUG
                    catch (Exception e)
                    {
                        Log.Info(String.Format("Read Exception: {0}", e));
                    }
#else
          catch (Exception)
          {
          }
#endif
          // This is added because the CPU scans so bloody fast that the loop would be pointless without it.
          // Need to find a better way.
          System.Threading.Thread.Sleep(1);
        } while (--tries != 0);

        return 0;
      }
      catch (Exception)
      {
        return 0;
      }
    } // Read
    /// <summary>
    /// Seek to a position in the TS stream.
    /// </summary>
    /// <param name="offset">Offset from position.</param>
    /// <param name="origin">Point of reference.</param>
    /// <returns>The new position in the current stream.</returns>
    public override long Seek(long offset, SeekOrigin origin)
    {
      // Calculate the minimum + maximum position values.
      long minPosition = tsWriterPosition + Convert.ToInt64(tsWriterPosition * .01);                      // tsWriterPosition + a extra 1% of the file so the writer doesn't get ahead of us.
      long maxPosition = tsFiles.Where(o => o != tsFiles.Last()).Sum(o => o.Length) + tsWriterPosition;   // Full length of all the previous TS files + the tsWriters current position.

      if (tsFiles.Count == 1)
      {
        minPosition = 0;
      }

      switch (origin)
      {
        case SeekOrigin.Begin:
          // Check if the offset is out of bounds.
          if (offset < 0 || (offset + minPosition) > maxPosition)
          {
            state = State.UnknownError;
            return -1;
          }

          tsReaderPosition = minPosition + offset;
          break;
        case SeekOrigin.Current:
          if ((tsReaderPosition + offset) < minPosition || (tsReaderPosition + offset) > maxPosition)
          {
            state = State.UnknownError;
            return -1;
          }

          tsReaderPosition += offset;
          break;
        case SeekOrigin.End:
          if (offset > 0 || (maxPosition + 1) < minPosition)
          {
            state = State.UnknownError;
            return -1;
          }

          tsReaderPosition = maxPosition + offset;
          break;
      }

      // Check we have the right file.
      long lengthTotal = 0;

      for (int i = 0; i < tsFiles.Count; i++)
      {
        lengthTotal += tsFiles[i].Length;

        if (tsReaderPosition < lengthTotal)
        {
          // Ok have the file.
          // Determine where the pointer will be in the file.
          long positionInFile = tsReaderPosition - (lengthTotal - tsFiles[i].Length); // Current Pos - (total length - last file length)

          // Check if the file isn't open.
          if (tsReaderFile != tsFiles[i])
          {
            // Change the file.
            ChangeTsFile(i);
          }

          // Set the position in the file.
          tsReaderPosition = positionInFile;
          //tsReader.Seek(tsReaderPosition, SeekOrigin.Begin);
          tsReader.Position = positionInFile;

          break;
        }
      }

      return tsReaderPosition;
    } // Seek
    /// <summary>
    /// Not Supported.
    /// </summary>
    /// <param name="value"></param>
    public override void SetLength(long value)
    {
      throw new NotSupportedException("This stream does not support writing.");
    } // SetLength
    /// <summary>
    /// Not Supported.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException("This stream does not support writing.");
    } // Write
    /// <summary>
    /// Close off the stream and associated resources.
    /// </summary>
    public override void Close()
    {
      state = State.InvalidFile;

      if (tsBuffer != null)
      {
        tsBuffer.Close();
        tsBuffer.Dispose();
      }
      if (tsReader != null)
      {
        tsReader.Close();
        tsReader.Dispose();
      }

      tsReader = null;
      tsBuffer = null;

      base.Close();
    } // Close
    #endregion
    #region Private methods
    /// <summary>
    /// Refresh the .tsBuffer details.
    /// </summary>
    private void RefreshTsBuffer()
    {
      //Log.Info("Refresh Triggered.");
      //Log.Info("iPiMPWeb - TsBuffer Refresh Triggered");
      if (tsBuffer == null)
      {
        state = State.InvalidFile;
        //Log.Info("iPiMPWeb - TsBuffer is null");
        return;
      }

      //LAYOUT:
      // 64bit    : current position
      // long     : files added
      // long     : files removed 
      // ... File list
      // long     : files added
      // long     : files removed

      // Used Int64 so it feels like MultiFileReader.
      // Used Int32 just to stress the point of a 4-byte block in the file.

      //Int64 currentPosition = 0; // Using tsWriterPosition instead since this is a waste, left here to help explain the .tsBuffer file.
      Int32 filesAdded = 0;
      Int32 filesRemoved = 0;
      Int32 filesAdded2 = 0;
      Int32 filesRemoved2 = 0;
      Int64 remainingLength = 0;

      byte[] fileList = null;
      byte[] correctedFileList = null;

      bool error;
      int tries = 10;

      #region Read tsBuffer
      do
      {
        try
        {
          error = false;

          // Read the latest details about the writer thread.
          // Check if the file size is acceptable.
          if (tsBuffer.Length <= (sizeof(Int64) + sizeof(long) + sizeof(long) + sizeof(char) + sizeof(long) + sizeof(long)))
          {
            // File size too small to be a valid.
            throw new InvalidDataException("File size is too small.");
          }

          //tsBuffer.Seek(0, SeekOrigin.Begin);
          tsBuffer.Position = 0;

          #region Extra DEBUG Output
          // You won't get far with this on but it does help to understand the .tsBuffer file.
#if EXTRA_DEBUG
                    Log.Info("------------------------");
                    Log.Info(String.Format(".TsBuffer File: pass {0}", tries));
                    Log.Info("------------------------");

                    Log.Info(String.Format("Current Pos     : {0}", tsBuffer.ReadInt64()));
                    Log.Info(String.Format("Files Added     : {0}", tsBuffer.ReadInt32()));
                    Log.Info(String.Format("Files Removed   : {0}", tsBuffer.ReadInt32()));
                    Log.Info(String.Format("Remaining Length: {0}", tsBuffer.BaseStream.Length - sizeof(Int64) - sizeof(long) - sizeof(long) - sizeof(long) - sizeof(long)));

                    Int64 newRemainingLength = tsBuffer.BaseStream.Length - sizeof(Int64) - sizeof(Int32) - sizeof(Int32) - sizeof(Int32) - sizeof(Int32);
                    byte[] filesListed = new byte[newRemainingLength + 1];
                    tsBuffer.Read(filesListed, 0, filesListed.Length - 1);

                    byte[] fixedList = new byte[filesListed.Length / 2];
                    for (int i = 0; i < filesListed.Length / 2; i++)
                    {
                        fixedList[i] = filesListed[i * 2];
                    }

                    System.Diagnostics.Debug.Write("Files           : ");
                    foreach (Byte item in fixedList)
                    {
                        System.Diagnostics.Debug.Write(String.Format("{0} ", Convert.ToString(item)));
                    }
                    Log.Info("");
                    System.Diagnostics.Debug.Write("Files Text      : ");
                    foreach (Byte item in fixedList)
                    {
                        System.Diagnostics.Debug.Write(String.Format("{0}", Convert.ToChar(item)));
                    }
                    Log.Info("");

                    Log.Info(String.Format("Files Added 2   : {0}", tsBuffer.ReadInt32()));
                    Log.Info(String.Format("Files Removed 2 : {0}", tsBuffer.ReadInt32()));

                    Log.Info("");
                    Log.Info("------------------------");
                    Log.Info("byte by byte.");
                    Log.Info("------------------------");

                    tsBuffer.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (tsBuffer.BaseStream.Position < tsBuffer.BaseStream.Length)
                    {
                        System.Diagnostics.Debug.Write(String.Format("{0} ", tsBuffer.BaseStream.ReadByte()));
                    }

                    Log.Info("");

                    tsBuffer.BaseStream.Seek(0, SeekOrigin.Begin);
#endif
          #endregion

          byte[] tsDetails = new byte[16];
          tsBuffer.Read(tsDetails, 0, 16);

          tsWriterPosition = BitConverter.ToInt64(tsDetails, 0);
          filesAdded = BitConverter.ToInt32(tsDetails, 8);
          filesRemoved = BitConverter.ToInt32(tsDetails, 12);

          // If no files added or removed, break the loop !
          if ((tsBufferAdded == filesAdded) && (tsBufferRemoved == filesRemoved))
          {
            break;
          }

          remainingLength = tsBuffer.Length - sizeof(Int64) - sizeof(Int32) - sizeof(Int32) - sizeof(Int32) - sizeof(Int32);

          // Above 100kb or below 0 seems stupid and figure out a problem !!!
          if ((remainingLength > 100000) || (remainingLength < 0))
          {
            if (remainingLength > 0)
            {
              throw new InvalidDataException("File size is too big.");
            }
            else
            {
              throw new InvalidDataException("File size is too small.");
            }
          }

          // Load the file list.
          fileList = new byte[remainingLength + 1];
          tsBuffer.Read(fileList, 0, fileList.Length - 1);

          correctedFileList = new byte[(fileList.Length / 2)];

          for (int i = 0; i < fileList.Length / 2; i++)
          {
            correctedFileList[i] = fileList[i * 2];
          }

          // Load the duplicate added / removed values.
          tsDetails = new byte[8];
          tsBuffer.Read(tsDetails, 0, 8);

          filesAdded2 = BitConverter.ToInt32(tsDetails, 0);
          filesRemoved2 = BitConverter.ToInt32(tsDetails, 4);

          // Check if they match.
          if ((filesAdded2 != filesAdded) || (filesRemoved2 != filesRemoved))
          {
            throw new InvalidDataException("Invalid data read from file.");
          }

          tsDetails = null;

          break;
        }
        catch (Exception)
        {
          error = true;
        }
      } while (--tries != 0);
      #endregion

      if (error)
      {
        // Didn't read .tsBuffer properly, throw a error.
        state = State.InvalidFile;
        return;
      }

      // Check if a file has been added or removed.
      if (tsBufferAdded != filesAdded || tsBufferRemoved != filesRemoved)
      {
        Int64 filesToRemove = Convert.ToInt64(filesRemoved) - tsBufferRemoved;
        Int64 filesToAdd = Convert.ToInt64(filesAdded) - tsBufferAdded;
        Int64 fileID = Convert.ToInt64(filesRemoved);
#if DEBUG
                Log.Info(String.Format("iPiMPWeb - TsBuffer - Files Added {0}, Removed {1}", filesToAdd, filesToRemove));
#endif

        // Removed files that aren't present anymore.
        while ((filesToRemove > 0) && (tsFiles.Count > 0))
        {
#if DEBUG
            Log.Info(String.Format("iPiMPWeb - TsBuffer - Removing file {0}", tsFiles[0]));
#endif
          // Remove the last file.
          tsFiles.RemoveAt(0);

          filesToRemove--;
        }

        // Create a list of files in the .tsbuffer file.
        String[] files = new String(Encoding.Default.GetChars(correctedFileList)).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
#if DEBUG
                if (files.Count() > 0)
                {
                    Log.Info(String.Format("iPiMPWeb - TsBuffer - FileListString: {0}", files[0]));
                    for (int i = 1; i < files.Count(); i++)
                    {
                        Log.Info(String.Format("                           {0}", files[i]));
                    }
                    Log.Info("");
                }
#endif

        // Check if there are more files to add then are in the list.
        if (filesToAdd > files.Count())
        {
          // Connected to a stream already running.
          filesToAdd = files.Count();
        }

        // Add any new files to the list.
        while (filesToAdd > 0)
        {
          tsFiles.Add(new TsFile(files[files.Count() - filesToAdd]));

          filesToAdd--;
        }

        // Update the local count.
        tsBufferAdded = filesAdded;
        tsBufferRemoved = filesRemoved;
      }
    } // RefreshTsBuffer
    /// <summary>
    /// Change the Ts File been read from.
    /// </summary>
    /// <param name="fileId">Id number of the new file.</param>
    private void ChangeTsFile(int fileId)
    {
      // Check if there is already a file open.
      if (tsReader != null)
      {
        tsReader.Close();
      }

      // Check if id exists.
      if (fileId >= 0 && fileId < tsFiles.Count)
      {
        int tries = 5; // Try to change the file 5 times before giving up.
        do
        {
          try
          {
            tsReader = new FileStream(tsFiles[fileId].Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            tsReaderFile = tsFiles[fileId];

            state = State.Playing;                // Set the state so the file is ready to play.
            tsReaderPosition = 0;                 // Reset the reader position to the start.
            return;
          }
          catch (IOException)
          {
            // Give it a moment.
            System.Threading.Thread.Sleep(10);
          }
        } while (--tries != 0);
      }

      // Something went wrong.
      state = State.InvalidFile;
    } // ChangeTsFile
    /// <summary>
    /// Setup the stream.
    /// </summary>
    /// <param name="data">Data stream containing the tsBuffer.</param>
    private void Load(Stream data)
    {
      tsBuffer = data;
      tsFiles = new List<TsFile>();

      // Update the tsBuffer details.
      RefreshTsBuffer();

      // Check if there is a file to read data from.
      if (tsFiles.Count > 0)
      {
        // There is a file to load.
        ChangeTsFile(tsFiles.Count - 1); // Load the last file.

        // Seek the end of the file.
        Seek(0, SeekOrigin.End);
      }
    } // Load
    #endregion
  }

  /// <summary>
  /// TS File details.
  /// </summary>
  internal class TsFile
  {
    private Int64 _length;
    private String _location;

    public TsFile()
    {
    }
    /// <summary>
    /// Store important file information.
    /// </summary>
    /// <param name="location">TS File location.</param>
    /// <remarks>Tries to pull the file length, be sure the file exists first.</remarks>
    /// <exception cref="FileNotFoundException">File doesn't exist to determine its length.</exception>
    public TsFile(String location)
    {
      try
      {
        _length = new FileInfo(location).Length;
      }
      catch (FileNotFoundException e)
      {
        throw e;
      }

      _location = location;
    }

    public TsFile(Int64 length, String location)
    {
      _length = length;
      _location = location;
    }

    /// <summary>
    /// Length of TS File.
    /// </summary>
    public Int64 Length
    {
      get
      {
        return _length;
      }
      set
      {
        _length = value;
      }
    }

    /// <summary>
    /// TS File location.
    /// </summary>
    public String Location
    {
      get
      {
        return _location;
      }
      set
      {
        _location = value;
      }
    }
  }
}