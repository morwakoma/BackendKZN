using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace eRecruitment.BusinessDomain.DAL.Entities.AppModels
{
    public class ProvinceListModel
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }

    }

    public class Competency
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
    }

    public class RoleListModel
    {
        public int ProvinceIDX { get; set; }
        public string ProvinceName { get; set; }

    }

    public class UserRoleModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        //public string UserId { get; set; }
        //public string RoleId { get; set; }
        public int OrganisationID { get; set; }
    }

    public class UserRoleDetailsGeneral
    {
        public string OrganisationName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public int AssignedDivisionDepartmentID { get; set; }

    }

    public class OrganisationModel
    {
        public int OrganisationID { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationName { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; }
        public int id { get; set; }
    }

    public class DepartmentModel
    {
        [Key]
        public int? DepartmentID { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Required]
        [Display(Name = "Organisation Name")]
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }

        [Display(Name = "Department Manager Name")]
        public string ManagerID { get; set; }
        public string ManagerName { get; set; }
        public int DivisionID { get; set; }
        public string DivisionDiscription { get; set; }
    }


    public class DepartmentManagerModel
    {
        [Key]
        public string UserID { get; set; }
        public string ManagerName { get; set; }

    }

    public class DivisionModel
    {
        [Key]
        public int DivisionID { get; set; }
        public string DivisionDiscription { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
    }

    public class DisabilityModel
    {
        [Key]
        public int DisabilityID { get; set; }
        public string Disability { get; set; }

    }

    public class DisclamerModel
    {
        [Key]
        public int DisclamerID { get; set; }
        public int OrginazationID { get; set; }

        //[RegularExpression(@"#([0-9][A-F])*", ErrorMessage = "Please enter valid Disclamer")]
        public string Disclamer { get; set; }

    }


    public class EmploymentTypeModel
    {
        [Key]
        public int EmploymentTypeID { get; set; }
        public string EmploymentType { get; set; }

    }

    public class InterviewCategoryModel
    {
        [Key]
        public int InterviewCatID { get; set; }
        public string InterviewCatDescription { get; set; }

    }

    public class InterviewTypeModel
    {
        [Key]
        public int InterviewTypeID { get; set; }
        public string InterviewTypeDescription { get; set; }

    }

    public class GeneralQuestionModel
    {
        [Key]
        public int id { get; set; }
        public string GeneralQuestionDesc { get; set; }
        public int OrganisationID { get; set; }
        public int QCategoryID { get; set; }

    }

    public class DisclaimerModel
    {
        [Key]
        public int DisclaimerID { get; set; }
        public int OrganisationID { get; set; }
        public string DisclaimerDescr { get; set; }

    }

    public class JobLevelModel
    {
        [Key]
        public int JobLevelID { get; set; }
        public int OrganisationID { get; set; }
        public string JobLevelName { get; set; }

    }
    public class JobLevelViewModel
    {
        public int JobLevelID { get; set; }
        public string OrganisationName { get; set; }
        public string JobLevelName { get; set; }
    }

    public class JobTitleModel
    {
        [Key]
        public int JobTitleID { get; set; }
        public int OrganisationID { get; set; }
        public string JobTitle { get; set; }
        public int JobLevelID { get; set; }
        public int SalaryCategoryID { get; set; }
        public int SalaryRangeID { get; set; }

    }
    public class JobTitleViewModel
    {
        [Key]
        public int JobTitleID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string JobTitle { get; set; }
        public int JobLevelID { get; set; }
        public String JobLevelName { get; set; }
        public int SalaryCategoryID { get; set; }
        public String CategoryDescr { get; set; }
        public int SalaryRangeID { get; set; }
        public string SalaryRange { get; set; }

    }

    public class JobJobSpecificQuestionModel
    {
        [Key]
        public int JobSpecificeQuestionID { get; set; }
        public int VacancyID { get; set; }
        public string JobSpecificeQuestionDesc { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyDate { get; set; }
        public string ModifiedBy { get; set; }
    }

        public class SalaryCategoryModel
    {
        [Key]
        public int SalaryCategoryID { get; set; }
        public int OrganisationID { get; set; }
        public string CategoryDescr { get; set; }

    }
    public class SalaryCategoryViewModel
    {
        public int SalaryCategoryID { get; set; }
        public int OrganisationID { get; set; }
        public string CategoryDescr { get; set; }
        public string OrganisationName { get; set; }
    }
    public class SalarySubCategoryModel
    {
        [Key]
        public int SalarySubCategoryID { get; set; }
        public int SalaryCategoryID { get; set; }
        public string Descr { get; set; }
        public int OrganisationID { get; set; }
    }
    public class SalarySubCategoryViewModel
    {
        public int SalarySubCategoryID { get; set; }
        public int SalaryCategoryID { get; set; }
        public string SalaryCategoryName { get; set; }
        public string Descr { get; set; }
        public string OrganisationName { get; set; }
        public int OrganisationID { get; set; }

    }
    public class SalaryStructureModel
    {
        [Key]
        public int SalaryStructureID { get; set; }
        public int OrganisationID { get; set; }
        public int JobTitleID { get; set; }
        public int SalaryCategoryID { get; set; }
        public string SalaryCategoryName { get; set; }
        public int JobLevelID { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int SalarySubCategoryID { get; set; }

    }
    public class SalaryStructureViewModel
    {
        public int SalaryStructureID { get; set; }
        public string OrganisationName { get; set; }
        public string JobTitle { get; set; }
        public string SalaryCategoryName { get; set; }
        public string JobLevelName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public String SalarySubCategoryName { get; set; }
        public int JobTitleID { get; set; }
    }
    public class LocationModel
    {
        [Key]
        public int LocationID { get; set; }
        public string LocationDiscription { get; set; }

    }

    public class QualificationTypeModel
    {
        [Key]
        public int QualificationTypeID { get; set; }
        public string QualificationTypeName { get; set; }

    }

    public class RejectReasonModel
    {
        [Key]
        public int RejectReasonID { get; set; }
        public string RejectReason { get; set; }

    }

    public class RetractReasonModel
    {
        [Key]
        public int RetractReasonID { get; set; }
        public string RetractReason { get; set; }

    }

    public class SkillModel
    {
        [Key]
        public int skillID { get; set; }
        public string skillName { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }

    }

    public class SkillsCategoryModel
    {
        [Key]
        public int CategoryID { get; set; }
        public string Description { get; set; }

    }

    public class SkillProficiencyModel
    {
        [Key]
        public int SkillProficiencyID { get; set; }
        public string SkillProficiency { get; set; }

    }

    public class WithdrawalReasonModel
    {
        [Key]
        public int WithdrawalReasonID { get; set; }
        public string WithdrawalReason { get; set; }

    }

    public class QuetionBanksModel
    {
        public int id { get; set; }
        [Required]
        public string GeneralQuestionDesc { get; set; }
        public int TypeID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public int QCategoryID { get; set; }
        public string QCategoryDescr { get; set; }

    }


    public class VacancyProfileModel
    {
        [Key]
        public int VacancyProfileID { get; set; }
        public int OrganisationID { get; set; }
        public string JobTitleID { get; set; }
        public int JobCategory { get; set; }
        public int JobSubCategory { get; set; }
        public int JobLevel { get; set; }
        public int SalaryRange { get; set; }
        public int SkillCategory { get; set; }
        public int QualificationsExperience { get; set; }
        public int TechnicalCompetencies { get; set; }
        public int AdditionalRequirements { get; set; }
        public int Disclaimer { get; set; }
        public int Responsibility { get; set; }

    }

    public class VacancyProfileViewModel
    {
        [Key]
        public int VacancyProfileID { get; set; }
        public int Organisation { get; set; }
        public string JobTitleID { get; set; }
        public int JobCategory { get; set; }
        public int JobSubCategory { get; set; }
        public int JobLevel { get; set; }
        public int SalaryRange { get; set; }
        public int SkillCategory { get; set; }
        public int QualificationsExperience { get; set; }
        public int TechnicalCompetencies { get; set; }
        public int AdditionalRequirements { get; set; }
        public int Disclaimer { get; set; }
        public int Responsibility { get; set; }

    }

    public class JobProfileModel
    {
        [Key]
        public int JobProfileID { get; set; }
        public int OrganisationID { get; set; }
        public int JobTitleID { get; set; }
        public string JobTitle { get; set; }
        public string CategoryDescr { get; set; }
        public string Descr { get; set; }
        public string JobLevelName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string CategoryID { get; set; }
        public int skillID { get; set; }
        public string VacancyPurpose { get; set; }
        public string QualificationAndExperience { get; set; }
        public string Knowledge { get; set; }
        public string TechnicalCompetenciesDescription { get; set; }
        public string AdditionalRequirements { get; set; }
        public string Disclaimer { get; set; }
        public string Responsibility { get; set; }
        [NotMapped]
        public string[] SelectedTechComps { get; set; }
        [NotMapped]
        public string[] SelectedLeadComps { get; set; }
        [NotMapped]
        public string[] SelectedBehaveComps { get; set; }

    }

    public class JobProfileViewModel
    {
        [Key]
        public int JobProfileID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public int JobTitleID { get; set; }
        public string JobTitle { get; set; }
        public int SalaryCategoryID { get; set; }
        public string CategoryDescr { get; set; }
        public int CategoryID { get; set; }
        public string Descr { get; set; }
        public string JobLevelName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string SkillCategory { get; set; }
        public string SkillName { get; set; }
        public string VacancyPurpose { get; set; }
        public string QualificationAndExperience { get; set; }
        public string Knowledge { get; set; }
        public string AdditionalRequirements { get; set; }
        public string Disclaimer { get; set; }
        public string Responsibility { get; set; }
        public string Skill { get; set; }
        [NotMapped]
        public int JobLevelID { get; set; }
        [NotMapped]
        public string[] SelectedTechComps { get; set; }
        [NotMapped]
        public string[] SelectedLeadComps { get; set; }
        [NotMapped]
        public string[] SelectedBehaveComps { get; set; }
        [NotMapped]
        public string TechComps { get; set; }
        [NotMapped]
        public string LeadComps { get; set; }
        [NotMapped]
        public string BehaveComps { get; set; }

    }

    public class VacancyModels
    {
        [Key]
        public int ID { get; set; }
        //  [Required]
        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }
        //  [Required]
        [Display(Name = "Vacancy Name")]
        public string VacancyName { get; set; }
        public int JobTitleID { get; set; }
        public string JobTitle { get; set; }
        public string CategoryDescr { get; set; }
        public string Descr { get; set; }
        public string JobLevelName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int CategoryID { get; set; }
        public string SkillCategory { get; set; }
        public string SkillName { get; set; }
        public string JobLevel { get; set; }
        public int JobLevelID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string Recruiter { get; set; }
        //public string VacancyQuestionID { get; set; }
        public IList<string> VacancyQuestionID { get; set; }

        public IList<string> VacancySkillID { get; set; }

        [EmailAddress]
        public string RecruiterEmail { get; set; }
        public string RecruiterTel { get; set; }
        public string SkillDescription { get; set; }
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }

        public string Manager { get; set; }
        public int EmploymentTypeID { get; set; }
        public string EmploymentType { get; set; }
        public string ContractDuration { get; set; }

        public string Salary { get; set; }
        public string SalaryRange { get; set; }

        [Display(Name = "Responsibilities")]
        public string VacancyPurpose { get; set; }
        public string Responsibility { get; set; }

        public string QualificationAndExperience { get; set; }
     
        public string Knowledge { get; set; }
        public string AdditonalRequirements { get; set; }
        [Required]
        [Display(Name = "Closing Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ClosingDate { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string CreatedDate { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ModifyDate { get; set; }

        public string Disclaimer { get; set; }
        public string WithdrawalReason { get; set; }
        public string RetractReason { get; set; }
        public int NumberOfOpenings { get; set; }

        public int VacancyNameID { get; set; }
        public int VacancyProfileID { get; set; }

       // [Required]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int DivisionID { get; set; }
        public string DivisionName { get; set; }
        public int VancyTypeID { get; set; }
        public string VacancyTypeName { get; set; }
        public int NumberOfDaysID { get; set; }
        public int GenderID { get; set; }
        public int RaceID { get; set; }
        public string DeligationReasons { get; set; }
        public int SalaryTypeID { get; set; }
        public string RecruiterUserId { get; set; }
        public string BPSVacancyNo { get; set; }
        [NotMapped]
        public string TechComps { get; set; }
        [NotMapped]
        public string LeadComps { get; set; }
        [NotMapped]
        public string BehaveComps { get; set; }

        //================Peter 20231101===========================
        public string JobSpecificQuestions { get; set; }
        //=========================================================

       
    }

    public class VacancyListModels
    {
        //,,,,, ,
        public int ID { get; set; }
        public string ReferenceNo { get; set; }
        // public string VacancyName { get; set; }

        //Peter Added the two fields on 20220907
        public string BPSVacancyNo { get; set; }
        public string Recruiter { get; set; }
        
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string EmploymentType { get; set; }
        public string Organisation { get; set; }
        public string CreatedDate { get; set; }
        public string ModifyDate { get; set; }
        public string ClosingDate { get; set; }
        public string Salary { get; set; }
        public string Status { get; set; }
        public int NumberOfOpenings { get; set; }
    }

    public class DownloadVacancyAd
    {
        public string ReferenceNo { get; set; }
        public string JobTitle { get; set; }
        public string JobLevel { get; set; }
        public string Organisation { get; set; }
        public string VacancyTypeName { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string Deviation { get; set; }
        public string Location { get; set; }
        public string NumberOfOpenings { get; set; }
        public string ContractDuration { get; set; }
        public string EmploymentType { get; set; }
        public string Salary { get; set; }
        public string VacancyPurpose { get; set; }
        public string Responsibility { get; set; }
        public string QualificationAndExperience { get; set; }
        public string TechnicalCompetenciesDescription { get; set; }
        public string OtherSpecialRequirements { get; set; }
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Disclaimer { get; set; }
        public string OrganisationName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string Knowledge { get; set; }
        public int JobProfileID { get; set; }
        public int JobLevelID { get; set; }
    }

    public class GeneralQuestionsModel
    {
        public int QuestionID { get; set; }
        public string QuestionDesc { get; set; }
        public int QCategoryID { get; set; }
        public string QCategoryDescr { get; set; }
    }

    public class VacancySITADepartmentModel
    {
        public int DepartmentID { get; set; }
        public string DepartmentDiscription { get; set; }

    }

    public class VacancyTypeModel
    {
        public int VancyTypeID { get; set; }
        public string VacancyTypeName { get; set; }
    }

    public class NotificationModel
    {
        public string EmailAddress { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string ReferenceNo { get; set; }
        public string VacancyName { get; set; }
    }

    public class DocumentModel
    {
        public int fkVacancyID { get; set; }
        public string sFileName { get; set; }
        public string FileData { get; set; }
        public string ContentType { get; set; }

    }

    public class VacancyHistoryModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string ReferenceNo { get; set; }
        public int OrganisationID { get; set; }
        public int DivisionID { get; set; }
        public int DepartmentID { get; set; }
        public int JobTitleID { get; set; }
        public string CategoryDescr { get; set; }
        public string Descr { get; set; }
        public string JobLevelName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Recruiter { get; set; }
        public string RecruiterEmail { get; set; }
        public string RecruiterTel { get; set; }
        public string Manager { get; set; }
        public int EmploymentTypeID { get; set; }
        public string ContractDuration { get; set; }
        public DateTime ClosingDate { get; set; }
        public int NumberOfOpenings { get; set; }
        public int VancyTypeID { get; set; }
        public string Location { get; set; }
        public int StatusID { get; set; }
        public int WithdrawalReasonID { get; set; }
        public string WithdrawalReasonOther { get; set; }
        public int RetractReasonID { get; set; }
        public int VacancyProfileID { get; set; }
    }

    public class ApproverModel
    {
        public string Fullname { get; set; }
        public string ApproverUserId { get; set; }
        public string RoleName { get; set; }
        public string RoleID { get; set; }

    }

    public class RecruiterModel
    {
        public string Fullname { get; set; }
        public string RecruiterUserId { get; set; }
        public string RoleName { get; set; }
        public string RoleID { get; set; }

    }

    public class ScreenedCandidateModel
    {
        public int ApplicationID { get; set; }
        public int VacancyID { get; set; }
        public string UserID { get; set; }
        public string IDNumber { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        public string CellNo { get; set; }
        public string EmailAddress { get; set; }
        public int ProvinceID { get; set; }
        public string RaceName { get; set; }
        public int RaceID { get; set; }
        public string Gender { get; set; }
        public int GenderID { get; set; }
        public string Disability { get; set; }
        public string QualificationTypeName { get; set; }
        public string jobTitle { get; set; }
        public string period { get; set; }
        public string skillName { get; set; }
        public string ProfessioonallyRegistered { get; set; }
        public string PreviouslyEmployedPS { get; set; }

        public int ProfileID { get; set; }
    }
    public class CandidateListToExcelModel
    {
        public string IDNumber { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        public string CellNo { get; set; }
        public string EmailAddress { get; set; }
        public string RaceName { get; set; }
        public string Gender { get; set; }
        public string Disability { get; set; }
        public string QualificationTypeName { get; set; }
        public string jobTitle { get; set; }
        public string period { get; set; }
        public string skillName { get; set; }
        public string ProfessioonallyRegistered { get; set; }
        public string PreviouslyEmployedPS { get; set; }
    }


    [Table("lutGender")]
    public class GenderModel
    {
        [Key]
        public int GenderID { get; set; }
        public string Gender { get; set; }
    }


    [Table("lutRace")]
    public class RaceModel
    {
        [Key]
        public int RaceID { get; set; }
        public string RaceName { get; set; }
    }

    public class QuestionCatergoryModel
    {
        [Key]
        public int QCategoryID { get; set; }
        public string QCategoryDescr { get; set; }

    }

    public class PublishDaysModel
    {
        [Key]
        public int NumberOfDaysID { get; set; }
        public string NumberOfDays { get; set; }

    }

    public class ProfileViewModel
    {
        [Key]
        public int pkProfileID { get; set; }
        public string Userid { get; set; }

        [Required]
        public string IDNumber { get; set; }
        public string PassportNumber { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string FirstName { get; set; }

        public string DateOfBirth { get; set; }

        [Required]
        public int? fkRaceID { get; set; }
        [Required]
        public int fkGenderID { get; set; }
        [Required]
        public string CellNo { get; set; }

        public string AlternativeNo { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        public string UnitNo { get; set; }
        public string ComplexName { get; set; }
        public string StreetNo { get; set; }
        public string Disability { get; set; }
        public string StreetName { get; set; }
        public string SuburbName { get; set; }
        [Required]
        public string City { get; set; }
        public string PostalCode { get; set; }
        [Required]
        public int? fkDisabilityID { get; set; }
        public int NatureOfDisability { get; set; }
        public string OtherNatureOfDisability { get; set; }
        [Required]
        public int SACitizen { get; set; }
        public int? fkNationalityID { get; set; }
        [Required]
        public int? fkProvinceID { get; set; }
        public int? fkWorkPermitID { get; set; }
        public string WorkPermitNo { get; set; }
        [Required]
        public int pkCriminalOffenseID { get; set; }
        [Required]
        public int? fkLanguageForCorrespondenceID { get; set; }
        [Required]
        public string TelNoDuringWorkingHours { get; set; }
        [Required]
        public int? MethodOfCommunicationID { get; set; }
        [Required]
        public string CorrespondanceDetails { get; set; }

        public int? TermsAndCondition { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }

        public string Matric { get; set; }
        public string DriversLicense { get; set; }

        public string Province { get; set; }
        //public int VacancyID { get; set; }
        public int ApplicationID { get; set; }

        [Required]
        public int? ProfessionallyRegisteredID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        //public DateTime RegistrationDate { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationBody { get; set; }
        [Required]
        public int? PreviouslyEmployedPS { get; set; }
        public int? ConditionsThatPreventsReEmploymentID { get; set; }
        public string ReEmployment { get; set; }
        public string PreviouslyEmployedDepartment { get; set; }
    }

    [Table("lutSkill")]
    public class SkillsModel
    {
        [Key]
        public int skillID { get; set; }
        public string skillName { get; set; }

    }

    [Table("lutYesorNo")]
    public class YesorNoModel
    {
        [Key]
        public int AnswerID { get; set; }
        public string Answer { get; set; }
    }

    [Table("lutCountry")]
    public class CountryModel
    {
        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }

    [Table("lutLanguage")]
    public class LanguageModel
    {
        [Key]
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }
    }

    [Table("lutLaguage_Proficiency")]
    public class LaguageProficiencyModel
    {
        [Key]
        public int LanguageProficiencyID { get; set; }
        public string LanguageProficiency { get; set; }
    }

    public class CitizenshipModel
    {
        [Key]
        public int LanguageProficiencyID { get; set; }
        public string LanguageProficiency { get; set; }
    }

    public class MethodOfCummunicationModel
    {
        public int MethodID { get; set; }
        public string MethodName { get; set; }

    }

    [Table("Education")]
    public class EducationModel
    {
        [Key]
        public int qualificationID { get; set; }
        [Required]
        [Display(Name = "Institution Name")]
        public string institutionName { get; set; }
        [Required]
        [Display(Name = "Qualification Name")]
        public string qualificationName { get; set; }
        [Required]
        [Display(Name = "Qualification Type")]
        public int QualificationTypeID { get; set; }

        public string certificateNumber { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }
        public int profileID { get; set; }

        public string QualificationTypeName { get; set; }
    }

    [Table("WorkHistory")]
    public class WorkHistoryModel
    {
        [Key]
        public int workHistoryID { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string companyName { get; set; }
        [Required]
        [Display(Name = "Job Title")]
        public string jobTitle { get; set; }
        [Required]
        [Display(Name = "Position Held")]
        public string positionHeld { get; set; }
        [Required]
        [Display(Name = "Department")]
        public string department { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public string startDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public string endDate { get; set; }
        public int profileID { get; set; }
        public string reasonForLeaving { get; set; }
        public int previouslyEmployedPS { get; set; }
        public string reEmployment { get; set; }
        public string previouslyEmployedDepartment { get; set; }

    }

    public class CandidateSkillsProfileModel
    {
        [Key]
        public int skillsProfileID { get; set; }
        [Display(Name = "Skill Name")]
        public string skillName { get; set; }
        [Display(Name = "Skill Proficiency")]
        public string SkillProficiency { get; set; }
    }

    public class CandidateLanguageProfileModel
    {
        [Key]
        public int profileLanguageID { get; set; }
        [Display(Name = "Language Name")]
        public string LanguageName { get; set; }
        [Display(Name = "Language Proficiency")]
        public string LanguageProficiency { get; set; }
    }

    [Table("Reference")]
    public class ReferenceModel
    {
        [Key]
        public int referrenceID { get; set; }
        [Required]
        [Display(Name = "Reference Name")]
        public string refName { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string companyName { get; set; }
        [Required]
        [Display(Name = "Position Held")]
        public string positionHeld { get; set; }
        [Required]
        [Display(Name = "Tel No")]
        public string telNo { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        public string emailAddress { get; set; }
        public int profileID { get; set; }
    }

    public class AttachmentModel
    {
        [Key]
        public int attachmentID { get; set; }
        public int profileID { get; set; }
        public string fileName { get; set; }
        public byte[] fileData { get; set; }
        public string contentType { get; set; }
        public string createdon { get; set; }
    }

    public class SalaryTypeModel
    {
        public int SalaryTypeID { get; set; }
        public string SalaryTypeDescr { get; set; }

    }

    public class DefinitionModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
    }
    public class BehaviouralCompModel
    {
        [Key]
        public int BehaviouralCompetencyID { get; set; }
        public Guid BehaviouralCompetencyGuidID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string BehaviouralComp { get; set; }
        public string BehaviouralCompDesc { get; set; }
        public Guid? UserID { get; set; }
    }

    public class LeadershipCompModel
    {
        [Key]
        public int LeadershipCompetencyID { get; set; }
        public Guid LeadershipCompetencyGuidID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string LeadershipComp { get; set; }
        public string LeadershipCompDesc { get; set; }
        public Guid? UserID { get; set; }
    }

    public class TechnicalCompModel
    {
        [Key]
        public int TechnicalCompetencyID { get; set; }
        public Guid TechnicalCompetencyGuidID { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string TechnicalComp { get; set; }
        public string TechnicalCompDesc { get; set; }
        public Guid? UserID { get; set; }
    }


    public class VacancySalary
    {
        public int SalaryID { get; set; }

        public int JobTitleID { get; set; }
        public String JobLevel { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        public int VacancyID { get; set; }






    }
}
