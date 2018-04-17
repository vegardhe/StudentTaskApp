using StudentTask.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StudentTask.Uwp.App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CoursesPage : Page
    {

        public ObservableCollection<Resource> CourseResources { get; set; }

        public ObservableCollection<Exercise> CourseExercises { get; set; }

        public CoursesPage()
        {
            this.InitializeComponent();
            CourseResources = new ObservableCollection<Resource>();
            CourseExercises = new ObservableCollection<Exercise>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Check if teacher is teacher for current course.
            if (DataSource.Users.Instance.SessionUser != null &&
                (DataSource.Users.Instance.SessionUser.GroupUsergroup == User.Usergroup.Admin ||
                 DataSource.Users.Instance.SessionUser.GroupUsergroup == User.Usergroup.Teacher))
            {
                NewCourseButton.Visibility = Visibility.Visible;
                EditCourseButton.Visibility = Visibility.Visible;
                ManageResourcesButton.Visibility = Visibility.Visible;
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
                catch (Exception)
                {
                    // TODO: Exception handling.
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
                catch (Exception)
                {
                    // TODO: Exception handling.
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
                foreach (var cr in CourseResources)
                {
                    if (cr.ResourceId == 0)
                        await DataSource.Resources.Instance.AddResource(cr, selectedCourse);
                    else
                    {
                        if (selectedCourse == null) continue;
                        var originalResource = selectedCourse.Resources.Find(r => r.ResourceId == cr.ResourceId);
                        if (originalResource.Name != cr.Name || originalResource.Link != cr.Link)
                            await DataSource.Resources.Instance.UpdateResource(cr);
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
                if (selectedCourse.Resources == null)
                {
                    await DataSource.Courses.Instance.GetCourseResources(selectedCourse);
                }

                if (selectedCourse.Exercises == null)
                {
                    await DataSource.Courses.Instance.GetCourseExercises(selectedCourse);
                }
            }
            UpdateCourseResources(selectedCourse);
            UpdateCourseExercises(selectedCourse);
        }

        private void UpdateCourseResources(Course course)
        {
            CourseResources.Clear();
            foreach (var r in course.Resources)
            {
                CourseResources.Add(r);
            }
        }

        private void UpdateCourseExercises(Course course)
        {
            CourseExercises.Clear();
            foreach (var e in course.Exercises)
            {
                CourseExercises.Add(e);
            }
        }

        private void AddResource(object sender, RoutedEventArgs e)
        {
            CourseResources.Add(new Resource { Name = "New Resource" } );
        }

        private void RemoveResource(object sender, RoutedEventArgs e)
        {
            CourseResources.Remove((Resource) ManageResourcesListView.SelectedItem);
        }

        private void ManageResourcesListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditResourcePanel.Visibility = ManageResourcesListView.SelectedItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
