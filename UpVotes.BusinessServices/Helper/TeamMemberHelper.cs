using System;
using System.Collections.Generic;
using System.Linq;
using UpVotes.BusinessEntities.Entities;
using UpVotes.DataModel;

namespace UpVotes.BusinessServices
{
    public class TeamMemberHelper
    {
        private UpVotesEntities _context = null;

        public List<TeamMemebersEntity> GetTeamMembers(int memberID, int companyID, int softwareID, int noOfRows)
        {
            using (_context = new UpVotesEntities())
            {
                var teamMembersList = (from a in _context.Sp_GetTeamMembers(memberID, companyID, softwareID, noOfRows)
                                                            select new TeamMemebersEntity()
                                                            {
                                                                MemberId = a.MemberID,
                                                                CompanyId = a.CompanyID,
                                                                SoftwareId = a.SoftwareID,
                                                                MemberName = a.MemberName,
                                                                PictureName = a.PictureName,
                                                                EmailId = a.EmailID,
                                                                Designation = a.Designation,
                                                                LinkedInProfile = a.LinkedInProfile,
                                                                StartDate = a.StartDate.ToString("MM/dd/yyyy"),
                                                                EndDate = a.EndDate == null ? string.Empty : Convert.ToDateTime(a.EndDate).ToString("MM/dd/yyyy")
                                                            }).ToList();

                return teamMembersList;
            }
        }

        public int SaveTeamMember(TeamMemebersEntity teamMember)
        {
            using (_context = new UpVotesEntities())
            {
                var endDate = string.IsNullOrEmpty(teamMember.EndDate) ? (DateTime?)null : DateTime.Parse(teamMember.EndDate);

                return teamMember.MemberId == 0
                    ? Convert.ToInt32(_context.Sp_InsTeamMember(teamMember.CompanyId, teamMember.SoftwareId,
                        teamMember.MemberName,
                        teamMember.PictureName, teamMember.Designation, teamMember.LinkedInProfile,
                        Convert.ToDateTime(teamMember.StartDate), endDate).FirstOrDefault())
                    : Convert.ToInt32(_context.Sp_UpdTeamMember(teamMember.MemberId, teamMember.MemberName,
                        teamMember.PictureName,
                        teamMember.Designation, teamMember.LinkedInProfile, Convert.ToDateTime(teamMember.StartDate),
                        endDate).FirstOrDefault());
            }
        }
    }
}
