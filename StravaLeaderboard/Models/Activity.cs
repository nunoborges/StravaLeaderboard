﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StravaLeaderboard.models
{
    public class Activity
    {
        //activity members
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActivityID { get; set; }
        public string Name { get; set; }
        public DateTime Start_date { get; set; }
        public int Achievement_count { get; set; }
        public int Comment_count { get; set; }
        public int Kudos_count { get; set; }
        public Boolean Flagged { get; set; } = false;
        public int Green_points { get; set; } = 0;
        public int Polka_points { get; set; } = 0;

        //foreign keys
        public int DayEventID { get; set; }
        public int AthleteID { get; set; }

        //reference members
        public DayEvent Event { get; set; }
        public Athlete Athlete { get; set; }
        [JsonIgnore]
        public List<ActivityResult> ActivityResults { get; set; }
    }

    public class ActivityResult
    {
        //members
        public int ActivityResultID { get; set; }
        public int Rank { get; set; }
        public int Strava_rank { get; set; }
        public int Elapsed_time { get; set; }
        public DateTime Start_date { get; set; }
        public int Points { get; set; } = 0;
        public Boolean Personal_best { get; set; } = false;

        //foreign keys
        public int ActivityID { get; set; }
        public int SegmentID { get; set; }

        //reference members
        public Activity Activity { get; set; }
        [JsonIgnore]
        public Segment Segment { get; set; }
    }

    //TODO: deprecate this model entity
    //public class AthleteEventResult
    //{
    //    public int AthleteEventResultID { get; set; }
    //    public int Green_points { get; set; }
    //    public int Polka_points { get; set; }

    //    //foreign key
    //    public int ActivityID { get; set; }

    //    //reference members
    //    [JsonIgnore]
    //    public Activity Activity { get; set; }
    //}
}
