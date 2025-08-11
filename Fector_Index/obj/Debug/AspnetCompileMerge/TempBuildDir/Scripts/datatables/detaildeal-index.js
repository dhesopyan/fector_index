$(document).ready(function () {
    $('#myTableDetail thead tr#GvFilterDetail th.filter').each(function () {
        var title = $('#myTableDetail thead tr.GvHeaderDetail th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    var table = $('#myTableDetail').DataTable({
        "sDom": '<"top"i>rt<"bottom"lp><"clear">',
        "autoWidth": false,
        "info": false,
        "bSort": false,
        "sAjaxSource": ajaxHandler,
        "bLengthChange": false,
        "oLanguage": {
            "sProcessing": "<div class='text-center'><img src='" + baseUrl + "/Content/Images/loading.gif'>"
        },
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": [
            { "sName": "DealDate", "sWidth": "15%" },
            { "sName": "DealNumber", "sWidth": "15%" },
            { "sName": "AccNum", "sWidth": "15%" },
            { "sName": "CurrencyDeal", "sWidth": "15%" },
            { "sName": "EqUSD", "sWidth": "25%", "sClass": "equsd" },
            { "sWidth": "10%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");

            if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
                $('.dataTables_paginate').css("display", "block").addClass("text-center");
            } else {
                $('.dataTables_paginate').css("display", "none");
            }

            triggerTotalEq();
        }
    });
    $("#GvHeaderDetail").after($("#GvFilterDetail"));
    $("#BtnFilterDetail").bind("click", function () {
        table.column(0).search($('#GvFilterDetail th input:eq(0)').val());
        table.column(1).search($('#GvFilterDetail th input:eq(1)').val());
        table.column(2).search($('#GvFilterDetail th input:eq(2)').val()).draw();
    });
    $("#BtnClearDetail").bind("click", function () {
        $('#GvFilterDetail th input:eq(0)').val("");
        $('#GvFilterDetail th input:eq(1)').val("");
        $('#GvFilterDetail th input:eq(2)').val("");
        $("#BtnFilterDetail").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilterDetail").trigger("click");
        }
    });
    String.prototype.replaceAll = function (str1, str2, ignore) {
        return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignore ? "gi" : "g")), (typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
    }
    function triggerTotalEq() {
        var totalequsd = 0

        var data = table.rows().data();
        data.each(function (value, index) {
            var curequsd = parseFloat(value[4].replaceAll(",", ""));
            totalequsd = totalequsd + curequsd;
        });

        //var table = document.getElementById("myTableDetail");
        //for (var i = 0, row; row = table.cell[i]; i++){
        //    alert('0');
        //    var curequsd = parseFloat($(this).find(".equsd").val());
        //    totalequsd = totalequsd + curequsd;
        //}

        $("#totalequsd").val(parseFloat(totalequsd).toLocaleString('en-US'));
    }
});