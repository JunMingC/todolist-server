using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Models;
using TodoListTest.Helpers;

namespace TodoListTest.Fixtures
{
    // source: https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
    public class TestDatabaseFixture
    {
        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            Utils.LoadEnvironmentVariables();

            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        // Seed Priorities
                        context.Priorities.AddRange(
                            new Priority
                            {
                                Name = "High",
                                Color = "#FF0000"    // Red
                            },
                            new Priority
                            {
                                Name = "Medium",
                                Color = "#FFFF00"    // Yellow
                            },
                            new Priority
                            {
                                Name = "Low",
                                Color = "#00FF00"    // Green
                            }
                        );

                        // Seed Statuses
                        context.Statuses.AddRange(
                            new Status
                            {
                                Name = "Not Started",
                                Color = "#FFFF00"    // Yellow
                            },
                            new Status
                            {
                                Name = "In Progress",
                                Color = "#0000FF"    // Blue 
                            },
                            new Status
                            {
                                Name = "Completed",
                                Color = "#00FF00"    // Green
                            }
                        );

                        // Seed Tags
                        context.Tags.AddRange(
                            new Tag
                            {
                                Name = "Personal",
                                Color = "#FF69B4"  // Hot Pink
                            },
                            new Tag
                            {
                                Name = "Work",
                                Color = "#4682B4"  // Steel Blue
                            },
                            new Tag
                            {
                                Name = "Research",
                                Color = "#8A2BE2"  // Blue Violet
                            },
                            new Tag
                            {
                                Name = "Development",
                                Color = "#32CD32"  // Lime Green
                            },
                            new Tag
                            {
                                Name = "Review",
                                Color = "#FFD700"  // Gold
                            },
                            new Tag
                            {
                                Name = "Training",
                                Color = "#D3D3D3"  // Light Gray
                            }
                        );

                        context.SaveChanges();

                        // Fetch tags for use in Todo seeding
                        var workTag = context.Tags.First(t => t.Name == "Work");
                        var researchTag = context.Tags.First(t => t.Name == "Research");
                        var trainingTag = context.Tags.First(t => t.Name == "Training");
                        var personalTag = context.Tags.First(t => t.Name == "Personal");
                        var developmentTag = context.Tags.First(t => t.Name == "Development");
                        var reviewTag = context.Tags.First(t => t.Name == "Review");

                        // Seed Todos with associated Tags
                        context.Todos.AddRange(
                            new Todo
                            {
                                Name = "Prepare project proposal",
                                Description = "Draft and finalize the project proposal for the new client.",
                                DueDate = new DateTime(2024, 8, 31),
                                PriorityId = 1, // High priority
                                StatusId = 2,   // In Progress
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag> { workTag, researchTag, trainingTag, personalTag, developmentTag, reviewTag }
                            },
                            new Todo
                            {
                                Name = "Team meeting",
                                Description = "Weekly sync-up with the team to discuss project status.",
                                DueDate = new DateTime(2025, 6, 10),
                                PriorityId = 3, // Medium priority
                                StatusId = 1,   // Not Started
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag> { workTag, researchTag, trainingTag, personalTag, developmentTag }
                            },
                            new Todo
                            {
                                Name = "Code review",
                                Description = "Review the code submitted by the development team.",
                                DueDate = new DateTime(2024, 9, 3),
                                PriorityId = 1, // High priority
                                StatusId = 2,   // In Progress
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag> { workTag, researchTag, trainingTag, personalTag }
                            },
                            new Todo
                            {
                                Name = "Update documentation",
                                Description = "Update the project documentation with the latest changes.",
                                DueDate = new DateTime(2024, 9, 20),
                                PriorityId = 1, // High priority
                                StatusId = 1,   // Not Started
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag> { workTag, researchTag, trainingTag }
                            },
                            new Todo
                            {
                                Name = "Client presentation",
                                Description = "Present the project progress to the client.",
                                DueDate = new DateTime(2024, 8, 27),
                                PriorityId = 3, // Medium priority
                                StatusId = 1,   // Not Started
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag> { workTag, researchTag }
                            },
                            new Todo
                            {
                                Name = "Research new technologies",
                                Description = "Explore new tools and frameworks that can be used in future projects.",
                                DueDate = new DateTime(2024, 7, 31),
                                PriorityId = 2, // Medium priority
                                StatusId = 3,   // Completed
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag> { workTag }
                            },
                            new Todo
                            {
                                Name = "Training session",
                                Description = "Conduct training for new team members on the project.",
                                DueDate = new DateTime(2024, 10, 3),
                                PriorityId = 2, // Medium priority
                                StatusId = 3,   // Completed
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag>()
                            },
                            new Todo
                            {
                                Name = "Empty Task 1",
                                Description = null,
                                DueDate = null, // No DueDate
                                PriorityId = null, // No PriorityId
                                StatusId = null, // No StatusId
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag>()  // No Tags
                            },
                            new Todo
                            {
                                Name = "Empty Task 2",
                                Description = null,
                                DueDate = null, // No DueDate
                                PriorityId = null, // No PriorityId
                                StatusId = null, // No StatusId
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag>()  // No Tags
                            },
                            new Todo
                            {
                                Name = "Empty Task 3",
                                Description = null,
                                DueDate = null, // No DueDate
                                PriorityId = null, // No PriorityId
                                StatusId = null, // No StatusId
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Tags = new List<Tag>()  // No Tags
                            }
                        );

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public TodoListContext CreateContext()
        {
            // Register DbContext
            return new TodoListContext(new DbContextOptionsBuilder<TodoListContext>().UseSqlServer(Environment.GetEnvironmentVariable("TodoListTestConnection")).Options);
        }
    }
}