$(document).ready(function () {
    $("#signup").on("mouseenter", function () {

        $("#signup").css({ "background-color": "#d0d0ce", "color": "black"});

    });
    $("#signup").on("mouseleave", function () {

        $("#signup").css({ "background-color": "black", "color": "white" });

    });

    
        $(".editYourProfileButton").on("click", function () {
          //$(".MyDetailsPageBody #EmployeeId").removeAttr('readonly');
          //  $(".MyDetailsPageBody #EmployeeName").removeAttr('readonly');
          //  $(".MyDetailsPageBody #MobileNumber").removeAttr('readonly');
          //  $(".MyDetailsPageBody #EmployeeBirthDate").removeAttr('readonly');
          //  $(".MyDetailsPageBody #Gender").removeAttr('readonly');
          //  $(".MyDetailsPageBody #email").removeAttr('readonly');
            $(".MyDetailsPageBody #password").removeAttr('readonly');
            $(".MyDetailsPageBody #signup").removeAttr('hidden');
            
        });


    var minDate;
    var maxDate;
    var mDate
    var j = jQuery.noConflict();
    j("#startDate").datepicker({
        onSelect: function () {
            minDate = j("#startDate").datepicker("getDate");
            var mDate = new Date(minDate.setDate(minDate.getDate()));
            maxDate = new Date(minDate.setDate(minDate.getDate() + 10));
            j("#endDate").datepicker("setDate", maxDate);
            j("#endDate").datepicker("option", "minDate", mDate);
            j("#endDate").datepicker("option", "maxDate", maxDate);
        }
    });
    var tdate = new Date();
    var ddate = new Date(tdate.setDate(tdate.getDate() + 30));
    j("#startDate").datepicker("setDate", new Date());
    j("#endDate").datepicker();
    j("#endDate").datepicker("setDate", ddate);

  
});
function enableTxt(p1, p2, p3, p4, p5) {
    $("#HolidayId").val(p1);
    $("#EmployeeId").val(p2);
    $("#StartDate").val(p3);
    $("#EndDate").val(p4);
    $("#Reason").val(p5);



};