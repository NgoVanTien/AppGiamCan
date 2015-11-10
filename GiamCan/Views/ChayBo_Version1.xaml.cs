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
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.System.Threading;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChayBo_Version1 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        //MainPage rootPage = MainPage.Current;
        string path;
        SQLite.Net.SQLiteConnection connection;

        private Accelerometer _accelerometer;
        private uint _desiredReportInterval;
       
        float x, x_old;
        float y, y_old;
        float z, z_old;
        private bool mInitialized;
       
        private const float NOISE = (float)0.5;

        bool isPause = false;//kiem tra co tam dung hay k
        long timeStart = 0; //time bat dau
        long saveTime = 0;
        long timeCount = 0;

        ThreadPoolTimer timeThread;
        //Time
        DispatcherTimer mytimer= new DispatcherTimer();

        public enum MediaState
        {
            Playing,
            Paused
        }
        private MediaState state = MediaState.Playing;
        public ChayBo_Version1()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                // Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
                // This value will be used later to activate the sensor.
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;
            }
            else
            {
                return; 
            }
            
        }

       

        private async void UpdateTime(ThreadPoolTimer timer)
        {
            //while (true)
            //{
                string s1, s2, s3;
                long currentTime = DateTime.Now.Ticks;
                long toDisplay = currentTime - timeStart;
                if (isPause) return;
                DateTime time = DateTime.FromBinary(toDisplay);
                if (isPause) return;
                timeCount = time.Second + time.Minute * 60 + time.Hour * 3600 + saveTime;
                long hour = timeCount / 3600;
                long timeMin = timeCount - (hour * 3600);
                long minutes = timeCount / 60;
                long timeSe = timeCount - (minutes * 60);
                await Dispatcher.RunAsync(
                           CoreDispatcherPriority.High,
                           () =>
                           {
                               if(hour<10)
                               {
                                    s1 = "0" + hour.ToString();
                               }
                               else
                               {
                                   s1 = hour.ToString();
                               }
                               if (minutes < 10)
                               {
                                   s2 = "0" + minutes.ToString();
                               }
                               else
                               {
                                   s2 = minutes.ToString();
                               }
                               if (timeSe < 10)
                               {
                                   s3 = "0" + timeSe.ToString();
                               }
                               else
                               {
                                   s3 = timeSe.ToString();
                               }
                               string s = s1+":"+s2+":"+s3;
                               timeblock.Text= s;
                           });

            //}
        }


      

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ScenarioEnableButton.IsEnabled = true;
            ScenarioDisableButton.IsEnabled = false;
            ScenarioPauseButton.IsEnabled = false;

        }
        /// <summary>
        /// Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        /// Event data that can be examined by overriding code. The event data is representative
        /// of the navigation that will unload the current Page unless canceled. The
        /// navigation can potentially be canceled by setting Cancel.
        /// </param>
        /// 
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (ScenarioDisableButton.IsEnabled)
            {
                Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

                // Restore the default report interval to release resources while the sensor is not in use
                _accelerometer.ReportInterval = 0;
            }

            base.OnNavigatingFrom(e);
        }
        /// <summary>
        /// This is the event handler for VisibilityChanged events. You would register for these notifications
        /// if handling sensor data when the app is not visible could cause unintended actions in the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">
        /// Event data that can be examined for the current visibility state.
        /// </param>
        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (ScenarioDisableButton.IsEnabled)
            {
                if (e.Visible)
                {
                    // Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
                    _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                }
                else
                {
                    // Disable sensor input (no need to restore the default reportInterval... resources will be released upon app suspension)
                    _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                }
            }
        }

        /// <summary>
        /// This is the event handler for ReadingChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 



        private bool hasChanged;
        private int counter;

       

        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerReading reading = e.Reading;
               
                x = (float)reading.AccelerationX;
                y = (float)reading.AccelerationY;
                z = (float)reading.AccelerationZ;
                if (!mInitialized)
                {
                    x_old = x;
                    y_old = y;
                    z_old = z;
                    mInitialized = true;
                }
                else
                {
                    double oldValue = ((x_old * x) + (y_old * y)) + (z_old * z);
                    double oldValueSqrT = Math.Abs(Math.Sqrt((double)(((x_old * x_old) + (y_old * y_old)) + (z_old * z_old))));
                    double newValue = Math.Abs(Math.Sqrt((double)(((x * x) + (y * y)) + (z * z))));
                    oldValue /= oldValueSqrT * newValue;
                    if ((oldValue <= 0.994) && (oldValue > 0.9))
                    {
                        if (!hasChanged)
                        {
                            hasChanged = true;
                            counter++; //here the counter
                            Steps.Text = String.Format("{0,5:####}", counter);
                            Distance.Text = String.Format("{0:0.000}",(double)counter*54/10000);
                        }
                        else
                        {
                            hasChanged = false;
                        }
                    }
                    x_old = x;
                    y_old = y;
                    z_old = z;
                    mInitialized = false;
                    ///Rank
                    if (counter>200)
                    {
                        Rank.Text = String.Format("{0,5:####}", 1);
                    }
                    if(counter>500)
                    {
                        Rank.Text = String.Format("{0,5:####}", 2);
                    }
                    if (counter > 1000)
                    {
                        Rank.Text = String.Format("{0,5:####}", 3);
                    }
                    if (counter > 3000)
                    {
                        Rank.Text = String.Format("{0,5:####}", 4);
                    }
                    if (counter > 5000)
                    {
                        Rank.Text = "Đồng";
                    }
                    if (counter > 7000)
                    {
                        Rank.Text = "Bạc";
                    }
                    if(counter >10000)
                    {
                        Rank.Text = "Vàng";
                    }
                }
                
            });
        }

        /// <summary>
        /// This is the click handler for the 'Enable' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioEnable(object sender, RoutedEventArgs e)
        {
            if (_accelerometer != null)
            {
                // Establish the report interval
                _accelerometer.ReportInterval = _desiredReportInterval;
                isPause = false;
                Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

                ScenarioEnableButton.IsEnabled = true;
                ScenarioPauseButton.IsEnabled = true;
                ScenarioDisableButton.IsEnabled = true;
                timeStart = DateTime.Now.Ticks;
                state = MediaState.Playing;
                ScenarioEnableButton.IsEnabled = false;
                timeThread = ThreadPoolTimer.CreatePeriodicTimer(UpdateTime, TimeSpan.FromMilliseconds(1000));
            }
            else
            {
                return;
            }
        }
        private void ScenarioPause(object sender, RoutedEventArgs e)
        {
            
            
            if (state == MediaState.Paused)
            {
                Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                state = MediaState.Playing;
                ScenarioPauseButton.Content = "Pause";
                isPause = false;
                saveTime = timeCount;
                timeStart = DateTime.Now.Ticks;
            }
            else
            if (state == MediaState.Playing)
            {
                state = MediaState.Paused;
                ScenarioPauseButton.Content = "Continue";
                isPause = true;
                Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                _accelerometer.ReportInterval = 0;
            }
            
        }
        /// <summary>
        /// This is the click handler for the 'Disable' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioDisable(object sender, RoutedEventArgs e)
        {
            Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(VisibilityChanged);
            _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

            // Restore the default report interval to release resources while the sensor is not in use
            _accelerometer.ReportInterval = 0;

            ScenarioEnableButton.IsEnabled = true;
            
            
            
        }
    }
    
}
