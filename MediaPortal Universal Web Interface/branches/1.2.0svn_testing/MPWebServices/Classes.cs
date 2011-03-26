using System;
using System.Collections.Generic;
using System.Text;
using TvDatabase;

namespace uWiMP.TVServer.MPWebServices.Classes
{
  public enum CardStatus
  {
    Idle,
    TimeShifting,
    Recording,
    Scanning,
    Grabbing_EPG
  }
  public class WebChannelGroup
  {
    public int idGroup;
    public string name;

    public WebChannelGroup()
    {
      idGroup = -1;
      name = "";
    }
    public WebChannelGroup(int idGroup, string name)
    {
      this.idGroup = idGroup;
      this.name = name;
    }
  }
  public class WebChannel
  {
    public int idChannel;
    public string displayName;
    public string name;
    public bool freeToAir;
    public bool grabEPG;
    public bool isRadio;
    public bool isTv;
    public bool isWebStream;

    public WebChannel() { }
    public WebChannel(Channel ch)
    {
      this.idChannel = ch.IdChannel;
      this.displayName = ch.DisplayName;
      this.name = ch.Name;
      this.freeToAir = ch.FreeToAir;
      this.grabEPG = ch.GrabEpg;
      this.isRadio = ch.IsRadio;
      this.isTv = ch.IsTv;
      this.isWebStream = ch.IsWebstream();
    }
  }
  public class WebShortProgram
  {
    public DateTime startTime;
    public DateTime endTime;
    public string title;
    public string description;

    public WebShortProgram() { }
    public WebShortProgram(Program prog)
    {
      if (prog == null)
        return;
      this.startTime = prog.StartTime;
      this.endTime = prog.EndTime;
      this.title = prog.Title;
      this.description = prog.Description;
    }
  }
  public class WebMiniEPG
  {
    public int idChannel;
    public string name;
    public bool isWebStream;
    public WebShortProgram epgNow;
    public WebShortProgram epgNext;

    public WebMiniEPG() { }
    public WebMiniEPG(Channel ch)
    {
      if (ch == null)
        return;
      this.idChannel = ch.IdChannel;
      this.name = ch.DisplayName;
      this.isWebStream = ch.IsWebstream();
      epgNow = new WebShortProgram(ch.CurrentProgram);
      epgNext=new WebShortProgram(ch.NextProgram);
    }
  }
  public class WebProgram
  {
    public string classification;
    public string description;
    public DateTime endTime;
    public string episodeName;
    public string episodeNum;
    public string episodeNumber;
    public string episodePart;
    public string genre;
    public int idChannel;
    public int idProgram;
    public DateTime originalAirDate;
    public int parentalRating;
    public string seriesNum;
    public int starRating;
    public DateTime startTime;
    public string Title;
    public string channelName;

    public WebProgram()
    {
    }
    public WebProgram(Program prog)
    {
      this.classification = prog.Classification;
      this.description = prog.Description;
      this.endTime = prog.EndTime;
      this.episodeName = prog.EpisodeName;
      this.episodeNum = prog.EpisodeNum;
      this.episodeNumber = prog.EpisodeNumber;
      this.episodePart = prog.EpisodePart;
      this.genre = prog.Genre;
      this.idChannel = prog.IdChannel;
      this.idProgram = prog.IdProgram;
      this.originalAirDate = prog.OriginalAirDate;
      this.parentalRating = prog.ParentalRating;
      this.seriesNum = prog.SeriesNum;
      this.starRating = prog.StarRating;
      this.startTime = prog.StartTime;
      this.Title = prog.Title;
      this.channelName = prog.ReferencedChannel().DisplayName;
    }
  }
  public class WebRecording
  {
    public string description;
    public DateTime endTime;
    public string episodeName;
    public string episodeNum;
    public string episodeNumber;
    public string episodePart;
    public string fileName;
    public string genre;
    public int idChannel;
    public int idRecording;
    public bool isManual;
    public int keepUntil;
    public DateTime keepUntilDate;
    public string seriesNum;
    public bool shouldBeDeleted;
    public DateTime startTime;
    public int stopTime;
    public int timesWatched;
    public string title;
    public string channelName;

    public WebRecording() { }
    public WebRecording(Recording rec)
    {
      this.description = rec.Description;
      this.endTime = rec.EndTime;
      this.episodeName = rec.EpisodeName;
      this.episodeNum = rec.EpisodeNum;
      this.episodeNumber = rec.EpisodeNumber;
      this.episodePart = rec.EpisodePart;
      this.fileName = rec.FileName;
      this.genre = rec.Genre;
      this.idChannel = rec.IdChannel;
      this.idRecording = rec.IdRecording;
      this.isManual = rec.IsManual;
      this.keepUntil = rec.KeepUntil;
      this.keepUntilDate = rec.KeepUntilDate;
      this.seriesNum = rec.SeriesNum;
      this.shouldBeDeleted = rec.ShouldBeDeleted;
      this.startTime = rec.StartTime;
      this.stopTime = rec.StopTime;
      this.timesWatched = rec.TimesWatched;
      this.title = rec.Title;
      try
      {
        this.channelName = Channel.Retrieve(rec.IdChannel).DisplayName;
      }
      catch (Exception)
      { }
    }
  }
  public class WebReceptionDetails
  {
    public int signalLevel;
    public int signalQuality;
  }
  public class WebTvServerStatus
  {
    public int idCard;
    public string cardName;
    public int cardType;
    public string cardTypeStr;
    public int status;
    public string statusStr;
    public int idChannel;
    public string channel;
    public string userName;
    public string recordingFolder;
    public string timeshiftFolder;
  }
  public class WebSchedule
  {
    public int bitrateMode;
    public DateTime canceled;
    public string directory;
    public bool doesUseEpisodeManagement;
    public DateTime endTime;
    public int idChannel;
    public string channelName;
    public int idSchedule;
    public bool isManual;
    public DateTime keepDate;
    public int keepMethod;
    public int maxAirings;
    public int postRecordInterval;
    public int preRecordInterval;
    public int priority;
    public string programName;
    public int quality;
    public int qualityType;
    public int recommendedCard;
    public int scheduleType;
    public string scheduleTypeStr;
    public bool series;
    public DateTime startTime;

    public WebSchedule() { }
    public WebSchedule(Schedule sched)
    {
      //this.bitrateMode = (int)sched.BitRateMode;
      this.canceled = sched.Canceled;
      this.directory = sched.Directory;
      this.doesUseEpisodeManagement = sched.DoesUseEpisodeManagement;
      this.endTime = sched.EndTime;
      this.idChannel = sched.IdChannel;
      this.channelName = Channel.Retrieve(sched.IdChannel).DisplayName;
      this.idSchedule = sched.IdSchedule;
      this.isManual = sched.IsManual;
      this.keepDate = sched.KeepDate;
      this.keepMethod = sched.KeepMethod;
      this.maxAirings = sched.MaxAirings;
      this.postRecordInterval = sched.PostRecordInterval;
      this.preRecordInterval = sched.PreRecordInterval;
      this.priority = sched.Priority;
      this.programName = sched.ProgramName;
      this.quality = sched.Quality;
      this.qualityType = (int)sched.QualityType;
      this.recommendedCard = sched.RecommendedCard;
      this.scheduleType = sched.ScheduleType;
      this.scheduleTypeStr = ((ScheduleRecordingType)sched.ScheduleType).ToString();
      this.series = sched.Series;
      this.startTime = sched.StartTime;
    }
  }

  public class WebTvServerUser
  {
    public int idCard;
    public int idChannel;
    public string name;

    public WebTvServerUser() { }
    public WebTvServerUser(TvControl.User user)
    {
      this.idCard = user.CardId;
      this.idChannel = user.IdChannel; 
      this.name = user.Name;
    }
  }
  public class WebTvResult
  {
    public int result;
    public string rtspURL;
    public WebTvServerUser user;
    public string timeshiftFile;

    public WebTvResult()
    {
      result=5; //unknown error
      rtspURL = "";
      user = new WebTvServerUser();

    }
    public WebTvResult(int result, string rtspURL,string timeshiftFile,TvControl.User u)
    {
      this.result = result;
      this.rtspURL = rtspURL;
      user = new WebTvServerUser(u);
      this.timeshiftFile = timeshiftFile;
    }
  }
  public class WebMovie
  {
    public int idMovie;
    public string genre;
    public string title;
    public string plot;
    public string file;

    public WebMovie() { }
    public WebMovie(int idMovie, string genre, string title, string plot, string file)
    {
      this.idMovie = idMovie;
      this.genre = genre;
      this.title = title;
      this.plot = plot;
      this.file = file;
    }

  }
  public class WebMusicTrack
  {
    public int idTrack;
    public string album;
    public string artist;
    public int trackno;
    public string title;
    public string file;
    public int duration;

    public WebMusicTrack() { }
    public WebMusicTrack(int idTrack, string album, string artist, int trackno,string title, string file,int duration)
    {
      this.idTrack = idTrack;
      this.album = album;
      this.artist = artist;
      this.trackno = trackno;
      this.title = title;
      this.file = file;
      this.duration = duration;
    }
  }
  public class WebPicture
  {
    public int idPicture;
    public string file;
    public int rotation;
    public DateTime taken;

    public WebPicture() { }
    public WebPicture(int idPicture, string file, int rotation, DateTime taken)
    {
      this.idPicture = idPicture;
      this.file = file;
      this.rotation = rotation;
      this.taken = taken;
    }
  }
  public class WebSeries
  {
    public int season;
    public int episode;
    public string seriesName;
    public string episodeName;
    public string episodePlot;
    public string filename;
    public string compositeId;

    public WebSeries() { }
    public WebSeries(int season, int episode, string seriesName, string episodeName, string episodePlot, string filename,string compositeId)
    {
      this.season = season;
      this.episode = episode;
      this.seriesName = seriesName;
      this.episodeName = episodeName;
      this.episodePlot = episodePlot;
      this.filename = filename;
      this.compositeId = compositeId;
    }
  }
  public class WebMovingPicture
  {
    public int id;
    public string title;
    public string plot;
    public string filename;
    public string genre;
    public string parentalRating;
    public int year;

    public WebMovingPicture() { }
    public WebMovingPicture(int id,string title, string plot, string filename, string genre, string parentalRating, int year)
    {
      this.id = id;
      this.title = title;
      this.plot = plot;
      this.filename = filename;
      this.genre = genre;
      this.parentalRating = parentalRating;
      this.year = year;
    }
  }
  public class SupportedMPFunctions
  {
    public bool myVideos;
    public bool myMusic;
    public bool myPictures;
    public bool myTvSeries;
    public bool movingPictures;

    public SupportedMPFunctions()
    {
      myVideos = false;
      myMusic = false;
      myPictures = false;
      myTvSeries = false;
      movingPictures = false;
    }
  }
}
