﻿using StudentTask.Model;
using System;
using System.Collections.Generic;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StudentTask.Uwp.App.DataSource;
using Exception = System.Exception;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TaskPage
    {

        public TaskPage()
        {
            InitializeComponent();
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditSplitView.IsPaneOpen = !EditSplitView.IsPaneOpen;
        }

        private async void SaveTask()
        {
            var changedTask = (Task)TasksListView.SelectedItem;
            if(changedTask != null && changedTask.TaskStatus == Task.Status.Finished)
                changedTask.CompletedOn = DateTimeOffset.Now;
            try
            {
                if (await DataSource.Tasks.Instance.UpdateTask(changedTask))
                {
                    EditSplitView.IsPaneOpen = false;
                }
            }
            catch (Exception ex)
            {
                await ex.Display("Saving task failed.");
                await ex.Log();
            }
        }

        private async void AddTask()
        {
            var newTask = new Task();
            newTask.DueDate=DateTimeOffset.Now;
            NewTaskDatePicker.MinDate=DateTimeOffset.Now;
            AddTaskContentDialog.DataContext = newTask;

            var result = await AddTaskContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                newTask.TaskStatus = Task.Status.Added;
                newTask.Users = new List<User> {ViewModel.SessionUser};
                ViewModel.Tasks.Add(await Tasks.Instance.AddTask(newTask));
            }
            catch (WebException ex)
            {
                await ex.Display("Failed to establish internet connection.");
                await ex.Log();
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to add task.");
                await ex.Log();
            }
        }

        private void TasksListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTask = (Task) TasksListView.SelectedItem;
            DetailsPanel.Visibility = selectedTask == null ? Visibility.Collapsed : Visibility.Visible;
            DueDateText.Visibility = selectedTask != null && selectedTask.DueDate == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ToggleFinishedTasks(object sender, RoutedEventArgs e)
        {
            TaskViewSource.Source = TaskViewSource.Source == ViewModel.Tasks ? ViewModel.ActiveTasks : ViewModel.Tasks;
        }
    }
}
