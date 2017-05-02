using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;

namespace Tymer
{
    public class TimeSlot : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public TimeLength Length { get; set; }
        public SolidColorBrush Color { get; set; }
        public bool Alert { get; set; }

        public TimeSlot(string Name, TimeLength Length, SolidColorBrush Color, bool Alert)
        {
            this.Name = Name;
            this.Length = Length;
            this.Color = Color;
            this.Alert = Alert;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }

    public struct TimeLength
    {
        public int Hours;
        public int Minutes;
        public int Seconds;

        public override string ToString()
        {
            string returnString = "";
            if (Hours > 0)
            {
                returnString += Hours + "h";
            }
            if (Minutes > 0)
            {
                returnString += Minutes + "m";
            }
            if (Seconds > 0)
            {
                returnString += Seconds + "s";
            }
            return returnString;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<TimeSlot> times { get; set; }
        BackgroundWorker workerTimer = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            times = new ObservableCollection<TimeSlot>();
            timesListView.ItemsSource = times;
            this.DataContext = this;

            workerTimer.WorkerReportsProgress = true;
            workerTimer.WorkerSupportsCancellation = true;
            workerTimer.DoWork += new DoWorkEventHandler(workerTimerWork);
            workerTimer.ProgressChanged += new ProgressChangedEventHandler(workerTimerProg);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new ListViewDragDropManager<TimeSlot>(timesListView);
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTimerDialog inputDialog = new CreateTimerDialog();

            if (inputDialog.ShowDialog() == true)
            {
                times.Add(inputDialog.NewTimer);
            }
        }

        private void timesListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (timesListView.SelectedItems.Count > 0)
                {
                    times.Remove((TimeSlot)timesListView.SelectedItems[0]);
                }
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (workerTimer.IsBusy)
            {
                workerTimer.CancelAsync();
                startButton.Content = "Start";
            }
            else
            {
                workerTimer.RunWorkerAsync();
                startButton.Content = "Stop";
            }
        }

        private void workerTimerWork(object sender, DoWorkEventArgs e)
        {
            while (!workerTimer.CancellationPending && times.Count != 0)
            {
                foreach (TimeSlot timeslot in times)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MainWindow1.Title = "Tymer (" + timeslot.Name + ")";
                        progBar.Visibility = Visibility.Visible;
                        progBar.Foreground = timeslot.Color;
                    }));
                    DateTime startTime = DateTime.Now;
                    DateTime finishTime = DateTime.Now.AddHours(timeslot.Length.Hours).AddMinutes(timeslot.Length.Minutes).AddSeconds(timeslot.Length.Seconds);
                    while (finishTime > DateTime.Now && !workerTimer.CancellationPending)
                    {
                        int percent = 100 - (int)(((finishTime - DateTime.Now).TotalMilliseconds / (finishTime - startTime).TotalMilliseconds) * 100);
                        workerTimer.ReportProgress(percent);
                        Thread.Sleep(50);
                    }

                    // end of timer
                    if (!workerTimer.CancellationPending)
                    {
                        System.Media.SystemSounds.Beep.Play();
                        if (timeslot.Alert == true)
                        {
                            MessageBox.Show("Timer \"" + timeslot.Name + "\" has finished.", "Tymer", MessageBoxButton.OK);
                        }
                    }
                }
            }
            workerTimer.ReportProgress(0);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow1.Title = "Tymer";
                progBar.Visibility = Visibility.Collapsed;
                startButton.Content = "Start";
            }));
        }

        private void workerTimerProg(object sender, ProgressChangedEventArgs e)
        {
            progBar.Value = e.ProgressPercentage;
        }
    }
}
