using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VideoScreenSaver
{

    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window, INotifyPropertyChanged
    {

        private bool mIsMuted = false;
        private bool mIsSuffleTurnedOn = false;
        private bool mIsShowMediaOnAllScreen = true;
        private double mVolume = 1;

        private GeneralCommand mAddCommand = null;

        private GeneralCommand mRemoveCommand = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConfigurationWindow()
        {
            Medias = new ObservableCollection<ListViewItemData>();
            RegistryConfig.MediaList.ForEach(i => Medias.Add(new ListViewItemData() { Name = i }));
            IsMuted = RegistryConfig.IsMuted;
            IsSuffleTurnedOn = RegistryConfig.IsSuffleTurnedOn;
            IsShowMediaOnAllScreen = RegistryConfig.IsShowMediaOnAllScreen;
            Volume = RegistryConfig.Volume * 100;

            InitializeComponent();
        }

        public bool IsMuted
        {
            get { return mIsMuted; }
            set
            {
                mIsMuted = value;
                RaisePropertyChangedEvent("IsMuted");
            }
        }

        public bool IsSuffleTurnedOn
        {
            get { return mIsSuffleTurnedOn; }
            set
            {
                mIsSuffleTurnedOn = value;
                RaisePropertyChangedEvent("IsSuffleTurnedOn");
            }
        }

        public bool IsShowMediaOnAllScreen
        {
            get { return mIsShowMediaOnAllScreen; }
            set
            {
                mIsShowMediaOnAllScreen = value;
                RaisePropertyChangedEvent("IsShowMediaOnAllScreen");
            }
        }

        public double Volume
        {
            get { return mVolume; }
            set
            {
                mVolume = value;
                RaisePropertyChangedEvent("Volume");
            }
        }

        public ObservableCollection<ListViewItemData> Medias { get; private set; }

        public GeneralCommand AddCommand
        {
            get
            {
                return mAddCommand ?? (mAddCommand = new GeneralCommand(AddCommandHandler, IsAddCommandAllowedHandler));
            }
        }

        public GeneralCommand RemoveCommand
        {
            get
            {
                return mRemoveCommand ?? (mRemoveCommand = new GeneralCommand(RemoveCommandHandler, IsRemoveCommandAllowedHandler));
            }
        }

        private void AddCommandHandler(object parameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            bool? result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    Medias.Add(new ListViewItemData() { Name = fileName });
                }
            }
        }

        private bool IsAddCommandAllowedHandler(object parameter)
        {
            return true;
        }

        private void RemoveCommandHandler(object parameter)
        {
            List<ListViewItemData> selectedItems = new List<ListViewItemData>();
            foreach (ListViewItemData item in lvItems.SelectedItems)
            {
                selectedItems.Add(item);
            }
            selectedItems.ForEach(i => Medias.Remove(i));
        }

        private bool IsRemoveCommandAllowedHandler(object parameter)
        {
            return lvItems.SelectedItems.Count > 0;
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            RegistryConfig.IsMuted = IsMuted;
            RegistryConfig.IsShowMediaOnAllScreen = IsShowMediaOnAllScreen;
            RegistryConfig.IsSuffleTurnedOn = IsSuffleTurnedOn;
            RegistryConfig.Volume = Volume / 100d;
            RegistryConfig.MediaList.Clear();
            foreach (ListViewItemData item in Medias)
            {
                RegistryConfig.MediaList.Add(item.Name);
            }
            RegistryConfig.Save();

            Application.Current.Shutdown();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveCommand.RaiseCanExecuteChanged();
        }

    }

}
