using SimpleImageOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace SimpleImageOrganizer
{
    /// <summary>
    /// Interaction logic for NewFolderWindow.xaml
    /// </summary>
    public partial class NewFolderWindow : Window
    {
        private ICollection<SubfolderEntry> _subfolders;

        private string _currentFolder;

        public NewFolderWindow(ICollection<SubfolderEntry> subfolders, string currentFolder)
        {
            if (subfolders == null) throw new NullReferenceException("Subfolders cannot be null!");
            if (string.IsNullOrEmpty(currentFolder)) throw new NullReferenceException("Current folder must be provided");
            _subfolders = subfolders;
            _currentFolder = currentFolder;
            InitializeComponent();
        }

        private void OnCreateButtonClick(object sender, RoutedEventArgs e)
        {
            if (FolderNameTextBox.Text == null || FolderNameTextBox.Text.Trim().Length == 0)
            {
                MessageBox.Show(Properties.Resources.EmptyNewFolderNameAlert);
                return;
            }
            DirectoryInfo newDirectory = Directory.CreateDirectory(_currentFolder + "\\" + FolderNameTextBox.Text);
            if (newDirectory.Exists)
            {
                _subfolders.Add(new SubfolderEntry()
                {
                    ShortFolderName = newDirectory.Name,
                    FullPath = newDirectory.FullName
                });
            }
            Close();
        }
    }
}
