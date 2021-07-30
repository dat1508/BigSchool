using Lab04.Models;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Http;

namespace Lab04.Controllers
{
    public class AttendancesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Attend(Course attendanceDto)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            if (context.Attendances.Any(p => p.Attendee == userID && p.IdCourse ==
            attendanceDto.Id))
            {
                // xóa thông tin khóa học đã đăng ký tham gia trong bảng Attendances
                context.Attendances.Remove(context.Attendances.SingleOrDefault(p =>
                p.Attendee == userID && p.IdCourse == attendanceDto.Id));
                context.SaveChanges();
                return Ok("cancel");
            }
            var attendance = new Attendance()
            {
                IdCourse = attendanceDto.Id,
                Attendee = User.Identity.GetUserId()
            };
            context.Attendances.Add(attendance);
            context.SaveChanges();
            return Ok();
        }
    }
}
