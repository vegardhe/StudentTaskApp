﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Windows.UI.Xaml.Navigation;
using StudentTask.Model;
using StudentTask.Uwp.App.DataSource;
using Template10.Mvvm;
using Task = System.Threading.Tasks.Task;

namespace StudentTask.Uwp.App.ViewModels
{
    /// <summary>
    ///     ViewModel for course page.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class CoursesPageViewModel : ViewModelBase
    {
        /// <summary>
        ///     Gets the courses.
        /// </summary>
        /// <value>
        ///     The courses.
        /// </value>
        public ObservableCollection<Course> Courses { get; private set; }

        /// <summary>
        ///     Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode,
            IDictionary<string, object> state)
        {
            if (Courses == null)
                try
                {
                    Courses = new ObservableCollection<Course>(
                        await DataSource.Courses.Instance.GetUserCourses(Users.Instance.SessionUser));
                }
                catch (HttpRequestException e)
                {
                    await e.Log();
                    await e.Display("Requesting courses timed out.");
                }
        }
    }
}