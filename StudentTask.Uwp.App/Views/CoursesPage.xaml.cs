using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using Task = System.Threading.Tasks.Task;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    ///     A page displaying courses that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// <seealso cref="Page" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class CoursesPage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CoursesPage" /> class.
        /// </summary>
        public CoursesPage()
        {
            InitializeComponent();
            CourseResources = new ObservableCollection<Resource>();
            CourseExercises = new ObservableCollection<Exercise>();
        }

        /// <summary>
        ///     Gets the course resources.
        /// </summary>
        /// <value>
        ///     The course resources.
        /// </value>
        public ObservableCollection<Resource> CourseResources { get; }

        /// <summary>
        ///     Gets the course exercises.
        /// </summary>
        /// <value>
        ///     The course exercises.
        /// </value>
        public ObservableCollection<Exercise> CourseExercises { get; }

        /// <summary>
        ///     Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that can be examined by overriding code. The event data is representative of the pending
        ///     navigation that will load the current Page. Usually the most relevant property to examine is Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Users.Instance.SessionUser != null &&
                (Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin ||
                 Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Teacher))
                NewCourseButton.Visibility = EditCourseButton.Visibility = ManageResourcesButton.Visibility =
                    NewExerciseButton.Visibility = DeleteCourseButton.Visibility = DeleteCourseButton.Visibility =
                        EditExerciseButton.Visibility = DeleteExerciseButton.Visibility =
                            AddUserToCourseButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Adds the course.
        /// </summary>
        private async void AddCourse()
        {
            var newCourse = new Course();
            AddCourseContentDialog.DataContext = newCourse;

            var result = await AddCourseContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                newCourse.Users = new List<User> {Users.Instance.SessionUser};
                Course addedCourse;
                if ((addedCourse = await Courses.Instance.AddCourse(newCourse)) != null)
                    ViewModel.Courses.Add(addedCourse);
            }
            catch (WebException ex)
            {
                await ex.Display("Failed to establish connection to internet.");
                await ex.Log();
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to add course");
                await ex.Log();
            }
        }

        /// <summary>
        ///     Deletes the course.
        /// </summary>
        private async void DeleteCourse()
        {
            var result = await DeleteCourseContentDialog.ShowAsync();
            var selectedCourse = (Course) CoursesListView.SelectedItem;

            if (result != ContentDialogResult.Primary) return;
            if (selectedCourse == null) return;
            try
            {
                if (await Courses.Instance.DeleteCourse(selectedCourse))
                    ViewModel.Courses.Remove(selectedCourse);
                else
                    await new MessageDialog("Failed to delete course.", "Error").ShowAsync();
            }
            catch (Exception e)
            {
                await e.Log();
                await e.Display("Failed to delete course.");
            }
        }

        /// <summary>
        ///     Deletes the exercise.
        /// </summary>
        private async void DeleteExercise()
        {
            ViewExerciseContentDialog.Hide();
            var result = await DeleteExerciseContentDialog.ShowAsync();
            var selectedExercise = (Exercise) ExercisesListView.SelectedItem;

            if (result != ContentDialogResult.Primary) return;
            if (selectedExercise == null) return;
            try
            {
                if (!await Tasks.Instance.DeleteTask(selectedExercise))
                    await new MessageDialog("Failed to delete exercise", "Error").ShowAsync();
            }
            catch (Exception e)
            {
                await e.Display("Failed to delete exercise.");
                await e.Log();
            }

            CourseExercises.Remove(selectedExercise);
            var selectedCourse = (Course)CoursesListView.SelectedItem;
            selectedCourse?.Exercises.Remove(selectedExercise);
        }

        /// <summary>
        ///     Adds the exercise.
        /// </summary>
        private async void AddExercise()
        {
            var newExercise = new Exercise();
            NewExerciseDatePicker.MinDate = DateTimeOffset.Now;
            AddExerciseContentDialog.DataContext = newExercise;

            var result = await AddExerciseContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                newExercise.TaskStatus = Model.Task.Status.Added;
                var selectedCourse = (Course) CoursesListView.SelectedItem;
                Exercise addedExercise;
                if ((addedExercise = await Courses.Instance.AddExercise(newExercise,
                        selectedCourse)) == null) return;
                CourseExercises.Add(addedExercise);
                selectedCourse?.Exercises.Add(addedExercise);
                ToggleExerciseVisibility(true);
            }
            catch (WebException ex)
            {
                await ex.Log();
                await ex.Display("Failed to establish connection to internet.");
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to add exercise.");
                await ex.Log();
            }
        }

        /// <summary>
        ///     Adds the user to course.
        /// </summary>
        private async void AddUserToCourse()
        {
            var result = await AddUserContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                var user = (User) AddUserContentDialog.DataContext;
                var course = (Course) CoursesListView.SelectedItem;

                if (!await Courses.Instance.AddUserToCourse(user, course))
                    await new MessageDialog("Failed to add user", "Error").ShowAsync();
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to add user.");
                await ex.Log();
            }
        }

        /// <summary>
        ///     Edits the exercise.
        /// </summary>
        private async void EditExercise()
        {
            ViewExerciseContentDialog.Hide();
            var selectedExercise = (Exercise) ExercisesListView.SelectedItem;
            EditExerciseContentDialog.DataContext = selectedExercise;
            var result = await EditExerciseContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                await Tasks.Instance.UpdateTask(selectedExercise);
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to edit exercise.");
                await ex.Log();
            }
        }

        /// <summary>
        ///     Edits the course.
        /// </summary>
        private async void EditCourse()
        {
            var selectedCourse = (Course) CoursesListView.SelectedItem;
            AddCourseContentDialog.DataContext = selectedCourse;
            var result = await EditCourseContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                if (!await Courses.Instance.UpdateCourse(selectedCourse))
                    await new MessageDialog("Failed to update course.", "Error").ShowAsync();
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to edit course.");
                await ex.Log();
            }
        }

        /// <summary>
        ///     Manages the resources.
        /// </summary>
        private async void ManageResources()
        {
            ManageResourcesContentDialog.DataContext = CourseResources;

            var result = await ManageResourcesContentDialog.ShowAsync();
            var selectedCourse = (Course) CoursesListView.SelectedItem;
            if (result != ContentDialogResult.Primary)
            {
                if (result == ContentDialogResult.Secondary)
                    UpdateCourseResources(selectedCourse);
            }
            else
            {
                if (selectedCourse == null) return;
                foreach (var selectedCourseResource in selectedCourse.Resources.ToArray())
                    if (CourseResources.All(r => r.ResourceId != selectedCourseResource.ResourceId))
                        await DeleteCourseResource(selectedCourseResource, selectedCourse);

                foreach (var courseResource in CourseResources)
                    if (courseResource.ResourceId == 0)
                    {
                        await AddCourseResource(courseResource, selectedCourse);
                    }
                    else
                    {
                        var originalResource =
                            selectedCourse.Resources.Find(r => r.ResourceId == courseResource.ResourceId);
                        if (originalResource.Name == courseResource.Name &&
                            originalResource.Link == courseResource.Link) continue;

                        await UpdateCourseResource(courseResource);
                        originalResource.Name = courseResource.Name;
                        originalResource.Link = courseResource.Link;
                    }
            }
        }

        /// <summary>
        ///     Updates the course resource.
        /// </summary>
        /// <param name="courseResource">The course resource.</param>
        /// <returns></returns>
        private async Task UpdateCourseResource(Resource courseResource)
        {
            try
            {
                if (!await DataSource.Resources.Instance.UpdateResource(courseResource))
                    await new MessageDialog("Failed to update resource.", "Error").ShowAsync();
            }
            catch (Exception e)
            {
                await e.Log();
                await e.Display("Failed to update resource.");
            }
        }

        /// <summary>
        ///     Adds the course resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        private async Task AddCourseResource(Resource resource, Course course)
        {
            try
            {
                course.Resources.Add(
                    await DataSource.Resources.Instance.AddResource(resource, course));
            }
            catch (WebException ex)
            {
                await ex.Log();
                await ex.Display("Failed to establish internet connection.");
            }
            catch (Exception ex)
            {
                await ex.Log();
                await ex.Display("Failed to add resource.");
            }
        }

        /// <summary>
        ///     Deletes the course resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="course">The course.</param>
        /// <returns></returns>
        private async Task DeleteCourseResource(Resource resource, Course course)
        {
            try
            {
                if (!await DataSource.Resources.Instance.DeleteResource(resource))
                {
                    await new MessageDialog("Failed to remove resource", "Error").ShowAsync();
                    return;
                }

                course.Resources.Remove(resource);
            }
            catch (Exception e)
            {
                await e.Display("Failed to remove resource.");
                await e.Log();
            }
        }

        /// <summary>
        ///     Handles the OnSelectionChanged event of the CoursesListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private async void CoursesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = (Course) CoursesListView.SelectedItem;
            if (selectedCourse != null)
            {
                if (selectedCourse.Resources == null)
                    try
                    {
                        await Courses.Instance.GetCourseResources(selectedCourse);
                    }
                    catch (HttpRequestException ex)
                    {
                        await ex.Log();
                        await ex.Display("Failed to get course resources.");
                    }

                if (selectedCourse.Exercises == null)
                    try
                    {
                        var exercises = await Courses.Instance.GetCourseExercises(selectedCourse);
                        ToggleExerciseVisibility(exercises.Length > 0);
                    }
                    catch (HttpRequestException ex)
                    {
                        await ex.Log();
                        await ex.Display("Failed to get course exercises.");
                    }
                else
                    ToggleExerciseVisibility(selectedCourse.Exercises.Count > 0);
            }
            else
                ToggleExerciseVisibility(false);

            UpdateCourseResources(selectedCourse);
            UpdateCourseExercises(selectedCourse);
        }

        /// <summary>
        /// Toggles the exercise visibility.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        private void ToggleExerciseVisibility(bool visible)
        {
            ExercisesListView.Visibility = visible
                ? (SeparatorLine.Visibility = Visibility.Visible)
                : (SeparatorLine.Visibility = Visibility.Collapsed);
        }

        /// <summary>
        ///     Updates the course resources.
        /// </summary>
        /// <param name="course">The course.</param>
        private void UpdateCourseResources(Course course)
        {
            if (course == null) return;
            CourseResources.Clear();
            foreach (var r in course.Resources)
                CourseResources.Add(new Resource{ResourceId = r.ResourceId, Name = r.Name, Link = r.Link});
        }

        /// <summary>
        ///     Updates the course exercises.
        /// </summary>
        /// <param name="course">The course.</param>
        private void UpdateCourseExercises(Course course)
        {
            if (course == null) return;
            CourseExercises.Clear();
            foreach (var e in course.Exercises) CourseExercises.Add(e);
        }

        /// <summary>
        ///     Adds the resource.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AddResource(object sender, RoutedEventArgs e) => CourseResources.Add(new Resource { Name = "New Resource" });

        /// <summary>
        ///     Removes the resource.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RemoveResource(object sender, RoutedEventArgs e) => CourseResources.Remove((Resource)ManageResourcesListView.SelectedItem);

        /// <summary>
        ///     Handles the OnSelectionChanged event of the ManageResourcesListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void ManageResourcesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditResourcePanel.Visibility = ManageResourcesListView.SelectedItems.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        ///     Handles the OnItemClick event of the ExercisesListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ItemClickEventArgs" /> instance containing the event data.</param>
        private async void ExercisesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var result = await ViewExerciseContentDialog.ShowAsync();

            if (result != ContentDialogResult.Primary) return;
            var selectedExercise = (Exercise)ExercisesListView.SelectedItem;

            if (selectedExercise == null) return;
            var copyExercise = new Model.Task
            {
                Title = selectedExercise.Title,
                Description = selectedExercise.Description,
                DueDate = selectedExercise.DueDate,
                DueTime = selectedExercise.DueTime,
                Users = new List<User> { Users.Instance.SessionUser },
                TaskStatus = Model.Task.Status.Added
            };
            await AddTask(copyExercise, NotificationCheckBox.IsChecked != null && NotificationCheckBox.IsChecked.Value);

            Users.Instance.Changed = true;
        }

        /// <summary>
        ///     Adds the task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="createToast"></param>
        /// <returns></returns>
        private static async Task AddTask(Model.Task task, bool createToast)
        {
            try
            {
                var createdTask = await Tasks.Instance.AddTask(task);
                if(createToast) TaskToast.CreateTaskToast(createdTask, 1440);
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
        ///     Users the automatic suggest box on suggestion chosen.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AutoSuggestBoxSuggestionChosenEventArgs" /> instance containing the event data.</param>
        private void UserAutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender,
            AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is User user) sender.Text = user.FullName;
        }

        /// <summary>
        ///     Users the automatic suggest box on text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AutoSuggestBoxTextChangedEventArgs" /> instance containing the event data.</param>
        private async void UserAutoSuggestBox_OnTextChanged(AutoSuggestBox sender,
            AutoSuggestBoxTextChangedEventArgs args)
        {
            if (Users.Instance.UserList == null)
                try
                {
                    await Users.Instance.GetUsers();
                }
                catch (Exception ex)
                {
                    await ex.Display("Failed to get users.");
                    await ex.Log();
                    return;
                }

            var suggestions = Users.Instance.UserList
                .Where(p => p.FullName.ToLower().Contains(sender.Text.ToLower())).ToArray();

            if (suggestions.Length > 0)
                sender.ItemsSource = suggestions;
            else
                sender.ItemsSource = new[] {"No results found"};
        }

        /// <summary>
        ///     Users the automatic suggest box on query submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AutoSuggestBoxQuerySubmittedEventArgs" /> instance containing the event data.</param>
        private void UserAutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is User user) AddUserContentDialog.DataContext = user;
        }
    }
}