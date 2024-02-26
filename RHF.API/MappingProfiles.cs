using AutoMapper;
using RHF.DAL;
using RHF.Shared;

public class MappingProfiles : Profile {
    public MappingProfiles()
    {
        CreateMap<BenefactorDTO, Benefactor>();
        CreateMap<BenefactorContributionsDTO, BenefactorContribution>();
        CreateMap<DonationsDetailDTO, DonationsDetail>();
        CreateMap<DonationsHeaderDTO, DonationsHeader>();
    }
}