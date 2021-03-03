using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AfterEffectsRendering
{
    public partial class MainWindow : System.Windows.Window
    {

        public Process cmd = new Process();
        FileSystemWatcher watcher;
        string renderfilepath = "C:\\Users\\Public";

        public MainWindow()
        {

            InitializeComponent();
            folderWatcher();
           
        }

        public void AddFiles()
        {
            string[] files = Directory.GetFiles(renderfilepath);
            foreach (string file in files)
            {
                if (System.IO.Path.GetExtension(file.ToString()) == ".png")
                {
                    string filename = System.IO.Path.GetFileName(file.ToString());
                    listBox.Items.Add(filename);
                }
            }
        }

        async Task RenderGraphic()
        {
            try
            {
                await Task.Delay(100);
                string strCmdText;
                strCmdText = "/k \"C:\\Program Files\\Adobe\\Adobe After Effects CC 2019\\Support Files\\aerender.exe\" -reuse -continueOnMissingFootage  -project C:\\Users\\Public\\Test.aep";
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.Arguments = strCmdText;
                cmd.Start();
            }
            catch (Exception ex) { }
        }

        private void button_Click_Close(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex) { }
        }


        private async void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "cmd")

                    {

                        p.Kill();

                    }
                }
               
                    await RenderGraphic();
            }
            catch (Exception ex) { }
        }

        private void listBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listBox.SelectedItem != null)
                {
                    IMAGEPREVIEW.Source = null;
                    string filename = listBox.SelectedItem.ToString();
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(renderfilepath + "\\" + filename);
                    image.EndInit();
                    IMAGEPREVIEW.Source = image;
                }
            }
            catch (Exception ex) { }
        }

        private void folderWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = renderfilepath;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.png*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                listBox.Items.Clear();
                string[] files = Directory.GetFiles(renderfilepath);
                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file.ToString()) == ".png")
                    {
                        string filename = System.IO.Path.GetFileName(file.ToString());
                        listBox.Items.Add(filename);
                    }


                }

                var directory = new DirectoryInfo(renderfilepath);
                var myFile = (from f in directory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();
            });
        }
    }
}
