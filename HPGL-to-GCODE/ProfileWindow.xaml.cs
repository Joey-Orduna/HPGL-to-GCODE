﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace HPGL_to_GCODE
{
    /// <summary>
    /// Interaktionslogik für ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        WindowMode mode;
        ProfileManager manager;
        List<Profile> profileList = new List<Profile>();
        public Profile SelectedProfile { get; set; }

        public ProfileWindow(ProfileManager profileManager, WindowMode windowMode)
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;

            mode = windowMode;
            manager = profileManager;

            SetUiLayout();
            UpdateListBox(profileList = profileManager.Profiles);

            ((INotifyCollectionChanged)ProfileListBox.Items).CollectionChanged += ProfileWindow_CollectionChanged;

            if (ProfileListBox.HasItems)
                ProfileListBox.SelectedIndex = 0;
        }

        private void ProfileWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ProfileListBox.SelectedIndex == -1 && ProfileListBox.HasItems)
                ProfileListBox.SelectedIndex = 0;
        }

        private void SetUiLayout()
        {
            if (mode == WindowMode.SelectData)
            {
                NewProfileButton.Visibility = Visibility.Hidden;
                EditProfileButton.Visibility = Visibility.Hidden;
                DuplicateProfileButton.Visibility = Visibility.Hidden;
                DeleteProfileButton.Visibility = Visibility.Hidden;
                ChooseProfileButton.Visibility = Visibility.Visible;

                ProfileListBox.MouseDoubleClick -= EditProfileButton_Click;
                ProfileListBox.MouseDoubleClick += ChooseProfileButton_Click;
            }
            else
            {
                NewProfileButton.Visibility = Visibility.Visible;
                EditProfileButton.Visibility = Visibility.Visible;
                DuplicateProfileButton.Visibility = Visibility.Visible;
                DeleteProfileButton.Visibility = Visibility.Visible;
                ChooseProfileButton.Visibility = Visibility.Hidden;

                ProfileListBox.MouseDoubleClick -= ChooseProfileButton_Click;
                ProfileListBox.MouseDoubleClick += EditProfileButton_Click;
            }
        }

        public enum WindowMode
        {
            SelectData,
            EditData
        }

        private void NewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Profile newProfile = new Profile();
            manager.Profiles.Add(newProfile);
            showSettingsWindow(newProfile);
        }

        private void showSettingsWindow(Profile p)
        {
            SettingsWindow settings = new SettingsWindow(p);
            settings.Closed += Settings_Closed;
            settings.ShowDialog();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileListBox.SelectedIndex == -1)
                return;

            Profile p = profileList[ProfileListBox.SelectedIndex];
            showSettingsWindow(p);
        }

        private void Settings_Closed(object sender, EventArgs e)
        {
            UpdateListBox(profileList);
        }

        private void DeleteProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileListBox.SelectedIndex == -1)
                return;

            if (MessageBoxResult.Yes == 
                MessageBox.Show("Are you sure to remove the selected Profile?", "Please confirm", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                manager.Profiles.RemoveAt(ProfileListBox.SelectedIndex);
                UpdateListBox(profileList);
            }
        }

        private void UpdateListBox(List<Profile> profiles)
        {
            ProfileListBox.Items.Clear();
            foreach (var profile in profiles)
            {
                ProfileListBox.Items.Add(profile.Profilename);
            } 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mode == WindowMode.EditData)
            {
                manager.SaveToFile();
            }
        }

        private void ChooseProfileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedProfile = profileList[ProfileListBox.SelectedIndex];
            Close();
        }

        private void DuplicateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileListBox.SelectedIndex == -1)
                return;

            Profile duplicateProfile = (Profile)manager.Profiles[ProfileListBox.SelectedIndex].Clone();
            duplicateProfile.Profilename += "_copy";
            manager.Profiles.Add(duplicateProfile);
            showSettingsWindow(duplicateProfile);
        }
    }
}
