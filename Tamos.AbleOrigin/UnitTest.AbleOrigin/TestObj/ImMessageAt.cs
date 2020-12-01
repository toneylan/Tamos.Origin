using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tamos.AbleOrigin.DataPersist;

namespace UnitTest.AbleOrigin
{
    [ShardingTable("im_message_at")]
    public class ImMessageAt
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
		public long Id { get; set; }
        
        [Column("chat_id")]
		public long ChatId { get; set; }
        
        [Column("msg_id")]
		public long MsgId { get; set; }
        
        [Column("at_user_id")]
		public int AtUserId { get; set; }
        
        [Column("is_read")]
		public bool IsRead { get; set; }
        
	}
}