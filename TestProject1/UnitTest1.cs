using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Controllers;
using StudentEfCoreDemo.Data;
using StudentEfCoreDemo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class StudentsControllerTests
{
    [Fact]
    public async Task GetStudents_ReturnsAllStudents()
    {
        var options = new DbContextOptionsBuilder<StudentContext>()
            .UseInMemoryDatabase(databaseName: "StudentDbTest")
            .Options;

        using (var context = new StudentContext(options))
        {
            context.Students.Add(new Student { Id = 1, FirstName = "Maija", LastName = "Meikäläinen", Age = 20 });
            context.Students.Add(new Student { Id = 2, FirstName = "Matti", LastName = "Meikäläinen", Age = 20 });
            await context.SaveChangesAsync();
        }

        using (var context = new StudentContext(options))
        {
            var controller = new StudentsController(context);

            var result = await controller.GetStudents();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
            var studentsList = Assert.IsAssignableFrom<IEnumerable<Student>>(actionResult.Value);

            Assert.Equal(2, studentsList.Count());
        }
    }

    [Fact]
    public async Task GetStudent_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<StudentContext>()
            .UseInMemoryDatabase(databaseName: "StudentDbTest_NotFound")
            .Options;

        using (var context = new StudentContext(options))
        {
            var controller = new StudentsController(context);
            var result = await controller.GetStudent(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

}
