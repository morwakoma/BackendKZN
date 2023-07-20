function SalarySubCategory() {
    $dropdown = $("#SalarySubCategoryID");

    var e = document.getElementById("SalaryCategoryID");
    var selectedItem = e.options[e.selectedIndex].value;
    var SalarySubCategory = document.getElementById("SalarySubCategoryID");

    var baseUrl = "GetSalarySubCategoryList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $dropdown.empty();

            $dropdown.append($('<option></option>').val('').html('--Select Salary Sub-Category--'));

            data.forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.Descr;
                option.value = item.SalarySubCategoryID;
                SalarySubCategory.appendChild(option);
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $dropdown.empty();
            alert("Error : Could not find Data for Salary Sub-Category ");
        }
    });


}

function JobLevel() {

    $JobLevelID = $("#JobLevelID");

    var e = document.getElementById("SalarySubCategoryID");
    var selectedItem = e.options[e.selectedIndex].value;
    var SalaryCategory = document.getElementById("JobLevelID");

    var baseUrl = "GetJobLevelIDList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $JobLevelID.empty();

            $JobLevelID.append($('<option></option>').val('').html('--Select Job Level--'));

            data.forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.JobLevelName;
                option.value = item.JobLevelID;
                SalaryCategory.appendChild(option);
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $JobLevelID.empty();
            alert("Error : Could not find Data for Job Level ");
        }
    });


}

function SalaryCategory() {
    var selected_val = $('#SalarySubCategory').find(":selected").attr('value');
    //var selected_val = 3;

    var baseUrl = "GetSalaryCategoryList?id=";
    $.ajax({  //ajax call
        type: "POST",      //method == POST
        url: baseUrl, //url to be called
        data: "id=" + selected_val, //data to be sends
        success: function (data) {
            //console.log(data);
            //alert(data[0].CategoryDescr);
            $('#SalaryCategoryID').val(data[0].CategoryDescr); // here we will set a value of text box    
        }
    });
}

function SalarySubCategoryForEdit() {

    $dropdown = $("#SalarySubCategoryID");
    var e = document.getElementById("SalaryCategoryID");
    var selectedItem = e.options[e.selectedIndex].value;
    var SalarySubCategory = document.getElementById("SalarySubCategoryID");

    // var baseUrl = '@Url.Action("GetSalarySubCategoryListForEdit")'; //This is not working, find out why


    $.ajax({
        type: "POST",
        url: "/Admin/GetSalarySubCategoryListForEdit",//this might not work in the servers
        data: { "id": selectedItem },
        dataType: "json",
        success: function (data) {

            $dropdown.empty();

            $dropdown.append($('<option></option>').val('').html('--Select Salary Sub-Category--'));

            data.forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.Descr;
                option.value = item.SalarySubCategoryID;
                SalarySubCategory.appendChild(option);
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $dropdown.empty();
            alert("Error : Could not find Data for Salary Sub-Category ");
        }
    });


}

function GetDepartment() {
    $dropdown = $("#DepartmentID");



    var e = document.getElementById("DivisionID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Department = document.getElementById("DepartmentID");



    /*debugger;*/
    var baseUrl = "GetDepartmentList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $dropdown.empty();

            document.getElementById("DepartmentID").disabled = false;

            $dropdown.append($('<option></option>').val('').html('--Select Department--'));

            data.forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.DepartmentName;
                option.value = item.DepartmentID;
                Department.appendChild(option);
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $dropdown.empty();
            $dropdown.append($('<option></option>').val('').html('--Select Department--'));
            alert("Error : Could not find Data for Department ");
        }
    });




}



function GetDepartmentForEdit() {
    $Departmentdropdown = $("#DepartmentID");

    var e = document.getElementById("DivisionID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Department = document.getElementById("DepartmentID");
    var DepartmentIndex = Department.options[Department.selectedIndex].value;




    var myUrl = $("#myDepartment").val();
    
   
    $.ajax({
        type: "POST",
        url: myUrl,
        data: { "id": selectedItem },
        success: function (data) {
            console.log(data);
            $Departmentdropdown.empty();
            document.getElementById("DepartmentID").disabled = false;
            $Departmentdropdown.append($('<option></option>').val('').html('--Select Department--'));



            data.forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.DepartmentName;
                option.value = item.DepartmentID;
                Department.appendChild(option);
                if (item.DepartmentID == DepartmentIndex) {
                    Department.value = DepartmentIndex;
                }
            });




        },
        error: function (xhr, ajaxOptions, thrownError) {
            $dropdown.empty();
            $dropdown.append($('<option></option>').val('').html('--Select Department--'));
            alert("Error : Could not find Data for Department ");
        }
    });
}


function GetJobProfile() {
    $txtCategoryDescr = $("#SalaryCategory");
    $txtSalarySubCategory = $("#SalarySubCategory");
    $txtJobLevelName = $("#JobLevelName");
    $txtMinValue = $("#MinValue");
    $txtMaxValue = $("#MaxValue");

    $txtVacancyPurpose = $("#VacancyPurpose");
    $txtResponsibility = $("#Responsibility");
    $txtQualificationAndExperience = $("#QualificationAndExperience");
    $txtKnowledge = $("#Knowledge");
    $txtTechComps = $("#TechComps");
    $txtLeadComps = $("#LeadComps");
    $txtBehaveComps = $("#BehaveComps");
    $txtAdditonalRequirements = $("#AdditonalRequirements");
    $txtDisclaimer = $("#Disclaimer");

    var e = document.getElementById("JobTitleID");
    var selectedItem = e.options[e.selectedIndex].value;
    var CategoryDescr = document.getElementById("SalaryCategory");
    var SalarySubCategory = document.getElementById("SalarySubCategory");
    var JobLevelName = document.getElementById("JobLevelName");
    var MinValue = document.getElementById("MinValue");
    var MaxValue = document.getElementById("MaxValue");

    var VacancyPurpose = document.getElementById("VacancyPurpose");
    var Responsibility = document.getElementById("Responsibility");
    var QualificationAndExperience = document.getElementById("QualificationAndExperience");
    var Knowledge = document.getElementById("Knowledge");
    var TechComp = document.getElementById("TechComps");
    var LeadComp = document.getElementById("LeadComps");
    var BehaveComp = document.getElementById("BehaveComps");
    var AdditonalRequirements = document.getElementById("AdditonalRequirements");
    var Disclaimer = document.getElementById("Disclaimer");
    var Leadership = document.getElementById("Leadership");

    var baseUrl = "GetJobProfileList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $txtCategoryDescr.empty();
            $txtSalarySubCategory.empty();
            $txtJobLevelName.empty();
            $txtMinValue.empty();
            $txtMaxValue.empty();

            $txtVacancyPurpose.empty();
            $txtResponsibility.empty();
            $txtQualificationAndExperience.empty();
            $txtKnowledge.empty();
            $txtTechComps.empty();
            $txtLeadComps.empty();
            $txtBehaveComps.empty();
            $txtAdditonalRequirements.empty();
            $txtDisclaimer.empty();

            if (data[0].JobLevelID < 17) {
                $("#Leadership").hide();
            } else {
                $("#Leadership").show();
            }

            data.forEach(function (item) {

                var option = document.getElementById('SalaryCategory');
                var option1 = document.getElementById('SalarySubCategory');
                var option2 = document.getElementById('JobLevelName');
                var option3 = document.getElementById('MinValue');
                var option4 = document.getElementById('MaxValue');

                var option5 = document.getElementById('VacancyPurpose');
                var option6 = document.getElementById('Responsibility');
                var option7 = document.getElementById('QualificationAndExperience');
                var option8 = document.getElementById('AdditonalRequirements');
                var option9 = document.getElementById('Disclaimer');
                var option10 = document.getElementById('TechComps');
                var option11 = document.getElementById('LeadComps');
                var option12 = document.getElementById('BehaveComps');
                var option13 = document.getElementById('Knowledge');

                option.value = item.CategoryDescr;
                option1.value = item.Descr;
                option2.value = item.JobLevelName;
                option3.value = item.MinValue;
                option4.value = item.MaxValue;

                option5.value = item.VacancyPurpose;
                option6.value = item.Responsibility;
                option7.value = item.QualificationAndExperience;
                option8.value = item.AdditionalRequirements;
                option9.value = item.Disclaimer;
                option10.value = item.TechComps;
                option11.value = item.LeadComps;
                option12.value = item.BehaveComps;
                option13.value = item.Knowledge;

                //CategoryDescr.text(item.CategoryDescr);
                //SalarySubCategory.text(item.Descr);
                //JobLevelName.text(item.JobLevelName);
                //MinValue.text(item.MinValue);
                //MaxValue.text(item.MaxValue);

                //VacancyPurpose.text(item.VacancyPurpose);
                //Responsibility.text(item.Responsibility);
                //QualificationAndExperience.text(item.QualificationAndExperience);
                //Knowledge.text(item.Knowledge);
                //TechComp.text(item.TechComps);
                //LeadComp.text(item.LeadComps);
                //BehaveComp.text(item.BehaveComps);
                //AdditonalRequirements.text(item.AdditionalRequirements);
                //Disclaimer.text(item.Disclaimer);

            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $txtCategoryDescr.empty();
            $txtSalarySubCategory.empty();
            $txtJobLevelName.empty();
            $txtMinValue.empty();
            $txtMaxValue.empty();

            $txtVacancyPurpose.empty();
            $txtResponsibility.empty();
            $txtQualificationAndExperience.empty();
            $txtKnowledge.empty();
            $txtTechComps.empty();
            $txtLeadComps.empty();
            $txtBehaveComps.empty();
            $txtAdditonalRequirements.empty();
            $txtDisclaimer.empty();
            //========Peter commented this on 20221018=============
            alert("Error : Could not find Data for Job Profile ");
            //=======================================================
            //=================Peter added this on 20221018==========
            /*  alert("Please make sure you select the valid job title.");*/
            //=======================================================
        }
    });
}


function GetJobProfileForEdit() {


    $txtCategoryDescr = $("#SalaryCategory");
    $txtSalarySubCategory = $("#SalarySubCategory");
    $txtJobLevelName = $("#JobLevelName");
    $txtMinValue = $("#MinValue");
    $txtMaxValue = $("#MaxValue");

    $txtVacancyPurpose = $("#VacancyPurpose");
    $txtResponsibility = $("#Responsibility");
    $txtQualificationAndExperience = $("#QualificationAndExperience");
    $txtTechComps = $("#TechComps");
    $txtLeadComps = $("#LeadComps");
    $txtBehaveComps = $("#BehaveComps");
    $txtAdditonalRequirements = $("#AdditonalRequirements");
    $txtDisclaimer = $("#Disclaimer");

    var e = document.getElementById("JobTitleID");
    var selectedItem = e.options[e.selectedIndex].value;
    var CategoryDescr = document.getElementById("SalaryCategory");
    var SalarySubCategory = document.getElementById("SalarySubCategory");
    var JobLevelName = document.getElementById("JobLevelName");
    var MinValue = document.getElementById("MinValue");
    var MaxValue = document.getElementById("MaxValue");

    var VacancyPurpose = document.getElementById("VacancyPurpose");
    var Responsibility = document.getElementById("Responsibility");
    var QualificationAndExperience = document.getElementById("QualificationAndExperience");
    var TechComp = document.getElementById("TechComps");
    var LeadComp = document.getElementById("LeadComps");
    var BehaveComp = document.getElementById("BehaveComps");
    var AdditonalRequirements = document.getElementById("AdditonalRequirements");
    var Disclaimer = document.getElementById("Disclaimer");
    var baseUrl = $("#myJobTitle").val();
    //var baseUrl = "GetJobProfileList?id ="

    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            //$txtCategoryDescr.empty();
            //$txtSalarySubCategory.empty();
            $txtJobLevelName.empty();
            //$txtMinValue.empty();
            //$txtMaxValue.empty();

            $txtVacancyPurpose.empty();
            $txtResponsibility.empty();
            $txtQualificationAndExperience.empty();
            $txtTechComps.empty();
            $txtLeadComps.empty();
            $txtBehaveComps.empty();
            $txtAdditonalRequirements.empty();
            $txtDisclaimer.empty();

            if (data[0].JobLevelID < 17) {
                $("#Leadership").hide();
            } else {
                $("#Leadership").show();
            }


            data.forEach(function (item) {

                var option = document.getElementById('SalaryCategory');
                var option1 = document.getElementById('SalarySubCategory');
                var option2 = document.getElementById('JobLevelName');
                var option3 = document.getElementById('MinValue');
                var option4 = document.getElementById('MaxValue');

                var option5 = document.getElementById('VacancyPurpose');
                var option6 = document.getElementById('Responsibility');
                var option7 = document.getElementById('QualificationAndExperience');
                var option8 = document.getElementById('AdditonalRequirements');
                var option9 = document.getElementById('Disclaimer');
                var option10 = document.getElementById('TechComps');
                var option11 = document.getElementById('LeadComps');
                var option12 = document.getElementById('BehaveComps');
                if (!(item.CategoryDescr === "")) {

                    option.value = item.CategoryDescr;

                }
                if (!(item.Descr === "")) {

                    option1.value = item.Descr;

                }
                if (!(item.JobLevelName === "")) {

                    option2.value = item.JobLevelName;

                }

                if (!(item.MinValue === "")) {

                    option3.value = item.MinValue;

                }

                if (!(item.MaxValue === "")) {

                    option4.value = item.MaxValue;

                }

                if (!(item.VacancyPurpose === "")) {

                    option5.value = item.VacancyPurpose;

                }
                if (!(item.Responsibility === "")) {

                    option6.value = item.Responsibility;

                }
                if (!(item.QualificationAndExperience === "")) {

                    option7.value = item.QualificationAndExperience;

                }
                if (!(item.AdditionalRequirements === "")) {

                    option8.value = item.AdditionalRequirements;

                }
                if (!(item.Disclaimer === "")) {

                    option9.value = item.Disclaimer;

                }
                if (!(item.TechComps === "")) {

                    option10.value = item.TechComps;

                }

                if (!(item.LeadComps === "")) {

                    option11.value = item.LeadComps;

                }

                if (!(item.BehaveComps === "")) {

                    option12.value = item.BehaveComps;

                }






                //CategoryDescr.text(item.CategoryDescr);
                //SalarySubCategory.text(item.Descr);
                //JobLevelName.text(item.JobLevelName);
                //MinValue.text(item.MinValue);
                //MaxValue.text(item.MaxValue);

                //VacancyPurpose.text(item.VacancyPurpose);
                //Responsibility.text(item.Responsibility);
                //QualificationAndExperience.text(item.QualificationAndExperience);
                //TechComp.text(item.TechComps);
                //LeadComp.text(item.LeadComps);
                //BehaveComp.text(item.BehaveComps);
                //AdditonalRequirements.text(item.AdditonalRequirements);
                //Disclaimer.text(item.Disclaimer);

            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $txtCategoryDescr.empty();
            $txtSalarySubCategory.empty();
            $txtJobLevelName.empty();
            $txtMinValue.empty();
            $txtMaxValue.empty();

            $txtVacancyPurpose.empty();
            $txtResponsibility.empty();
            $txtQualificationAndExperience.empty();
            $txtTechComps.empty();
            $txtLeadComps.empty();
            $txtBehaveComps.empty();
            $txtAdditonalRequirements.empty();
            $txtDisclaimer.empty();
            alert("Error : Could not find Data for Job Profile ");
        }
    });
}

function GetSalaryStructure() {
    $txtCategoryDescr = $("#SalaryCat");
    $txtSalarySubCat = $("#SalarySubCat");
    $txtJobLevel = $("#JobLevel");
    $txtMin = $("#Min");
    $txtMax = $("#Max");

    var e = document.getElementById("JobTitleID");
    var selectedItem = e.options[e.selectedIndex].value;
    var CategoryDescr = document.getElementById("SalaryCat");
    var SalarySubCategory = document.getElementById("SalarySubCat");
    var JobLevelName = document.getElementById("JobLevel");
    var MinValue = document.getElementById("Min");
    var MaxValue = document.getElementById("Max");
    var LeadList = document.getElementById("lstChosenLead");

    var baseUrl = "GetSalaryStructureList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $txtCategoryDescr.empty();
            $txtSalarySubCat.empty();
            $txtJobLevel.empty();
            $txtMin.empty();
            $txtMax.empty();

            // $dropdown.append($('<option></option>').val('').html('--Select Department--'));

            data.forEach(function (item) {

                var option = document.getElementById('SalaryCat');
                var option1 = document.getElementById('SalarySubCat');
                var option2 = document.getElementById('JobLevel');
                var option3 = document.getElementById('Min');
                var option4 = document.getElementById('Max');


                //alert(item.MaxValue);
                option.value = item.CategoryDescr;
                option1.value = item.Descr;
                option2.value = item.JobLevelName;
                //option3.value = item.MinValue;
                //option4.value = item.MaxValue;
                option3.value = convertToCommar(item.MinValue);
                option4.value = convertToCommar(item.MaxValue);

                if (item.JobLevelID >= 17) {
                    $('#lstChosenLead').multiselect('enable');
                }
                else {

                    $('option', $('#lstChosenLead')).each(function (element) {
                        $('#lstChosenLead').multiselect('deselect', $(this).val());
                    });

                    document.getElementById("LeadComps").value = '';
                    $('#lstChosenLead').multiselect('disable');
                }


                CategoryDescr.text(item.CategoryDescr);
                SalarySubCategory.text(item.Descr);
                JobLevelName.text(item.JobLevelName);
                MinValue.text(item.MinValue);
                MaxValue.text(item.MaxValue);

            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $txtCategoryDescr.empty();
            $txtSalarySubCat.empty();
            $txtJobLevel.empty();
            $txtMin.empty();
            $txtMax.empty();
            alert("Error : Could not find Data for Job Profile ");
        }
    });
}

function GetSalaryStructureForEdit() {
    $txtCategoryDescr = $("#SalaryCat");
    $txtSalarySubCat = $("#SalarySubCat");
    $txtJobLevel = $("#JobLevel");
    $txtMin = $("#Min");
    $txtMax = $("#Max");

    var e = document.getElementById("JobTitleID");
    var selectedItem = e.options[e.selectedIndex].value;
    var CategoryDescr = document.getElementById("SalaryCat");
    var SalarySubCategory = document.getElementById("SalarySubCat");
    var JobLevelName = document.getElementById("JobLevel");
    var MinValue = document.getElementById("Min");
    var MaxValue = document.getElementById("Max");
    var LeadList = document.getElementById("lstChosenLead");


    var baseUrl = "/Admin/GetSalaryStructureList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $txtCategoryDescr.empty();
            $txtSalarySubCat.empty();
            $txtJobLevel.empty();
            $txtMin.empty();
            $txtMax.empty();

            // $dropdown.append($('<option></option>').val('').html('--Select Department--'));

            data.forEach(function (item) {

                var option = document.getElementById('SalaryCat');
                var option1 = document.getElementById('SalarySubCat');
                var option2 = document.getElementById('JobLevel');
                var option3 = document.getElementById('Min');
                var option4 = document.getElementById('Max');

                //alert(item.MaxValue);
                option.value = item.CategoryDescr;
                option1.value = item.Descr;
                option2.value = item.JobLevelName;
                option3.value = item.MinValue;
                option4.value = item.MaxValue;

                if (item.JobLevelID >= 17) {
                    $('#lstChosenLead').multiselect('enable');
                }
                else {

                    $('option', $('#lstChosenLead')).each(function (element) {
                        $('#lstChosenLead').multiselect('deselect', $(this).val());
                    });

                    document.getElementById("LeadComps").value = '';
                    $('#lstChosenLead').multiselect('disable');
                }

                CategoryDescr.text(item.CategoryDescr);
                SalarySubCategory.text(item.Descr);
                JobLevelName.text(item.JobLevelName);
                MinValue.text(item.MinValue);
                MaxValue.text(item.MaxValue);
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error : Could not find Data for Job Profile ");
        }
    });
}

function GetSkillPerCatergoryForEditSkill() {
    $divSkills = $("#Skills");

    var e = document.getElementById("SkillsID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Skills = document.getElementById("Skills");

    var skillList = $('.listOfSkills> input');

    var baseUrl = "/Admin/GetSkillPerCatergory?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $divSkills.empty();

            data.forEach(function (item) {
                var l = document.createElement('label');

                var checkbox = document.createElement('input');
                checkbox.type = "checkbox";
                checkbox.name = "sk_" + item.skillID;
                checkbox.value = item.skillID;
                checkbox.id = item.skillID;
                if (skillList.length > 0) {
                    for (var i = 0; i < skillList.length; i++) {
                        if (skillList[i].value == checkbox.value) {
                            checkbox.checked = true;
                        }
                    }
                }
                var div = document.createElement('div');

                //l.appendChild(checkbox);
                // creating label for checkbox 
                var label = document.createElement('label');

                label.htmlFor = "id";
                // appending the created text to  
                // the created label tag  
                label.appendChild(document.createTextNode(item.skillName));

                div.appendChild(checkbox);
                div.appendChild(label);
                document.getElementById("Skills").appendChild(div);
                //document.getElementById("Skill").appendChild(label);
                //document.getElementById("Skills").appendChild("</br>");
                //document.body.appendChild(checkbox)
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error : Could not find Data for Skills ");
            $divSkills.empty();
        }
    });


}

function GetSkillsPerCatergory() {
    $divSkills = $("#Skills");

    var e = document.getElementById("CategoryID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Skills = document.getElementById("Skills");

    var baseUrl = "GetSkillsPerCatergoryList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $divSkills.empty();

            data.forEach(function (item) {
                var l = document.createElement('label');

                var checkbox = document.createElement('input');
                checkbox.type = "checkbox";
                checkbox.name = "sk_" + item.skillID;
                checkbox.value = item.skillID;
                checkbox.id = item.skillID;

                var div = document.createElement('div');
                //l.appendChild(checkbox);
                // creating label for checkbox 
                var label = document.createElement('label');

                label.htmlFor = "id";
                // appending the created text to  
                // the created label tag  
                label.appendChild(document.createTextNode(item.skillName));


                div.appendChild(checkbox);
                div.appendChild(label);
                document.getElementById("Skills").appendChild(div);
                //document.body.appendChild(checkbox)
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $divSkills.empty();
            alert("Error : Could not find Data for Skills ");
        }
    });

}

function GetSkillPerCatergory() {
    $divSkills = $("#Skill");

    var e = document.getElementById("CategoryID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Skills = document.getElementById("Skill");

    var baseUrl = "GetSkillPerCatergory?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $divSkills.empty();

            data.forEach(function (item) {
                var l = document.createElement('label');

                var checkbox = document.createElement('input');
                checkbox.type = "checkbox";
                checkbox.name = "sk_" + item.skillID;
                checkbox.value = item.skillID;
                checkbox.id = item.skillID;

                var div = document.createElement('div');
                //l.appendChild(checkbox);
                // creating label for checkbox 
                var label = document.createElement('label');

                label.htmlFor = "id";
                // appending the created text to  
                // the created label tag  
                label.appendChild(document.createTextNode(item.skillName));


                div.appendChild(checkbox);
                div.appendChild(label);
                document.getElementById("Skill").appendChild(div);

            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $divSkills.empty();
            alert("Error : Could not find Data for Skills ");
        }
    });


}

function GetSkillPerCatergoryForEdit() {
    $divSkills = $("#Skill");

    var e = document.getElementById("CategoryID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Skills = document.getElementById("Skill");

    var skillList = $('.listOfSkills> input');

    var baseUrl = "/Vacancy/GetSkillPerCatergory?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $divSkills.empty();

            data.forEach(function (item) {
                var l = document.createElement('label');

                var checkbox = document.createElement('input');
                checkbox.type = "checkbox";
                checkbox.name = "sk_" + item.skillID;
                checkbox.value = item.skillID;
                checkbox.id = item.skillID;
                if (skillList.length > 0) {
                    for (var i = 0; i < skillList.length; i++) {
                        if (skillList[i].value == checkbox.value) {
                            checkbox.checked = true;
                        }
                    }
                }
                var div = document.createElement('div');

                //l.appendChild(checkbox);
                // creating label for checkbox 
                var label = document.createElement('label');

                label.htmlFor = "id";
                // appending the created text to  
                // the created label tag  
                label.appendChild(document.createTextNode(item.skillName));

                div.appendChild(checkbox);
                div.appendChild(label);
                document.getElementById("Skill").appendChild(div);
                //document.getElementById("Skill").appendChild(label);
                //document.getElementById("Skills").appendChild("</br>");
                //document.body.appendChild(checkbox)
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $divSkills.empty();
            alert("Error : Could not find Data for Skills ");
        }
    });

}

function isNumberKey(evt) {
    //var e = evt || window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function ValidateAlpha(evt) {
    var keyCode = (evt.which) ? evt.which : evt.keyCode
    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 123) && keyCode != 32)

        return false;
    return true;
}

function CheckEmployment() {


    var d = $("#EmploymentTypeID").val();
    if (d == 1 || d == 3 || d == 4 || d == 5) {
        $("#ContractDuration").show();

    }
    else {

        $("#ContractDuration").hide();
    }
}

function CheckEmploymentEdit() {


    var d = $("#EmploymentTypeID").val();
    if (d == 1 || d == 3 || d == 4 || d == 5) {
        $("#ContractDuration").show();

    }
    else {
        document.getElementById("ContractDuration").innerHTML = "";
        $("#ContractDuration").hide();
    }
}



function GetVacancyID(d) {
    document.getElementById("hdnVacancyID").value = d;
    $('hdnVacancyID').val(d);
    //alert(d);
}

function GetRejectReason() {
    var d = $('#RejectReasonID').val();
    //For now Unknown is equal to 4
    if (d == 4) {
        $('#RejectReason').attr('required', 'required');
        $('#divRejectReason').show();
    } else {
        $('#RejectReason').removeAttr('required');
        $('#divRejectReason').hide();
    }
}

function SelectedIndexChangedEmploymentType() {
    $(function () {
        $("#EmploymentTypeID").change(function () {
            var value = $(this).val();

            //alert(value);
            if (value == 1) {
                $("#ContractDuration").show();
            }
            else {

                $("#ContractDuration").hide();
            }

        })
    })
}

function CheckContractDuration() {
    var d = $("#EmploymentTypeID").val();
    if (d == 13 || d == 14 || d == 15) {
        $("#ContractDuration").show();

    }
    else {

        $("#ContractDuration").hide();
    }
}

function GetRetractReason() {
    var d = $('#RetractReasonID').val();
    //For now Unknown is equal to 3
    if (d == 3) {
        $('#RetractReason').attr('required', 'required');
        $('#divReason').show();
        //$("#RetractReasonID").attr("required", true);
        //document.getElementById("RetractReasonID").setAttribute("required", "true");
    } else {
        $('#RetractReason').removeAttr('required');
        $('#divReason').hide();
    }
}

function GetWithdrawalReason() {
    var d = $('#WithdrawalReasonID').val();
    //For now Unknown is equal to 3
    if (d == 8) {
        $('#WithdrawalReason').attr('required', 'required');
        $('#divWithdrawalReason').show();
        //$("#RetractReasonID").attr("required", true);
        //document.getElementById("RetractReasonID").setAttribute("required", "true");
    } else {
        $('#WithdrawalReason').removeAttr('required');
        $('#divWithdrawalReason').hide();
    }
}

function GetVacancyID(d) {
    document.getElementById("hdnVacancyID").value = d;
    $('hdnVacancyID').val(d);
    //alert(d);
}

function GetVacancyIDForRetract(d) {
    document.getElementById("hdnVacancyIDFor").value = d;
    $('hdnVacancyIDFor').val(d);
    //alert(d);
}

function GetQuestionBanksPerVacancy() {
    $divQuestions = $("#Questions");

    var e = document.getElementById("VacancyID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Questions = document.getElementById("Questions");

    var questionsList = $('.listOfQuestions> input');

    var baseUrl = "GetQuestionBanksPerVacancy?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $divQuestions.empty();

            data.forEach(function (item) {
                var l = document.createElement('label');

                var checkbox = document.createElement('input');
                checkbox.type = "checkbox";
                checkbox.name = "sk_" + item.id;
                checkbox.value = item.id;
                checkbox.id = item.id;
                if (questionsList.length > 0) {
                    for (var i = 0; i < questionsList.length; i++) {
                        if (questionsList[i].value == checkbox.value) {
                            checkbox.checked = true;
                        }
                    }
                }
                var div = document.createElement('div');

                //l.appendChild(checkbox);
                // creating label for checkbox 
                var label = document.createElement('label');

                label.htmlFor = "id";
                // appending the created text to  
                // the created label tag  
                label.appendChild(document.createTextNode("\u200b" + "\u200b" + item.GeneralQuestionDesc));

                div.appendChild(checkbox);
                div.appendChild(label);
                document.getElementById("Questions").appendChild(div);
                //document.getElementById("Skill").appendChild(label);
                //document.getElementById("Skills").appendChild("</br>");
                //document.body.appendChild(checkbox)
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Error : Could not find Data for Killer Questions ");
            $divQuestions.empty();
        }
    });


}

function GetApprover() {
    $dropdown = $("#ApproverUserId");
    $Recruiterdropdown = $("#RecruiterUserId");

    var e = document.getElementById("DepartmentID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Approver = document.getElementById("ApproverUserId");
    var Recruiter = document.getElementById("RecruiterUserId");

    var baseUrl = "GetApproverByDepartmentList?id=";
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $dropdown.empty();
            $Recruiterdropdown.empty();

            $Recruiterdropdown.append($('<option></option>').val('').html('--Select Recruiter--'));
            $dropdown.append($('<option></option>').val('').html('--Select Approver--'));

            data[0].forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.Fullname;
                option.value = item.ApproverUserId;
                Approver.appendChild(option);
            });
            data[1].forEach(function (item) {

                var recruiter = document.createElement('option');
                recruiter.text = item.Fullname;
                recruiter.value = item.RecruiterUserId;
                Recruiter.appendChild(recruiter);
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $dropdown.empty();
            $Recruiterdropdown.empty();

            $Recruiterdropdown.append($('<option></option>').val('').html('--Select Recruiter--'));
            $dropdown.append($('<option></option>').val('').html('--Select Approver--'));

            alert("Error : Could not find Data for Approver ");
        }
    });
}

function GetApproverForEdit() {
    $dropdown = $("#ApproverUserId");
    $Recruiterdropdown = $("#RecruiterUserId");

    var e = document.getElementById("DepartmentID");
    var selectedItem = e.options[e.selectedIndex].value;
    var Approver = document.getElementById("ApproverUserId");
    var Recruiter = document.getElementById("RecruiterUserId");
    var ApproverIndex = Approver.options[Approver.selectedIndex].value;
    var RecruiterIndex = Recruiter.options[Recruiter.selectedIndex].value;

    var myUrl = $("#myApprover").val();
    $.ajax({
        type: "POST",
        url: myUrl,
        data: { "id": selectedItem },
        success: function (data) {

            $dropdown.empty();
            $Recruiterdropdown.empty();

            $Recruiterdropdown.append($('<option></option>').val('').html('--Select Recruiter--'));
            $dropdown.append($('<option></option>').val('').html('--Select Approver--'));

            data[0].forEach(function (item) {
                var option = document.createElement('option');
                option.text = item.Fullname;
                option.value = item.ApproverUserId;
                Approver.appendChild(option);
                if (item.ApproverUserId == ApproverIndex) {
                    Approver.value = ApproverIndex;
                }
            });
            data[1].forEach(function (item) {

                var recruiter = document.createElement('option');
                recruiter.text = item.Fullname;
                recruiter.value = item.RecruiterUserId;
                Recruiter.appendChild(recruiter);
                if (item.RecruiterUserId == RecruiterIndex) {
                    Recruiter.value = RecruiterIndex;
                }
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $dropdown.empty();
            $Recruiterdropdown.empty();

            $Recruiterdropdown.append($('<option></option>').val('').html('--Select Recruiter--'));
            $dropdown.append($('<option></option>').val('').html('--Select Approver--'));

            alert("Error : Could not find Data for Approver ");
        }
    });
}

function CheckVacancyType() {
    var d = $("#VancyTypeID").val();
    if (d == 4 || d == 5 || d == 6) {
        $("#DeligationReasons").show();
    }
    else {
        $("#DeligationReasons").hide();
    }
}

function GetSalary() {
    var d = $("#SalaryTypeID").val();
    if (d == 3) {
        $("#Min").show();
        $("#Max").show();
    }
    else {
        $("#Min").hide();
        $("#Max").hide();
    }
}

function checkBPS(event) {

    var bpsNum = document.getElementById("BPSVacancyNo").value;
    var patt = new RegExp("^(?:[A-Za-z]{3}[0-9]{5})(?:,[A-Za-z]{3}[0-9]{5})*$");
    var test = patt.test(bpsNum);
    if (!test) {
        event.preventDefault();
        alert("Please Check Your BSP NUmber and Make Sure the Formart is Correct eg XXX00000,xxx55555");
    }




}
/**
 * By khutso
 * */
function validateVacancyBPSNumber() {


    var VBPSNumber = $("#BPSVacancyNo").val();
    var BPSNumberExixt = false;

    var baseUrl = $("#myBPSVacancyNoAPI").val();;
    $.ajax({
        type: "POST",
        url: baseUrl,
        data: { BPSNumber: VBPSNumber },
        async: false,
        // traditional: true, 
        //contentType: "application/json; charset=utf-8",
        success: function (data) {

            console.log(data);

            if (data > 0) {
                BPSNumberExixt = true;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {

            alert("Error :Please contact system administrator ");
        }
    });

    return BPSNumberExixt;
}



/**
 * By khutso
 * */
//function checkVacancyBPSNumber() {
    

//    var VBPSNumber = $("#BPSVacancyNo").val();
//    var BPSNumberExixt = false;

//    var baseUrl = $("#myBPSVacancyNo").val();;
//    $.ajax({
//        type: "POST",
//        url: baseUrl,
//        data: { BPSVacancyNo: VBPSNumber },
//        async: false,
//        // traditional: true, 
//        //contentType: "application/json; charset=utf-8",
//        success: function (data) {

//            if (data > 0) {
//                BPSNumberExixt = true;
//            }
//        },
//        error: function (xhr, ajaxOptions, thrownError) {

//            alert("Error :Please contact system administrator ");
//        }
//    });

//    return BPSNumberExixt;
//}
/**
 * By khutso
 * */
function generalCheckBoxValidation(checkbox) {
    checkbox = "." + checkbox;
    var check = false;
    $(checkbox).each(function () {
        check = $(this).find('.checkbox > label > input:checkbox').is(":checked");

    });

    return check;
}
/**
 * By khutso
 * */
function customValidation(event) {
    
    var validateBPSNumber = checkVacancyBPSNumber();
    //call question bank validation
    var validQualification = generalCheckBoxValidation('box-body-qualification');
    var validAnnaulSalary = generalCheckBoxValidation('box-body-annaul-salary');
    var validNoticePeriod = generalCheckBoxValidation('box-body-notice-period');
    var validExperience = generalCheckBoxValidation('box-body-experience');
    let closeDate = new Date($("#ClosingDate").val());
    let today = new Date();
    let mymodal = $('#myModal');
    var BPSVacancyNo = $("#BPSVacancyNo").val();
    var DivisionID = $("#DivisionID").val();
    var JobTitleID = $("#JobTitleID").val();
    var DepartmentID = $("#DepartmentID").val();
    var SalaryTypeID = $("#SalaryTypeID").val();
    var RecruiterTel = $("#RecruiterTel").val();
    var RecruiterUserId = $("#RecruiterUserId").val();
    var GenderID = $("#GenderID").val();
    var RaceID = $("#RaceID").val();
    var EmploymentTypeID = $("#EmploymentTypeID").val();
    var NumberOfOpenings = $("#NumberOfOpenings").val();
    var VancyTypeID = $("#VancyTypeID").val();
    var file1 = $("#file1").val();
    var ApproverUserId = $("#ApproverUserId").val();
    //================Peter 20221024=======================
    var location = $("#LocationID").val();
    //=====================================================
    
    if ($("#myBPSVacancyNoHidden").val() === "EditVacancy") {

        validateBPSNumber = false;
    }

    if (validateBPSNumber) {
        // prevent the form submit
        event.preventDefault();
        //display the error message
        mymodal.find('.modal-body').text('BPS Vacany Number already Exist');
        mymodal.modal('show');
    }
    else if (closeDate <= today) {
        event.preventDefault(); // prevent the form submit
        //display the error message
        mymodal.find('.modal-body').text('Closing date cannot be less or equals to Today`s Date');
        mymodal.modal('show');

        //============Khutso - Commented by Peter on 20221017, BA's adviced the message should be to a specific field=============
        //} else if ($("#BPSVacancyNo").val() === "" || $("#DivisionID").val() === ""|| $("#JobTitleID").val() === "" ||
        //    $("#DepartmentID").val() === "" || $("#SalaryTypeID").val() === "" || $("#RecruiterTel").val() === "" ||
        //    $("#RecruiterUserId").val() === "" || $("#GenderID").val() === "" || $("#RaceID").val() === ""|| $("#EmploymentTypeID").val() ==="" ||
        //    $("#NumberOfOpenings").val() === "" || VancyTypeID === "" || file1 === "" || ApproverUserId === ""
        //    || !(validQualification) || !(validAnnaulSalary) || !(validNoticePeriod) || !(validExperience)) {

        //    event.preventDefault();//prevent the form submit
        //     //display the error message
        //    mymodal.find('.modal-body').text('Required fields can not be empty, make sure all required fields are not empty');
        //    mymodal.modal('show');
        //========================================================================================================================
        //========================================Peter - new code, specific field================================================20221017
    } else if ($("#BPSVacancyNo").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('BPS Vacancy Number cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#BPSVacancyNo").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#DivisionID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Division cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#DivisionID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#JobTitleID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Job Title cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#JobTitleID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#DepartmentID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Department cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#DepartmentID").focus();
    }
    //========================================================================================================================
    else if ($("#SalaryTypeID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Salary Type cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#SalaryTypeID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#RecruiterTel").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Recruiter telephone number cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#RecruiterTel").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#GenderID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Gender cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#GenderID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#RaceID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Race cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#RaceID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#EmploymentTypeID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Employment type cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#EmploymentTypeID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#NumberOfOpenings").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Number of openings cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#NumberOfOpenings").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#VancyTypeID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Vacancy type cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#VancyTypeID").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#VancyTypeID").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Vacancy type cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#VancyTypeID").focus();
    }
    //========================================================================================================================

    //========================================Peter - new code, specific field================================================20221017
    else if ($("#file1").val() === "") {

        
        if (!(controllerAction[2] === "EditVacancy")) {

            event.preventDefault();//prevent the form submit
            mymodal.find('.modal-body').text('Attachment cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
            mymodal.modal('show');
            $("#file1").focus();

        }

       
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if ($("#ApproverUserId").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Vacancy Approver cannot be empty, please make sure all required fields that are marked with red asterik are not empty');
        mymodal.modal('show');
        $("#ApproverUserId").focus();
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if (!(validQualification)) {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Qualifications cannot be empty, please select your qualification(s)');
        mymodal.modal('show');
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if (!(validAnnaulSalary)) {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Annual Salary cannot be empty, please select your annual salary');
        mymodal.modal('show');
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if (!(validNoticePeriod)) {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Notice period cannot be empty, please select your notice period');
        mymodal.modal('show');
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221017
    else if (!(validExperience)) {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Experience cannot be empty, please select your experience');
        mymodal.modal('show');
    }
    //========================================================================================================================
    //========================================Peter - new code, specific field================================================20221024
    else if ($("#Location").val() === "") {
        event.preventDefault();//prevent the form submit
        mymodal.find('.modal-body').text('Please select location');
        mymodal.modal('show');
        $("#Location").focus();
    }
    //========================================================================================================================

    else {

        document.forms["AddVacanyForm"].submit();

    }
}

function convertToCommar(rand) {
    var strRand = String(rand);
    return strRand.replace('.', ',');
}

// convert , -> .

function convertToDot(rand) {
    var strRand = String(rand);
    return strRand.replace(',', '.');
}

function CheckProfessionallyRegistered() {
    var d = $("#ProfessionallyRegisteredID").val();
    //alert($("#ProfessionallyRegisteredID").val());
    if (d == 1) {
        $("#RegistrationDate").show();
        $("#RegistrationNumber").show();
        $("#RegistrationBody").show();

        document.getElementById("RegistrationDate").required = true;
        document.getElementById("RegistrationNumber").required = true;
        document.getElementById("RegistrationBody").required = true;
    }
    else {
        $("#RegistrationDate").hide();
        $("#RegistrationNumber").hide();
        $("#RegistrationBody").hide();

        document.getElementById("RegistrationDate").required = false;
        document.getElementById("RegistrationNumber").required = false;
        document.getElementById("RegistrationBody").required = false;
    }
}