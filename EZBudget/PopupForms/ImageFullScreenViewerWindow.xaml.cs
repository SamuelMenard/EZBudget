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

namespace EZBudget.PopupForms
{
    /// <summary>
    /// Interaction logic for ImageFullScreenViewerWindow.xaml
    /// </summary>
    public partial class ImageFullScreenViewerWindow : Window
    {

        public string ImageURL { get; set; }
        public ImageFullScreenViewerWindow(string imageURL)
        {
            InitializeComponent();
            this.Title = (Path.GetFileName(imageURL) == "")?"No Image": Path.GetFileName(imageURL);
            this.ImageURL = imageURL;

            this.DataContext = this;
        }
    }
}
