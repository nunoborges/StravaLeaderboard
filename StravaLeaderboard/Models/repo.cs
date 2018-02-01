﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace StravaLeaderboard.models
{
    public class Club
    {
        public int ClubID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
    }

    public class Activity
    {
        //activity members
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start_date { get; set; }
        public int Achievement_count { get; set; }
        public int Comment_count { get; set; }
        public int Kudos_count { get; set; }
        public Boolean Flagged { get; set; }

        //reference members
        public Athlete Athlete { get; set; }
        public int EventID { get; set; }
    }

    public class Athlete
    {
        //Athlete details members
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Uri Profile { get; set; }

        public List<SegmentLeaderboard> SegmentLeaderboard { get; set; }

        //Segment Results
        private int segmentCount = 0;
        public int SegmentCount
        {   get
            { return segmentCount; }
            set
            { segmentCount = value; }
        }

        private int greenPoints = 0;
        public int GreenPoints
        {
            get
            { return greenPoints; }
            set
            { greenPoints = value; }
        }

        private int polkaPoints = 0;
        public int PolkaPoints
        {
            get
            { return polkaPoints; }
            set
            { polkaPoints = value; }
        }

        private int totalPoints = 0;
        public int TotalPoints
        {
            get
            { return totalPoints; }
            set
            { totalPoints = value; }
        }
    }

    public class SegmentLeaderboard
    {
        public int SegmentLeaderboardID { get; set; }
        public string SegmentType { get; set; }
        public int Rank { get; set; } = 0;
        public int Elapsed_time { get; set; }
        public DateTime Start_date { get; set; }

        //calculated
        public int Points { get; set; }
    }

    public class SegmentEntries
    {
        public int SegmentEntriesID { get; set; }
        public int Entry_count { get; set; }

        //reference members
        public List<Entries> Entries { get; set; }
    }

    public class Entries
    {
        public int EntriesID { get; set; }
        public string Athlete_name { get; set; }
        public int Elapsed_time { get; set; }
        public int Rank { get; set; }
        public DateTime Start_date { get; set; }
    }



}