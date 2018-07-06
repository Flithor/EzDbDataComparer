using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace EasyDatabaseCompare
{
    public class TopMsg : ContentControl
    {
        public TopMsg()
        {
            T = new DispatcherTimer();
            T.Tick += T_Tick;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            T.IsEnabled = false;
            HideAnime.Begin(TopMsgGrid);
        }

        DispatcherTimer T { get; }

        Grid TopMsgGrid { get; set; }
        Border MsgDisplay { get; set; }
        Label MsgContent { get; set; }


        Storyboard ShowAnime { get; set; }
        Storyboard HideAnime { get; set; }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TopMsgGrid = Template.FindName("topMsgGrid", this) as Grid;
            MsgContent = Template.FindName("MsgContent", this) as Label;
            MsgDisplay = Template.FindName("MsgDisplay", this) as Border;
            ShowAnime = Template.Resources["showAnime"] as Storyboard;
            HideAnime = Template.Resources["hideAnime"] as Storyboard;
        }

        public void ShowMessage(string msg, double showTime = 2000)
        {
            if (T.IsEnabled)
            {
                ShowAnime.Stop(TopMsgGrid);
                MsgDisplay.Opacity = 0;
            }
            MsgContent.Content = msg;
            T.Interval = TimeSpan.FromMilliseconds(showTime);
            T.IsEnabled = true;
            ShowAnime.Begin(TopMsgGrid);
        }
    }
}
