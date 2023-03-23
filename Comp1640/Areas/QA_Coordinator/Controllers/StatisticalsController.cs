
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
            MyChart();
            StatisticalCategoryWithIdeas();
            return View();
        }

        public void MyChart()
        {
            // Define the data for the chart
            var data = new
            {
                labels = new[] { "January", "February", "March", "April", "May", "June", "July" },
                datasets = new[]
                {
                    new {
                        label = "Sales",
                        data = new[] { 65, 59, 80, 81, 56, 55, 40 },
                        backgroundColor = "rgba(75,192,192,1)",
                        borderColor = "rgba(75,192,192,1)",
                        borderWidth = 1
                        }
                }
            };

            // Define the options for the chart
            var options = new
            {
                responsive = true,
                title = new { display = true, text = "Sales Chart" },
                scales = new
                {
                    yAxes = new[] { new { ticks = new { beginAtZero = true } } }
                }
            };

            // Pass the data and options to the view
            ViewBag.ChartData = data;
            ViewBag.ChartOptions = options;

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
                    title = new { display = true, text = " Chart" },
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


    }
}
