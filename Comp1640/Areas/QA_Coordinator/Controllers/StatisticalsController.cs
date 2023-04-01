
using Comp1640.Data;
using Comp1640.EmailService;
using Comp1640.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace Comp1640.Areas.QA_Coordinator.Controllers
{
    [Area(SD.Area_QA_COORDINATOR)]
    public class StatisticalsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StatisticalsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            
            StatisticalCategoryWithIdeas();
            StatisticalTopicWithIdeas();
            StatisticalDepartmentWithIdeas();
            StatisticalIdeasWithComments();
            StatisticalDepartmentWithContributors();
            return View();
        }

        

        public void StatisticalCategoryWithIdeas()
        {
            List<int> ideasList = new List<int>();
            var categoryList = _db.Categories.ToList();
            var categoryNameList = categoryList.Select(x => x.Name).ToArray();
            var ideaList = _db.Ideas.ToList();

            foreach (var category in categoryList)
            {
                ideasList.Add(ideaList.Where(i => i.CategoryID == category.Id).ToList().Count);
            }
            int[] ideas = ideasList.ToArray();
            var data = new
            {
                labels = categoryNameList,
                datasets = new[]
                {
                    new {
                    label = "Ideas",
                    data = ideas,
                    backgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"

                    },
                    borderColor = "rgb(75,192,192,1)",
                    borderWidth = 0,
                    hoverBackgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"
                    },
                    }

                }
            };

            // Define the options for the chart
            var options = new
            {
                responsive = true,
                plugins = new {
                    title = new { display = true, text = " Category" },
                    legend = new
                    {
                        position = "top",
                        labels = new
                        {
                            boxWidth = 11,
                            fondColor = "#757681",
                            fontSize = 11
                        }
                    },
                }

            };

            ViewBag.ChartDataStatisticalCategoryWithIdeas = data;
            ViewBag.ChartOptionsStatisticalCategoryWithIdeas = options;
        }


        public void StatisticalTopicWithIdeas()
        {
            List<int> ideasList = new List<int>();
            var topicList = _db.Topics.ToList();
            var topicNameList = topicList.Select(x => x.Name).ToArray();
            var ideaList = _db.Ideas.ToList();

            foreach (var topic in topicList)
            {
                ideasList.Add(ideaList.Where(i => i.TopicID == topic.Id).ToList().Count);
            }
            int[] ideas = ideasList.ToArray();
            var data = new
            {
                labels = topicNameList,
                datasets = new[]
                {
                    new {
                    label = "Ideas",
                    data = ideas,
                    backgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"

                    },
                    borderColor = "rgb(75,192,192,1)",
                    borderWidth = 0,
                    hoverBackgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"
                    },
                    }

                }
            };

            // Define the options for the chart
            var options = new
            {
                responsive = true,
                plugins = new
                {
                    title = new { display = true, text = " Topic" },
                    legend = new
                    {
                        position = "top",
                        labels = new
                        {
                            boxWidth = 11,
                            fondColor = "#757681",
                            fontSize = 11
                        }
                    },
                }

            };

            ViewBag.ChartDataStatisticalTopicWithIdeas = data;
            ViewBag.ChartOptionsStatisticalTopicWithIdeas = options;
        }

        public void StatisticalDepartmentWithIdeas()
        {
            List<int> ideasList = new List<int>();
            var departmentList = _db.Departments.ToList();
            var departmentNameList = departmentList.Select(x => x.Name).ToArray();
            var ideaList = _db.Ideas.ToList();
            

            foreach (var department in departmentList)
            {
                int ideaOfDepartment = 0;
                int ideaOfUser = 0;
                var userList = _db.ApplicationUsers.Where(_ => _.DepartmentId == department.Id).ToList();
                foreach (var user in userList)
                {
                    ideaOfUser = ideaList.Where(i => i.UserID == user.Id).ToList().Count;
                    ideaOfDepartment += ideaOfUser;
                }
                ideasList.Add(ideaOfDepartment);
            }
            int[] ideas = ideasList.ToArray();;
            var data = new
            {
                labels = departmentNameList,
                datasets = new[]
                {
                    new {
                    label = "Ideas",
                    data = ideas,
                    backgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"

                    },
                    borderColor = "rgb(75,192,192,1)",
                    borderWidth = 0,
                    hoverBackgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"
                    },
                    }

                }
            };

            // Define the options for the chart
            var options = new
            {
                responsive = true,
                plugins = new
                {
                    title = new { display = true, text = " Deparment" },
                    legend = new
                    {
                        position = "top",
                        labels = new
                        {
                            boxWidth = 11,
                            fondColor = "#757681",
                            fontSize = 11
                        }
                    },
                }

            };

            ViewBag.ChartDataStatisticalDepartmentWithIdeas = data;
            ViewBag.ChartOptionsStatisticalDepartmentWithIdeas = options;
        }

        public void StatisticalDepartmentWithContributors()
        {
            List<int> contributorsList = new List<int>();
            var departmentList = _db.Departments.ToList();
            var departmentNameList = departmentList.Select(x => x.Name).ToArray();

            var viewList = _db.Views.ToList();
            

            foreach (var department in departmentList)
            {

                int viewOfUser = 0;
                int viewOfDepartment = 0;
                var userList = _db.ApplicationUsers.Where(_ => _.DepartmentId == department.Id).ToList();
                foreach (var user in userList)
                {
                    
                    viewOfUser = viewList.Where(i => i.UserID == user.Id).ToList().Count;
                    viewOfUser = 1;
                    viewOfDepartment += viewOfUser;
                }
                contributorsList.Add(viewOfDepartment);
            }
            int[] contributors = contributorsList.ToArray();

            var data = new
            {
                labels = departmentNameList,
                datasets = new[]
                {
                    new {
                    label = "Contributors",
                    data = contributors,
                    backgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"

                    },
                    borderColor = "rgb(75,192,192,1)",
                    borderWidth = 0,
                    hoverBackgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"
                    },
                    }

                }
            };

            // Define the options for the chart
            var options = new
            {
                responsive = true,
                plugins = new
                {
                    title = new { display = true, text = " Department" },
                    legend = new
                    {
                        position = "top",
                        labels = new
                        {
                            boxWidth = 11,
                            fondColor = "#757681",
                            fontSize = 11
                        }
                    },
                }

            };

            ViewBag.ChartDataStatisticalDepartmentWithContributors = data;
            ViewBag.ChartOptionsStatisticalDepartmentWithContributors = options;
        }

        public void StatisticalIdeasWithComments()
        {

            List<int> commentsList = new List<int>();
            var ideasList = _db.Ideas.ToList();
            var ideaContentList = ideasList.Select(x => x.Content).ToArray();
            var commentList = _db.Comments.ToList();
            

            foreach ( var idea in ideasList )
            {
                    commentsList.Add(commentList.Where(_ => _.IdealID == idea.Id).ToList().Count);
            }

            commentsList.ToArray();

            var data = new
            {
                labels = ideaContentList,
                datasets = new[]
                {
                    new {
                    label = "Comments",
                    data = commentsList,
                    backgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"

                    },
                    //borderColor = "rgb(75,192,192,1)",
                    //borderWidth = 0,
                    hoverBackgroundColor = new[]
                    {
                        "#4dc9f6",
                        "#f67019",
                        "#f53794",
                        "#537bc4",
                        "#acc236",
                        "#166a8f",
                        "#00a950",
                        "#58595b",
                        "#8549ba"
                    },
                    }

                }
            };

            // Define the options for the chart
            var options = new
            {
                indexAxis = 'y',
                elements = new
                {
                    bar = new
                    {
                        borderWidth= 2,
                    }
                },
                responsive = true,
                plugins = new
                {
                    
                    title = new { display = true, text = " Comments" },
                    legend = new
                    {
                        position = "right",
                        labels = new
                        {
                            boxwidth = 11,
                            fondcolor = "#757681",
                            fontsize = 11
                        }
                    },
                },
                scales = new
                {
                    yAxes = new[] { new { ticks = new { beginAtZero = true } } }
                }

            };

            ViewBag.ChartDataStatisticalIdeasWithComments = data;
            ViewBag.ChartOptionsStatisticalIdeasWithComments = options;
        }
    }

    
}
