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
using Exception = System.Exception;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoursesPage
    {

        public ObservableCollection<Resource> CourseResources { get; }

        public ObservableCollection<Exercise> CourseExercises { get; }

        public CoursesPage()
        {
            InitializeComponent();
            CourseResources = new ObservableCollection<Resource>();
            CourseExercises = new ObservableCollection<Exercise>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Users.Instance.SessionUser != null &&
                (Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin ||
                 Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Teacher))
            {
                NewCourseButton.Visibility = EditCourseButton.Visibility = ManageResourcesButton.Visibility =
                    NewExerciseButton.Visibility = DeleteCourseButton.Visibility = DeleteCourseButton.Visibility =
                        EditExerciseButton.Visibility = DeleteExerciseButton.Visibility =
                            AddUserToCourseButton.Visibility = Visibility.Visible;
            }
        }

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
                {
                    ViewModel.Courses.Add(addedCourse);
                }
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

        private async void DeleteCourse()
        {
            var result = await DeleteCourseContentDialog.ShowAsync();
            var selectedCourse = (Course) CoursesListView.SelectedItem;

            if (result != ContentDialogResult.Primary) return;
            if (selectedCourse == null) return;
            try
            {
                if (!await Courses.Instance.DeleteCourse(selectedCourse))
                    await new MessageDialog("Failed to delete course.", "Error").ShowAsync();
            }
            catch (Exception e)
            {
                await e.Log();
                await e.Display("Failed to delete course.");
            }
            ViewModel.Courses.Remove(selectedCourse);
        }

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
        }

        private async void AddExercise()
        {
            var newExercise = new Exercise();
            NewExerciseDatePicker.MinDate=DateTimeOffset.Now;
            AddExerciseContentDialog.DataContext = newExercise;

            var result = await AddExerciseContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    newExercise.TaskStatus = Task.Status.Added;
                    var selectedCourse = (Course) CoursesListView.SelectedItem;
                    Exercise addedExercise;
                    if ((addedExercise = await Courses.Instance.AddExercise(newExercise,
                            selectedCourse)) != null)
                    {
                        CourseExercises.Add(addedExercise);
                    }
                }
                catch (WebException ex)
                {
                    await ex.Log();
                    await ex.Display("Failed to establish connection to internet.");
                }
                catch(Exception ex)
                {
                    await ex.Display("Failed to add exercise.");
                    await ex.Log();
                }
            }
        }

        private async void AddUserToCourse()
        {
            var result = await AddUserContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                var user = (User)AddUserContentDialog.DataContext;
                var course = (Course)CoursesListView.SelectedItem;

                if (!await Courses.Instance.AddUserToCourse(user, course))
                    await new MessageDialog("Failed to add user", "Error").ShowAsync();
            }
            catch (Exception ex)
            {
                await ex.Display("Failed to add user.");
                await ex.Log();
            }
        }

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

        private async void EditCourse()
        {
            var selectedCourse = (Course)CoursesListView.SelectedItem;
            AddCourseContentDialog.DataContext = selectedCourse;
            var result = await EditCourseContentDialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            try
            {
                if (!await Courses.Instance.UpdateCourse(selectedCourse))
                {
                    await new MessageDialog("Failed to update course.", "Error").ShowAsync();
                }

            }
            catch (Exception ex)
            {
                await ex.Display("Failed to edit course.");
                await ex.Log();
            }
        }

        private async void ManageResources()
        {
            ManageResourcesContentDialog.DataContext = CourseResources;

            var result = await ManageResourcesContentDialog.ShowAsync();
            var selectedCourse = (Course)CoursesListView.SelectedItem;
            if (result != ContentDialogResult.Primary)
            {
                if (result == ContentDialogResult.Secondary)
                    UpdateCourseResources(selectedCourse);
            }
            else
            {
                // TODO: Make this prettier.
                if (selectedCourse == null) return;
                foreach (var selectedCourseResource in selectedCourse.Resources.ToArray())
                {
                    if (CourseResources.Contains(selectedCourseResource)) continue;
                    try
                    {
                        if (!await DataSource.Resources.Instance.DeleteResource(selectedCourseResource))
                        {
                            await new MessageDialog("Failed to remove resource", "Error").ShowAsync();
                            return;
                        }
                        selectedCourse.Resources.Remove(selectedCourseResource);
                    }
                    catch (Exception e)
                    {
                        await e.Display("Failed to remove resource.");
                        await e.Log();
                        return;
                    }
                }

                foreach (var courseResource in CourseResources)
                {
                    if (courseResource.ResourceId == 0)
                    {
                        try
                        {
                            selectedCourse.Resources.Add(
                                await DataSource.Resources.Instance.AddResource(courseResource, selectedCourse));
                        }
                        catch (WebException ex)
                        {
                            await ex.Log();
                            await ex.Display("Failed to establish internet connection.");
                            return;
                        }
                        catch (Exception ex)
                        {
                            await ex.Log();
                            await ex.Display("Failed to add resource.");
                            return;
                        }
                    }   
                    else
                    {
                        var originalResource =
                            selectedCourse.Resources.Find(r => r.ResourceId == courseResource.ResourceId);
                        if (originalResource.Name == courseResource.Name && originalResource.Link == courseResource.Link) continue;

                        try
                        {
                            if (await DataSource.Resources.Instance.UpdateResource(courseResource)) continue;
                            await new MessageDialog("Failed to update resource.", "Error").ShowAsync();
                            return;
                        }
                        catch (Exception e)
                        {
                            await e.Log();
                            await e.Display("Failed to update resource.");
                            return;
                        }
                    }
                }
            }
        }

        private async void CoursesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = (Course) CoursesListView.SelectedItem;
            if (selectedCourse != null)
            {
                ExercisesListView.Visibility = Visibility.Visible;
                SeparatorLine.Visibility = Visibility.Visible;

                if (selectedCourse.Resources == null)
                {
                    try
                    {
                        await Courses.Instance.GetCourseResources(selectedCourse);
                    }
                    catch (HttpRequestException ex)
                    {
                        await ex.Log();
                        await ex.Display("Failed to get course resources.");
                    }
                }

                if (selectedCourse.Exercises == null)
                {
                    try
                    {
                        await Courses.Instance.GetCourseExercises(selectedCourse);
                    }
                    catch (HttpRequestException ex)
                    {
                        await ex.Log();
                        await ex.Display("Failed to get course exercises.");
                    }
                }
                    
            }
            else
            {
                ExercisesListView.Visibility = Visibility.Collapsed;
                SeparatorLine.Visibility = Visibility.Collapsed;
            }
            UpdateCourseResources(selectedCourse);
            UpdateCourseExercises(selectedCourse);
        }

        private void UpdateCourseResources(Course course)
        {
            if (course == null) return;
            CourseResources.Clear();
            foreach (var r in course.Resources)
            {
                CourseResources.Add(r);
            }
        }

        private void UpdateCourseExercises(Course course)
        {
            if (course == null) return;
            CourseExercises.Clear();
            foreach (var e in course.Exercises)
            {
                CourseExercises.Add(e);
            }
        }

        private void AddResource(object sender, RoutedEventArgs e) => CourseResources.Add(new Resource { Name = "New Resource" });

        private void RemoveResource(object sender, RoutedEventArgs e) => CourseResources.Remove((Resource)ManageResourcesListView.SelectedItem);

        private void ManageResourcesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditResourcePanel.Visibility = ManageResourcesListView.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ExercisesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var result = await ViewExerciseContentDialog.ShowAsync();

            if (result != ContentDialogResult.Primary) return;
            var selectedExercise = (Exercise) ExercisesListView.SelectedItem;

            if (selectedExercise == null) return;
            var copyExercise = new Task
            {
                Title = selectedExercise.Title,
                Description = selectedExercise.Description,
                DueDate = selectedExercise.DueDate,
                DueTime = selectedExercise.DueTime,
                Users = new List<User> {Users.Instance.SessionUser},
                TaskStatus = Task.Status.Added
            };

            try
            {
                await Tasks.Instance.AddTask(copyExercise);
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
            Users.Instance.Changed = true;
        }

        private void UserAutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is User user)
            {
                sender.Text = user.FullName;
            }
        }

        private async void UserAutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (Users.Instance.UserList == null)
            {
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
            }

            var suggestions = Users.Instance.UserList
                .Where(p => p.FullName.ToLower().Contains(sender.Text.ToLower())).ToArray();
            
            if(suggestions.Length > 0)
                sender.ItemsSource = suggestions;
            else
            {
                sender.ItemsSource = new[] {"No results found"};
            }
        }

        private void UserAutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is User user)
            {
                AddUserContentDialog.DataContext = user;
            }
        }
    }
}
