﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StravaLeaderboard.Data;
using StravaLeaderboard.models;
using StravaLeaderboard.API;
using StravaLeaderboard.Http;
using Microsoft.EntityFrameworkCore;

namespace StravaLeaderboard.Pages
{
    public class AdminModel : PageModel
    {
        
        private DataContext _db;
        public APITokens _apitokens { get; set; }
        public AdminModel(DataContext db, IOptions<APITokens> apitokens)
        {
            _db = db;
            _apitokens = apitokens.Value;
        }

        //[TempData]
        //public string Message { get; set; }

        [BindProperty]
        public string SeasonID { get; set; }
        [BindProperty]
        public int DayEventID { get; set; }
        [BindProperty]
        public int UserClubID { get; set; }
        [BindProperty]
        public string UserKeyword { get; set; }

        public List<Club> ClubsSelection { get; set; }
        public List<Season> Seasons { get; set; }
        public List<DayEvent> DayEvents { get; set; }
        public List<EventSegment> EventSegments { get; set; }
        public List<Segment> Segments { get; set; }

        public async Task OnGetAsync(int? id)
        {
            ClubsSelection = await _db.Clubs.ToListAsync();
            Seasons = await _db.Seasons.ToListAsync();
            DayEvents = await _db.DayEvents.ToListAsync();
            EventSegments = await _db.EventSegments.ToListAsync();
            Segments = await _db.Segments.ToListAsync();
        }

        //SegmentActivities is a collection of Activities for each Segment on that DayEvent
        //public List<JSONActivity> SegmentActivities = new List<JSONActivity>();
        //public List<Segment> SegmentLeaderboard = new List<Segment>();

        public async Task<IActionResult> OnPostAsync()
        {
            //TODO: model should recognize if this button was clicked already
            if (!ModelState.IsValid)
            {
                return Page();
            }

            EventSegments = await _db.EventSegments
                .Where(i => i.DayEventID == DayEventID)
                .AsNoTracking()
                .ToListAsync();

            //get club activities - limited to top 200
            List<JSONActivity> JSONActivities = await GetStravaActivities();
            //parse out activites with <Club.Keyword> in title
            JSONActivities = ParseActivities(JSONActivities);

            for (int x = 0; x < EventSegments.Count; x++)
            {
                //get Segment Leaderboard for list of segments
                RAWResults SegmentEntries = await GetSegmentEntries(EventSegments[x].SegmentID);
                //add efforts to Activity where usernames match between the segment and activity
                List<JSONActivity> Activities = AddSegmentEntries(SegmentEntries, JSONActivities);
                //save activities and athletes if new (to database)
                SaveActivities(Activities);
            }

            //TODO: only go to /leaderboard if there are no errors
            return RedirectToPage("/Leaderboard");
        }

        public async Task<List<JSONActivity>> GetStravaActivities()
        {
            int? stravaClubID = GetStravaClubID();
            string getUrl = string.Format("{0}/{1}/activities?per_page={2}&access_token={3}", 
                Endpoints.Club, 
                stravaClubID, 
                200, 
                _apitokens.Access_Token
                );
            string json = await Strava.Http.WebRequest.SendGetAsync(new Uri(getUrl));

            return Unmarshaller<List<JSONActivity>>.Unmarshal(json);
        }

        private int? GetStravaClubID()
        {
            return _db.Clubs.Where(i => i.ClubID == UserClubID).Select(s => s.StravaClubID).First();
        }

        public List<JSONActivity> ParseActivities(List<JSONActivity> activities)
        {
            List<JSONActivity> ParsedActivities = (from activity in activities
                                                   where activity.Name.ToLower().Contains(UserKeyword)
                                                   select activity).ToList();
            return ParsedActivities;
        }

        public void SaveActivities(List<JSONActivity> jsonActivities)
        {
            jsonActivities = pruneActivitiesByEventAthlete(jsonActivities);

            foreach (JSONActivity jsonActivity in jsonActivities)
            {
                if (!(_db.Athletes.Any(o => o.AthleteID == jsonActivity.Athlete.Id)))
                {
                    CreateNewAthlete(jsonActivity);
                }
                //TODO: Have to add athlete and seasonathlete first
                //check if user exists
                // if not, then add athlete and seasonathlete instance
                if (!(_db.Activities.Any(o => o.ActivityID == jsonActivity.Id)))
                {
                    CreateNewActivity(jsonActivity);
                }
            }
        
            //var allCourses = context.Courses;
            //var instructorCourses = new HashSet<int>(
            //    instructor.CourseAssignments.Select(c => c.CourseID));
            //AssignedCourseDataList = new List<AssignedCourseData>();
            //foreach (var course in allCourses)
            //{
            //    AssignedCourseDataList.Add(new AssignedCourseData
            //    {
            //        CourseID = course.CourseID,
            //        Title = course.Title,
            //        Assigned = instructorCourses.Contains(course.CourseID)
            //    });
            //}
        }

        private void CreateNewActivity(JSONActivity jsonActivity)
        {
            Activity activity = new Activity
            {
                ActivityID = jsonActivity.Id,
                Name = jsonActivity.Name,
                Start_date = jsonActivity.Start_date,
                Achievement_count = jsonActivity.Achievement_count,
                Comment_count = jsonActivity.Comment_count,
                Kudos_count = jsonActivity.Kudos_count,
                Flagged = jsonActivity.Flagged,
                EventID = DayEventID,
                AthleteID = jsonActivity.Athlete.Id
            };
            _db.Activities.Add(activity);
            _db.SaveChanges();
        }

        private void CreateNewAthlete(JSONActivity jsonActivity)
        {
            Athlete athlete = new Athlete
            {
                AthleteID = jsonActivity.Athlete.Id,
                FirstName = jsonActivity.Athlete.FirstName,
                LastName = jsonActivity.Athlete.LastName,
                Date_joined = DateTime.Now,
                Profile = jsonActivity.Athlete.Profile.ToString(),
                Sex = jsonActivity.Athlete.Sex,
                UserName = jsonActivity.Athlete.UserName
            };
            _db.Athletes.Add(athlete);
            SeasonAthlete seasonAthlete = new SeasonAthlete
            {
                AthleteID = jsonActivity.Athlete.Id,
                SeasonID = Convert.ToInt32(SeasonID)
            };
            _db.SeasonAthletes.Add(seasonAthlete);
            _db.SaveChanges();
        }

        // Remove all activities that didn't have an instance of SegmentResults
        // which means they didn't participate in the Club's Event for that day
        private List<JSONActivity> pruneActivitiesByEventAthlete(List<JSONActivity> jsonActivities)
        {
            return (from activity in jsonActivities
                    where activity.Athlete.SegmentResults != null
                    select activity).ToList();
        }

        private async Task<RAWResults> GetSegmentEntries(int Segment)
        {
            int? stravaClubID = GetStravaClubID();
            string getUrl = string.Format("{0}/{1}/leaderboard?club_id={2}&per_page={3}&context_entries={4}&date_range={5}&access_token={6}",
                Endpoints.Leaderboard,
                Segment.ToString(),
                stravaClubID,
                200,
                0,
                "this_year",
                _apitokens.Access_Token
                );
            string json = await Strava.Http.WebRequest.SendGetAsync(new Uri(getUrl));

            return Unmarshaller<RAWResults>.Unmarshal(json);
        }

        private List<JSONActivity> AddSegmentEntries(RAWResults segmentEntries, List<JSONActivity> activities)
        {
            //int points = 10;
            int rankCounter = 1;

            foreach (var segmentEntry in segmentEntries.Entries)
            {
                foreach (JSONActivity activity in activities)
                {
                    if (segmentEntry.Athlete_name.ToLower().Equals(activity.Athlete.FirstName.ToLower() + " " + activity.Athlete.LastName.Substring(0, 1).ToLower() + "."))
                    {
                        activity.Athlete.SegmentResults = new JSONResults()
                        {
                            Rank = rankCounter,
                            Start_date = segmentEntry.Start_date,
                            Elapsed_time = segmentEntry.Elapsed_time
                            //Points = points
                        };

                        //activity.Athlete.TotalPoints = activity.Athlete.TotalPoints + points;
                        ////TODO: if polka points then add
                        //activity.Athlete.PolkaPoints = activity.Athlete.PolkaPoints + points;
                        ////TODO: if green points then add
                        //activity.Athlete.GreenPoints = activity.Athlete.GreenPoints + points;
                        //points = (points == 0) ? 0 : points - 2;
                        //activity.Athlete.SegmentCount++;
                        rankCounter++;
                        break;
                    }
                }
            }

            return activities;
        }
    }
}
