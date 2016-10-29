using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server.Tables;

namespace CustomerModel
{
    public abstract class SyncableEntity : ITableData
    {
        // protected CustomEntity();
        /*
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [TableColumn(TableColumnType.CreatedAt)]
        public DateTimeOffset? CreatedAt { get; set; }
        [TableColumn(TableColumnType.Deleted)]
        public bool Deleted { get; set; }
        [Key]
        [TableColumn(TableColumnType.Id)]
        public string Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [TableColumn(TableColumnType.UpdatedAt)]
        public DateTimeOffset? UpdatedAt { get; set; }
        [TableColumn(TableColumnType.Version)]
        [Timestamp]
        public byte[] Version { get; set; }
        */

        [Key]
        [TableColumnAttribute(TableColumnType.Id)]
        public string Id { get; set; }

        [Timestamp]
        [TableColumnAttribute(TableColumnType.Version)]
        public byte[] Version { get; set; }

        [Index(IsClustered = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [TableColumnAttribute(TableColumnType.CreatedAt)]
        public DateTimeOffset? CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [TableColumnAttribute(TableColumnType.UpdatedAt)]
        public DateTimeOffset? UpdatedAt { get; set; }

        [TableColumnAttribute(TableColumnType.Deleted)]
        public bool Deleted { get; set; }

    }
}
