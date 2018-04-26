using StudentTask.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                    
                }
            }
            catch (Exception)
            {
                // TODO: Exception handling
            }
        }

        private async void AddTask()
        {
            var newTask = new Task();
            NewTaskDatePicker.MinDate=DateTimeOffset.Now;
            AddTaskContentDialog.DataContext = newTask;

            var result = await AddTaskContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    newTask.TaskStatus = Task.Status.Added;
                    newTask.Users = new List<User>{ ViewModel.SessionUser };
                    Task addedTask;
                    if ((addedTask = await DataSource.Tasks.Instance.AddTask(newTask)) != null)
                    {
                        ViewModel.Tasks.Add(addedTask);
                    }
                }
                catch (Exception)
                {
                    // TODO: Exception handling.
                }
            }
        }

        private void TasksListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTask = (Task) TasksListView.SelectedItem;
            DetailsPanel.Visibility = selectedTask == null ? Visibility.Collapsed : Visibility.Visible;
            DueDateText.Visibility = selectedTask != null && selectedTask.DueDate == null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
