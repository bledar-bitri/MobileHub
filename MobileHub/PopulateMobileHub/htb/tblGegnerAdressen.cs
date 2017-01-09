namespace PopulateMobileHub.htb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblGegnerAdressen")]
    public partial class tblGegnerAdressen
    {
        [Key]
        public int GAID { get; set; }

        public int? GAGegner { get; set; }

        public int? GAType { get; set; }

        [StringLength(50)]
        public string GAStrasse { get; set; }

        [StringLength(3)]
        public string GAZipPrefix { get; set; }

        [StringLength(10)]
        public string GAZIP { get; set; }

        [StringLength(50)]
        public string GAOrt { get; set; }

        public DateTime? GATimeStamp { get; set; }

        public int? GAUserID { get; set; }

        [StringLength(60)]
        public string GAName1 { get; set; }

        [StringLength(60)]
        public string GAName2 { get; set; }

        [StringLength(60)]
        public string GAName3 { get; set; }

        public decimal GALatitude { get; set; }

        public decimal GALongitude { get; set; }

        [Required]
        [StringLength(100)]
        public string GADescription { get; set; }
    }
}
