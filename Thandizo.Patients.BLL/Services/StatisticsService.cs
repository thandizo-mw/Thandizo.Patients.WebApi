using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thandizo.DAL.Models;
using Thandizo.DataModels.General;
using Thandizo.DataModels.Statistics;

namespace Thandizo.Patients.BLL.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly thandizoContext _context;

        public StatisticsService(thandizoContext context)
        {
            _context = context;
        }

        public async Task<OutputResponse> GetByDistricts()
        {
            var statistics = await (from p in _context.Patients
                                    join d in _context.Districts on p.DistrictCode equals d.DistrictCode
                                    join r in _context.Regions on d.RegionId equals r.RegionId
                                    join s in _context.PatientStatuses on p.PatientStatusId equals s.PatientStatusId
                                    group p by new
                                    {
                                        d.DistrictName,
                                        s.PatientStatusName,
                                        r.RegionName,
                                        d.Latitude,
                                        d.Longitude
                                    } into ds
                                    orderby ds.Key.PatientStatusName
                                    select new DistrictStatisticsDTO
                                    {
                                        TotalNumberOfPatients = ds.Count(),
                                        DistrictName = ds.Key.DistrictName,
                                        PatientStatusName = ds.Key.PatientStatusName,
                                        RegionName = ds.Key.RegionName,
                                        Longitude = ds.Key.Longitude,
                                        Latitude = ds.Key.Latitude
                                    }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = statistics
            };
        }

        public async Task<OutputResponse> GetByRegion()
        {
            var statistics = await (from p in _context.Patients
                                    join d in _context.Districts on p.DistrictCode equals d.DistrictCode
                                    join r in _context.Regions on d.RegionId equals r.RegionId
                                    join s in _context.PatientStatuses on p.PatientStatusId equals s.PatientStatusId
                                    group p by new
                                    {
                                        s.PatientStatusName,
                                        r.RegionName,
                                        r.Latitude,
                                        r.Longitude
                                    } into ds
                                    orderby ds.Key.PatientStatusName
                                    select new RegionalStatisticsDTO
                                    {
                                        TotalNumberOfPatients = ds.Count(),
                                        PatientStatusName = ds.Key.PatientStatusName,
                                        RegionName = ds.Key.RegionName,
                                        Longitude = ds.Key.Longitude,
                                        Latitude = ds.Key.Latitude
                                    }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = statistics
            };
        }

        public async Task<OutputResponse> GetByNation()
        {
            var statistics = await (from p in _context.Patients
                                    join d in _context.Districts on p.DistrictCode equals d.DistrictCode
                                    join r in _context.Regions on d.RegionId equals r.RegionId
                                    join s in _context.PatientStatuses on p.PatientStatusId equals s.PatientStatusId
                                    group p by new { s.PatientStatusName } into ds
                                    orderby ds.Key.PatientStatusName
                                    select new NationalStatisticsDTO
                                    {
                                        TotalNumberOfPatients = ds.Count(),
                                        PatientStatusName = ds.Key.PatientStatusName,
                                    }).ToListAsync();

            return new OutputResponse
            {
                IsErrorOccured = false,
                Result = statistics
            };
        }
    }
}
