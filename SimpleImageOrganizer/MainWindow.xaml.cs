using Microsoft.WindowsAPICodePack.Dialogs;
using SimpleImageOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimpleImageOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex IMAGE_PATTERN = new Regex(".*\\.((jpg)|(png)|(JPEG)|(JPG)|(PNG))");

        public static readonly DependencyProperty CurrentFolderProperty =
            DependencyProperty.Register("CurrentFolder", typeof(string), typeof(MainWindow),new UIPropertyMetadata(string.Empty));

        public string CurrentFolder { get { return (string)this.GetValue(CurrentFolderProperty); } private set {
                this.SetValue(CurrentFolderProperty, value);
                this.OnFolderChange(value);
            } }

        private int CurrentImageIndex { get; set; }

        public static readonly DependencyProperty CurrentImageDisplayIndexProperty =
           DependencyProperty.Register("CurrentImageDisplayIndex", typeof(int), typeof(MainWindow), new UIPropertyMetadata(0));

        public int CurrentImageDisplayIndex { get { return (int)this.GetValue(CurrentImageDisplayIndexProperty); } private set { this.SetValue(CurrentImageDisplayIndexProperty, value); } }

        public static readonly DependencyProperty ImageCountProperty =
           DependencyProperty.Register("ImageCount", typeof(int), typeof(MainWindow), new UIPropertyMetadata(0));

        public int ImageCount { get { return (int)this.GetValue(ImageCountProperty); } private set { this.SetValue(ImageCountProperty, value); } }

        private ICollection<SubfolderEntry> _subfolders = new ObservableCollection<SubfolderEntry>();
        public IEnumerable<SubfolderEntry> Subfolders { get { return _subfolders; } }

        public string SelectedFolderName { get; set; }

        public static readonly DependencyProperty CurrentImageProperty =
           DependencyProperty.Register("CurrentImage", typeof(ImageSource), typeof(MainWindow), new UIPropertyMetadata(null));

        public ImageSource CurrentImage { get { return (ImageSource)this.GetValue(CurrentImageProperty); } private set { this.SetValue(CurrentImageProperty, value); } }

        public ICollection<string> Images { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog folderBrowserDialog = new CommonOpenFileDialog())
            {
                folderBrowserDialog.IsFolderPicker = true;
                folderBrowserDialog.Multiselect = false;
                folderBrowserDialog.EnsurePathExists = true;
                folderBrowserDialog.Title = Properties.Resources.FolderSelectionTitle;
                folderBrowserDialog.ShowPlacesList = true;
                CommonFileDialogResult result = folderBrowserDialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    CurrentFolder = folderBrowserDialog.FileName;
                }
            }
        }

        private void OnNewFolderButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentFolder))
            {
                NewFolderWindow newFolderWindow = new NewFolderWindow(_subfolders, CurrentFolder);
                newFolderWindow.ShowDialog();
            }
        }

        private void OnFolderChange(string newFolder)
        {
            bool enableButton = !string.IsNullOrEmpty(newFolder);
            MoveToButton.IsEnabled = enableButton;
            NewFolderButton.IsEnabled = enableButton;
            if (Directory.Exists(newFolder))
            {
                IEnumerable<string> subdirectories = Directory.EnumerateDirectories(newFolder);
                IEnumerable<SubfolderEntry> subfolderEntries = subdirectories.Select(path => {
                    return new SubfolderEntry()
                    {
                        ShortFolderName = path.Substring(path.LastIndexOf('\\') + 1),
                        FullPath = path
                    };
                });
                _subfolders.Clear();
                foreach(SubfolderEntry entry in subfolderEntries)
                {
                    _subfolders.Add(entry);
                }
                IEnumerable<string> files = Directory.EnumerateFiles(newFolder);
                Images = files.Where(path => IMAGE_PATTERN.IsMatch(path)).ToList();
                ImageCount = Images.Count();
                if (ImageCount > 0)
                {
                    CurrentImageIndex = 0;
                    CurrentImageDisplayIndex = 1;
                    CurrentImage = GetCurrentImageSource();
                    NextImageButton.IsEnabled = CurrentImageDisplayIndex < ImageCount;
                }
            }
        }

        private void OnNextImageButtonClick(object sender, RoutedEventArgs e)
        {
            if (ImageCount > 0)
            {
                CurrentImageIndex++;
                CurrentImageDisplayIndex++;
                CurrentImage = GetCurrentImageSource();
                if (CurrentImageDisplayIndex == ImageCount)
                {
                    NextImageButton.IsEnabled = false;
                }
                if (CurrentImageDisplayIndex > 1)
                {
                    PreviousImageButton.IsEnabled = true;
                }
            }
        }

        private void OnPreviousImageButtonClick(object sender, RoutedEventArgs e)
        {
            if (ImageCount > 0)
            {
                CurrentImageIndex--;
                CurrentImageDisplayIndex--;
                CurrentImage = GetCurrentImageSource();
                if (CurrentImageDisplayIndex == 1)
                {
                    PreviousImageButton.IsEnabled = false;
                }
                if (CurrentImageDisplayIndex < ImageCount)
                {
                    NextImageButton.IsEnabled = true;
                }
            }
        }

        private void OnMoveToButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedFolderName == null || SelectedFolderName.Trim().Length == 0)
            {
                MessageBox.Show("A destination folder must be selected");
                return;
            }
            string currentImagePath = Images.ElementAt(CurrentImageIndex);
            if (File.Exists(currentImagePath) && Directory.Exists(CurrentFolder + "\\" + SelectedFolderName))
            {
                string fileName = currentImagePath.Substring(currentImagePath.LastIndexOf("\\") + 1);
                Images.Remove(currentImagePath);
                ImageCount--;
                if (CurrentImageDisplayIndex >= ImageCount)
                {
                    CurrentImageDisplayIndex = ImageCount;
                    CurrentImageIndex = ImageCount - 1;
                }
                CurrentImage = GetCurrentImageSource();
                PreviousImageButton.IsEnabled = CurrentImageDisplayIndex > 1;
                NextImageButton.IsEnabled = CurrentImageDisplayIndex < ImageCount;
                File.Move(currentImagePath, CurrentFolder + "\\" + SelectedFolderName + "\\" + fileName);
            }
        }

        private ImageSource GetCurrentImageSource()
        {
            if (ImageCount > 0)
            {
                Uri currentImageUri = new Uri(Images.ElementAt(CurrentImageIndex));
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = currentImageUri;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            return null;
        }
    }
}
