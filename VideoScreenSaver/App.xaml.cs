using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace VideoScreenSaver
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private HwndSource mWindownWithWPFContent;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string firstArgument = string.Empty;

            if (e.Args.Length > 0)
            {
                firstArgument = e.Args[0].ToLower().Trim();
            }

            if (string.Empty.Equals(firstArgument) || firstArgument.StartsWith("/s"))
            {
                List<IInitializable> windows = new List<IInitializable>();
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (RegistryConfig.IsShowMediaOnAllScreen)
                    {
                        MainWindow window = new MainWindow();
                        window.Left = screen.Bounds.Left;
                        window.Top = screen.Bounds.Top;
                        window.Width = screen.Bounds.Width;
                        window.Height = screen.Bounds.Height;
                        windows.Add(window);
                        if (screen != Screen.PrimaryScreen) window.IsMuted = true;
                        window.Show();
                    }
                    else
                    {
                        if (screen == Screen.PrimaryScreen)
                        {
                            MainWindow window = new MainWindow();
                            window.Left = screen.Bounds.Left;
                            window.Top = screen.Bounds.Top;
                            window.Width = screen.Bounds.Width;
                            window.Height = screen.Bounds.Height;
                            windows.Add(window);
                            window.Show();
                        }
                        else
                        {
                            BlackWindow window = new BlackWindow();
                            window.Left = screen.Bounds.Left;
                            window.Top = screen.Bounds.Top;
                            window.Width = screen.Bounds.Width;
                            window.Height = screen.Bounds.Height;
                            windows.Add(window);
                            window.Show();
                        }
                    }
                }

                VideoScreenSaver.MainWindow.Start();
                windows.ForEach(i => i.InitializeWindowEvents());
                windows.Clear();
            }
            else if (e.Args[0].ToLower().StartsWith("/p"))
            {
                // Handle cases where arguments are separated by colon. 
                // Examples: /c:1234567 or /P:1234567
                string secondArgument = string.Empty;
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (e.Args.Length > 1)
                {
                    secondArgument = e.Args[1];
                }

                long parentWindowHandle = 0;
                if (!string.IsNullOrEmpty(secondArgument) && long.TryParse(secondArgument, out parentWindowHandle))
                {
                    MainWindow window = new MainWindow();
                    Int32 previewHandle = Convert.ToInt32(parentWindowHandle);
                    IntPtr pPreviewHnd = new IntPtr(previewHandle);
                    RECT lpRect = new RECT();
                    bool bGetRect = Win32API.GetClientRect(pPreviewHnd, ref lpRect);

                    HwndSourceParameters sourceParameters = new HwndSourceParameters("sourceParams");
                    sourceParameters.PositionX = 0;
                    sourceParameters.PositionY = 0;
                    sourceParameters.Height = lpRect.Bottom - lpRect.Top;
                    sourceParameters.Width = lpRect.Right - lpRect.Left;
                    sourceParameters.ParentWindow = pPreviewHnd;
                    sourceParameters.WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);

                    mWindownWithWPFContent = new HwndSource(sourceParameters);
                    mWindownWithWPFContent.Disposed += (o, args) => window.Close();
                    mWindownWithWPFContent.RootVisual = window.MainGrid;

                    VideoScreenSaver.MainWindow.Start();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Missing or invalid parent windows handle.", "Video Screen Saver", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Current.Shutdown();
                }
            }
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                ConfigurationWindow window = new VideoScreenSaver.ConfigurationWindow();
                window.ShowDialog();
            }
        }

    }

}
