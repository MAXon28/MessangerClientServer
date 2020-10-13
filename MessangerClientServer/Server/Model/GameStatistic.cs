using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model
{
    [Table("GameStatistics")]
    class GameStatistic
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int AllGameWithUsers { get; set; }

        public int WinWithUsers { get; set; }

        public int LoseWithUsers { get; set; }

        public int AllGameWithComputer { get; set; }

        public int WinWithComputer { get; set; }

        public int LoseWithComputer { get; set; }

        public virtual User User { get; set; }
    }
}