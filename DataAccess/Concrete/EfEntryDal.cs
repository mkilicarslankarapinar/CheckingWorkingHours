﻿using Core.EntityFramework;
using DataAccess.Abstract;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfEntryDal:EfEntityRepositoryBase<Entry, CheckingWorkingHoursContext>, IEntryDal
    {
    }
}
