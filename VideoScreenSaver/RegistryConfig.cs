using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VideoScreenSaver
{

    public static class RegistryConfig
    {

        private static readonly string mConfigurationFolder;
        private static readonly string mConfigurationFile;

        static RegistryConfig()
        {
            MediaList = new List<string>();
            mConfigurationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JZO", "VideoScreenSaver");
            mConfigurationFile = Path.Combine(mConfigurationFolder, "Settings.xml");

            try
            {
                if (!Directory.Exists(mConfigurationFolder))
                {
                    Directory.CreateDirectory(mConfigurationFolder);
                }

                if (File.Exists(mConfigurationFile))
                {
                    using (FileStream fs = new FileStream(mConfigurationFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                        Configuration config = (Configuration)serializer.Deserialize(fs);
                        IsMuted = config.IsMuted;
                        IsSuffleTurnedOn = config.IsSuffleTurnedOn;
                        IsShowMediaOnAllScreen = config.IsShowMediaOnAllScreen;
                        Volume = config.Volume;
                        MediaList = config.MediaList;
                    }
                }

            }
            catch (Exception)
            {
            }

            //MediaList.Add(@"F:\Video Clips\Spiritual\Earth Music to to relax - (Enya Vangelis) New Age Relax 2016 2015a.mp4");
            //MediaList.Add(@"F:\Video Clips\Spiritual\Earth Music to to relax - (Enya Vangelis) New Age Relax 2016 2015.mp4");
            //MediaList.Add(@"F:\Video Clips\Spiritual\Healing Guardian Angels 2 - (Enya _ Vangelis) Spiritual Guides - love - New Age 2016 2015.mp4");
            //MediaList.Add(@"F:\Video Clips\Spiritual\Healing Guardian Angels - Spiritual Guids - Heaven (Vangelis _ Enya) - New Age 2014 2015.mp4");

            //IsShowMediaOnAllScreen = true;
        }

        public static bool IsMuted { get; set; }

        public static bool IsSuffleTurnedOn { get; set; }

        public static bool IsShowMediaOnAllScreen { get; set; }

        public static double Volume { get; set; }

        public static List<string> MediaList { get; set; }

        public static void Save()
        {
            if (!Directory.Exists(mConfigurationFolder))
            {
                Directory.CreateDirectory(mConfigurationFolder);
            }
            using (FileStream fs = new FileStream(mConfigurationFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                Configuration config = new VideoScreenSaver.Configuration();
                config.IsMuted = IsMuted;
                config.IsShowMediaOnAllScreen = IsShowMediaOnAllScreen;
                config.IsSuffleTurnedOn = IsSuffleTurnedOn;
                config.Volume = Volume;
                config.MediaList = MediaList;
                serializer.Serialize(fs, config);
            }
        }

    }

    [Serializable]
    public class Configuration
    {

        private readonly List<string> mMediaList = new List<string>();

        public Configuration()
        {
            Volume = 1;
        }

        public bool IsMuted { get; set; }

        public bool IsSuffleTurnedOn { get; set; }

        public bool IsShowMediaOnAllScreen { get; set; }

        public double Volume { get; set; }

        public List<string> MediaList
        {
            get { return mMediaList; }
            set
            {
                mMediaList.Clear();
                if (value != null)
                {
                    value.ForEach(i =>
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            string item = i.Trim();
                            if (!string.IsNullOrEmpty(item))
                            {
                                mMediaList.Add(item);
                            }
                        }
                    });
                }
            }
        }

    }

}
