using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ElderlyHealthMonitor.Application.DTOs;
using ElderlyHealthMonitor.Domain.Entities;

namespace ElderlyHealthMonitor.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ElderlyProfile, ElderlyDto>().ReverseMap();
            CreateMap<Device, DeviceDto>().ReverseMap();
            CreateMap<SensorReading, SensorReadingDto>()
            .ForMember(d => d.Value, opt => opt.MapFrom(s => s.ValueDouble))
            .ReverseMap()
            .ForMember(d => d.ValueDouble, opt => opt.MapFrom(s => s.Value));
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<Alert, AlertDto>().ReverseMap();
        }
    }
}
