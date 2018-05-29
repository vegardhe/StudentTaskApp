using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using System;
using System.Collections.Generic;
using System.Net;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class TaskPage
    {
        private List<ToastComboBoxItem> ToastTimeList { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskPage" /> class.
        /// </summary>
        public TaskPage()
        {
            InitializeComponent();

            ToastTimeList = new List<ToastComboBoxItem>
            {
                new ToastComboBoxItem("Disabled", -1),
                new ToastComboBoxItem("3 Minutes", 3),
                new ToastComboBoxItem("15 Minutes", 15),
                new ToastComboBoxItem("30 Minutes", 30),
                new ToastComboBoxItem("1 Hour", 60),
                new ToastComboBoxItem("3 Hours", 180),
                new ToastComboBoxItem("6 Hours", 360),
                new ToastComboBoxItem("12 Hours", 720),
                new ToastComboBoxItem("1 Day", 1440)
            };
        }

        /// <summary>
        ///     Handles the OnClick event of the EditButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditSplitView.IsPaneOpen = !EditSplitView.IsPaneOpen;
        }

        /// <summary>
        ///     Saves the task.
        /// </summary>
        private async void SaveTask()
        {
            var changedTask = (Task) TasksListView.SelectedItem;
            if (changedTask != null && changedTask.TaskStatus == Task.Status.Finished)
                changedTask.CompletedOn = DateTimeOffset.Now;
            try
            {
                SavedProgressRing.IsActive = true;
                if (await Tasks.Instance.UpdateTask(changedTask))
                {
                    EditSplitView.IsPaneOpen = false;
                    await ShowSavedText();
                }
                else
                {
                    await new MessageDialog("Saving task failed.").ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await ex.Display("Saving task failed.");
                await ex.Log();
            }
            finally
            {
                SavedProgressRing.IsActive = false;
            }
        }

        private async System.Threading.Tasks.Task ShowSavedText()
        {
            SavedProgressRing.IsActive = false;
            SavedTextBlock.Visibility = Visibility.Visible;
            await System.Threading.Tasks.Task.Delay(3000);
            SavedTextBlock.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Adds the task.
        /// </summary>
        private async void AddTask()
        {
            var newTask = new Task {DueDate = DateTimeOffset.Now};
            NewTaskDatePicker.MinDate = DateTimeOffset.Now;
            AddTaskContentDialog.DataContext = newTask;
            ToastTimeComboBox.SelectedIndex = 0;

            var result = await AddTaskContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                newTask.TaskStatus = Task.Status.Added;
                newTask.Users = new List<User> { ViewModel.SessionUser };
                var createdTask = await Tasks.Instance.AddTask(newTask);
                ViewModel.Tasks.Add(createdTask);
                ViewModel.ActiveTasks.Add(createdTask);
                AddToast(createdTask);
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

        /// <summary>
        ///     Adds the toast.
        /// </summary>
        /// <param name="task">The task.</param>
        private void AddToast(Task task)
        {
            var selectedToastTime = (ToastComboBoxItem)ToastTimeComboBox.SelectedItem;
            if (selectedToastTime != null && selectedToastTime.Value != -1)
            {
                TaskToast.CreateTaskToast(task, selectedToastTime.Value);
            }
        }

        /// <summary>
        ///     Handles the OnSelectionChanged event of the TasksListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void TasksListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTask = (Task) TasksListView.SelectedItem;
            DetailsPanel.Visibility = selectedTask == null ? Visibility.Collapsed : Visibility.Visible;
            NotesPanel.Visibility = DetailsPanel.Visibility;
            DueDateText.Visibility = selectedTask != null && selectedTask.DueDate == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        ///     Toggles the finished tasks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ToggleFinishedTasks(object sender, RoutedEventArgs e)
        {
            TaskViewSource.Source = TaskViewSource.Source == ViewModel.Tasks ? ViewModel.ActiveTasks : ViewModel.Tasks;
        }
    }
}