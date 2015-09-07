using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
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
using Windows.ApplicationModel.DataTransfer;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace App5
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GetFactorPage : Page
    {
        public GetFactorPage()
        {
            this.InitializeComponent();
        }

        private static double normalizeFactor=0;

        public static double NormalizeFactor { get; set; }

        private int ways = 0;
        private void MenuButton1_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = this.Parent as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (MySplitView.IsPaneOpen == true)
            {
                MySplitView.IsPaneOpen = false;
                FirstColumn.Width = new GridLength(50);
            }
            else
            {
                MySplitView.IsPaneOpen = true;
                FirstColumn.Width = new GridLength(125);
                LastColumn.Width = new GridLength(25);
            }
            //MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void comboBoxSourceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxSourceType.SelectedIndex)
            {
                case 1:
                    {
                        ways = 1;
                        textBlockVar1Tilte.Text = "功率损失";
                        textBlockVar1Tilte.Visibility = (Visibility)0;
                        textBlockVar1Symbol.Text = "P";
                        textBlockVar1Symbol.Visibility = (Visibility)0;
                        textBlockVar1Equal.Visibility = (Visibility)0;
                        textBlockVar1Unit.Visibility = (Visibility)0;
                        textBlockVar1Unit.Text = "W/m";
                        textBlockVar2Title.Text = "粒子能量";
                        textBlockVar2Symbol.Text = "E";
                        comboBoxConlombUnitSelector.Visibility = (Visibility)1;
                        comboBoxcurrentUnitSelector.Visibility = (Visibility)1;
                        comboBoxEnergyUnitSelector.Visibility = (Visibility)0;
                        textBlockVar3Title.Text = "总长度";
                        textBlockVar3Title.Visibility = (Visibility)0;
                        textBlockVar3Symbol.Text = "L";
                        textBlockVar3Symbol.Visibility = (Visibility)0;
                        textBlockVar3Equal.Visibility = (Visibility)0;
                        textBlockVar3Unit.Text = "m";
                        textBlockVar3Unit.Visibility = (Visibility)0;
                        textBoxVar1.Text = "";
                        textBoxVar1.Visibility = (Visibility)0;
                        textBoxVar2.Text = "";
                        textBoxVar3.Text = "";
                        textBoxVar3.Visibility = (Visibility)0;
                        break;
                    }
                case 2:
                    {
                        ways = 2;
                        textBlockVar1Tilte.Text = "";
                        textBlockVar1Tilte.Visibility = (Visibility)1;
                        textBlockVar1Symbol.Text = "";
                        textBlockVar1Symbol.Visibility = (Visibility)1;
                        textBlockVar1Equal.Visibility = (Visibility)1;
                        textBlockVar1Unit.Visibility = (Visibility)1;
                        textBlockVar1Unit.Text = "";
                        textBlockVar2Title.Text = "标称电荷量";
                        textBlockVar2Symbol.Text = "C";
                        comboBoxConlombUnitSelector.Visibility = (Visibility)0;
                        comboBoxcurrentUnitSelector.Visibility = (Visibility)1;
                        comboBoxEnergyUnitSelector.Visibility = (Visibility)1;
                        textBlockVar3Title.Text = "重复频率";
                        textBlockVar3Title.Visibility = (Visibility)0;
                        textBlockVar3Symbol.Text = "f";
                        textBlockVar3Symbol.Visibility = (Visibility)0;
                        textBlockVar3Equal.Visibility = (Visibility)0;
                        textBlockVar3Unit.Text = "Hz";
                        textBlockVar3Unit.Visibility = (Visibility)0;
                        textBoxVar1.Text = "";
                        textBoxVar1.Visibility = (Visibility)1;
                        textBoxVar2.Text = "";
                        textBoxVar3.Text = "";
                        textBoxVar3.Visibility = (Visibility)0;
                        break;
                    }
                default:
                    {
                        ways = 0;
                        textBlockVar1Tilte.Visibility = Visibility.Collapsed;  
                        textBlockVar1Symbol.Visibility = (Visibility)1;
                        textBlockVar1Equal.Visibility = (Visibility)1;
                        textBlockVar1Unit.Visibility = (Visibility)1;                       
                        textBlockVar2Title.Text = "束流流强";
                        textBlockVar2Symbol.Text = "I";
                        comboBoxConlombUnitSelector.Visibility = (Visibility)1;
                        comboBoxcurrentUnitSelector.Visibility = (Visibility)0;
                        comboBoxEnergyUnitSelector.Visibility = (Visibility)1;
                        //textBlockVar3Title.Text = " ";
                        textBlockVar3Title.Visibility = (Visibility)1;
                        //textBlockVar3Symbol.Text = " ";
                        textBlockVar3Symbol.Visibility = (Visibility)1;
                        textBlockVar3Equal.Visibility = (Visibility)1;
                        //textBlockVar3Unit.Text = " ";
                        textBlockVar3Unit.Visibility = (Visibility)1;
                        //textBoxVar1.Text = " ";
                        textBoxVar1.Visibility = (Visibility)1;
                        textBoxVar2.Text = "";
                        //textBoxVar3.Text = " ";
                        textBoxVar3.Visibility = (Visibility)1;
                        break;
                    }                   
            }
        }

        private bool convertReady = true;
        private async void showErrorMessage(TextBox currentTextbox,string textBoxName)
        {
            try
            {
                double test = Convert.ToDouble(currentTextbox.Text);
            }
            catch (FormatException)
            {
                convertReady = false;
                MessageDialog dlg = new MessageDialog(textBoxName+"输入格式不正确，请以3.3E06的形式输入", "输入格式错误");
                await dlg.ShowAsync();
                textBlockFactorNumber.Text = "1";
            }
        }

        //private void GiveData()
        //{
        //    DataPackage Datas = new DataPackage();
        //    Datas.SetText
        //}

        private void buttonConvert_Click(object sender, RoutedEventArgs e)
        {
            switch (ways)
            {
                case 1:
                    {
                        convertReady = true;
                        showErrorMessage(textBoxVar1,"功率设定");
                        showErrorMessage(textBoxVar2,"能量设定");
                        showErrorMessage(textBoxVar3, "长度设定");
                        if (convertReady)
                        {
                            double power = Convert.ToDouble(textBoxVar1.Text) * Convert.ToDouble(textBoxVar3.Text);
                            double energy = Convert.ToDouble(textBoxVar2.Text);
                            double factor = 1E-19;
                            switch (comboBoxEnergyUnitSelector.SelectedIndex)
                            {
                                case 0:
                                    {
                                        factor *= 1E9;
                                        break;
                                    }
                                case 1:
                                    {
                                        factor *= 1E6;
                                        break;
                                    }
                                case 2:
                                    {
                                        factor *= 1E3;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            factor = 1000 / factor;
                            switch (comboBoxFactorUnit.SelectedIndex)
                            {
                                case 0:
                                    {
                                        factor = factor * 1E-12 ;
                                        break;
                                    }
                                case 1:
                                    {
                                        factor = factor * 1E-9;
                                        break;
                                    }
                                case 2:
                                    {
                                        factor = factor * 1E-6;
                                        break;
                                    }
                                case 4:
                                    {
                                        factor = factor * 1E-3;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            NormalizeFactor = 2.25 * power * factor / energy;
                            textBlockFactorNumber.Text = NormalizeFactor.ToString();
                        }
                        else
                        {
                            textBlockFactorNumber.Text = "";
                        }
                        break;
                    }
                case 2:
                    {
                        convertReady = true;
                        showErrorMessage(textBoxVar2, "标称电荷量设定");
                        showErrorMessage(textBoxVar3, "重复频率设定");
                        if (convertReady)
                        {
                            double coulomb = Convert.ToDouble(textBoxVar2.Text);
                            double frequency = Convert.ToDouble(textBoxVar3.Text);
                            coulomb *= frequency;
                            double factor = 1;
                            switch (comboBoxConlombUnitSelector.SelectedIndex)
                            {
                                case 1:
                                    {
                                        factor = 1E-3;
                                        break;
                                    }
                                case 2:
                                    {
                                        factor = 1E-6;
                                        break;
                                    }
                                case 3:
                                    {
                                        factor = 1E-9;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            switch (comboBoxFactorUnit.SelectedIndex)
                            {
                                case 0:
                                    {
                                        factor = factor * 1E-12 * 1E22;
                                        break;
                                    }
                                case 1:
                                    {
                                        factor = factor * 1E-9 * 1E22;
                                        break;
                                    }
                                case 2:
                                    {
                                        factor = factor * 1E-6 * 1E22;
                                        break;
                                    }
                                case 4:
                                    {
                                        factor = factor * 1E-3 * 1E22;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            NormalizeFactor = 2.25 * coulomb * factor;
                            textBlockFactorNumber.Text = NormalizeFactor.ToString();
                        }
                        else
                        {
                            textBlockFactorNumber.Text = "";
                        }
                        break;
                    }
                default:
                    {
                        convertReady = true;
                        showErrorMessage(textBoxVar2,"电流设定");
                        if (convertReady)
                        {
                            double current = Convert.ToDouble(textBoxVar2.Text);
                            double factor = 1;
                            current = current * 2.25;
                            switch (comboBoxcurrentUnitSelector.SelectedIndex)
                            {
                                case 0:
                                    {
                                        break;
                                    }
                                case 1:
                                    {
                                        factor = 1E-3;
                                        break;
                                    }
                                case 2:
                                    {
                                        factor = 1E-6;
                                        break;
                                    }
                                case 3:
                                    {
                                        factor = 1E-9;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            switch (comboBoxFactorUnit.SelectedIndex)
                            {
                                case 0:
                                    {
                                        factor = factor * 1E-12 * 1E22;
                                        break;
                                    }
                                case 1:
                                    {
                                        factor = factor * 1E-9 * 1E22;
                                        break;
                                    }
                                case 2:
                                    {
                                        factor = factor * 1E-6 * 1E22;
                                        break;
                                    }
                                case 4:
                                    {
                                        factor = factor * 1E-3 * 1E22;
                                        break;
                                    }
                                default:
                                    break;
                            }
                            NormalizeFactor = current * factor;
                            textBlockFactorNumber.Text = NormalizeFactor.ToString();
                        }
                        else
                        {
                            textBlockFactorNumber.Text = "";
                        }
                        break;
                    }
            }
            DataPackage datas = new DataPackage();
            datas.SetText(NormalizeFactor.ToString());
            Clipboard.SetContent(datas);
        }
    }

}
