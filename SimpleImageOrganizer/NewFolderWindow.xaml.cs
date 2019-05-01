using SimpleImageOrganizer.Models;
using System;
using System.Collections.Generic;
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
            InitializeComponent();
        }
    }
}
