﻿using System;

namespace FeedbackService.Database.Models
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public int StarsAmount { get; set; }

        public Feedback(Guid id,
            string? text,
            int starsAmount)
        {
            Id = id;
            Text = text;
            StarsAmount = starsAmount;
        }
    }
}