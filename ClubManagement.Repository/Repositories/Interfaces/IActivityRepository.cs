using ClubManagement.Repository.Basic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Activity = ClubManagement.Repository.Models.Activity;

namespace ClubManagement.Repository.Repositories.Interfaces
{
    public interface IActivityRepository : IGenericRepository<Activity>
    {
    }
}
