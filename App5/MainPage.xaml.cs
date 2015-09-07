using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.Collections;
using OxyPlot;
using OxyPlot.Windows;
using OxyPlot.Series;
using OxyPlot.Axes;
using Windows.ApplicationModel.DataTransfer;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace App5
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public double maxValue = 0.0;

        public Int32 maxIndex = 0;

        public static ArrayList results = new ArrayList();

        //public static ArrayList Results
        //{
        //    get;set;
        //}

        public int xbins, ybins, zbins, xcorofMax, ycorofMax, zcorofMax;

        public double xmin, ymin, zmin, xmax, ymax, zmax, xwidth, ywidth, zwidth;

        private void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = this.Parent as Frame;
            rootFrame.Navigate(typeof(GetFactorPage));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            lineModel = new MainViewModel();
            setLineModel();
            LineSeries yLine = new LineSeries();
            yLine.Title = "X=" + xcorofMax.ToString() + " ,Z=" + zcorofMax.ToString() + "时结果随Y轴变化对数曲线";
            for (int i = 0; i < ybins; i++)
            {
                int index = zcorofMax * ybins * xbins + i * xbins + xcorofMax;
                double currentY = ymin + i * ywidth;
                yLine.Points.Add(new DataPoint(currentY, Convert.ToDouble(results[index])));
            }
            lineModel.MyModel.Series.Add(yLine);
            this.DataContext = new MainViewModel();
            this.DataContext = lineModel;
        }

        private MainViewModel lineModel = new MainViewModel();

        private void MenuButton3_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame !=null)
            {
                this.Frame.Navigate(typeof(PlotPage));
            }
       
        }

        private MainViewModel logModel = new MainViewModel();

        private void setLogModel()
        {
            LinearAxis AxiX = new LinearAxis();            
            LogarithmicAxis AxiY = new LogarithmicAxis();            
            AxiX.Position = AxisPosition.Bottom;         
            AxiY.Position = AxisPosition.Left;
            this.logModel.MyModel.Axes.Add(AxiX);
            this.logModel.MyModel.Axes.Add(AxiY);
        }

        private void setLineModel()
        {
            LinearAxis AxiX = new LinearAxis();
            LinearAxis AxiY = new LinearAxis();
            AxiX.Position = AxisPosition.Bottom;
            AxiY.Position = AxisPosition.Left;
            this.lineModel.MyModel.Axes.Add(AxiX);
            this.lineModel.MyModel.Axes.Add(AxiY);
        }

        void OnUICommandNoClicked(IUICommand cmd)
        {
            Frame rootFrame = this.Parent as Frame;
            rootFrame.Navigate(typeof(GetFactorPage));
            return;
        }

        void OnUICommandYesClicked(IUICommand cmd)
        {
            factor = 1;
        }

        double factor = App5.GetFactorPage.NormalizeFactor;

        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxXMax.Text=="")
            {
                MessageDialog dlg = new MessageDialog("请首先选择USRBIN文件载入", "未选择USRBIN文件");
                await dlg.ShowAsync();
                return;
            }
            if (App5.GetFactorPage.NormalizeFactor!=0)
            {
                factor = App5.GetFactorPage.NormalizeFactor;
            }
            if (factor==0)
            {
                 MessageDialog dlg = new MessageDialog("未选择粒子系数，点击确定按入射粒子数为1继续计算，点击取消则导航至系数换算页面取得粒子系数", "未选择粒子系数");
                 UICommand yescmd = new UICommand("确定", new UICommandInvokedHandler(OnUICommandYesClicked));
                 UICommand nocmd = new UICommand("取消", new UICommandInvokedHandler(OnUICommandNoClicked));
                 dlg.Commands.Add(yescmd);
                 dlg.Commands.Add(nocmd);
                 await dlg.ShowAsync();
                 return;
            }   
            maxValue = 0.0;
            maxIndex = 0;
            for (int i = 0; i < numbersOfResults; i++)
            {
                if (Convert.ToDouble(results[i])>maxValue)
                {
                    maxValue = Convert.ToDouble(results[i]);
                    maxIndex = i;
                }
            }
            maxValue *= factor;
            zcorofMax = maxIndex / (xbins * ybins);
            ycorofMax = (maxIndex % (xbins * ybins)) / xbins;
            xcorofMax = (maxIndex % (xbins * ybins)) % xbins;
            string contents;
            contents = "最大值为： " + maxValue.ToString() + "  最大值索引序数为：" + (maxIndex + 1).ToString() + "  三维坐标为（" + xcorofMax.ToString() + ", " + ycorofMax.ToString() + ", " + zcorofMax.ToString() + "）"; 
            MessageDialog showResult = new MessageDialog(contents, "最大值及其坐标：");
            await showResult.ShowAsync();
        }

        int numbersOfResults = 0;
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        public async void button1_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".usrbin");
            StorageFile file = await openPicker.PickSingleFileAsync();
            try
            {
                IList<string> fileContents = await FileIO.ReadLinesAsync(file);
                ArrayList parameters = new ArrayList();
                results = new ArrayList();
                double f;
                numbersOfResults = 0;
                for (int i = 2; i < 5; i++)
                {
                    string[] currentLineSplits = fileContents[i].Split();
                    foreach (string s in currentLineSplits)
                    {
                        try
                        {
                            f = Convert.ToDouble(s);
                        }
                        catch (FormatException)
                        {
                            continue;
                        }
                        parameters.Add(f);
                    }
                }
                // 初始化X、Y、Z的范围（最大最小值），道数和道宽，并将其显示在面板对应的位置
                textBoxXMin.Text = parameters[0].ToString();
                xmin = Convert.ToDouble(parameters[0]);
                textBoxYMin.Text = parameters[4].ToString();
                ymin = Convert.ToDouble(parameters[4]);
                textBoxZMin.Text = parameters[8].ToString();
                zmin = Convert.ToDouble(parameters[8]);
                textBoxXMax.Text = parameters[1].ToString();
                xmax = Convert.ToDouble(parameters[1]);
                textBoxYMax.Text = parameters[5].ToString();
                ymax = Convert.ToDouble(parameters[5]);
                textBoxZMax.Text = parameters[9].ToString();
                zmax = Convert.ToDouble(parameters[9]);
                textBoxXbins.Text = parameters[2].ToString();
                xbins = Convert.ToInt32(parameters[2]);
                textBoxYbins.Text = parameters[6].ToString();
                ybins = Convert.ToInt32(parameters[6]);
                textBoxZbins.Text = parameters[10].ToString();
                zbins = Convert.ToInt32(parameters[10]);
                xwidth = Convert.ToDouble(parameters[3]);
                ywidth = Convert.ToDouble(parameters[7]);
                zwidth = Convert.ToDouble(parameters[11]);
                // 读入结果数据，并存储到results中
                int numbers = xbins * ybins * zbins; // 用于存储数据表格的总行数
                                                     // 由于USRBIN结果存储方式为10个数据一行，因此通过这种方式判断出所需要的行数
                if ((numbers % 10) != 0)
                {
                    numbers = (numbers / 10) + 1;
                }
                else
                {
                    numbers = numbers / 10;
                }
                for (int i = 0; i < numbers; i++)
                {
                    string[] splits = fileContents[i + 9].Split();
                    for (int j = 7; j < splits.Length; j += 2)
                    {
                        f = Convert.ToDouble(splits[j]);
                        results.Add(f);
                        numbersOfResults++;
                    }
                }
            }
            catch (NullReferenceException)
            {
                return;
                //throw;
            }
            
            //int currentLine = 2;
            //string[] currentLineSplits = fileContents[currentLine].Split();
            //ArrayList parameters = new ArrayList();
            //results = new ArrayList();
            //double f;
            //numbersOfResults = 0;
            //for (int i = 2; i < 5; i++)
            //{
            //    string[] currentLineSplits = fileContents[i].Split();
            //    foreach (string s in currentLineSplits)
            //    {
            //        try
            //        {
            //            f = Convert.ToDouble(s);
            //        }
            //        catch (FormatException)
            //        {
            //            continue;
            //        }
            //        parameters.Add(f);
            //    }
            //}
            //// 初始化X、Y、Z的范围（最大最小值），道数和道宽，并将其显示在面板对应的位置
            //textBoxXMin.Text = parameters[0].ToString ();
            //xmin = Convert.ToDouble(parameters[0]);
            //textBoxYMin.Text = parameters[4].ToString();
            //ymin = Convert.ToDouble(parameters[4]);
            //textBoxZMin.Text = parameters[8].ToString();
            //zmin = Convert.ToDouble(parameters[8]);
            //textBoxXMax.Text = parameters[1].ToString();
            //xmax = Convert.ToDouble(parameters[1]);
            //textBoxYMax.Text = parameters[5].ToString();
            //ymax = Convert.ToDouble(parameters[5]);
            //textBoxZMax.Text = parameters[9].ToString();
            //zmax = Convert.ToDouble(parameters[9]);
            //textBoxXbins.Text = parameters[2].ToString();
            //xbins = Convert.ToInt32(parameters[2]);
            //textBoxYbins.Text = parameters[6].ToString();
            //ybins = Convert.ToInt32(parameters[6]);
            //textBoxZbins.Text = parameters[10].ToString();
            //zbins = Convert.ToInt32(parameters[10]);
            //xwidth = Convert.ToDouble(parameters[3]);
            //ywidth = Convert.ToDouble(parameters[7]);
            //zwidth = Convert.ToDouble(parameters[11]);
            //// 读入结果数据，并存储到results中
            //int numbers = xbins * ybins * zbins; // 用于存储数据表格的总行数
            //// 由于USRBIN结果存储方式为10个数据一行，因此通过这种方式判断出所需要的行数
            //if ((numbers % 10) != 0)
            //{
            //    numbers = (numbers / 10) + 1;
            //}
            //else
            //{
            //    numbers = numbers / 10;
            //}
            //for (int i = 0; i < numbers; i++)
            //{
            //    string[] splits = fileContents[i + 9].Split();
            //    for (int j = 7; j < splits.Length; j += 2)
            //    {
            //        f = Convert.ToDouble(splits[j]);
            //        results.Add(f);
            //        numbersOfResults++;
            //    }
            //}
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (MySplitView.IsPaneOpen == true)
            {
                MySplitView.IsPaneOpen = false;
                FirstColumn.Width = new Windows.UI.Xaml.GridLength(50);
            }
            else
            {
                MySplitView.IsPaneOpen = true;
                FirstColumn.Width = new Windows.UI.Xaml.GridLength(100);
            }
            //MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            logModel = new MainViewModel();
            setLogModel();
            LineSeries logYLine = new LineSeries();
            logYLine.Title = "X=" + xcorofMax.ToString() + " ,Z=" + zcorofMax.ToString() + "时结果随Y轴变化对数曲线";
            for (int i = 0; i < ybins; i++)
            {
                int index = zcorofMax * ybins * xbins + i * xbins + xcorofMax;
                double currentY = ymin + i * ywidth;
                logYLine.Points.Add(new DataPoint(currentY, Convert.ToDouble(results[index])*factor));
            }
            logModel.MyModel.Series.Add(logYLine);
            this.DataContext = new MainViewModel();
            this.DataContext = logModel;
            //var pdfe = new PdfExporter();
            //DataPackage newc = new DataPackage();
            //var stream = File.Create("aaa");
            //pdfe.Export(logModel.MyModel, stream, 600, 400);
        }
    }
}
