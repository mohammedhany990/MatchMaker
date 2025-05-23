﻿namespace MatchMaker.Core.Entities
{
    public class Message : BaseEntity<int>
    {
        //public int Id { get; set; }

        public string SenderUsername { get; set; }
        public string RecipientUsername { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }


        public int SenderId { get; set; }
        public AppUser Sender { get; set; }


        public int RecipientId { get; set; }
        public AppUser Recipient { get; set; }

    }
}
