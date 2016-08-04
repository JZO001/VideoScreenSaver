using System;
using System.Windows;
using System.Windows.Input;

namespace VideoScreenSaver
{
    /// <summary>
    /// Interaction logic for BlackWindow.xaml
    /// </summary>
    public partial class BlackWindow : Window, IInitializable
    {

        private readonly DateTime mStartupTime = DateTime.Now;

        public BlackWindow()
        {
            InitializeComponent();
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
