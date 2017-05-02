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

namespace Tymer
{
    /// <summary>
    /// Interaction logic for CreateTimerDialog.xaml
    /// </summary>
    public partial class CreateTimerDialog : Window
    {
        public CreateTimerDialog()
        {
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public TimeSlot NewTimer
        {
            get
            {
                string nameString = nameTextBox.Text;
                string timeString = timeTextBox.Text;
                TimeLength timeLength = new TimeLength();

                if (timeString.Contains("h"))
                {
                    try
                    {
                        timeLength.Hours = Convert.ToInt32(timeString.Split('h')[0]);
                        timeString = timeString.Split('h')[1];
                    }
                    catch
                    {
                        timeLength.Hours = 0;
                    }
                }
                else
                {
                    timeLength.Hours = 0;
                }

                if (timeString.Contains("m"))
                {
                    try
                    {
                        timeLength.Minutes = Convert.ToInt32(timeString.Split('m')[0]);
                        timeString = timeString.Split('m')[1];
                    }
                    catch
                    {
                        timeLength.Minutes = 0;
                    }
                }
                else
                {
                    timeLength.Minutes = 0;
                }

                if (timeString.Contains("s"))
                {
                    try
                    {
                        timeLength.Seconds = Convert.ToInt32(timeString.Split('s')[0]);
                        timeString = timeString.Split('s')[1];
                    }
                    catch
                    {
                        timeLength.Seconds = 0;
                    }
                }
                else
                {
                    timeLength.Seconds = 0;
                }

                return new TimeSlot(nameString, timeLength, (SolidColorBrush)colorSquare.Fill, alertCheckBox.IsChecked.GetValueOrDefault());
            }
        }

        private void colorDialogButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            var result = cd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                colorSquare.Fill = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
            }
            else
            {
                colorSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            }
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
