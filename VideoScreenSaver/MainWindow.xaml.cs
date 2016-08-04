using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace VideoScreenSaver
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IInitializable
    {

        private readonly DateTime mStartupTime = DateTime.Now;
        private static readonly List<MediaElement> mMediaPlayers = new List<MediaElement>();
        private static int mMediaPlayersEnd = 0;
        private static readonly List<string> mMedias = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            Topmost = false;
#endif
            mMediaPlayers.Add(MediaPlayer);
            Interlocked.Increment(ref mMediaPlayersEnd);
            MediaPlayer.Volume = RegistryConfig.Volume;
        }

        public static void Start()
        {
            if (RegistryConfig.IsSuffleTurnedOn && RegistryConfig.MediaList.Count > 1)
            {
                Random rnd = new Random();
                List<string> source = new List<string>(RegistryConfig.MediaList);
                while (source.Count > 0)
                {
                    if (source.Count == 1)
                    {
                        mMedias.Add(source[0]);
                        source.Clear();
                    }
                    else
                    {
                        int index = rnd.Next(0, source.Count);
                        mMedias.Add(source[index]);
                        source.RemoveAt(index);
                    }
                }
            }
            else
            {
                mMedias.AddRange(RegistryConfig.MediaList);
            }

            mMediaPlayers[0].MediaFailed += MediaPlayer_MediaFailed;
            mMediaPlayers.ForEach(i => 
            {
                if (RegistryConfig.IsMuted) i.IsMuted = true;
                i.MediaEnded += MediaPlayer_MediaEnded;
            });

            PlayNext();
        }

        public bool IsMuted
        {
            get { return MediaPlayer.IsMuted; }
            set { MediaPlayer.IsMuted = value; }
        }

        private static void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            PlayNext();
        }

        private static void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (Interlocked.Increment(ref mMediaPlayersEnd) == mMediaPlayers.Count)
            {
                PlayNext();
            }
        }

        private static void PlayNext()
        {
            if (mMedias.Count > 0)
            {
                string file = mMedias[0];
                if (mMedias.Count > 1)
                {
                    mMedias.RemoveAt(0);
                    mMedias.Add(file);
                }
                mMediaPlayersEnd = 0;
                mMediaPlayers.ForEach(i =>
                {
                    i.Source = new Uri(file, UriKind.Absolute);
                    i.Play();
                });
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
            MediaPlayer.Stop();
        }

        public void InitializeWindowEvents()
        {
            this.MouseDown += (s, e) => { Application.Current.Shutdown(); };
            this.MouseMove += (s, e) =>
            {
                if (mStartupTime.AddSeconds(Consts.MOUSE_EVENT_REACTION_DELAY).Ticks < DateTime.Now.Ticks)
                    Application.Current.Shutdown();
            };
            this.PreviewMouseMove += (s, e) =>
            {
                if (mStartupTime.AddSeconds(Consts.MOUSE_EVENT_REACTION_DELAY).Ticks < DateTime.Now.Ticks)
                    Application.Current.Shutdown();
            };
            this.PreviewKeyDown += (s, e) => { Application.Current.Shutdown(); };
            this.KeyDown += (s, e) => { Application.Current.Shutdown(); };
        }

    }

}
