﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuggyDemoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace BuggyDemoWeb.Controllers
{
    public class MemoryLeakController : Controller
    {
        private IMemoryCache cache;
        private static ConcurrentBag<string> _myListKeepsGrowing= new ConcurrentBag<string>();

        public MemoryLeakController(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet("memoryleak/concatonate-strings-oom")]
        public IActionResult ConcatString()
        {
            var html = "<table cellpadding=\"0\" cellspacing=\"0\"><tbody><tr>";
            var newrocord = new DataRecord() { FirstName = "Marco", LastName = "Polo", Address1 = "Lichfield Road", Address2 = "", City = "", State = "Indiana" };

            foreach (var rec in newrocord.MyList)
            {
                html += html + string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td> </tr>", rec.FirstName, rec.LastName, rec.Address1, rec.Address2, rec.City, rec.State);
            }

            html += html + "</table>";

            return Ok(html);
        }

        [HttpGet("memoryleak/static-object-leak")]
        public IActionResult StaticObjects()
        {
            var val = new DataRecord() { FirstName = "Mark", LastName = "Smith", Address1 = "Wem Street", Address2 = "", City = "Lichfield", State = "Ohio" };

            var val1 = new DataRecord() { FirstName = "Pete", LastName = "Jones", Address1 = "Lawrence Ave", Address2 = "on the corner", City = "", State = "Oregon" };

            var val2 = new DataRecord() { FirstName = "Andrew", LastName = "Jordan", Address1 = "Coleman Ave", Address2 = "", City = "", State = "Michigan" };

            var val3 = new DataRecord() { FirstName = "Marco", LastName = "Stephen", Address1 = "Lichfield Road", Address2 = "", City = "", State = "Indiana" };

            return Ok(val3.TotalCount);
        }

        [HttpGet("memoryleak/static-object-leak-v2")]
        public ActionResult<string> AddToStaticStringList()
        {
            var data = new String('x', 20 * 1024);
            _myListKeepsGrowing.Add(data);

            return data;
        }

        /// <summary>
        /// Create a 20k string and return it...
        /// </summary>
        /// <returns></returns>
        [HttpGet("memoryleak/allocate-large-strings")]
        public ActionResult<string> AllocateALargeString()
        {
            return new String('x', 20 * 1024);
        }

        [HttpGet("memoryleak/native-leak")]
        public void GetFileProvider()
        {
            var fp = new PhysicalFileProvider(Environment.CurrentDirectory);
            fp.Watch("*.txt");
        }
    }
}
