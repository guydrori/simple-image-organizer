using Microsoft.WindowsAPICodePack.Dialogs;
using SimpleImageOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleImageOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty CurrentFolderProperty =
            DependencyProperty.Register("CurrentFolder", typeof(string), typeof(MainWindow),new UIPropertyMetadata(string.Empty));

        public string CurrentFolder { get { return (string)this.GetValue(CurrentFolderProperty); } private set { this.SetValue(CurrentFolderProperty, value); } }

        public static readonly DependencyProperty CurrentImageIndexProperty =
           DependencyProperty.Register("CurrentImageIndex", typeof(int), typeof(MainWindow), new UIPropertyMetadata(0));

        public int CurrentImageIndex { get { return (int)this.GetValue(CurrentImageIndexProperty); } private set { this.SetValue(CurrentImageIndexProperty, value); } }

        public static readonly DependencyProperty ImageCountProperty =
           DependencyProperty.Register("ImageCount", typeof(int), typeof(MainWindow), new UIPropertyMetadata(0));

        public int ImageCount { get { return (int)this.GetValue(ImageCountProperty); } private set { this.SetValue(ImageCountProperty, value); } }

        private ICollection<SubfolderEntry> _subfolders = new ObservableCollection<SubfolderEntry>();
        public IEnumerable<SubfolderEntry> Subfolders { get { return _subfolders; } }

        private string _selectedFolderName;
        public string SelectedFolderName
        {
            get { return _selectedFolderName; }
            set
            {
                if (!value.Equals(_selectedFolderName))
                {
                    Console.WriteLine(string.Format("setting folder to: {0}", value));
                    _selectedFolderName = value;
                }
            }
        }

        public MainWindow()
        {
            _subfolders = new ObservableCollection<SubfolderEntry>() {
                   new SubfolderEntry {ShortFolderName="Test",FullPath="Test"}
            };
            InitializeComponent();
            ImageCount = 100;
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
    }
}
