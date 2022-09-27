using System.ComponentModel.DataAnnotations;

namespace WebApiDemo.Entities
{
    // 复合主键使用场景

    public class UserBook
    {
        public string UserId { get; set; }
        public int BookId { get; set; }

        public string Name { get; set; }
    }
}
