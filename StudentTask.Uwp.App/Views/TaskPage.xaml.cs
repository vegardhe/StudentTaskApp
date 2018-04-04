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
using StudentTask.Model;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TaskPage : Page
    {

        public TaskPage()
        {
            this.InitializeComponent();
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditSplitView.IsPaneOpen = !EditSplitView.IsPaneOpen;
        }

        private async void SaveTask()
        {
            var changedTask = (Task)TasksListView.SelectedItem;
            if(changedTask.TaskStatus == Task.Status.Finished)
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
            AddTaskContentDialog.DataContext = newTask;

            var result = await AddTaskContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    newTask.TaskStatus = Task.Status.Added;
                    Task addedTask;
                    if ((addedTask = await DataSource.Tasks.Instance.AddTask(newTask)) != null)
                    {
                        ViewModel.Tasks.Add(addedTask);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Exception handling.
                }
            }
        }
    }
}
