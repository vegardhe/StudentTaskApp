﻿using StudentTask.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
            if (DataSource.Users.Instance.SessionUser != null &&
                (DataSource.Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Admin ||
                 DataSource.Users.Instance.SessionUser.GroupUserGroup == User.UserGroup.Teacher))
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
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    newCourse.Users = new List<User>{ DataSource.Users.Instance.SessionUser };
                    Course addedCourse;
                    if ((addedCourse = await DataSource.Courses.Instance.AddCourse(newCourse)) != null)
                    {
                        ViewModel.Courses.Add(addedCourse);
                    }
                }
                catch (Exception ex)
                {
                    await ex.Display("Failed to add course");
                    await ex.Log();
                }
            }
        }

        private async void DeleteCourse()
        {
            var result = await DeleteCourseContentDialog.ShowAsync();
            var selectedCourse = (Course) CoursesListView.SelectedItem;

            if (result == ContentDialogResult.Primary)
            {
                if (selectedCourse != null)
                {
                    await DataSource.Courses.Instance.DeleteCourse(selectedCourse);
                    ViewModel.Courses.Remove(selectedCourse);
                }
            }
        }

        private async void DeleteExercise()
        {
            ViewExerciseContentDialog.Hide();
            var result = await DeleteExerciseContentDialog.ShowAsync();
            var selectedExercise = (Exercise) ExercisesListView.SelectedItem;

            if (result == ContentDialogResult.Primary)
            {
                if (selectedExercise != null)
                {
                    await DataSource.Tasks.Instance.DeleteTask(selectedExercise);
                    CourseExercises.Remove(selectedExercise);
                }
            }
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
                    var selectedCourse = (Course)CoursesListView.SelectedItem;
                    Exercise addedExercise;
                    if ((addedExercise = await DataSource.Courses.Instance.AddExercise(newExercise,
                            selectedCourse)) != null)
                    {
                        CourseExercises.Add(addedExercise);
                    }
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
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    var user = (User)AddUserContentDialog.DataContext;
                    var course = (Course)CoursesListView.SelectedItem;

                    await DataSource.Courses.Instance.AddUserToCourse(user, course);
                }
                catch (Exception ex)
                {
                    await ex.Display("Failed to add user.");
                    await ex.Log();
                }
            }
        }

        private async void EditExercise()
        {
            ViewExerciseContentDialog.Hide();
            var selectedExercise = (Exercise) ExercisesListView.SelectedItem;
            EditExerciseContentDialog.DataContext = selectedExercise;
            var result = await EditExerciseContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await DataSource.Tasks.Instance.UpdateTask(selectedExercise);
                }
                catch (Exception ex)
                {
                    await ex.Display("Failed to edit exercise.");
                    await ex.Log();
                }
            }
        }

        private async void EditCourse()
        {
            var selectedCourse = (Course)CoursesListView.SelectedItem;
            AddCourseContentDialog.DataContext = selectedCourse;
            var result = await EditCourseContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await DataSource.Courses.Instance.UpdateCourse(selectedCourse);
                }
                catch (Exception ex)
                {
                    await ex.Display("Failed to edit course.");
                    await ex.Log();
                }
            }
        }

        private async void ManageResources()
        {
            ManageResourcesContentDialog.DataContext = CourseResources;

            var result = await ManageResourcesContentDialog.ShowAsync();
            var selectedCourse = (Course)CoursesListView.SelectedItem;
            if (result == ContentDialogResult.Primary)
            {
                // TODO: Make this prettier.
                if (selectedCourse == null) return;
                foreach (var selectedCourseResource in selectedCourse.Resources.ToArray())
                {
                    if (CourseResources.Contains(selectedCourseResource)) continue;
                    await DataSource.Resources.Instance.DeleteResource(selectedCourseResource);
                    selectedCourse.Resources.Remove(selectedCourseResource);
                }

                foreach (var courseResource in CourseResources)
                {
                    if (courseResource.ResourceId == 0)
                        selectedCourse.Resources.Add(await DataSource.Resources.Instance.AddResource(courseResource, selectedCourse));
                    else
                    {
                        var originalResource = selectedCourse.Resources.Find(r => r.ResourceId == courseResource.ResourceId);
                        if (originalResource.Name != courseResource.Name || originalResource.Link != courseResource.Link)
                            await DataSource.Resources.Instance.UpdateResource(courseResource);
                    }
                }
            }
            else if (result == ContentDialogResult.Secondary)
                UpdateCourseResources(selectedCourse);
        }

        private async void CoursesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCourse = (Course) CoursesListView.SelectedItem;
            if (selectedCourse != null)
            {
                ExercisesListView.Visibility = Visibility.Visible;
                SeparatorLine.Visibility = Visibility.Visible;

                if (selectedCourse.Resources == null)
                    await DataSource.Courses.Instance.GetCourseResources(selectedCourse);

                if (selectedCourse.Exercises == null)
                    await DataSource.Courses.Instance.GetCourseExercises(selectedCourse);
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

            if (result == ContentDialogResult.Primary)
            {
                var selectedExercise = (Exercise) ExercisesListView.SelectedItem;

                if (selectedExercise == null) return;
                var copyExercise = new Task
                {
                    Title = selectedExercise.Title,
                    Description = selectedExercise.Description,
                    DueDate = selectedExercise.DueDate,
                    DueTime = selectedExercise.DueTime,
                    Users = new List<User> {DataSource.Users.Instance.SessionUser},
                    TaskStatus = Task.Status.Added
                };

                if (await DataSource.Tasks.Instance.AddTask(copyExercise) != null)
                {
                    DataSource.Users.Instance.Changed = true;
                }
            }
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
            if (DataSource.Users.Instance.UserList == null)
                await DataSource.Users.Instance.GetUsers();
            var suggestions = DataSource.Users.Instance.UserList
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
