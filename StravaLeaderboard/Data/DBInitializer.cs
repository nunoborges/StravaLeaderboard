﻿using StravaLeaderboard.models;
using System;
using System.Linq;

namespace StravaLeaderboard.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();

            // Look for any clubs.
            if (context.Clubs.Any())
            {
                return;   // DB has been seeded
            }

            var clubs = new Club[]
            {
                new Club{ClubID=1,StravaClubID=238810,Name="SMITE",Location="Volcano", Country="Watopia"},
                new Club{ClubID=2,StravaClubID=238810,Name="SMITE-London",Location="Radio Tower", Country="Watopia"},
                new Club{ClubID=3,StravaClubID=238810,Name="SMITE-Watopia",Location="Hank's House", Country="Watopia"}
            };
            
            foreach (Club club in clubs)
            {
                context.Clubs.Add(club);
            }
            context.SaveChanges();

            
            // Look for any seasons.
            if (context.Seasons.Any())
            {
                return;   // DB has been seeded
            }

            var seasons = new Season[]
            {
                new Season{SeasonID=1,ClubID=1,Start_date=DateTime.Now,End_date=DateTime.Now, Keyword="smite"},
                new Season{SeasonID=2,ClubID=2,Start_date=DateTime.Now,End_date=DateTime.Now, Keyword="london"},
                new Season{SeasonID=3,ClubID=3,Start_date=DateTime.Now,End_date=DateTime.Now, Keyword="watopia"}
            };

            foreach (Season season in seasons)
            {
                context.Seasons.Add(season);
            }
            context.SaveChanges();


            // Look for any events.
            if (context.DayEvents.Any())
            {
                return;   // DB has been seeded
            }

            var dayevents = new DayEvent[]
            {
                new DayEvent{DayEventID=1,Date=DateTime.Now,SeasonID=1},
                new DayEvent{DayEventID=2,Date=DateTime.Now,SeasonID=2},
                new DayEvent{DayEventID=3,Date=DateTime.Now,SeasonID=3},
                new DayEvent{DayEventID=4,Date=DateTime.Now,SeasonID=1}
            };

            foreach (DayEvent dayevent in dayevents)
            {
                context.DayEvents.Add(dayevent);
            }
            context.SaveChanges();


            // Look for any segments.
            if (context.Segments.Any())
            {
                return;   // DB has been seeded
            }

            var segments = new Segment[]
            {
                new Segment{SegmentID=13855855,Name="s1",Type="green",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=14485439,Name="s2",Type="polka",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=13521759,Name="s3",Type="polka",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=14250115,Name="s4",Type="green",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=14063868,Name="s5",Type="polka",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=13619366,Name="s6",Type="polka",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=16730849,Name="s7",Type="polka",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=16730862,Name="s8",Type="green",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=16730897,Name="s9",Type="green",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=16730888,Name="s10",Type="polka",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=16936841,Name="s11",Type="green",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1},
                new Segment{SegmentID=16730909,Name="s12",Type="green",World="watopia",Distance=1,Average_grade=3,Total_elevation_gain=2,Star_count=1}
            };

            foreach (Segment segment in segments)
            {
                context.Segments.Add(segment);
            }
            context.SaveChanges();


            // Look for any eventsegments.
            if (context.DayEventSegments.Any())
            {
                return;   // DB has been seeded
            }

            var eventsegments = new DayEventSegment[]
            {
                new DayEventSegment{DayEventID=1,SegmentID=13855855},
                new DayEventSegment{DayEventID=1,SegmentID=14485439},
                new DayEventSegment{DayEventID=1,SegmentID=13521759},
                new DayEventSegment{DayEventID=1,SegmentID=14250115},
                new DayEventSegment{DayEventID=2,SegmentID=14063868},
                new DayEventSegment{DayEventID=2,SegmentID=13619366},
                new DayEventSegment{DayEventID=3,SegmentID=16730849},
                new DayEventSegment{DayEventID=3,SegmentID=16730862},
                new DayEventSegment{DayEventID=4,SegmentID=16730849},
                new DayEventSegment{DayEventID=4,SegmentID=16730862},
                new DayEventSegment{DayEventID=4,SegmentID=16730897},
                new DayEventSegment{DayEventID=4,SegmentID=16730888},
                new DayEventSegment{DayEventID=4,SegmentID=16936841},
                new DayEventSegment{DayEventID=4,SegmentID=16730909}
            };

            foreach (DayEventSegment eventsegment in eventsegments)
            {
                context.DayEventSegments.Add(eventsegment);
            }
            context.SaveChanges();

        }
    }
}