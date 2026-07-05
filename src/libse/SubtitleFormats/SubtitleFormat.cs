using Nikse.SubtitleEdit.Core.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Nikse.SubtitleEdit.Core.SubtitleFormats
{
    public abstract class SubtitleFormat
    {
        public static bool SubtitleFormatsOrderChanged { get; set; }

        private static IList<SubtitleFormat> _allSubtitleFormats;
        private static IList<SubtitleFormat> _subtitleFormatsWithDefaultOrder;

        protected static readonly char[] SplitCharColon = { ':' };

        /// <summary>
        /// Text formats supported by Subtitle Edit
        /// </summary>
        public static IEnumerable<SubtitleFormat> AllSubtitleFormats
        {
            get
            {
                if (_allSubtitleFormats != null)
                {
                    if (SubtitleFormatsOrderChanged)
                    {
                        SubtitleFormatsOrderChanged = false;
                        _allSubtitleFormats = GetOrderedFormatsList(_subtitleFormatsWithDefaultOrder);
                    }

                    return _allSubtitleFormats;
                }

                _allSubtitleFormats = new List<SubtitleFormat>
                {
                    new AdvancedSubStationAlpha(),
                    new SubStationAlpha(),
                    new SubRip(),
                    new YouTubeSbv(),
                    new AdobeAfterEffectsFTME(),
                    new WebVTT()
                };               

                _subtitleFormatsWithDefaultOrder = new List<SubtitleFormat>(_allSubtitleFormats);
                _allSubtitleFormats = GetOrderedFormatsList(_subtitleFormatsWithDefaultOrder);

                return _allSubtitleFormats;
            }
        }

        protected int _errorCount;

        public abstract string Extension
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public virtual bool IsTimeBased => true;

        public bool IsFrameBased => !IsTimeBased;

        public string FriendlyName => $"{Name} ({Extension})";

        public int ErrorCount => _errorCount;

        public virtual bool IsMine(List<string> lines, string fileName)
        {
            var subtitle = new Subtitle();
            var oldFrameRate = Configuration.Settings.General.CurrentFrameRate;
            LoadSubtitle(subtitle, lines, fileName);
            Configuration.Settings.General.CurrentFrameRate = oldFrameRate;
            return subtitle.Paragraphs.Count > _errorCount;
        }

        public abstract string ToText(Subtitle subtitle, string title);

        public abstract void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName);

        public bool IsVobSubIndexFile => Extension.Equals(".idx", StringComparison.Ordinal);

        public virtual void RemoveNativeFormatting(Subtitle subtitle, SubtitleFormat newFormat)
        {
        }

        public virtual List<string> AlternateExtensions => new List<string>();

        public static int MillisecondsToFrames(double milliseconds)
        {
            return MillisecondsToFrames(milliseconds, Configuration.Settings.General.CurrentFrameRate);
        }

        public static int MillisecondsToFrames(double milliseconds, double frameRate)
        {
            return (int)Math.Round(milliseconds / (TimeCode.BaseUnit / GetFrameForCalculation(frameRate)), MidpointRounding.AwayFromZero);
        }

        public static double GetFrameForCalculation(double frameRate)
        {
            if (Math.Abs(frameRate - 23.976) < 0.001)
            {
                return 24000.0 / 1001.0;
            }
            if (Math.Abs(frameRate - 29.97) < 0.001)
            {
                return 30000.0 / 1001.0;
            }
            if (Math.Abs(frameRate - 59.94) < 0.001)
            {
                return 60000.0 / 1001.0;
            }

            return frameRate;
        }

        public static int MillisecondsToFramesMaxFrameRate(double milliseconds)
        {
            var frames = (int)Math.Round(milliseconds / (TimeCode.BaseUnit / GetFrameForCalculation(Configuration.Settings.General.CurrentFrameRate)), MidpointRounding.AwayFromZero);
            if (frames >= Configuration.Settings.General.CurrentFrameRate)
            {
                frames = (int)(Configuration.Settings.General.CurrentFrameRate - 0.01);
            }

            return frames;
        }

        public static int FramesToMilliseconds(double frames)
        {
            return FramesToMilliseconds(frames, Configuration.Settings.General.CurrentFrameRate);
        }

        public static int FramesToMilliseconds(double frames, double frameRate)
        {
            return (int)Math.Round(frames * (TimeCode.BaseUnit / GetFrameForCalculation(frameRate)), MidpointRounding.AwayFromZero);
        }

        public static int FramesToMillisecondsMax999(double frames)
        {
            var ms = (int)Math.Round(frames * (TimeCode.BaseUnit / GetFrameForCalculation(Configuration.Settings.General.CurrentFrameRate)), MidpointRounding.AwayFromZero);
            return Math.Min(ms, 999);
        }

        public virtual bool HasStyleSupport => false;

        public bool BatchMode { get; set; }
        public double? BatchSourceFrameRate { get; set; }

        public static string ToUtf8XmlString(XmlDocument xml, bool omitXmlDeclaration = false)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = omitXmlDeclaration,
            };
            var result = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(result, settings))
            {
                xml.Save(xmlWriter);
            }

            return result.ToString().Replace(" encoding=\"utf-16\"", " encoding=\"utf-8\"").Trim();
        }

        public virtual bool IsTextBased => true;

        protected static TimeCode DecodeTimeCodeFramesTwoParts(string[] tokens)
        {
            if (tokens == null)
            {
                return new TimeCode();
            }

            if (tokens.Length != 2)
            {
                throw new InvalidOperationException();
            }

            return new TimeCode(0, 0, int.Parse(tokens[0]), FramesToMillisecondsMax999(int.Parse(tokens[1])));
        }

        protected static TimeCode DecodeTimeCodeFramesFourParts(string[] tokens)
        {
            if (tokens == null)
            {
                return new TimeCode();
            }

            if (tokens.Length != 4)
            {
                throw new InvalidOperationException();
            }

            if (tokens[0] == "--" && tokens[1] == "--" && tokens[2] == "--" && tokens[3] == "--")
            {
                return new TimeCode(TimeCode.MaxTimeTotalMilliseconds);
            }

            if (tokens[0] == "-" && tokens[1] == "-" && tokens[2] == "-" && tokens[3] == "-")
            {
                return new TimeCode(TimeCode.MaxTimeTotalMilliseconds);
            }

            return new TimeCode(int.Parse(tokens[0], CultureInfo.InvariantCulture), int.Parse(tokens[1], CultureInfo.InvariantCulture), int.Parse(tokens[2], CultureInfo.InvariantCulture), FramesToMillisecondsMax999(int.Parse(tokens[3], CultureInfo.InvariantCulture)));
        }

        protected static TimeCode DecodeTimeCodeMsFourParts(string[] tokens)
        {
            if (tokens == null)
            {
                return new TimeCode();
            }

            if (tokens.Length != 4)
            {
                throw new InvalidOperationException();
            }

            return new TimeCode(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]), int.Parse(tokens[3]));
        }

        protected static TimeCode DecodeTimeCodeFrames(string timestamp, char[] splitChars)
        {
            return DecodeTimeCodeFramesFourParts(timestamp.Split(splitChars, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Load subtitle type of 'formats' from file.
        /// </summary>
        /// <param name="formats">List of possible formats</param>
        /// <param name="fileName">Name of subtitle file</param>
        /// <param name="subtitle">Subtitle to load file into</param>
        /// <returns>The format of the file, null of not format match found</returns>
        public static SubtitleFormat LoadSubtitleFromFile(SubtitleFormat[] formats, string fileName, Subtitle subtitle)
        {
            if (formats == null || formats.Length == 0 || string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var list = new List<string>(File.ReadAllLines(fileName, LanguageAutoDetect.GetEncodingFromFile(fileName)));
            foreach (var subtitleFormat in formats)
            {
                if (subtitleFormat.IsMine(list, fileName))
                {
                    subtitleFormat.LoadSubtitle(subtitle, list, fileName);
                    return subtitleFormat;
                }
            }
            return null;
        }

        /// <summary>
        /// Load subtitle from a list of lines and a file name (the last can be null).
        /// </summary>
        /// <param name="lines">Text lines from subtitle file</param>
        /// <param name="fileName">Optional file name</param>
        /// <returns>Subtitle, null if format not recognized</returns>
        public static Subtitle LoadSubtitleFromLines(List<string> lines, string fileName)
        {
            if (lines == null || lines.Count == 0)
            {
                return null;
            }

            var subtitle = new Subtitle();
            foreach (var subtitleFormat in AllSubtitleFormats)
            {
                if (subtitleFormat.IsMine(lines, fileName))
                {
                    subtitleFormat.LoadSubtitle(subtitle, lines, fileName);
                    return subtitle;
                }
            }

            return null;
        }

        public static SubtitleFormat[] GetBinaryFormats(bool batchMode)
        {
            return new SubtitleFormat[]
            {
                new Ebu { BatchMode = batchMode },
                new Pac { BatchMode = batchMode },
                new PacUnicode(),
                new Cavena890 { BatchMode = batchMode },
                new Spt(),
                new CheetahCaption(),
                new CheetahCaptionOld(),
                new TSB4(),
                new Chk(),
                new Ayato(),
                new CapMakerPlus(),
                new Ultech130(),
                new NciCaption(),
                new AvidStl(),
                new WinCaps32(),
                new IsmtDfxp(),
                new Spt(),
                new Sptx(),
                new IaiSub(),
                new ELRStudioClosedCaption(),
                new CaptionsInc(),
                new TimeLineMvt(),
                new Cmaft(),
                new Pns(),
                new PlayCaptionsFreeEditor(),
                new VideoCdDat(),
            };
        }

        public static SubtitleFormat[] GetTextOtherFormats()
        {
            return new SubtitleFormat[]
            {
                new NkhCuePoints(),
                new DlDd(),
                new Ted20(),
                new Captionate(),
                new TimeLineAscii(),
                new TimeLineFootageAscii(),
                new TimedTextImage(),
                new FinalCutProImage(),
                new SpuImage(),
                new Dost(),
                new SeImageHtmlIndex(),
                new BdnXml(),
                new Wsb(),
                new JsonTypeOnlyLoad1(),
                new JsonTypeOnlyLoad2(),
                new JsonTypeOnlyLoad3(),
                new JsonTypeOnlyLoad4(),
                new JsonTypeOnlyLoad5(),
                new JsonTypeOnlyLoad6(),
                new TranscriptiveJson(),
                new KaraokeCdgCreatorText(),
                new VidIcelandic(),
                new JsonArchtime(),
                new Rdf1(),
                new CombinedXml(),
                new AudacityLabels(),
                new Fte(),
                new ClqttJson(),
            };
        }

        public static SubtitleFormat FromName(string formatName, SubtitleFormat defaultFormat)
        {
            var trimmedFormatName = formatName.Trim();
            foreach (var format in AllSubtitleFormats)
            {
                if (format.Name.Trim().Equals(trimmedFormatName, StringComparison.OrdinalIgnoreCase) ||
                    format.FriendlyName.Trim().Equals(trimmedFormatName, StringComparison.OrdinalIgnoreCase))
                {
                    return format;
                }
            }

            return defaultFormat;
        }

        private static IList<SubtitleFormat> GetOrderedFormatsList(IEnumerable<SubtitleFormat> unorderedFormatsList)
        {
            IEnumerable<SubtitleFormat> newSelectedFormats = new[] { Utilities.GetSubtitleFormatByFriendlyName(Configuration.Settings.General.DefaultSubtitleFormat) };
            if (!string.IsNullOrEmpty(Configuration.Settings.General.FavoriteSubtitleFormats))
            {
                newSelectedFormats = newSelectedFormats.Union(Configuration.Settings.General.FavoriteSubtitleFormats.Split(';').Select(formatName => Utilities.GetSubtitleFormatByFriendlyName(formatName)));
            }

            return newSelectedFormats.Union(unorderedFormatsList).ToList();
        }
    }
}
