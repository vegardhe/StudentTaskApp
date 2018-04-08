using System;
using System.Collections.Generic;
using System.Data.Entity;
using StudentTask.Model;

namespace StudentTask.Data.Access
{
    /// <summary>
    /// Reseeds the database if the model is changed.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DropCreateDatabaseIfModelChanges{StudentTaskContext}" />
    public class StudentTaskDbInitializer : DropCreateDatabaseIfModelChanges<StudentTaskContext>
    {
        /// <summary>
        /// A method that should be overridden to actually add data to the context for seeding.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="context">The context to seed.</param>
        protected override void Seed(StudentTaskContext context)
        {
            var sampleResource =
                context.Resources.Add(new Resource {Link = "http://www.it.hiof.no/algdat/", Name = "Course Page"});

            var sampleExcercise = context.Exercises.Add(new Exercise
            {
                Title = "Flyplassen",
                Description =
                    "I den første obligatoriske oppgaven skal det lages et Java-program som simulerer et køsystem. Det skal brukes tidsdrevet simulering, dvs. at programmet skal 'gå i små tidssteg' og simulere hva som skjer innenfor hver tidsperiode.",
                DueDate = new DateTimeOffset(2018, 2, 9, 23, 0, 0, new TimeSpan()),
                DueTime = new TimeSpan(0, 1, 0, 0),
                TaskStatus = Task.Status.Added,
            });

            var sampleCourse = context.Courses.Add(new Course
            {
                CourseCode = "ITF103242",
                Name = "Algoritmer og Datastrukturer",
                Information =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc eget nulla ut ipsum tempor tristique. Sed consequat lorem felis, vel fringilla enim posuere fermentum. Curabitur efficitur tortor id mi iaculis luctus. Aliquam dictum leo risus, vel lacinia sem dignissim a. Aenean tortor dui, condimentum a pretium sit amet, iaculis id massa. Donec nec iaculis lectus, eget vulputate nisl. Cras ullamcorper urna id rhoncus accumsan. Vestibulum gravida eros at erat laoreet elementum. Praesent rhoncus lectus ipsum, at sagittis mi vestibulum ac. Suspendisse tincidunt egestas cursus. Donec varius eros in arcu finibus sodales. Integer cursus cursus massa, et suscipit metus suscipit ac. Cras ut vehicula nisl. Donec at lorem a sem mattis tincidunt id quis dolor. Sed condimentum egestas justo. Sed quis volutpat dui, vitae maximus lorem.",
                Exercises = new List<Exercise> { sampleExcercise },
                Resources = new List<Resource> { sampleResource }
            });

            var sampleCourseTwo = context.Courses.Add(new Course
            {
                CourseCode = "ITD20106",
                Name = "Statistikk og økonomi",
                Information =
                    "Proin a ex lectus. Sed blandit ante ligula, et cursus dui ornare a. Quisque tincidunt cursus volutpat. In hac habitasse platea dictumst. Cras eu placerat metus. Nullam malesuada vehicula diam et laoreet. Morbi bibendum diam eu velit porta consequat. Mauris posuere rhoncus augue, ac rutrum massa tempor et. Maecenas non diam sed lacus luctus varius ac sed mauris. Mauris eleifend, neque vehicula pharetra tempus, dolor velit ornare lacus, suscipit rutrum nisi erat sed turpis. Curabitur sodales eget turpis sed sagittis. Donec egestas mauris id sapien blandit fermentum. Maecenas placerat enim diam, ac porttitor enim tincidunt quis. Fusce at ligula a nulla interdum ornare in id arcu.",
                Exercises = new List<Exercise>(),
                Resources = new List<Resource>()
            });

            var sampleTask = context.Tasks.Add(new Task
            {
                Title = "Read chapter 4, AlgDat",
                Description = "Read pages 45-67.",
                DueDate = new DateTimeOffset(2018, 2, 2, 23, 0, 0, new TimeSpan()),
                DueTime = new TimeSpan(0, 1, 0, 0),
                TaskStatus = Task.Status.Added
            });

            context.Users.Add(new User
            {
                FirstName = "Vegard",
                LastName = "Hermansen",
                Username = "vegardhe",
                GroupUsergroup = User.Usergroup.Admin,
                Courses = new List<Course> { sampleCourse, sampleCourseTwo },
                Email = "vegardhe@hiof.no",
                Password = "Password123", //TODO: Add hashing
                Tasks = new List<Task> { sampleTask, sampleExcercise }
            });

            base.Seed(context);
        }
    }
}