using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DevExtremeAspNetCoreApp3.Models.EF;

namespace DevExtremeAspNetCoreApp3.Controllers
{
    [Route("api/[controller]/[action]")]
    public class StudentsController : Controller
    {
        private qwertyContext _context;

        public StudentsController(qwertyContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var students = _context.Students.Select(i => new {
                i.StudentId,
                i.Surename,
                i.Name,
                i.Course
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "StudentId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(students, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Student();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Students.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.StudentId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Students.FirstOrDefaultAsync(item => item.StudentId == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.Students.FirstOrDefaultAsync(item => item.StudentId == key);

            _context.Students.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Student model, IDictionary values) {
            string STUDENT_ID = nameof(Student.StudentId);
            string SURENAME = nameof(Student.Surename);
            string NAME = nameof(Student.Name);
            string COURSE = nameof(Student.Course);

            if(values.Contains(STUDENT_ID)) {
                model.StudentId = Convert.ToInt32(values[STUDENT_ID]);
            }

            if(values.Contains(SURENAME)) {
                model.Surename = Convert.ToString(values[SURENAME]);
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(COURSE)) {
                model.Course = Convert.ToInt32(values[COURSE]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}