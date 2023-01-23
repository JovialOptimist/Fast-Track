// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Networking.Connectivity;
using Windows.Security.Authentication.Identity.Provider;
using Windows.UI.ViewManagement;
using WMPLib;

using Windows.UI.Core;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Windows.UI.WindowManagement;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WhatAboutaHundredColumns
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        List<string> urlsList = new List<string>
        {
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Mellow Quiet\",
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Mellow Upbeat\",
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Regular Battle\",
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Intense Battle!\",
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Intense Battle!\ABSOLUTE FINAL BOSS DEATH MUSIC\",
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\Special Songs (Situational)\",
            @"C:\Users\jac\OneDrive\Documents\Some Songs for Zac\"
        };
        WindowsMediaPlayer wplayer = new WindowsMediaPlayer();
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer settingsTimer = new DispatcherTimer();
        int seconds = 0;
        double percentageOfSong = 0;
        int weirdChecker = 0;
        int previousValue = 0;

        public MainWindow()
        {
            this.InitializeComponent();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 465, Height = 300 });

            SelectorThing.SelectionChanged += SelectorThing_SelectionChanged;
            wplayer.MediaChange += Wplayer_MediaChange;
            previous.Click += Previous_Click;
            playpause.Click += Playpause_Click;
            skip.Click += Skip_Click;
            settings.Click += Settings_Click;
            Volume.ValueChanged += Volume_ValueChanged;
            previous.Holding += Previous_Holding;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            settingsTimer.Interval = TimeSpan.FromSeconds(1);
            settingsTimer.Tick += SettingsTimer_Tick;


            /*******************************
             * A WHOLE BUNCH OF STOLEN CODE
             * 
             * it's about title bar coloring
             * 
             * 
             * ****************************/
            Microsoft.UI.Windowing.AppWindow m_AppWindow = GetAppWindowForCurrentWindow();
            m_AppWindow.Title = "Jac's Music Player";

            //if (Microsoft.UI.Windowing.AppWindowTitleBar.IsCustomizationSupported())
            //{
            //    if (m_AppWindow is null)
            //    {
            //        m_AppWindow = GetAppWindowForCurrentWindow();
            //    }
            //    var titleBar = m_AppWindow.TitleBar;

            //    // Set active window colors
            //    titleBar.ForegroundColor = Colors.White;
            //    titleBar.BackgroundColor = Colors.BlueViolet;
            //    titleBar.ButtonForegroundColor = Colors.White;
            //    titleBar.ButtonBackgroundColor = Colors.Violet;
            //    titleBar.ButtonHoverForegroundColor = Colors.DarkCyan;
            //    titleBar.ButtonHoverBackgroundColor = Colors.White;
            //    titleBar.ButtonPressedForegroundColor = Colors.Gray;
            //    titleBar.ButtonPressedBackgroundColor = Colors.LightGreen;

            //    // Set inactive window colors
            //    titleBar.InactiveForegroundColor = Colors.Gainsboro;
            //    titleBar.InactiveBackgroundColor = Colors.SeaGreen;
            //    titleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
            //    titleBar.ButtonInactiveBackgroundColor = Colors.SeaGreen;
            //}
        }

        private Microsoft.UI.Windowing.AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(wndId);
        }


        private void SettingsTimer_Tick(object sender, object e) {
            if (!(settingsTimer.Interval.Ticks % 2 == 0))
            {
                previousValue = wplayer.settings.volume;
            }
            else
            {
                if (previousValue == wplayer.settings.volume)
                {
                    Volume.Visibility = Visibility.Collapsed;
                    settingsTimer.Stop();
                }
                else
                {
                    previousValue = wplayer.settings.volume;
                }
            }
        }

        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            wplayer.settings.volume = (int)Volume.Value;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (Volume.Visibility == Visibility.Visible)
            {
                Volume.Visibility = Visibility.Collapsed;
            }
            else
            {
                Volume.Visibility = Visibility.Visible;
            }
            settingsTimer.Start();
        }

        private void Previous_Holding(object sender, HoldingRoutedEventArgs e)
        {
        }

        public static string formatSeconds(int seconds)
        {
            int minutes = 0;
            while ((seconds - 60) >= 0)
            {
                seconds -= 60;
                minutes++;
            }
            if (seconds >= 10)
            {
                return minutes + ":" + seconds;
            }
            else
            {
                return minutes + ":0" + seconds;
            }
        }


        private void Timer_Tick(object sender, object e)
        {
            seconds += 1;
            percentageOfSong = (seconds / wplayer.currentMedia.duration) * 100;
            try
            {
                DurationSlider.Value = percentageOfSong;
            } catch (Exception ex)
            {
            }

            timeElapsed.Text = formatSeconds(seconds);
            timeRemaining.Text = formatSeconds((int)wplayer.currentMedia.duration - seconds);

            if (percentageOfSong >= 100)
            {
                seconds = 0;
                percentageOfSong = 0;
            }

        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            wplayer.controls.next();
            seconds = 0;
            percentageOfSong = 0;
            DurationSlider.Value = 0;
        }

        private void Playpause_Click(object sender, RoutedEventArgs e)
        {
            if (wplayer.playState == WMPPlayState.wmppsPlaying)
            {
                wplayer.controls.pause();
            }
            else
            {
                wplayer.controls.play();
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            wplayer.controls.previous();
            seconds = 0;
            percentageOfSong = 0;
            DurationSlider.Value = 0;
        }

        private void Wplayer_MediaChange(object Item)
        {
            if (!(titleTextBox.Text == wplayer.currentMedia.name))
            {
                String bar = wplayer.currentMedia.name;
                String[] bar2 = bar.Split(' ');


                if ((bar.Length > 30) || (bar2.Length >= 6))
                {
                    bar = bar.Substring(0, 22) + "...";
                }
                titleTextBox.Text = bar;
                timer.Start();

                String foo = wplayer.currentMedia.getItemInfo("album");
                foo = foo.Split("Original Soundtrack")[0];
                foo = foo.Split("ORIGINAL SOUNDTRACK")[0];
                foo = foo.Split(" - ")[0];
                foo = foo.Split(" ~ ")[0];
                foo = foo.Split("~")[0];
                foo = foo.Split("Soundtrack")[0];
                foo = foo.Split("OST")[0];
                foo = foo.Split("Original")[0];
                foo = foo.Split("Gamerip")[0];
                foo = foo.Split("Official")[0];
                foo = foo.Split("Music: ")[0];
                foo = foo.Split("Sound")[0];
                foo = foo.Split("&")[0];
                if (foo == "")
                {
                    foo = "Unknown Artist";
                }
                gameTextBox.Text = foo;
                DurationSlider.Value = 0;
                seconds = 0;
                percentageOfSong = 0;
            }
            else
            {
                // false alarm
            }
            

            //String bar = wplayer.currentMedia.getItemInfo(""));
        }

        private void SelectorThing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            wplayer.controls.stop();
            PlayPlaylistIndex(SelectorThing.SelectedIndex, urlsList, wplayer);
        }
        public static void PlayPlaylistIndex(int index, List<string> urlsList, WindowsMediaPlayer wplayer)
        {
            string fileStartGibberish = urlsList[index];
            string[] filePathsArray = Directory.GetFiles(fileStartGibberish, "*.mp3", SearchOption.AllDirectories);
            List<string> filePathsList = new List<string>(filePathsArray);



            IWMPPlaylist masterPlaylist = wplayer.playlistCollection.newPlaylist("Master Playlist");
            int numOfSongs = filePathsArray.Length;

            Random rand = new Random();
            for (int i = 0; i < numOfSongs; i++)
            {
                // Add songs randomly
                string randomListItem = filePathsList[rand.Next(0, filePathsList.Count)];
                IWMPMedia songToLoad = wplayer.newMedia(randomListItem);
                masterPlaylist.appendItem(@songToLoad);
                filePathsList.Remove(randomListItem);
            }

            wplayer.currentPlaylist = masterPlaylist;
            string currentName = wplayer.currentMedia.name;
            int playlistCount = wplayer.currentPlaylist.count;
            wplayer.controls.next();

        }
    }
}
