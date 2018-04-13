using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestMakerFreeWebApp.Data.Models
{
    public class Token
    {
        #region constructor
            public Token() {}
        #endregion

        #region Properties
            [Key]
            [Required]
            public int Id { get; set; }

            [Required]
            public string ClientId { get; set; }

            [Required]
            public string Value { get; set; }

            public int Type { get; set; }

            [Required]
            public string UserId { get; set; }

            [Required]
            public DateTime CreatedDate { get; set; }
        #endregion  

            [ForeignKey("UserId")]
            public virtual ApplicationUser User { get; set; }

    }
}
